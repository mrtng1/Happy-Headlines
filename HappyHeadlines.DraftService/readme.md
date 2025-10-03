# HappyHeadlines DraftService

## Setting up the environment
ConnectionStrings__DefaultConnection=Host=localhost;Port=5432;Database=HappyHeadlines_Drafts;Username=x;Password=

## Setting up the database
+ Create the database in your Postgres instance
    - HappyHeadlines_Drafts

+ Run: 
  + dotnet ef migrations add InitialCreate
  + dotnet ef database update