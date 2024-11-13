#!/bin/sh

dotnet publish -f net9.0-ios -p:ApplicationVersion=$1 -p:ApplicationDisplayVersion=$2 ./../../BolWallet/BolWallet.csproj