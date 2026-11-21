using System.Globalization;
using System.Reflection;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Somtumzing.Backend.Api.Data;
using Somtumzing.Backend.Api.Models;

namespace Somtumzing.Backend.Api.Pages.Admin;

[Authorize(Policy = "BackOffice")]
public class MasterDataModel : PageModel
{
    private readonly GameDbContext _context;

    // รายการตาราง Master Data ที่อนุญาตให้จัดการผ่านหน้าจอนี้
    private static readonly Dictionary<string, Type> AllowedTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        { "GameEvent", typeof(GameEvent) },
        { "GameAchievement", typeof(GameAchievement) },
        { "Stage", typeof(Stage) },
        { "StageConfig", typeof(StageConfig) },
        { "Upgrade", typeof(Upgrade) },
        { "Mission", typeof(Mission) },
        { "SabotageCard", typeof(SabotageCard) },
        { "ChaosEventDefinition", typeof(ChaosEventDefinition) },
        { "Challenge", typeof(Challenge) }
    };

    private static readonly HashSet<string> SystemFieldNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "CreatedDate",
        "CreatedBy",
        "UpdatedDate",
        "UpdatedBy"
    };

    public MasterDataModel(GameDbContext context)
    {
        _context = context;
    }

    public IReadOnlyList<string> AvailableEntities => AllowedTypes.Keys.ToList();

    [BindProperty(SupportsGet = true)]
    public string? Entity { get; set; }

    [BindProperty(SupportsGet = true)]
    public Guid? EditPk { get; set; }

    [BindProperty(SupportsGet = true)]
    public bool IsCreating { get; set; }

    public IReadOnlyList<FieldSchemaViewModel> Schema { get; private set; } = Array.Empty<FieldSchemaViewModel>();

    public IReadOnlyList<FieldSchemaViewModel> VisibleFields => Schema.Where(f => !f.IsSystemField).ToList();

    public IReadOnlyList<RowViewModel> Rows { get; private set; } = Array.Empty<RowViewModel>();

    public Dictionary<string, string?> FormValues { get; private set; } = new(StringComparer.OrdinalIgnoreCase);

    public bool IsModalOpen => !string.IsNullOrWhiteSpace(Entity) && (IsCreating || EditPk.HasValue);

    public string ModalTitle => EditPk.HasValue
        ? $"แก้ไข: {Entity}"
        : $"เพิ่มข้อมูล: {Entity}";

    [TempData]
    public string? SuccessMessage { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(Entity))
        {
            return Page();
        }

        if (!AllowedTypes.TryGetValue(Entity, out var clrType))
        {
            ErrorMessage = $"ไม่พบประเภทข้อมูล '{Entity}' ในระบบ";
            Entity = null;
            return Page();
        }

        // ปรับตัวพิมพ์ชื่อ Entity ให้ตรงกับ Key มาตรฐาน
        Entity = AllowedTypes.Keys.First(k => string.Equals(k, Entity, StringComparison.OrdinalIgnoreCase));

        await LoadEntityMetadataAndRowsAsync(clrType, cancellationToken);

        if (EditPk.HasValue && EditPk.Value != Guid.Empty)
        {
            var existingEntity = await _context.FindAsync(clrType, new object[] { EditPk.Value }, cancellationToken);
            if (existingEntity is null)
            {
                ErrorMessage = "ไม่พบข้อมูลที่ต้องการแก้ไข";
                return RedirectToPage(new { Entity });
            }

            PopulateFormValuesFromEntity(clrType, existingEntity, isNewRecord: false);
        }
        else if (IsCreating)
        {
            var defaultInstance = Activator.CreateInstance(clrType)!;
            PopulateFormValuesFromEntity(clrType, defaultInstance, isNewRecord: true);
        }

        return Page();
    }

    public async Task<IActionResult> OnPostSaveAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(Entity) || !AllowedTypes.TryGetValue(Entity, out var clrType))
        {
            ErrorMessage = "ประเภทข้อมูลไม่ถูกต้อง";
            return RedirectToPage();
        }

        Entity = AllowedTypes.Keys.First(k => string.Equals(k, Entity, StringComparison.OrdinalIgnoreCase));

        var form = await Request.ReadFormAsync(cancellationToken);
        var rawPk = form["Field_PK"].ToString();
        var isUpdate = Guid.TryParse(rawPk, out var parsedPk) && parsedPk != Guid.Empty;

        object targetEntity;
        if (isUpdate)
        {
            var found = await _context.FindAsync(clrType, new object[] { parsedPk }, cancellationToken);
            if (found is null)
            {
                ErrorMessage = "ไม่พบข้อมูลที่ต้องการอัปเดตในระบบ";
                return RedirectToPage(new { Entity });
            }
            targetEntity = found;
        }
        else
        {
            targetEntity = Activator.CreateInstance(clrType)!;
            SetPropertyValue(clrType, targetEntity, "PK", Guid.NewGuid());
            SetPropertyValue(clrType, targetEntity, "CreatedDate", DateTimeOffset.UtcNow);
            SetPropertyValue(clrType, targetEntity, "CreatedBy", GetCurrentAdminId());
        }

        SetPropertyValue(clrType, targetEntity, "UpdatedDate", DateTimeOffset.UtcNow);
        SetPropertyValue(clrType, targetEntity, "UpdatedBy", GetCurrentAdminId());

        try
        {
            foreach (var prop in GetScalarProperties(clrType))
            {
                if (prop.Name.Equals("PK", StringComparison.OrdinalIgnoreCase) || SystemFieldNames.Contains(prop.Name))
                {
                    continue;
                }

                var formKey = $"Field_{prop.Name}";
                var rawValue = form.ContainsKey(formKey) ? form[formKey].ToString() : null;
                var convertedValue = ConvertFormValue(prop, rawValue);
                prop.SetValue(targetEntity, convertedValue);
            }

            if (!isUpdate)
            {
                _context.Add(targetEntity);
            }

            await _context.SaveChangesAsync(cancellationToken);
            SuccessMessage = "บันทึกข้อมูลเรียบร้อย";
            return RedirectToPage(new { Entity });
        }
        catch (Exception ex)
        {
            ErrorMessage = $"ไม่สามารถบันทึกได้: {ex.InnerException?.Message ?? ex.Message}";

            // โหลดตารางและคืนค่าที่ผู้ใช้กรอกค้างไว้ใน Modal เพื่อไม่ต้องพิมพ์ใหม่
            await LoadEntityMetadataAndRowsAsync(clrType, cancellationToken);
            if (isUpdate)
            {
                EditPk = parsedPk;
                IsCreating = false;
            }
            else
            {
                EditPk = null;
                IsCreating = true;
            }

            FormValues["PK"] = isUpdate ? parsedPk.ToString() : string.Empty;
            foreach (var prop in GetScalarProperties(clrType))
            {
                var formKey = $"Field_{prop.Name}";
                if (form.ContainsKey(formKey))
                {
                    FormValues[prop.Name] = form[formKey].ToString();
                }
            }

            return Page();
        }
    }

    public async Task<IActionResult> OnPostDeleteAsync(string entity, Guid pk, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(entity) || !AllowedTypes.TryGetValue(entity, out var clrType))
        {
            ErrorMessage = "ประเภทข้อมูลไม่ถูกต้อง";
            return RedirectToPage();
        }

        var normalizedEntity = AllowedTypes.Keys.First(k => string.Equals(k, entity, StringComparison.OrdinalIgnoreCase));

        try
        {
            var target = await _context.FindAsync(clrType, new object[] { pk }, cancellationToken);
            if (target is null)
            {
                ErrorMessage = "ไม่พบข้อมูลที่ต้องการลบ";
                return RedirectToPage(new { Entity = normalizedEntity });
            }

            _context.Remove(target);
            await _context.SaveChangesAsync(cancellationToken);
            SuccessMessage = "ลบข้อมูลเรียบร้อย";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"ลบข้อมูลไม่สำเร็จ: {ex.InnerException?.Message ?? ex.Message}";
        }

        return RedirectToPage(new { Entity = normalizedEntity });
    }

    private async Task LoadEntityMetadataAndRowsAsync(Type clrType, CancellationToken cancellationToken)
    {
        var props = GetScalarProperties(clrType);

        Schema = props.Select(p =>
        {
            var underlying = Nullable.GetUnderlyingType(p.PropertyType);
            var effectiveType = underlying ?? p.PropertyType;
            var isNullable = underlying is not null || (!p.PropertyType.IsValueType && IsReferencePropertyNullable(p));

            return new FieldSchemaViewModel
            {
                Name = p.Name,
                TypeName = effectiveType.Name,
                IsNullable = isNullable,
                IsPrimaryKey = p.Name.Equals("PK", StringComparison.OrdinalIgnoreCase),
                IsSystemField = SystemFieldNames.Contains(p.Name),
                InputKind = DetermineInputKind(p.Name, effectiveType)
            };
        }).ToList();

        var setMethod = typeof(DbContext)
            .GetMethods()
            .First(m => m.Name == nameof(DbContext.Set) && m.IsGenericMethod && m.GetParameters().Length == 0)
            .MakeGenericMethod(clrType);

        var queryable = setMethod.Invoke(_context, null)!;

        var asNoTrackingMethod = typeof(EntityFrameworkQueryableExtensions)
            .GetMethods()
            .First(m =>
            {
                if (m.Name != nameof(EntityFrameworkQueryableExtensions.AsNoTracking) || !m.IsGenericMethod)
                    return false;
                var ps = m.GetParameters();
                return ps.Length == 1 && ps[0].ParameterType.IsGenericType &&
                       ps[0].ParameterType.GetGenericTypeDefinition() == typeof(IQueryable<>);
            })
            .MakeGenericMethod(clrType);

        var noTrackingQueryable = asNoTrackingMethod.Invoke(null, new[] { queryable })!;

        var toListAsyncMethod = typeof(EntityFrameworkQueryableExtensions)
            .GetMethods()
            .First(m =>
            {
                if (m.Name != nameof(EntityFrameworkQueryableExtensions.ToListAsync) || !m.IsGenericMethod)
                    return false;
                var ps = m.GetParameters();
                return ps.Length == 2 && ps[1].ParameterType == typeof(CancellationToken);
            })
            .MakeGenericMethod(clrType);

        var task = (Task)toListAsyncMethod.Invoke(null, new[] { noTrackingQueryable, cancellationToken })!;
        await task.ConfigureAwait(false);

        var resultProperty = task.GetType().GetProperty("Result")!;
        var rawItems = (System.Collections.IEnumerable)resultProperty.GetValue(task)!;

        var pkProp = clrType.GetProperty("PK");
        var codeProp = clrType.GetProperty("Code");

        var itemsList = rawItems.Cast<object>().ToList();
        if (codeProp is not null)
        {
            itemsList = itemsList
                .OrderBy(item => codeProp.GetValue(item)?.ToString() ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        var rowList = new List<RowViewModel>(itemsList.Count);
        foreach (var item in itemsList)
        {
            var pkVal = pkProp?.GetValue(item) is Guid g ? g : Guid.Empty;
            var cells = new Dictionary<string, CellValueViewModel>(StringComparer.OrdinalIgnoreCase);

            foreach (var prop in props)
            {
                if (SystemFieldNames.Contains(prop.Name))
                {
                    continue;
                }

                var val = prop.GetValue(item);
                cells[prop.Name] = FormatCellValue(val);
            }

            rowList.Add(new RowViewModel
            {
                Pk = pkVal,
                Cells = cells
            });
        }

        Rows = rowList;
    }

    private void PopulateFormValuesFromEntity(Type clrType, object instance, bool isNewRecord)
    {
        FormValues.Clear();
        foreach (var prop in GetScalarProperties(clrType))
        {
            if (SystemFieldNames.Contains(prop.Name))
            {
                continue;
            }

            var val = prop.GetValue(instance);
            if (prop.Name.Equals("PK", StringComparison.OrdinalIgnoreCase))
            {
                FormValues[prop.Name] = isNewRecord ? string.Empty : val?.ToString() ?? string.Empty;
                continue;
            }

            if (isNewRecord && prop.Name.Equals("Status", StringComparison.OrdinalIgnoreCase) && string.IsNullOrWhiteSpace(val?.ToString()))
            {
                FormValues[prop.Name] = "ACTIVE";
                continue;
            }

            if (val is null)
            {
                FormValues[prop.Name] = string.Empty;
            }
            else if (val is bool b)
            {
                FormValues[prop.Name] = b ? "true" : "false";
            }
            else if (val is DateTimeOffset dto)
            {
                FormValues[prop.Name] = isNewRecord && dto == default
                    ? DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture)
                    : dto.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture);
            }
            else if (val is DateTime dt)
            {
                FormValues[prop.Name] = isNewRecord && dt == default
                    ? DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture)
                    : dt.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture);
            }
            else if (val is decimal dec)
            {
                FormValues[prop.Name] = dec.ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                FormValues[prop.Name] = val.ToString() ?? string.Empty;
            }
        }
    }

    private static List<PropertyInfo> GetScalarProperties(Type clrType)
    {
        return clrType
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead && p.CanWrite && IsSupportedScalarType(p.PropertyType))
            .ToList();
    }

    private static bool IsSupportedScalarType(Type type)
    {
        var effective = Nullable.GetUnderlyingType(type) ?? type;
        return effective.IsPrimitive
            || effective.IsEnum
            || effective == typeof(string)
            || effective == typeof(decimal)
            || effective == typeof(Guid)
            || effective == typeof(DateTime)
            || effective == typeof(DateTimeOffset);
    }

    private static bool IsReferencePropertyNullable(PropertyInfo property)
    {
        var nullabilityContext = new NullabilityInfoContext();
        var info = nullabilityContext.Create(property);
        return info.WriteState == NullabilityState.Nullable;
    }

    private static string DetermineInputKind(string propName, Type effectiveType)
    {
        if (effectiveType == typeof(bool))
            return "boolean";
        if (effectiveType == typeof(int) || effectiveType == typeof(long) || effectiveType == typeof(short))
            return "integer";
        if (effectiveType == typeof(decimal) || effectiveType == typeof(double) || effectiveType == typeof(float))
            return "decimal";
        if (effectiveType == typeof(DateTimeOffset) || effectiveType == typeof(DateTime) ||
            propName.Contains("Date", StringComparison.OrdinalIgnoreCase) ||
            propName.Contains("AtUtc", StringComparison.OrdinalIgnoreCase))
            return "datetime";
        if (propName.Contains("Config", StringComparison.OrdinalIgnoreCase) ||
            propName.Contains("Json", StringComparison.OrdinalIgnoreCase) ||
            propName.Contains("Effects", StringComparison.OrdinalIgnoreCase) ||
            propName.Contains("Actions", StringComparison.OrdinalIgnoreCase))
            return "textarea";

        return "text";
    }

    private static CellValueViewModel FormatCellValue(object? val)
    {
        if (val is null)
        {
            return new CellValueViewModel { DisplayText = "-" };
        }

        if (val is bool b)
        {
            return new CellValueViewModel
            {
                IsBoolean = true,
                BoolValue = b,
                DisplayText = b ? "ใช่" : "ไม่"
            };
        }

        if (val is DateTimeOffset dto)
        {
            return new CellValueViewModel
            {
                DisplayText = dto.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss 'UTC'", CultureInfo.InvariantCulture)
            };
        }

        if (val is decimal dec)
        {
            return new CellValueViewModel
            {
                DisplayText = dec.ToString("0.##", CultureInfo.InvariantCulture)
            };
        }

        var str = val.ToString();
        return new CellValueViewModel
        {
            DisplayText = string.IsNullOrEmpty(str) ? "-" : str
        };
    }

    private static object? ConvertFormValue(PropertyInfo prop, string? rawValue)
    {
        var underlying = Nullable.GetUnderlyingType(prop.PropertyType);
        var targetType = underlying ?? prop.PropertyType;
        var isNullable = underlying is not null || !prop.PropertyType.IsValueType;

        if (string.IsNullOrWhiteSpace(rawValue))
        {
            if (targetType == typeof(string))
            {
                return IsReferencePropertyNullable(prop) ? null : string.Empty;
            }
            if (isNullable)
            {
                return null;
            }
            if (targetType == typeof(int)) return 0;
            if (targetType == typeof(long)) return 0L;
            if (targetType == typeof(short)) return (short)0;
            if (targetType == typeof(decimal)) return 0m;
            if (targetType == typeof(bool)) return false;
            if (targetType == typeof(Guid)) return Guid.Empty;
            if (targetType == typeof(DateTimeOffset)) return DateTimeOffset.UtcNow;
            return Activator.CreateInstance(targetType);
        }

        var trimmed = rawValue.Trim();

        if (targetType == typeof(string))
            return rawValue;
        if (targetType == typeof(bool))
            return bool.TryParse(trimmed, out var b) && b;
        if (targetType == typeof(int))
            return int.Parse(trimmed, CultureInfo.InvariantCulture);
        if (targetType == typeof(long))
            return long.Parse(trimmed, CultureInfo.InvariantCulture);
        if (targetType == typeof(short))
            return short.Parse(trimmed, CultureInfo.InvariantCulture);
        if (targetType == typeof(decimal))
            return decimal.Parse(trimmed, CultureInfo.InvariantCulture);
        if (targetType == typeof(Guid))
            return Guid.Parse(trimmed);
        if (targetType == typeof(DateTimeOffset))
        {
            if (DateTime.TryParseExact(
                    trimmed,
                    new[] { "yyyy-MM-ddTHH:mm", "yyyy-MM-ddTHH:mm:ss" },
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                    out var exactDt))
            {
                return new DateTimeOffset(exactDt, TimeSpan.Zero);
            }
            var parsed = DateTimeOffset.Parse(trimmed, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
            return parsed.ToUniversalTime();
        }
        if (targetType == typeof(DateTime))
        {
            return DateTime.Parse(trimmed, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
        }

        return Convert.ChangeType(trimmed, targetType, CultureInfo.InvariantCulture);
    }

    private static void SetPropertyValue(Type clrType, object target, string propertyName, object? value)
    {
        var prop = clrType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        if (prop is not null && prop.CanWrite)
        {
            prop.SetValue(target, value);
        }
    }

    private Guid? GetCurrentAdminId()
    {
        var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(idClaim, out var guid) ? guid : null;
    }

    public sealed class FieldSchemaViewModel
    {
        public string Name { get; init; } = string.Empty;
        public string TypeName { get; init; } = string.Empty;
        public bool IsNullable { get; init; }
        public bool IsPrimaryKey { get; init; }
        public bool IsSystemField { get; init; }
        public string InputKind { get; init; } = "text";
    }

    public sealed class RowViewModel
    {
        public Guid Pk { get; init; }
        public Dictionary<string, CellValueViewModel> Cells { get; init; } = new();
    }

    public sealed class CellValueViewModel
    {
        public bool IsBoolean { get; init; }
        public bool BoolValue { get; init; }
        public string DisplayText { get; init; } = "-";
    }
}
