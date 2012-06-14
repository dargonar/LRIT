@echo off
REM ************************************************************************
REM LRIT - DataCenter
REM ************************************************************************

set version=2.4.0

call:packme LRITCore DataCenterCore
call:packme TestClient DataCenterTestClient
call:packmews LRITRx LRITDataCenter
call:packmews LRITAISWS LRITAIS
call:packmews SIGOWS LRITSIGO
call:packmeweb LRITWeb LRITUi

goto final

:error
echo "Error building ..."
:final
exit /B

:packmeweb
del /Q %~1-%version%.zip

rmdir /Q /S %~1
mkdir %~1

xcopy /E /y %~2\* %~1

del /Q %~1\*.pdb
del /Q %~1\*.cs

cd %~1
7z.exe a -y -tzip "..\%~1-%version%.zip" *
cd ..

rmdir /Q /S %~1
goto:eof


:packme
del /Q %~1-%version%.zip

rmdir /Q /S %~1
mkdir %~1

copy /y %~2\bin\Release\* %~1
copy /y %~2\*.bat %~1

del /Q %~1\*vshost*
del /Q %~1\*.pdb
del /Q %~1\*.xml

cd %~1
7z.exe a -y -tzip "..\%~1-%version%.zip" *
cd ..

rmdir /Q /S %~1
goto:eof

:packmews
del /Q %~1-%version%.zip

rmdir /Q /S %~1
mkdir %~1
mkdir %~1\bin

copy /y %~2\bin\* %~1\bin
del /Q %~1\bin\*vshost*
del /Q %~1\bin\*.pdb
del /Q %~1\bin\*.xml

copy /y %~2\*.asmx %~1
copy /y %~2\*.config %~1

cd %~1
7z.exe a -y -tzip "..\%~1-%version%.zip" *
cd ..

rmdir /Q /S %~1
goto:eof