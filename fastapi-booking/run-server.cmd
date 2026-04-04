@echo off
chcp 65001 >nul
cd /d "%~dp0"

if not exist ".venv\" (
  echo Создаю .venv ...
  python -m venv .venv
)
call .venv\Scripts\activate.bat
echo Проверка зависимостей...
python -m pip install -q -r requirements.txt
echo Сайт: http://127.0.0.1:8000/   ^(Swagger: /docs^)
python -m uvicorn app.main:app --host 127.0.0.1 --port 8000
pause
