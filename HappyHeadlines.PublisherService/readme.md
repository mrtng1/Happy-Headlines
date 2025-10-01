# Publisher Service

## Setup

Add an .env file to project directory in the following format

RABBITMQ_HOSTNAME=localhost
RABBITMQ_PORT=5672
RABBITMQ_USERNAME=guest
RABBITMQ_PASSWORD=guest
RABBITMQ_QUEUE_NAME=ArticleQueue
RABBITMQ_VIRTUAL_HOST=/

Make sure the RabbitMQ server is running and accessible with the provided credentials.