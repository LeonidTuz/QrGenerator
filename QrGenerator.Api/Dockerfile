FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["QrGenerator.sln", "."]
COPY ["QrGenerator.Api/QrGenerator.Api.csproj", "QrGenerator.Api/"]
COPY ["QrGenerator.Application/QrGenerator.Application.csproj", "QrGenerator.Application/"]
COPY ["QrGenerator.Domain/QrGenerator.Domain.csproj", "QrGenerator.Domain/"]
COPY ["QrGenerator.Infrastructure/QrGenerator.Infrastructure.csproj", "QrGenerator.Infrastructure/"]

RUN dotnet restore "QrGenerator.Api/QrGenerator.Api.csproj"

COPY . .

RUN dotnet publish "QrGenerator.Api/QrGenerator.Api.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "QrGenerator.Api.dll"]