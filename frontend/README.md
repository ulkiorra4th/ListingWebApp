# Listing Frontend (Vite + React + TS + Tailwind)

## Запуск
- `npm install`
- `.env` на основе `.env.example` (по умолчанию `VITE_API_BASE_URL=/api`, `VITE_API_PROXY_TARGET=http://localhost:8080`).
- Dev: `npm run dev` (порт 5173).
- Build: `npm run build` / `npm run preview`.
- В docker-compose задайте `VITE_API_PROXY_TARGET=http://backend:8080` и/или `VITE_API_BASE_URL=http://backend:8080/api`, чтобы фронт ходил в сервис `backend`.

## Архитектура
- `src/api` — axios-инстанс с интерсепторами и хранением токенов/refresh cookie.
- `src/services` — реальные вызовы API (auth, accounts, profiles с загрузкой аватара, currencies, wallets, items/item-entries, listings, trading, trade-transactions).
- `src/hooks` — работа с сервисами и состоянием (auth-степы, кошелек, предметы, лоты, транзакции, валюты, профили).
- `src/pages` — `LandingPage` (пошаговая авторизация: аккаунт → верификация → профиль → опциональная загрузка аватара), `DashboardPage` (формы для реальных операций без моков).
- `src/components` — UI и layout.

## Особенности
- Моки убраны: все данные идут в бэкенд. Убедитесь, что API поднят и токены/роли корректны.
- После создания профиля доступен опциональный шаг загрузки аватара (`PATCH /api/v1/accounts/{accountId}/profiles/{profileId}/icon`).
- Дашборд работает в парадигме точечных операций (получить сущность по Id, создать entry/listing, купить listing, кредит/дебет кошелька, получить транзакцию).
