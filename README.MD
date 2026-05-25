# OrderHub


## English

### Description

OrderHub is an application with a client and a backend service.

The project was implemented as my work on a test technical assignment for a Fullstack Developer position. According to the assignment, the application should track customer orders and show daily customer payment totals for a selected period.

The project is started with Docker Compose and consists of:

- `client` — Angular client application;
- `backend` — ASP.NET Core backend application;
- `postgres` — PostgreSQL database.

### Project structure

```text
root/
  docker-compose.yml
  docker-compose.tests.yml

  backend/
    Dockerfile
    Dockerfile.Tests

  client/
    Dockerfile
```

### Requirements

Docker must be installed.

You can check it with:

```bash
docker --version
docker compose version
```

### Environment variables

Main variables can be provided through a `.env` file in the project root.

Example `.env`:

```env
POSTGRES_USER=admin
POSTGRES_PASSWORD=123456
POSTGRES_DB=postgres

DB_PORT=5432
BACKEND_PORT=5008
CLIENT_PORT=4200

ASPNETCORE_ENVIRONMENT=Development
NG_APP_API_URL=http://localhost:5008
```

If the `.env` file is not created, default values from `docker-compose.yml` will be used.

### Running the application

Run from the project root:

```bash
docker compose up --build
```

After startup, the services will be available at:

```text
Client: http://localhost:4200
Backend: http://localhost:5008
Swagger: http://localhost:5008/swagger
PostgreSQL: localhost:5432
```

If a port is changed through `.env`, use the configured value.

### Running in detached mode

```bash
docker compose up --build -d
```

### Stopping containers

```bash
docker compose down
```

### Stopping containers and removing database data

```bash
docker compose down --volumes
```

This command removes the PostgreSQL volume, so database data will be lost.

### Rebuilding without cache

```bash
docker compose build --no-cache
docker compose up
```

### Running only the backend

```bash
docker compose up --build backend postgres
```

### Running only the client

```bash
docker compose up --build client
```

Before starting the client, the backend must be available at the address configured in `NG_APP_API_URL`.

### Running tests

Tests are started with a separate compose file:

```bash
docker compose -f docker-compose.tests.yml up --build --abort-on-container-exit --exit-code-from tests
```

After tests finish, test containers can be removed with:

```bash
docker compose -f docker-compose.tests.yml down --volumes
```

### Full cleanup of project Docker environment

```bash
docker compose down --volumes --remove-orphans
docker compose -f docker-compose.tests.yml down --volumes --remove-orphans
```

### Important notes

The Angular client receives `NG_APP_API_URL` during image build. If the backend URL is changed, the client image must be rebuilt:

```bash
docker compose build --no-cache client
docker compose up
```

---

## Русский

### Описание

OrderHub — приложение с клиентской и серверной частью.

Проект выполнен как моя работа над тестовым техническим заданием для позиции Fullstack-разработчика. По условиям задания нужно реализовать приложение для отслеживания заказов клиентов и просмотра поденных сумм платежей клиента за выбранный период.

Проект запускается через Docker Compose и состоит из:

- `client` — клиентское приложение Angular;
- `backend` — серверное приложение ASP.NET Core;
- `postgres` — база данных PostgreSQL.

### Структура проекта

```text
root/
  docker-compose.yml
  docker-compose.tests.yml

  backend/
    Dockerfile
    Dockerfile.Tests

  client/
    Dockerfile
```

### Требования

Для запуска нужен установленный Docker.

Проверить установку можно командами:

```bash
docker --version
docker compose version
```

### Переменные окружения

Основные переменные можно передать через `.env` файл в корне проекта.

Пример `.env`:

```env
POSTGRES_USER=admin
POSTGRES_PASSWORD=123456
POSTGRES_DB=postgres

DB_PORT=5432
BACKEND_PORT=5008
CLIENT_PORT=4200

ASPNETCORE_ENVIRONMENT=Development
NG_APP_API_URL=http://localhost:5008
```

Если `.env` файл не создан, будут использованы значения по умолчанию из `docker-compose.yml`.

### Запуск приложения

Из корня проекта выполнить:

```bash
docker compose up --build
```

После запуска будут доступны:

```text
Клиент: http://localhost:4200
Сервер: http://localhost:5008
Swagger: http://localhost:5008/swagger
PostgreSQL: localhost:5432
```

Если порт изменён через `.env`, нужно использовать указанное там значение.

### Запуск в фоне

```bash
docker compose up --build -d
```

### Остановка контейнеров

```bash
docker compose down
```

### Остановка с удалением данных базы

```bash
docker compose down --volumes
```

Эта команда удалит том PostgreSQL, поэтому данные базы будут потеряны.

### Пересборка без кэша

```bash
docker compose build --no-cache
docker compose up
```

### Запуск только серверной части

```bash
docker compose up --build backend postgres
```

### Запуск только клиентской части

```bash
docker compose up --build client
```

Перед запуском клиента серверная часть должна быть доступна по адресу, указанному в `NG_APP_API_URL`.

### Запуск тестов

Для запуска тестов используется отдельный compose-файл:

```bash
docker compose -f docker-compose.tests.yml up --build --abort-on-container-exit --exit-code-from tests
```

После завершения тестов можно удалить тестовые контейнеры:

```bash
docker compose -f docker-compose.tests.yml down --volumes
```

### Полная очистка Docker-окружения проекта

```bash
docker compose down --volumes --remove-orphans
docker compose -f docker-compose.tests.yml down --volumes --remove-orphans
```

### Важные замечания

Клиентское приложение Angular получает значение `NG_APP_API_URL` на этапе сборки образа. Поэтому если адрес сервера был изменён, клиентский образ нужно пересобрать:

```bash
docker compose build --no-cache client
docker compose up
```