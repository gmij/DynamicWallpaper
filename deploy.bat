


set projectName="src/DynamicWallpaper.csproj"
set publishDir="bin/release/publish"

dotnet publish %projectName% -c Release -p:PublishReadyToRun=true -p:SelfContained=true -p:PublishSingleFile=true -p:PublishDir=../%publishDir%




dotnet build MSIPackage/DynamicWallpaper.Setup.wixproj 

signtool.exe sign /tr http://timestamp.digicert.com /td sha256 /fd sha256 /sha1 "079f57ca8b7f873500afed9742fa3f95b6fbc3e4"  E:\work\DynamicWallpaper.CS\MsiPackage\bin\x64\Debug\DynamicWallpaper.Setup.msi

copy E:\work\DynamicWallpaper.CS\MsiPackage\bin\x64\Debug\DynamicWallpaper.Setup.msi .\bin\msi\DynamicWallpaper.Setup_SIGN.msi