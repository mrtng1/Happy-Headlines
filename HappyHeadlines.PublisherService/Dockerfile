FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["HappyHeadlines.PublisherService/HappyHeadlines.PublisherService.csproj", "HappyHeadlines.PublisherService/"]
RUN dotnet restore "HappyHeadlines.PublisherService/HappyHeadlines.PublisherService.csproj"
COPY . .
WORKDIR "/src/HappyHeadlines.PublisherService"
RUN dotnet build "HappyHeadlines.PublisherService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HappyHeadlines.PublisherService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HappyHeadlines.PublisherService.dll"]
