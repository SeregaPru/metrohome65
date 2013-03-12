rem set destfolder=s:\_work\BIN
set destfolder=D:\Работа\wm\_out

rem set srcfolder=D:\Работа\wm\metrohome65
set srcfolder=D:\Работа\wm\metrohome65

rmdir /S /Q %destfolder%
mkdir %destfolder%

mkdir %destfolder%\buttons
copy %srcfolder%\metrohome65\buttons\*.* %destfolder%\buttons\
mkdir %destfolder%\icons
mkdir %destfolder%\icons\small
copy %srcfolder%\metrohome65\icons\*.* %destfolder%\icons\
copy %srcfolder%\metrohome65\icons\small\*.* %destfolder%\icons\small\
mkdir %destfolder%\wallpapers
copy %srcfolder%\metrohome65\wallpapers\*.* %destfolder%\wallpapers\

copy %srcfolder%\_deploy\*.dll %destfolder%\
copy %srcfolder%\_deploy\*.exe %destfolder%\
