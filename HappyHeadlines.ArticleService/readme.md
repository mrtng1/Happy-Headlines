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

+ Run: 
  + dotnet ef migrations add InitialCreate
  + dotnet ef database update