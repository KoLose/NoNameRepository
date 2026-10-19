# Контекст проекта

Дата сохранения: 19 октября 2026 года.

## Цель проекта

MVP «Наставник по soft skills» на Blazor WebAssembly + ASP.NET Core Web API.

Основные направления:

- мини-курсы по soft skills и тайм-менеджменту;
- напоминания;
- мероприятия и лайфхаки (планируются);
- общение студентов и наставников;
- роли: пользователь/студент, наставник, администратор.

На текущем этапе реализуется регистрация и вход в Blazor-клиенте. Дизайн страниц намеренно минимальный: визуальную часть будет делать другой участник команды.

## Структура решения

- `BlazorApp` — Blazor WebAssembly клиент.
- `WorldSkillsAPI` — ASP.NET Core Web API.
- `WorldSkillsAPI.sln` — solution.
- PostgreSQL: база `WorldSkillsDB`, локальный порт `5432`.

Локальные адреса профилей HTTPS:

- Blazor: `https://localhost:7190`;
- API: `https://localhost:7244`;
- Swagger API: `https://localhost:7244/swagger`.

## Что сделано на клиенте

### Вход

Файл: `BlazorApp/Pages/Login.razor`.

- Маршрут `/login`.
- Поля логина и пароля.
- Вызов `ApiRequest.AuthUser`.
- Вывод сообщения API при ошибке.
- При успехе токен сохраняется через `AuthService`, затем выполняется переход на `/`.

### Регистрация

Файл: `BlazorApp/Pages/Register.razor`.

- Маршрут `/register`.
- Поля имени, логина, пароля и подтверждения пароля.
- Проверка заполнения полей, длины пароля и совпадения паролей.
- Вызов `ApiRequest.RegUser`.
- После успешной регистрации выполняется переход на `/login`.

### Состояние авторизации

Файл: `BlazorApp/Services/AuthService.cs`.

- Токен сохраняется в `localStorage` под ключом `authToken`.
- Сервис предоставляет `Token`, `IsLoggedIn`, `InitializeAsync`, `LoginAsync` и `LogoutAsync`.
- `AuthService` зарегистрирован как scoped-сервис в `BlazorApp/Program.cs`.
- `MainLayout.razor` восстанавливает токен из `localStorage` после первого рендера.

### ApiRequest

Файл: `BlazorApp/ApiRequest/ApiRequest.cs`.

Из `ReadAsync` удалён `EnsureSuccessStatusCode`, чтобы клиент мог прочитать JSON с полями `status` и `message` из ответов API с кодами 400/404.

### Навигация

В `BlazorApp/Layout/NavMenu.razor` добавлены ссылки на вход и регистрацию. Удалена ссылка на уже удалённую страницу Counter.

## Изменения API

В `WorldSkillsAPI/Program.cs` добавлена CORS-политика для локального Blazor-клиента:

- `https://localhost:7190`;
- `http://localhost:5263`.

Без неё браузер блокирует запросы клиента к API на другом порту.

Текущий API сохраняет и сравнивает пароли в открытом виде. Для MVP тестовые данные соответствуют этому поведению, но перед реальным использованием необходимо добавить хеширование паролей.

## Тестовые данные

Создан повторно запускаемый PostgreSQL-скрипт:

`WorldSkillsAPI/Database/seed.sql`

Он добавляет:

- роли и справочники;
- наставников, студентов и администратора;
- 4 мини-курса;
- 10 уроков;
- практические задания;
- избранные курсы;
- прогресс;
- напоминания;
- сообщения в обсуждениях;
- пример ответа студента и оценки наставника.

Запуск из корня репозитория:

```bash
dotnet ef database update --project WorldSkillsAPI
psql -h localhost -p 5432 -U postgres -d WorldSkillsDB \
  -f WorldSkillsAPI/Database/seed.sql
```

Запуск из папки `WorldSkillsAPI`:

```bash
dotnet ef database update
psql -h localhost -p 5432 -U postgres -d WorldSkillsDB \
  -f Database/seed.sql
```

Тестовые аккаунты:

| Роль | Логин | Пароль |
|---|---|---|
| Наставник | `mentor@skillup.ru` | `mentor123` |
| Наставник | `coach@skillup.ru` | `mentor123` |
| Студент | `anna@student.ru` | `student123` |
| Студент | `ivan@student.ru` | `student123` |
| Студент | `olga@student.ru` | `student123` |
| Администратор | `admin@skillup.ru` | `admin123` |

## Запуск проекта

Нужно одновременно запустить `WorldSkillsAPI` и `BlazorApp` с HTTPS-профилями.

На macOS Rider может показать ошибку:

`Default system browser option is not supported for Blazor apps. Please select a Chromium-based browser.`

Решение: в `Run → Edit Configurations` для `BlazorApp` выбрать Google Chrome, Microsoft Edge или другой Chromium-браузер. Альтернатива — установить `launchBrowser: false` в `BlazorApp/Properties/launchSettings.json` и открыть `https://localhost:7190` вручную.

## Проверки

- `WorldSkillsAPI` собирается без ошибок; в проекте есть существующие nullable-предупреждения.
- Цель компиляции Blazor (`dotnet msbuild ... -t:Compile`) проходит успешно.
- Полный `dotnet build BlazorApp/BlazorApp.csproj` в текущем окружении зависал без сообщения об ошибке после запуска процесса сборки.
- SQL-скрипт создан и проверен на отсутствие проблем форматирования, но база автоматически не изменялась.

## Git и рабочее дерево

Последний известный коммит: `16d180d home page`.

В рабочем дереве есть незакоммиченные и частично staged изменения. Помимо авторизации и seed-скрипта присутствуют изменения пользователя, включая `Home.razor`, миграцию `Second`, `appsettings.json` и удаление `WeatherForecast.cs`. Их нельзя откатывать или перезаписывать без отдельного запроса.

Перед следующим коммитом нужно внимательно проверить `git status` и отделить нужные изменения.

## Возможные следующие шаги

1. Проверить регистрацию и вход при одновременно запущенных API, PostgreSQL и Blazor.
2. Добавить выход из аккаунта с вызовом `ApiRequest.Logout`.
3. Добавить страницу профиля через `ApiRequest.Profile`.
4. Ограничить приватные страницы для неавторизованных пользователей.
5. Подключить страницы списка курсов, уроков и прогресса.
6. Добавить API и клиентские страницы для напоминаний.
7. Перейти с открытых паролей на безопасное хеширование.
