@echo off
setlocal

:: Releaseディレクトリの初期化
rmdir /s /q Release
mkdir Release

copy bin\Release\*.dll Release
copy bin\Release\YukkuriUtil.exe Release
copy ..\ReadMe.txt Release
copy ..\LICENSE.txt Release

mkdir Release\softalk
copy Resources\SofTalk.ini Release\softalk

mkdir Release\audioout