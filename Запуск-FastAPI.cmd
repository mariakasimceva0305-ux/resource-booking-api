@echo off
chcp 65001 >nul
set "ROOT=%~dp0"
set "PYAPI=%ROOT%fastapi-booking"

where python >nul 2>&1
if errorlevel 1 (
  echo Python не найден в PATH. Установите с python.org ^(включите «Add to PATH»^).
  pause
  exit /b 1
)

echo Первый запуск может долго ставить пакеты — смотрите окно сервера.
start "Booking API (Python)" cmd /k "%PYAPI%\run-server.cmd"

echo Жду порт 8000 ^(до 90 сек^)...
powershell -NoProfile -ExecutionPolicy Bypass -Command ^
  "$ok=$false; for($i=0;$i -lt 45;$i++) { try { $t=New-Object Net.Sockets.TcpClient; $t.Connect('127.0.0.1',8000); if($t.Connected){ $t.Close(); $ok=$true; break } } catch {} try{ $t.Dispose() } catch {}; Start-Sleep -Seconds 2 }; if(-not $ok){ Write-Host 'Сервер не ответил за 90 сек — смотрите окно с ошибкой.' }"

call "%ROOT%open-site.cmd" "http://127.0.0.1:8000/"
echo Ссылка вручную: http://127.0.0.1:8000/
echo Окно с сервером не закрывайте.
pause
