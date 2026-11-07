using System;

namespace Somtumzing.Backend.Api.Models;

/// <summary>
/// โมเดลเก็บสถานะภารกิจและความคืบหน้าของผู้เล่น (Player Missions)
/// </summary>
public sealed class PlayerMission : IGameDataEntity
{
    // บรรทัดที่ 12: กำหนด Primary Key (PK) ตามโครงสร้าง IGameDataEntity
    public Guid PK { get; set; }
    
    // บรรทัดที่ 14: รหัสโค้ดประจำตารางสำหรับระบบ CodeCounters
    public string Code { get; set; } = string.Empty;
    
    // บรรทัดที่ 17: วันเวลาที่สร้างข้อมูล
    public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;
    
    // บรรทัดที่ 20: วันเวลาที่อัปเดตข้อมูลล่าสุด
    public DateTimeOffset UpdatedDate { get; set; } = DateTimeOffset.UtcNow;
    
    // บรรทัดที่ 23: สถานะข้อมูล (ACTIVE, etc.)
    public string Status { get; set; } = "ACTIVE";
    
    // บรรทัดที่ 26: ผู้สร้างและผู้แก้ไขรายการ
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }

    // บรรทัดที่ 30: Foreign Key เชื่อมโยงไปยังผู้เล่น (Users.Id)
    public Guid PlayerPK { get; set; }
    
    // บรรทัดที่ 33: คีย์ระบุประเภทภารกิจ (เช่น "serve-10")
    public string MissionKey { get; set; } = string.Empty;
    
    // บรรทัดที่ 36: ความคืบหน้าปัจจุบันของภารกิจ
    public int Progress { get; set; } = 0;
    
    // บรรทัดที่ 39: เป้าหมายที่ต้องทำให้สำเร็จ
    public int Target { get; set; } = 10;
    
    // บรรทัดที่ 42: สถานะการกดรับรางวัล
    public bool Claimed { get; set; } = false;

    // บรรทัดที่ 45: Navigation Property สำหรับความสัมพันธ์กับ ApplicationUser
    public ApplicationUser? User { get; set; }
}