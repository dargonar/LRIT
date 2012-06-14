@echo off
copy /y bin\debug\* .

FOR /F "tokens=2 delims==" %%a in ('type Program.cs ^| FIND "VERSION ="') do set _VER=%%a
set _VER=%_VER:"=%
set _VER=%_VER:;=%
set _VER=%_VER: =%

echo Packing for version %_VER%

del /f /q *.config > nul
rmdir /Q /S obj

echo lrit-core-%_VER%.zip

"c:\Program Files\7-Zip\7z.exe" a "..\lrit-core-%_VER%.zip" * > nul
