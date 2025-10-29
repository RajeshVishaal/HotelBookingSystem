FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["BookingService/BookingService.csproj", "BookingService/"]
COPY ["Common/Common.csproj", "Common/"]
RUN dotnet restore "BookingService/BookingService.csproj"
COPY . .
WORKDIR "/src/BookingService"
RUN dotnet publish "BookingService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "BookingService.dll"]
