# QrGenerator

Приложение для генерации QR-кодов по данным заезда: авторизация по телефону, хранение в PostgreSQL, просмотр истории, отправка в Telegram.

## Запуск через Docker Compose

### Требования

- Docker и Docker Compose
- (Опционально) Токен Telegram-бота — для отправки QR в чат пользователя

### 1. Секреты и переменные окружения

Создайте в корне репозитория файл `.env` (по образцу `.env.example`):

```bash
cp .env.example .env
```

Пример `.env` для локального запуска:

```
POSTGRES_USER=app
POSTGRES_PASSWORD=app
JWT_SECRET=MyLocalSecretKeyAtLeast32CharactersLong
JWT_ISSUER=QrGenerator
JWT_AUDIENCE=QrGeneratorClient
TELEGRAM_BOT_TOKEN=Ваш токен
TELEGRAM_BOT_USERNAME=Имя бота без @
```

Пример `.env` с Telegram:

```
TELEGRAM_BOT_TOKEN=1234567890:AAHxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
TELEGRAM_BOT_USERNAME=verycool_bot
```

Остальные переменные можно не указывать — подставятся значения по умолчанию.

**Куда класть:** файл `.env` только в корне репозитория (рядом с `docker-compose.yml`). В `.gitignore` уже есть `.env` — в репозиторий он не попадёт.

### 2. Запуск

В корне репозитория выполните:

```bash
docker compose up --build
```

Дождитесь сообщения о том, что приложение слушает порт 8080.

### 3. Проверка

- **Веб-интерфейс:** http://localhost:8080  
  Страница входа по номеру телефона, форма создания QR, история, просмотр QR.
- **Swagger:** http://localhost:8080/swagger  
  Документация и вызов API.

### 4. Остановка

```bash
docker compose down
```

Данные БД сохраняются в volume `qr_pgdata`. Чтобы удалить и их:

```bash
docker compose down -v
```

---

## Запуск без Docker (если так удобнее)

### Требования

- .NET 9 SDK
- PostgreSQL (доступен на localhost, создана база и пользователь)

### 1. База данных

Создайте БД и пользователя (например, в `psql`):

```sql
CREATE USER app WITH PASSWORD 'app';
CREATE DATABASE "QrGenerator" OWNER app;
```

### 2. Секреты в appsettings

В `QrGenerator.Api/appsettings.Development.json` или в переменных окружения задайте:

- `ConnectionStrings__DefaultConnection` — строка подключения к PostgreSQL (Host=localhost;Port=5432;Database=QrGenerator;Username=app;Password=app).
- `Jwt__Secret` — секрет для JWT (длинная строка).
- `Jwt__Issuer`, `Jwt__Audience` — при необходимости.
- `Telegram__BotToken` — если нужна отправка в Telegram.
- `Telegram__BotUsername`

Либо создайте `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=QrGenerator;Username=app;Password=app"
  },
  "Jwt": {
    "Secret": "YourDevelopmentSecretKey",
    "Issuer": "QrGenerator",
    "Audience": "QrGeneratorClient",
    "ExpirationMinutes": 30
  },
  "Telegram": {
    "BotToken": "",
    "BotUsername": ""
  }
}
```

### 3. Запуск

```bash
cd QrGenerator.Api
dotnet run
```

Миграции применяются при старте. Откройте http://localhost:5109 (или порт из launchSettings) и http://localhost:5109/swagger.

---

## API 

- **POST /api/auth/login** — вход по номеру телефона. Тело: `{ "phoneNumber": "+79991234567" }`. Ответ: `{ "accessToken": "...", "tokenType": "Bearer" }`.
- **POST /api/qr** — создание QR (данные заезда). Требуется заголовок `Authorization: Bearer <token>`.
- **GET /api/qr/{id}** — просмотр QR по id.
- **GET /api/qr/history** — история QR текущего пользователя.
- **POST /api/qr/{id}/send-telegram** — отправить QR в Telegram (нужна привязка).
- **GET /api/telegram/link-url** — (с JWT) получить ссылку для привязки Telegram. Пользователь открывает ссылку и нажимает Start в боте — привязка в одно действие.
  
Привязка Telegram: на сайте нажать «Привязать Telegram» → открыть ссылку → в чате бота нажать **Start** . После этого при создании нового QR он автоматически будет отправлен в этот чат, а кнопка «В Telegram» в истории позволяет переотправить выбранный QR.

---