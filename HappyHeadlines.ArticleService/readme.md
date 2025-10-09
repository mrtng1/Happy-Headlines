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

  REDIS__URL=localhost:6379

  RABBITMQ_HOSTNAME=message-broker
  RABBITMQ_PORT=5672
  RABBITMQ_USERNAME=guest
  RABBITMQ_PASSWORD=guest
  RABBITMQ_QUEUE_NAME=ArticleQueue
  RABBITMQ_VIRTUAL_HOST=/
  RABBITMQ_PREFETCH_COUNT=1

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