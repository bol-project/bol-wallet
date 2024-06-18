#!/bin/sh

xcrun notarytool log --keychain-profile "bolwallet-notarytool-password" $1 $1.json