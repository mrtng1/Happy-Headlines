FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["HappyHeadlines.WebApi/HappyHeadlines.ArticleService.csproj", "HappyHeadlines.ArticleService/"]
COPY ["HappyHeadlines.Core/HappyHeadlines.Core.csproj", "HappyHeadlines.Core/"]
RUN dotnet restore "HappyHeadlines.ArticleService/HappyHeadlines.ArticleService.csproj"
COPY . .
WORKDIR "/src/HappyHeadlines.WebApi"
RUN dotnet build "HappyHeadlines.ArticleService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HappyHeadlines.ArticleService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HappyHeadlines.ArticleService.dll"]
