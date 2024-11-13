#!/bin/sh

dotnet build -f net9.0-maccatalyst -c Release -p:RuntimeIdentifier=maccatalyst-arm64 ./../../BolWallet.csproj