FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["HappyHeadlines.WebApi/HappyHeadlines.WebApi.csproj", "HappyHeadlines.WebApi/"]
COPY ["HappyHeadlines.Core/HappyHeadlines.Core.csproj", "HappyHeadlines.Core/"]
RUN dotnet restore "HappyHeadlines.WebApi/HappyHeadlines.WebApi.csproj"
COPY . .
WORKDIR "/src/HappyHeadlines.WebApi"
RUN dotnet build "HappyHeadlines.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HappyHeadlines.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HappyHeadlines.WebApi.dll"]
