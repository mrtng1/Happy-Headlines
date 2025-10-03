# HappyHeadlines CommentService

## Setting up the environment
+ add an .env file in the HappyHeadlines.CommentService directory withe the following values filled out:

ConnectionStrings__DefaultConnection=Host=localhost;Port=5432;Database=HappyHeadlines_Comments;Username=x;Password=
PROFANITY_SERVICE_URL=

## Setting up the database
+ Create the database in your Postgres instance
    - HappyHeadlines_Comments

+ Run: 
  + dotnet ef migrations add InitialCreate
  + dotnet ef database update