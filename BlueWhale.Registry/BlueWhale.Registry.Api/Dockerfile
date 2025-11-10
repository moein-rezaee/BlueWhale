FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["BlueWhale.sln", "./"]
COPY ["BlueWhale.Registry/BlueWhale.Registry.Api/BlueWhale.Registry.Api.csproj", "BlueWhale.Registry/BlueWhale.Registry.Api/"]
COPY ["BlueWhale.Registry/BlueWhale.Registry.Application/BlueWhale.Registry.Application.csproj", "BlueWhale.Registry/BlueWhale.Registry.Application/"]
COPY ["BlueWhale.Registry/BlueWhale.Registry.Domain/BlueWhale.Registry.Domain.csproj", "BlueWhale.Registry/BlueWhale.Registry.Domain/"]
COPY ["BlueWhale.Registry/BlueWhale.Registry.Infrastructure/BlueWhale.Registry.Infrastructure.csproj", "BlueWhale.Registry/BlueWhale.Registry.Infrastructure/"]

RUN dotnet restore "BlueWhale.sln"

COPY . .
RUN dotnet build "BlueWhale.sln" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BlueWhale.sln" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 5260
ENV ASPNETCORE_URLS=http://+:5260

ENTRYPOINT ["dotnet", "BlueWhale.Registry.Api.dll"]
