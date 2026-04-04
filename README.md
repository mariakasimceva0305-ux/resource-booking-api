# ЯЗИМЕП II — задание 1: сервис бронирования ресурсов

**Учебный веб-сервис:** бронирование переговорных и оборудования через **REST API** (роли User/Admin, JWT, SQLite, проверка пересечений по времени, лимит броней в день, группы доступа к ресурсам).

**Что внутри репозитория**

| Часть | Технологии | Для чего |
|--------|------------|----------|
| Основная демонстрация | **FastAPI** + HTML-интерфейс | Запуск без .NET: `Запуск.cmd` → сайт `http://127.0.0.1:8000/` |
| Вариант по методичке | **C# / ASP.NET Core** | Слои Controller → Service → Repository, Swagger |
| Документ с заданием | `язимеп 2.pdf` | Формулировка из методички |

Публичный репозиторий (**техническое имя:** `resource-booking-api`): [github.com/mariakasimceva0305-ux/resource-booking-api](https://github.com/mariakasimceva0305-ux/resource-booking-api)

---

## Как запустить (явно)

| Шаг | Действие |
|-----|----------|
| 1 | В корне проекта дважды щёлкните **`Запуск.cmd`**. |
| 2 | Не закрывайте окно, где появится **`Uvicorn running on http://127.0.0.1:8000`** (или окно `dotnet run` для C#). |
| 3 | В браузере откройте **http://127.0.0.1:8000/** — визуальный сайт (форма входа, ресурсы, брони). |
| 4 | Вход админа: **`admin@local`** / **`Admin123!`**. |

- Текстовая шпаргалка: файл **`КАК_ЗАПУСТИТЬ.txt`** в корне.
- Техническая документация API: **http://127.0.0.1:8000/docs**
- Если у вас установлен **.NET**, `Запуск.cmd` может поднять C# — тогда сайт: **https://localhost:7288/**

---

Две реализации одной логики:

| Вариант | Папка | Нужно установить |
|--------|--------|-------------------|
| **FastAPI (Python)** | `fastapi-booking/` | только **Python 3.10+** |
| C# / ASP.NET Core | `src/BookingService.Api` | [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) |

Если **не хотите ставить .NET**, используйте FastAPI: **`Запуск.cmd`** или **`Запуск-FastAPI.cmd`** — откроется **сайт с формами** (**http://127.0.0.1:8000/**): вход, ресурсы, брони. Документация для разработчиков: **/docs**. База: `fastapi-booking/booking.db`. Админ: `admin@local` / `Admin123!`.

Реализация под **ЯЗИМЕП II**: HTTP API, JWT, SQLite, слои **Router → Service → Repository**, схемы (DTO), пересечения по времени, лимит бронирований в день, роли User/Admin, группы доступа к ресурсам, документация OpenAPI.

При **пустой** таблице ресурсов при старте добавляются **примеры** (переговорные, проектор, зал, ноутбук и т.д.). Ресурс «Зал для презентаций» доступен только пользователям с группой **`staff`** (укажите при регистрации). Чтобы заполнить заново — удалите файл БД и перезапустите сервер.

---

## C# — требования

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

## Запуск

**Проще всего (Windows):** двойной щелчок по **`Запуск.cmd`** в корне папки `11`. Если установлен **.NET SDK**, поднимется C# API и откроется `https://localhost:7288/`. Если **.NET нет**, скрипт **сам запустит FastAPI** и откроет **визуальный сайт** **`http://127.0.0.1:8000/`**. Окно с сервером не закрывайте.

Или вручную:

```bash
cd src/BookingService.Api
dotnet run
```

В браузере: **`https://localhost:7288/`** (главная) или **`https://localhost:7288/swagger`** (порт см. в `Properties/launchSettings.json`).

После первого запуска создаётся файл `booking.db` в рабочей каталоге API и добавляется администратор:

- **Email:** `admin@local`
- **Пароль:** `Admin123!`

## Настройки (`appsettings.json`)

- `ConnectionStrings:Default` — строка SQLite.
- `Jwt:Key` — секрет **не короче 32 символов** (для продакшена вынести в переменные окружения).
- `BookingLimits:MaxBookingsPerDay` — максимум **активных** броней с датой начала в одном UTC-сутки на одного пользователя (по умолчанию `5`).

## API (кратко)

| Метод | Путь | Доступ |
|--------|------|--------|
| POST | `/api/auth/register` | все |
| POST | `/api/auth/login` | все |
| GET | `/api/resources` | JWT; список с учётом группы доступа |
| POST | `/api/resources` | JWT, роль **Admin** |
| GET | `/api/bookings/mine` | JWT |
| POST | `/api/bookings` | JWT |
| PUT | `/api/bookings/{id}` | JWT (своя бронь или Admin) |
| POST | `/api/bookings/{id}/cancel` | JWT (своя бронь или Admin) |
| GET | `/api/admin/stats` | JWT, роль **Admin** |

В Swagger нажмите **Authorize** и вставьте JWT (без префикса `Bearer`).

## Регистрация с группой доступа

В теле `register` можно передать `accessGroup`. Ресурс с полем `requiredAccessGroup` видят только пользователи с той же группой (и администратор видит все ресурсы в списке).

## Тесты

```bash
dotnet test
```

## Структура решения

- `BookingService.Domain` — сущности и пересечение интервалов.
- `BookingService.Application` — DTO, контракты репозиториев, сервисы, JWT.
- `BookingService.Infrastructure` — EF Core, репозитории, сид админа.
- `BookingService.Api` — контроллеры, middleware ошибок, DI, Swagger.

## Репозиторий на GitHub

- Техническое имя репозитория: **`resource-booking-api`**
- Адрес: **[github.com/mariakasimceva0305-ux/resource-booking-api](https://github.com/mariakasimceva0305-ux/resource-booking-api)**
- Текст для поля **Description** и теги **Topics** — в файле **`ОПИСАНИЕ_ДЛЯ_GITHUB.txt`** (скопируйте на страницу репозитория → **⚙ About**).
- Если репозиторий ещё назывался иначе: на GitHub **Settings → General → Repository name** → введите **`resource-booking-api`** и сохраните.

В Git **не** попадают (`.gitignore`): **`fastapi-booking/.venv`**, **`*.db`**, **`bin/obj`**.

Выгрузка или обновление:

```bash
cd "c:\Users\Мария\OneDrive\Desktop\11"
git remote add origin https://github.com/mariakasimceva0305-ux/resource-booking-api.git
# если remote уже есть: git remote set-url origin https://github.com/mariakasimceva0305-ux/resource-booking-api.git
git push -u origin main
```

Авторизация: [Personal Access Token](https://github.com/settings/tokens) или **GitHub Desktop**.

Файл **`язимеп 2.pdf`**: при проблемах с размером репозитория уберите из коммита или используйте [Git LFS](https://git-lfs.com/).
