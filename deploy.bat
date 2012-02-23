set destfolder=s:\_work\BIN
rem set destfolder=D:\Work\wm\_out
set srcfolder=D:\Работа\wm\metrohome65
rem set srcfolder=D:\Work\wm\metrohome65

rmdir /S /Q %destfolder%
mkdir %destfolder%

mkdir %destfolder%\buttons
copy %srcfolder%\metrohome65\buttons\*.* %destfolder%\buttons\
mkdir %destfolder%\icons
mkdir %destfolder%\icons\small
copy %srcfolder%\metrohome65\icons\*.* %destfolder%\icons\
copy %srcfolder%\metrohome65\icons\small\*.* %destfolder%\icons\small\

copy %srcfolder%\metrohome65\Widgets\ClockWidgets\bin\Debug\*.dll %destfolder%
copy %srcfolder%\metrohome65\Widgets\PhoneWidgets\bin\Debug\*.dll %destfolder%
copy %srcfolder%\metrohome65\Widgets\StatusWidgets\bin\Debug\*.dll %destfolder%
copy %srcfolder%\metrohome65\bin\Debug\*.exe %destfolder%
copy %srcfolder%\metrohome65\bin\Debug\*.dll %destfolder%
