#!/bin/sh

########
########
##  select freerdp sub project
##  run script option example 
##  this.sh -s freerdp | freerdp_dxecam
##
##


DEF_DEFAULT_FREERDP="freerdp"
DEF_DXECAM_FREERDP="freerdp_dxecam"

## init option
#while getopts s:r: flag
while getopts s: flag
do
    case "${flag}" in
        s) sel_freerdp=${OPTARG};;
#        r) ReleaseType=${OPTARG};;
    esac
done



# change current working build directory
# default
if [ -z "$sel_freerdp" ] 
then
  sel_freerdp="freerdp"

  echo "select freerdp option -s [freerdp | freerdp_dxecam]"

fi

echo "select freerdp: $sel_freerdp";


cd ./$sel_freerdp


## apply patch  source
##patch -p1 < ../build_script/rdpsnd_pulse.c.patch

#start region for clear cache
rm CMakeCache.txt
rm -rf CMakeFiles/
#end region for clear cache

# build cmake select options

if [ $sel_freerdp = $DEF_DXECAM_FREERDP ] ;
then

        echo "Run cmake $DEF_DXECAM_FREERDP";
        cmake -DWITH_GSTREAMER_1_0=ON .
else
#default

        echo "Run cmake $DEF_DEFAULT_FREERDP";
        cmake .
fi

cmake --build . --config Debug


# doen copy framework for linux

cd ..

rm -rf ./build/debug

mkdir build
mkdir build/debug


cp -rf ./$sel_freerdp/client/X11/xfreerdp ./build/debug/.

## apply patch header
#cd build/Debug
#patch -p1 < ../../build_script/wtypes.h.patch

