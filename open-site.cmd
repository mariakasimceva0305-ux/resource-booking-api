@echo off
chcp 65001 >nul
REM Открыть сайт в браузере (вызывается после старта сервера)
set "URL=%~1"
if "%URL%"=="" set "URL=http://127.0.0.1:8000/"

echo Открываю: %URL%

REM Надёжнее, чем start "" "http://..." на некоторых системах
rundll32 url.dll,FileProtocolHandler "%URL%"
if errorlevel 1 (
  start "" "%URL%"
)

exit /b 0
