@echo off
chcp 65001 >nul
set "ROOT=%~dp0"
set "API=%ROOT%src\BookingService.Api"
set "PYAPI=%ROOT%fastapi-booking"

where dotnet >nul 2>&1
if not errorlevel 1 goto run_dotnet

where python >nul 2>&1
if errorlevel 1 (
  echo Не найден ни dotnet, ни python в PATH.
  echo Установите Python 3 с https://www.python.org/downloads/ ^(галочка «Add to PATH»^).
  pause
  exit /b 1
)

echo .NET не найден — запускаю FastAPI ^(Python^).
echo.
echo ВАЖНО: первый раз pip может ставить пакеты 1-3 минуты — дождитесь в окне сервера строки:
echo   Uvicorn running on http://127.0.0.1:8000
echo.
start "Booking API (Python)" cmd /k "%PYAPI%\run-server.cmd"

echo Жду, пока порт 8000 начнёт отвечать ^(до 90 сек^)...
powershell -NoProfile -ExecutionPolicy Bypass -Command ^
  "$ok=$false; for($i=0;$i -lt 45;$i++) { try { $t=New-Object Net.Sockets.TcpClient; $t.Connect('127.0.0.1',8000); if($t.Connected){ $t.Close(); $ok=$true; break } } catch {} try{ $t.Dispose() } catch {}; Start-Sleep -Seconds 2 }; if(-not $ok){ Write-Host 'Порт 8000 не поднялся за 90 сек. Смотрите ошибки в окне сервера.' }"

call "%ROOT%open-site.cmd" "http://127.0.0.1:8000/"
echo.
echo Если браузер не открылся — скопируйте в адресную строку:
echo   http://127.0.0.1:8000/
echo Окно «Booking API ^(Python^)» не закрывайте.
pause
exit /b 0

:run_dotnet
echo Запуск API ^(C# / .NET^)...
start "Booking API" cmd /k "cd /d "%API%" && dotnet run"

echo Жду порт ^(до 60 сек^)...
powershell -NoProfile -ExecutionPolicy Bypass -Command ^
  "for($i=0;$i -lt 30;$i++) { try { $t=New-Object Net.Sockets.TcpClient; $t.Connect('127.0.0.1',7288); if($t.Connected){ $t.Close(); break } } catch {} try{ $t.Dispose() } catch {}; Start-Sleep -Seconds 2 }"

call "%ROOT%open-site.cmd" "https://localhost:7288/"
echo.
echo Если не открылось — вручную: https://localhost:7288/
echo Окно "Booking API" не закрывайте.
pause
exit /b 0
