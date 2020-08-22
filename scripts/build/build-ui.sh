#!/bin/bash
cd gui
npm install
npm run build

cp -r dist/. ../src/HttPlaceholder/gui