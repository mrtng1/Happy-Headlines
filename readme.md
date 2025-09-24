# Happy Headlines - Development of Large Systems

## X-axis split

+ Creates 3 replicas of the ArticleService
+ Run with `docker-compose up` from project root directory

## Y-axis split

+ The database is split into 7 different region databases plus 1 global database

## Logging
+ Logging is done using Serilog - Serilog.AspNetCore
+ Logs are viewed using Seq

+ To Run Seq container manually without docker-compose use:
`docker run -d --name happyheadlines-seq -e ACCEPT_EULA=Y -e SEQ_FIRSTRUN_ADMINPASSWORD=guest -p 5341:80 datalust/seq:latest`


# HappyHeadlines ArticleService

## Setting up the environment
+ add an .env file in the HappyHeadlines.ArticleService directory withe the following values filled out:
  ConnectionStrings__Global=Host=localhost;Database=ArticlesGlobal;Username=x;Password=
  ConnectionStrings__Africa=Host=localhost;Database=ArticlesAfrica;Username=x;Password=
  ConnectionStrings__Antarctica=Host=localhost;Database=ArticlesAntarctica;Username=x;Password=
  ConnectionStrings__Asia=Host=localhost;Database=ArticlesAsia;Username=x;Password=
  ConnectionStrings__Europe=Host=localhost;Database=ArticlesEurope;Username=x;Password=
  ConnectionStrings__NorthAmerica=Host=localhost;Database=ArticlesNorthAmerica;Username=x;Password=
  ConnectionStrings__Australia=Host=localhost;Database=ArticlesAustralia;Username=x;Password=
  ConnectionStrings__SouthAmerica=Host=localhost;Database=ArticlesSouthAmerica;Username=x;Password=

## Setting up the database
+ Create the databases in your Postgres instance
  - ArticlesGlobal
  - ArticlesAfrica
  - ArticlesAntarctica
  - ArticlesAsia
  - ArticlesEurope
  - ArticlesNorthAmerica
  - ArticlesAustralia
  - ArticlesSouthAmerica

+ Run: dotnet ef migrations add InitialCreate
+ dotnet ef database update



# HappyHeadlines CommentService

## Setting up the environment
+ add an .env file in the HappyHeadlines.DraftService directory withe the following values filled out:

ConnectionStrings__DefaultConnection=Host=localhost;Port=5432;Database=HappyHeadlines_Comments;Username=x;Password=

## Setting up the database
+ Create the database in your Postgres instance
  - HappyHeadlines_Comments

+ Run: dotnet ef migrations add InitialCreate
+ dotnet ef database update

# HappyHeadlines DraftService

## Setting up the environment
ConnectionStrings__DefaultConnection=Host=localhost;Port=5432;Database=HappyHeadlines_Drafts;Username=x;Password=

## Setting up the database
+ Create the database in your Postgres instance
  - HappyHeadlines_Drafts

+ Run: dotnet ef migrations add InitialCreate
+ dotnet ef database update