#!/bin/sh

dotnet publish -f net8.0-ios -p:ApplicationVersion=$1 -p:ApplicationDisplayVersion=$2 ./../../BolWallet/BolWallet.csproj