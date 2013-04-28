set destfolder=D:\Работа\wm\SHARED\metrohome65
set srcfolder=D:\Работа\wm\metrohome65

rmdir /S /Q D:\Работа\wm\SHARED\metrohome65
mkdir D:\Работа\wm\SHARED\metrohome65

rmdir /S /Q %destfolder%
mkdir %destfolder%

mkdir %destfolder%\buttons
copy %srcfolder%\metrohome65\buttons\*.* %destfolder%\buttons\
mkdir %destfolder%\Settings
copy %srcfolder%\metrohome65\Settings\*.* %destfolder%\Settings\
mkdir %destfolder%\icons
mkdir %destfolder%\icons\small
copy %srcfolder%\metrohome65\icons\*.* %destfolder%\icons\
copy %srcfolder%\metrohome65\icons\small\*.* %destfolder%\icons\small\
mkdir %destfolder%\wallpapers
copy %srcfolder%\metrohome65\wallpapers\*.* %destfolder%\wallpapers\

copy %srcfolder%\_deploy\*.dll %destfolder%
copy %srcfolder%\_deploy\*.exe %destfolder%
