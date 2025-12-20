# ListingWebApp

Маркетплейс для торговли внутриигровыми предметами: учетные записи с верификацией по email, профили пользователей с аватарами, инвентарь и лоты, кошельки и валюты, операции покупки/продажи, фоновые уведомления.

## Состав проекта
- `backend/ListingWebApp.Api` - ASP.NET Core 8 Web API, Swagger, авторизация/куки.
- `backend/ListingWebApp.Application` - бизнес-логика (аккаунты, профили, кошельки, валюты, предметы, листинги, сделки), доменные модели и DTO.
- `backend/ListingWebApp.Persistence.Postgres` - EF Core + PostgreSQL, миграции, конфигурации сущностей и репозитории.
- `backend/ListingWebApp.Infrastructure.*` - интеграции (JWT/криптография, Redis-кеш, SMTP-рассылка, MinIO/S3-хранилище).
- `backend/ListingWebApp.Application.Tests` - модульные тесты сервисов на фейковых репозиториях/инфраструктуре.
- `frontend/` - Vite + React + TypeScript + Tailwind + Axios: лендинг с пошаговой авторизацией/верификацией и дашборд с вызовами API.
- `devops/docker-compose.yml` - сервисы backend/frontend + Postgres, Redis, MinIO, pgAdmin.

## Требования
- Backend: .NET 8 SDK, PostgreSQL, Redis, MinIO/S3-совместимое хранилище, SMTP для отправки кодов.
- Frontend: Node.js 18+, npm.
- Для docker-окружения: Docker + Docker Compose.

## Быстрый старт
### В Docker
1) Создайте файл `.env` в директории devops с и заполните переменные (минимум):
```
POSTGRES_DB=listing
POSTGRES_USER=listing
POSTGRES_PASSWORD=listing
POSTGRES_CONNECTION_STRING=Host=postgres;Port=5432;Database=listing;Username=listing;Password=listing
REDIS_CONNECTION_STRING=redis:6379
REDIS_KEY_PREFIX=listing
REDIS_DB=0
JWT_SECRET_KEY=change_me_to_long_random_string
JWT_EXPIRES_MINUTES=15
EMAIL_VERIFICATION_TEMPLATE_FILE_PATH=ListingWebApp.Api/Resources/verification_template.html
EMAIL_SENDER_ADDRESS=no-reply@example.com
EMAIL_DISPLAY_NAME=ListingWebApp
EMAIL_SENDER_MAIL_PASSWORD=your_smtp_password
EMAIL_SMTP_HOST=smtp.example.com
EMAIL_SMTP_PORT=587
MINIO_REGION=us-east-1
S3_OPTIONS_ENDPOINT=minio:9000
S3_OPTIONS_ACCESSKEY=minio
S3_OPTIONS_SECRETKEY=minio123
S3_OPTIONS_BUCKET=listing-media
S3_OPTIONS_USESSL=false
PGADMIN_EMAIL=admin@example.com
PGADMIN_PASSWORD=admin
MINIO_ROOT_USER=minio
MINIO_ROOT_PASSWORD=minio123
```
2) Запустите `docker compose -f devops/docker-compose.yml up --build`. Backend будет доступен на `http://localhost:8080`, frontend - `http://localhost:5173`, Swagger - `http://localhost:8080/swagger`.

### Локально
Backend:
1) Настройте `backend/ListingWebApp.Api/appsettings.json` или переменные окружения:
   - `ConnectionStrings__Postgres` - строка к Postgres.
   - `RedisOptions__ConnectionString`, `KeyPrefix`, `Database` - доступ к Redis.
   - `JwtOptions__SecretKey` (длинная строка) и `JwtOptions__ExpiresMinutes`.
   - `EmailServiceOptions__*` - SMTP и путь к шаблону письма `Resources/verification_template.html`.
   - `S3Options__*` - параметры MinIO/S3 (endpoint, ключи, bucket, UseSsl).
2) В dev-среде применяются миграции автоматически (`ASPNETCORE_ENVIRONMENT=Development`).
3) Запустите: `dotnet restore` и `dotnet run --project backend/ListingWebApp.Api/ListingWebApp.Api.csproj`.

Frontend:
1) В каталоге `frontend/` выполните `npm install`.
2) Создайте `.env` (или экспортируйте переменные):
```
VITE_API_BASE_URL=http://localhost:8080/api
VITE_API_PROXY_TARGET=http://localhost:8080
```
3) Запустите `npm run dev` (порт 5173) либо `npm run build && npm run preview`.

## Основные возможности API
- **Auth**: регистрация/логин с refresh+access токенами, обновление токена, logout, подтверждение email.
- **Accounts**: получение/удаление аккаунта, смена статуса администратором.
- **Profiles**: CRUD профилей, загрузка/получение URL аватара (MinIO/S3).
- **Currencies**: справочник валют с иконками, включение/ограничения переводов.
- **Wallets**: создание кошельков на аккаунт и валюту, кредит/дебет, upsert, получение баланса.
- **Items & Item entries**: создание типов предметов, привязка экземпляров к владельцам.
- **Listings & Trading**: публикация лотов, смена статусов, покупка с переводом баланса, закрытием лота и записью транзакции.
- **Trade transactions**: получение совершенных сделок.

## Тесты
- Модульные тесты backend: `dotnet test backend/ListingWebApp.Application.Tests/ListingWebApp.Application.Tests.csproj`.
- Frontend тестов нет.

## БЕЗОПАСНОСТЬ
- **Хранение секретов**: пароли хэшируются PBKDF2 HMACSHA512 со 100000 итерациями и случайной солью (`Infrastructure.Security/CryptographyService.cs`). Refresh-токены хэшируются SHA-256 перед сохранением в БД, что снижает риск утечки сырых токенов.
- **Валидация учетных данных**: пароли проверяются на сложность регуляркой (`Common/Constants/RegexPatterns.cs`), email - базовой regex-валидацией. Доменные фабрики моделей возвращают ошибки валидации при некорректных данных.
- **JWT и сессии**: access-токен содержит `accountId`, роль и `sessionId`, срок жизни задается `JwtOptions`. При валидации JWT дополнительно проверяется существование и активность сессии в БД (`Program.cs`, проверка через `ISessionsRepository`). ClockSkew = 0, роль берется из claim. Logout удаляет сессию, refresh ротирует пару токенов.
- **Refresh-токен**: сервер задает `refreshToken` в куку `SameSite=None; HttpOnly; Max-Age=30d` (HttpOnly форсируется `CookiePolicy`). Для dev включен `Secure=false`, в production его нужно включить. Хэш токена хранится в таблице `Sessions`.
- **Подтверждение email**: при регистрации генерируется код длиной 6, его хэш и соль кладутся в Redis с TTL 10 минут (`AuthService`, `CacheService`). Фоновой сервис читает очередь сообщений и отправляет письмо по шаблону (`VerificationMessageSenderBackgroundService` + `Infrastructure.Email`), статус аккаунта меняется на `Verified` только после успешной проверки.
- **Ролевая и объектная защита**: контроллеры помечены `[Authorize(Roles="User,Admin")]`, админские операции (создание справочников, смена статусов) ограничены ролью `Admin`. Операции с кошельками дополнительно сверяют `accountId` из JWT и пути запроса, исключая доступ к чужим кошелькам (`WalletsController`).
- **Клиентские токены**: фронт кладет access/refresh в `localStorage` и дублирует refresh в куку для совместимости; axios-интерсептор добавляет Bearer и пытается автообновить токен при 401 (`frontend/src/api/client.ts`). Авторизованные маршруты фронта защищены `ProtectedRoute` и стадией готовности (`useAuth`). 
- **CORS и куки**: CORS разрешен только для `http://localhost:5173`, включены `AllowCredentials` и `HttpOnlyPolicy.Always` для куки. SameSite=None позволяет работать с раздельными доменами фронта/бэка.
- **Логирование и аудит**: Serilog пишет в консоль и ротационный файл `logs/listing-*.log`; ошибки аутентификации/покупок/кошельков логируются в сервисах.
