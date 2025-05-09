﻿$ErrorActionPreference = "Stop"
pushd $PSScriptRoot

function Run-DotNet {
    ..\0install.ps1 run --batch --version 9.0.200.. https://apps.0install.net/dotnet/sdk.xml @args
    if ($LASTEXITCODE -ne 0) {throw "Exit Code: $LASTEXITCODE"}
}

# Build docs
Run-DotNet tool restore
Run-DotNet docfx --logLevel=warning --warningsAsErrors docfx.json

popd
