#!/bin/sh

dotnet build -t:Run -f net8.0-ios -c Release -p:RuntimeIdentifier=ios-arm64 -p:_DeviceName=$1 ./../../BolWallet/BolWallet.csproj