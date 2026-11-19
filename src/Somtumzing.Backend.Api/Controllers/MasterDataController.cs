using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Somtumzing.Backend.Api.Data;
using Somtumzing.Backend.Api.Models;
using System.Reflection;

namespace Somtumzing.Backend.Api.Controllers;

[Authorize(Roles = "Admin")]
[Route("api/[controller]")]
[ApiController]
public class MasterDataController : ControllerBase
{
    private readonly GameDbContext _context;

    // รายชื่อโมเดลที่เป็น Master Data ที่เราอนุญาตให้ทำการ CRUD ผ่านระบบ Dynamic นี้
    private static readonly Dictionary<string, Type> AllowedTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        { "GameEvent", typeof(GameEvent) },
        { "GameAchievement", typeof(GameAchievement) },
        { "Stage", typeof(Stage) },
        { "StageConfig", typeof(StageConfig) },
        { "Upgrade", typeof(Upgrade) },
        { "Mission", typeof(Mission) },
        { "SabotageCard", typeof(SabotageCard) },
        { "ChaosEventDefinition", typeof(ChaosEventDefinition) }
    };

    public MasterDataController(GameDbContext context)
    {
        _context = context;
    }

    // 1. ดึงรายการประเภทของ Master Data ทั้งหมด
    [HttpGet("types")]
    public IActionResult GetMasterTypes()
    {
        return Ok(AllowedTypes.Keys.Select(k => new { Name = k }));
    }

    // 2. ดึง Schema ของ Model ที่เลือก เพื่อไปวาดหน้าต่างฟอร์มกรอกข้อมูลอัตโนมัติบนหน้าเว็บ
    [HttpGet("schema/{entityName}")]
    public IActionResult GetSchema(string entityName)
    {
        if (!AllowedTypes.TryGetValue(entityName, out var type))
            return BadRequest("ไม่พบโมเดลประเภทนี้ในระบบ");

        var properties = type.GetProperties()
            .Select(p => new
            {
                Name = p.Name,
                Type = Nullable.GetUnderlyingType(p.PropertyType)?.Name ?? p.PropertyType.Name,
                IsNullable = p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>),
                IsPrimaryKey = p.Name == "PK",
                IsSystemField = new[] { "CreatedDate", "CreatedBy", "UpdatedDate", "UpdatedBy" }.Contains(p.Name)
            });

        return Ok(properties);
    }

    // 3. ดึงข้อมูลทั้งหมดของโมเดลที่เลือก
    [HttpGet("data/{entityName}")]
    public async Task<IActionResult> GetData(string entityName)
    {
        if (!AllowedTypes.TryGetValue(entityName, out var type))
            return BadRequest("ไม่พบโมเดลประเภทนี้ในระบบ");

        var dbSet = _context.GetType()
            .GetMethods()
            .First(m => m.Name == "Set" && m.IsGenericMethod)
            .MakeGenericMethod(type)
            .Invoke(_context, null);

        var listTask = typeof(EntityFrameworkQueryableExtensions)
            .GetMethod("ToListAsync")
            .MakeGenericMethod(type)
            .Invoke(null, new[] { dbSet, default(CancellationToken) });

        var list = await (dynamic)listTask;
        return Ok(list);
    }

    // 4. บันทึกเพิ่มข้อมูลใหม่ หรือแก้ไขข้อมูลเดิม
    [HttpPost("save/{entityName}")]
    public async Task<IActionResult> Save(string entityName, [FromBody] System.Text.Json.JsonElement payload)
    {
        if (!AllowedTypes.TryGetValue(entityName, out var type))
            return BadRequest("ไม่พบโมเดลประเภทนี้ในระบบ");

        var isNew = !payload.TryGetProperty("PK", out var pkProp) || pkProp.GetGuid() == Guid.Empty;
        object entity;

        if (isNew)
        {
            entity = Activator.CreateInstance(type)!;
            SetPropertyValue(entity, "PK", Guid.NewGuid());
            SetPropertyValue(entity, "CreatedDate", DateTimeOffset.UtcNow);
            SetPropertyValue(entity, "CreatedBy", GetCurrentAdminId());
        }
        else
        {
            var pk = pkProp.GetGuid();
            entity = await _context.FindAsync(type, pk);
            if (entity == null) return NotFound("ไม่พบข้อมูลชิ้นนี้ในระบบ");
        }

        SetPropertyValue(entity, "UpdatedDate", DateTimeOffset.UtcNow);
        SetPropertyValue(entity, "UpdatedBy", GetCurrentAdminId());

        // บันทึกฟิลด์อื่นๆ จาก UI ลงอ็อบเจกต์
        foreach (var prop in type.GetProperties())
        {
            if (prop.Name == "PK" || new[] { "CreatedDate", "CreatedBy", "UpdatedDate", "UpdatedBy" }.Contains(prop.Name))
                continue;

            if (payload.TryGetProperty(prop.Name, out var value))
            {
                var targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                object? convertedValue = null;

                if (value.ValueKind != System.Text.Json.JsonValueKind.Null)
                {
                    if (targetType == typeof(Guid)) convertedValue = Guid.Parse(value.GetString()!);
                    else if (targetType == typeof(int)) convertedValue = value.GetInt32();
                    else if (targetType == typeof(long)) convertedValue = value.GetInt64();
                    else if (targetType == typeof(decimal)) convertedValue = value.GetDecimal();
                    else if (targetType == typeof(bool)) convertedValue = value.GetBoolean();
                    else if (targetType == typeof(DateTimeOffset)) convertedValue = DateTimeOffset.Parse(value.GetString()!);
                    else convertedValue = value.GetString();
                }

                prop.SetValue(entity, convertedValue);
            }
        }

        if (isNew) _context.Add(entity);
        await _context.SaveChangesAsync();

        return Ok(new { Success = true, Message = "บันทึกข้อมูลเรียบร้อยแล้ว!" });
    }

    // 5. ลบข้อมูล Master Data
    [HttpDelete("delete/{entityName}/{pk}")]
    public async Task<IActionResult> Delete(string entityName, Guid pk)
    {
        if (!AllowedTypes.TryGetValue(entityName, out var type))
            return BadRequest("ไม่พบโมเดลประเภทนี้ในระบบ");

        var entity = await _context.FindAsync(type, pk);
        if (entity == null) return NotFound("ไม่พบข้อมูลชิ้นนี้ในระบบ");

        _context.Remove(entity);
        await _context.SaveChangesAsync();

        return Ok(new { Success = true, Message = "ลบข้อมูลสำเร็จแล้ว!" });
    }

    private void SetPropertyValue(object obj, string propName, object? value)
    {
        var prop = obj.GetType().GetProperty(propName);
        if (prop != null && prop.CanWrite) prop.SetValue(obj, value);
    }

    private Guid? GetCurrentAdminId()
    {
        var idClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(idClaim, out var guid) ? guid : null;
    }
}
