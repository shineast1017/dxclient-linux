#!/bin/bash


if [ $# -eq 0 ];then
 target=default
 echo "target default"
 cp -rp ../custom/${target}/deb/** ./Package/deb_preset/static_install/deb
else
 target=$1
 echo "target: $1"
 cp -rp ../custom/${target}/deb/** ./Package/deb_preset/static_install/deb
 cp -rp ../custom/${target}/client/** ./client
fi
./replaceBuildDate.sh
