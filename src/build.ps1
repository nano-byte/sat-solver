Param ($Version = "1.0-dev")
$ErrorActionPreference = "Stop"
pushd $PSScriptRoot

dotnet msbuild -v:Quiet -t:Restore -t:Build -p:Configuration=Release -p:Version=$Version
if ($LASTEXITCODE -ne 0) {throw "Exit Code: $LASTEXITCODE"}

popd
