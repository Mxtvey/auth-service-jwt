# ExpenseFlow

Мини-приложение для учёта личных трат на ASP.NET Core и PostgreSQL.

## Что умеет

- регистрация и логин по JWT
- просмотр текущего пользователя
- создание, редактирование, удаление и просмотр списка трат
- минимальный фронт на чистом HTML/CSS/JS
- раздельные страницы доступа и работы с покупками

## Стек

- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- JWT Bearer Authentication
- HTML / CSS / JavaScript

## Страницы

- `/login.html` - вход и регистрация
- `/expenses.html` - список трат и форма редактирования

## API

- `POST /auth/register`
- `POST /auth/login`
- `GET /auth/me`
- `GET /expenses`
- `POST /expenses`
- `PUT /expenses/{expenseId}`
- `DELETE /expenses/{expenseId}`

## Как запустить

1. Создайте PostgreSQL базу данных.
2. Проверьте настройки в `appsettings.Local.json`.
3. Примените миграции.
4. Запустите проект.

Пример команд:

```bash
dotnet ef database update
dotnet run
```

После запуска откройте:

- `http://localhost:5190/login.html`

## Конфигурация

В репозитории:

- `appsettings.json` содержит безопасные шаблонные значения
- `appsettings.Local.json` используется только локально и добавлен в `.gitignore`

Нужно настроить:

- `ConnectionStrings:DefaultConnection`
- `Jwt:Issuer`
- `Jwt:Audience`
- `Jwt:Key`
- `Jwt:LifetimeMinutes`

## Что можно улучшить дальше

- фильтры по категории и периоду
- дашборд с месячной статистикой
- категории как отдельная таблица
- тесты на сервисы и контроллеры
- refresh token и более полный auth flow

## Примечания

- новые пароли хранятся в виде хэша
- старые plaintext-пароли из локальной учебной базы автоматически мигрируют на хэш после успешного логина
