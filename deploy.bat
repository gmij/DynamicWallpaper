


set projectName="src/DynamicWallpaper.csproj"
set publishDir="bin/release/publish"

dotnet publish %projectName% -c Release -p:PublishReadyToRun=true -p:SelfContained=true -p:PublishSingleFile=true -p:PublishDir=../%publishDir%




