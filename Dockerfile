# ใช้ .NET SDK สำหรับ Build โค้ด
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# คัดลอกไฟล์ .csproj มาเฉพาะโฟลเดอร์ที่จำเป็น
COPY ["src/Somtumzing.Backend.Api/Somtumzing.Backend.Api.csproj", "src/Somtumzing.Backend.Api/"]
RUN dotnet restore "src/Somtumzing.Backend.Api/Somtumzing.Backend.Api.csproj"

# คัดลอกไฟล์ทั้งหมดและเข้าไป Publish ในโฟลเดอร์โปรเจกต์
COPY . .
WORKDIR "/src/src/Somtumzing.Backend.Api"
RUN dotnet publish "Somtumzing.Backend.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ใช้ .NET Runtime สำหรับรันแอพ
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Somtumzing.Backend.Api.dll"]