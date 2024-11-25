[CmdletBinding()]
param (
    [Parameter(Mandatory=$true)][string]$t
)

dotnet publish .\..\..\BolWallet\BolWallet.csproj -f net9.0-windows10.0.19041.0 -c Release --self-contained -p:RuntimeIdentifierOverride=win10-x64 -p:PackageCertificateThumbprint=$t