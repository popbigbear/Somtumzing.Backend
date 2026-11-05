# ใช้ .NET SDK สำหรับ Build โค้ด
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# คัดลอกไฟล์โปรเจกต์มาทำการ Restore แพ็กเกจ
COPY ["src/Somtamzing.Backend.Api/Somtamzing.Backend.Api.csproj", "src/Somtamzing.Backend.Api/"]
RUN dotnet restore "src/Somtamzing.Backend.Api/Somtamzing.Backend.Api.csproj"

# คัดลอกโค้ดทั้งหมดแล้วทำการ Publish
COPY . .
WORKDIR "/src/src/Somtamzing.Backend.Api"
RUN dotnet publish "Somtamzing.Backend.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ใช้ .NET Runtime สำหรับรันแอพ (ขนาดเล็กและเบา)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .

# กำหนดจุดเริ่มต้นรันโปรเจกต์ (เปลี่ยนชื่อ DLL ให้ตรงกับโปรเจกต์ของคุณ)
ENTRYPOINT ["dotnet", "Somtamzing.Backend.Api.dll"]