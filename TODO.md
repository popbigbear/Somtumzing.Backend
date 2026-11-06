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



==============================
ตรวจ OpenAPI ล่าสุดแล้วครับ Base URL คือ:

https://somtumzingbackend-production.up.railway.app

ชุดที่ 1: API ที่มีอยู่และนำขึ้นใช้งานได้
1. Health check
GET /api/v1/system/status
ใช้ตรวจว่า backend ทำงานอยู่

2. Guest login
POST /api/v1/auth/guest
Content-Type: application/json
จาก schema ปัจจุบัน endpoint นี้ไม่มี request body

Response:

{
  "playerId": "uuid",
  "accessToken": "string|null",
  "refreshToken": "string|null",
  "accessTokenExpiresAtUtc": "2026-11-06T12:00:00Z",
  "refreshTokenExpiresAtUtc": "2026-12-06T12:00:00Z"
}
3. Guest login แบบเก่า
POST /api/v1/auth/guestold
Content-Type: application/json
Request:

{
  "deviceId": "device-unique-id",
  "platform": "web"
}
Response:

{
  "token": "string|null",
  "player": {
    "id": "uuid",
    "username": "string|null",
    "createdAtUtc": "2026-11-06T12:00:00Z"
  }
}
แนะนำให้ใช้ /auth/guest เป็นตัวหลัก และใช้ /auth/guestold เฉพาะกรณีที่ frontend เดิมยังต้องรองรับ

4. Refresh token
POST /api/v1/auth/refresh
Content-Type: application/json
Request:

{
  "refreshToken": "อย่างน้อย 64 ตัวอักษร และไม่เกิน 128 ตัวอักษร"
}
Response: เหมือน AuthSessionResponse

5. Logout
POST /api/v1/auth/logout
Content-Type: application/json
Request:

{
  "refreshToken": "refresh-token"
}
Response: 200 OK ไม่มี body

6. อ่านข้อมูลผู้เล่นปัจจุบัน
GET /api/v1/auth/me
Authorization: Bearer {accessToken}
Response:

{
  "id": "uuid",
  "username": "string|null",
  "createdAtUtc": "2026-11-06T12:00:00Z"
}
7. แก้ชื่อผู้เล่น
PATCH /api/v1/auth/me
Authorization: Bearer {accessToken}
Content-Type: application/json
Request:

{
  "displayName": "ส้มตำซิ่ง"
}
หมายเหตุ: schema ใช้ชื่อ response ว่า PlayerResponse และ field ปัจจุบันคือ username แต่ request ใช้ displayName ควรยืนยันกับ backend ว่าต้องการใช้ชื่อใดเป็นมาตรฐาน

8. Game config
GET /api/v1/config
Authorization: Bearer {accessToken}
Response:

{
  "version": "1.0.0",
  "maxEnergy": 10,
  "energyRegenSeconds": 300,
  "dailyRewardEnabled": true,
  "maintenanceMode": false,
  "supportedPlatforms": ["web", "mobile"],
  "serverTimeUtc": "2026-11-06T12:00:00Z"
}
9. Shop upgrades
GET /api/v1/shop/upgrades
Authorization: Bearer {accessToken}
Response:

{
  "upgrades": [
    {
      "id": "string",
      "name": "string",
      "description": "string",
      "cost": 150,
      "level": 1,
      "maxLevel": 5,
      "isUnlocked": true
    }
  ]
}
Error format
API ระบุ error เป็น RFC 7807 Problem Details:

{
  "type": "string|null",
  "title": "string|null",
  "status": 401,
  "detail": "รายละเอียด error",
  "instance": "string|null"
}
สถานะที่ frontend ควรรองรับ:

401 token หมดอายุหรือไม่มีสิทธิ์ → เรียก refresh หรือ guest login ใหม่
403 ไม่มีสิทธิ์
400 request ผิดรูปแบบ
404 ไม่พบข้อมูล
409 ข้อมูลชนกันหรือทำรายการซ้ำ
429 เรียก API ถี่เกินไป
500 backend error
สิ่งที่ยังไม่มีใน Live API specification
ตอนนี้ Swagger ยัง ไม่มี endpoint สำหรับระบบเกมที่เราทำใน Mock API ได้แก่:

POST /runs
POST /runs/{runId}/finish
GET /leaderboard
POST /shop/upgrades/{upgradeId}/purchase
GET /players/me หรือข้อมูล coins/energy/upgrades แบบเต็ม
ดังนั้นก่อนเชื่อม frontend จริง จำเป็นต้องเพิ่ม API กลุ่มนี้ใน Backend หรือส่ง specification เพิ่มมาให้ผม โดยเฉพาะ start run, finish run, purchase upgrade และ leaderboard ครับ



