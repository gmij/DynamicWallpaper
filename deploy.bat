


set projectName="src/DynamicWallpaper.csproj"
set cmdProjectName="DynamicWallpaper.Command/DynamicWallpaper.Command.csproj"
set publishDir="bin/release/publish"

dotnet publish %projectName% -c Release -p:PublishReadyToRun=true -p:SelfContained=true -p:PublishSingleFile=true -p:PublishDir=../%publishDir%

dotnet publish %cmdProjectName% -c Release -p:PublishReadyToRun=true -p:SelfContained=true -p:PublishTrimmed=true -p:PublishSingleFile=true -p:PublishDir=../%publishDir%


