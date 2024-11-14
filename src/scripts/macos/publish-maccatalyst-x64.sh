#!/bin/sh

dotnet build -f net9.0-maccatalyst -c Release -p:RuntimeIdentifier=maccatalyst-x64 ./../../BolWallet.csproj