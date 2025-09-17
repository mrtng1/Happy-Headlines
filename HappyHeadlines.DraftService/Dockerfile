FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["HappyHeadlines.DraftService/HappyHeadlines.DraftService.csproj", "HappyHeadlines.DraftService/"]
RUN dotnet restore "HappyHeadlines.DraftService/HappyHeadlines.DraftService.csproj"
COPY . .
WORKDIR "/src/HappyHeadlines.DraftService"
RUN dotnet build "HappyHeadlines.DraftService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HappyHeadlines.DraftService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HappyHeadlines.DraftService.dll"]
