# ใช้ .NET SDK สำหรับ Build โค้ด
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# คัดลอกไฟล์ .csproj มาเฉพาะโฟลเดอร์ที่จำเป็น
COPY ["src/Somtamzing.Backend.Api/Somtamzing.Backend.Api.csproj", "src/Somtamzing.Backend.Api/"]
RUN dotnet restore "src/Somtamzing.Backend.Api/Somtamzing.Backend.Api.csproj"

# คัดลอกไฟล์ทั้งหมดและเข้าไป Publish ในโฟลเดอร์โปรเจกต์
COPY . .
WORKDIR "/src/src/Somtamzing.Backend.Api"
RUN dotnet publish "Somtamzing.Backend.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ใช้ .NET Runtime สำหรับรันแอพ
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Somtamzing.Backend.Api.dll"]