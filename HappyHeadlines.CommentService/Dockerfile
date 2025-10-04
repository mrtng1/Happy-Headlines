FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["HappyHeadlines.CommentService/HappyHeadlines.CommentService.csproj", "HappyHeadlines.CommentService/"]
RUN dotnet restore "HappyHeadlines.CommentService/HappyHeadlines.CommentService.csproj"
COPY . .
WORKDIR "/src/HappyHeadlines.CommentService"
RUN dotnet build "HappyHeadlines.CommentService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HappyHeadlines.CommentService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HappyHeadlines.CommentService.dll"]
