#!/bin/sh

xcrun notarytool submit --keychain-profile "bolwallet-notarytool-password" --wait $1