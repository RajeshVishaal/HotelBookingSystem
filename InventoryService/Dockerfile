FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["InventoryService/InventoryService.csproj", "InventoryService/"]
COPY ["Common/Common.csproj", "Common/"]
RUN dotnet restore "InventoryService/InventoryService.csproj"

COPY . .
WORKDIR "/src/InventoryService"
RUN dotnet publish "InventoryService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "InventoryService.dll"]
