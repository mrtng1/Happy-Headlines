# Happy Headlines - Development of Large Systems

## X-axis split

Creates 3 replicas of the ArticleService
Run with `docker-compose up` from project root directory

## Y-axis split

The database is split into 7 different region databases plus 1 global database

## DB Migrations
1. `dotnet ef migrations add InitialCreate --project HappyHeadlines.Infrastructure --startup-project HappyHeadlines.ArticleService`
2. The rest of the migrations get applied at project startup
