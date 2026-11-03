# 📋 Somtumzing.Backend - TODO List

รายการสิ่งที่ควรพัฒนาต่อเพื่อความสมบูรณ์และเชื่อมต่อฐานข้อมูลจริง (Supabase PostgreSQL):

## 🎮 Game Configuration (`GameConfigController.cs`)
- [ ] **Database Integration for Config:** สร้างตาราง `GameConfigs` ใน PostgreSQL เพื่อเก็บค่าการตั้งค่าเกม (เช่น `MaxEnergy`, `EnergyRegenSeconds`, `Version`, `MaintenanceMode`) แทนการใช้ค่า Hardcoded ในโค้ด
- [ ] **Admin Config API:** เพิ่ม Endpoint สำหรับให้ Admin สามารถแก้ไขค่า Config ของเกมได้แบบ Real-time

## 🛒 Shop & Upgrades (`ShopController.cs`)
- [ ] **Database Tables for Shop:** สร้างตาราง `ShopItems` (รายการอัปเกรดทั้งหมด เช่น ครกพิเศษ, สากไทเทเนียม) และตาราง `PlayerUpgrades` (บันทึกว่าผู้เล่นคนไหนซื้อเลเวลไหนไปแล้ว)
- [ ] **Purchase Upgrade API:** เพิ่ม Endpoint สำหรับให้ผู้เล่นกดซื้อ/อัปเกรดไอเทม (เช่น `POST /api/shop/upgrades/{id}/buy`) พร้อมระบบตัดเงิน/แต้มในเกม

## 🛡️ General Backend & Security
- [ ] **Rate Limiting:** ตรวจสอบและตั้งค่า Rate Limiting เพิ่มเติมสำหรับ Endpoint ที่มีความเสี่ยงถูก Spam (เช่น ร้านค้า หรือการอัปเดตโปรไฟล์)
- [ ] **Integration Tests:** เพิ่มการทดสอบระบบแบบ End-to-End (Integration Tests) ด้วย Testcontainers หรือ In-Memory DB ที่จำลองการทำธุรกรรมพร้อมกันหลาย ๆ คน (Concurrency)
