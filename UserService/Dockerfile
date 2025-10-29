FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["UserService/UserService.csproj", "UserService/"]
COPY ["Common/Common.csproj", "Common/"]
RUN dotnet restore "UserService/UserService.csproj"

COPY . .
WORKDIR "/src/UserService"
RUN dotnet publish "UserService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "UserService.dll"]
