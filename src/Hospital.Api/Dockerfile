# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 复制所有 csproj 并 restore（利用 layer 缓存）
COPY src/Hospital.Domain/Hospital.Domain.csproj Hospital.Domain/
COPY src/Hospital.Application/Hospital.Application.csproj Hospital.Application/
COPY src/Hospital.Infrastructure/Hospital.Infrastructure.csproj Hospital.Infrastructure/
COPY src/Hospital.Api/Hospital.Api.csproj Hospital.Api/
RUN dotnet restore Hospital.Api/Hospital.Api.csproj

# 复制源代码并 publish
COPY src/ .
RUN dotnet publish Hospital.Api/Hospital.Api.csproj -c Release -o /app

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

COPY --from=build /app .

USER app
ENTRYPOINT ["dotnet", "Hospital.Api.dll"]
