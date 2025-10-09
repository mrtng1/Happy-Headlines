# Happy Headlines - Development of Large Systems

## System Architecture

###  X-axis split

+ Creates 3 replicas of the ArticleService
+ Run with `docker-compose up` from project root directory

### Y-axis split

+ The database is split into 7 different region databases plus 1 global database

## Logging
+ Logging is done using Serilog - Serilog.AspNetCore
+ Logs are viewed using Seq

+ To Run Seq container manually without docker-compose use:
`docker run -d --name happyheadlines-seq -e ACCEPT_EULA=Y -e SEQ_FIRSTRUN_ADMINPASSWORD=guest -p 5341:80 datalust/seq:latest`


## Caching

prerequisite for Prometheus & Grafana startup:
`docker network create monitoring`

### Redis
+ To Run Redis container manually without docker-compose use:
`docker run -d --name happy-headlines-redis -p 6379:6379 redis:alpine`

### Prometheus
+ To Run Prometheus container manually without docker-compose use:
`docker run -d --name prometheus -p 9090:9090 --network monitoring -v "$(pwd)/prometheus.yml:/etc/prometheus/prometheus.yml" prom/prometheus`

+ query prometheus for `article_cache_hits_total` or `article_cache_misses_total`

Prometheus stores the metrics in a queryable database, from /metrics endpoint of each microservice.

### Grafana
`docker run -d --name grafana -p 3000:3000 --network monitoring grafana/grafana`
+ login with admin/admin

Grafana is used to visualize the metrics stored in Prometheus.

## General Setup

### Database setup
Create a `db.env` file in the solution root directory with the following content:

POSTGRES_USER=
POSTGRES_PASSWORD=

### Microservices Setup

Each microservice contains a readme.md file with instructions on how to run it individually.

## Running the system using docker compose

After filling out all the environment files run docker compose up from the solution root directory.
