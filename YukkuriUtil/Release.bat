@echo off
setlocal

:: Releaseディレクトリの初期化
rmdir /s /q Release
mkdir Release

copy bin\Release\Livet.dll Release
copy bin\Release\Microsoft.Expression.Interactions.dll Release
copy bin\Release\NAudio.dll Release
copy bin\Release\System.Windows.Interactivity.dll Release
copy bin\Release\YukkuriUtil.exe Release
copy Resources\ReadMe.txt Release

mkdir Release\softalk
copy Resources\SofTalk.ini Release\softalk

mkdir Release\audioout