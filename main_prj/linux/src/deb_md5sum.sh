#!/bin/bash

cd $1
find . -type f -not -path "*DEBIAN*"  -print0 |  xargs -0 md5sum | awk '{ print $1 "  " substr($2,3) }' > ./DEBIAN/md5sums
