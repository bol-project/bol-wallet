#!/bin/sh

dotnet build -f net8.0-maccatalyst -c Release -p:RuntimeIdentifier=maccatalyst-arm64 ./../BolWallet.csproj