#!/bin/bash

FILES="./client/bin/config/*.ini"

for f in $FILES
do
   echo "Processing $f file.."
   sed -i "s/{builddate}/$(date '+%Y-%m-%d')/g" $f
done

