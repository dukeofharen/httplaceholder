#!/bin/bash
# Make sure you have imagemagick installed
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
MEDIA_ROOT="$DIR/../../media"
BASE_LOGO="$MEDIA_ROOT/logo_square.png"
PUBLIC_ROOT="$DIR/../../gui/public"

# Android
convert "$BASE_LOGO" -resize 192x192 "$PUBLIC_ROOT/img/icons/android-chrome-192x192.png"
convert "$BASE_LOGO" -resize 512x512 "$PUBLIC_ROOT/img/icons/android-chrome-512x512.png"
convert "$BASE_LOGO" -resize 192x192 "$PUBLIC_ROOT/img/icons/android-chrome-maskable-192x192.png"
convert "$BASE_LOGO" -resize 512x512 "$PUBLIC_ROOT/img/icons/android-chrome-maskable-512x512.png"

# Apple
convert -flatten "$BASE_LOGO" -resize 180x180 "$PUBLIC_ROOT/img/icons/apple-touch-icon.png"
convert -flatten "$BASE_LOGO" -resize 60x60 "$PUBLIC_ROOT/img/icons/apple-touch-icon-60x60.png"
convert -flatten "$BASE_LOGO" -resize 76x76 "$PUBLIC_ROOT/img/icons/apple-touch-icon-76x76.png"
convert -flatten "$BASE_LOGO" -resize 120x120 "$PUBLIC_ROOT/img/icons/apple-touch-icon-120x120.png"
convert -flatten "$BASE_LOGO" -resize 152x152 "$PUBLIC_ROOT/img/icons/apple-touch-icon-152x152.png"
convert -flatten "$BASE_LOGO" -resize 180x180 "$PUBLIC_ROOT/img/icons/apple-touch-icon-180x180.png"

# Favicons
convert "$BASE_LOGO" -resize 32x32 "$PUBLIC_ROOT/favicon.ico"
convert "$BASE_LOGO" -resize 32x32 "$PUBLIC_ROOT/img/icons/favicon-32x32.png"
convert "$BASE_LOGO" -resize 16x16 "$PUBLIC_ROOT/img/icons/favicon-16x16.png"

# Microsoft
convert "$BASE_LOGO" -resize 144x144 "$PUBLIC_ROOT/img/icons/msapplication-icon-144x144.png"
convert "$BASE_LOGO" -resize 150x150 "$PUBLIC_ROOT/img/icons/mstile-150x150.png"