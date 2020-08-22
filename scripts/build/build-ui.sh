#!/bin/bash
cd gui
npm install
npm run build
cp -r gui/dist/. src/HttPlaceholder/gui