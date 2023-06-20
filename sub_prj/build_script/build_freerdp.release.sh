#!/bin/sh

########
########
##  run script option example 
##  select freerdp sub project
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

# change current working build directory
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
        cmake -DWITH_GSTREAMER_1_0=ON -DWITH_OPENH264=ON  -DWITH_OPENCL=ON -DCMAKE_INSTALL_PREFIX:PATH=BuildDone .
else
#default

        echo "Run cmake $DEF_DEFAULT_FREERDP";
        cmake -DWITH_OPENH264=ON  -DWITH_OPENCL=ON -DCMAKE_INSTALL_PREFIX:PATH=BuildDone .
fi

cmake --build . --config Release 

make install


# doen copy framework for linux

cd ..

rm -rf ./build/release

if [ -d "build" ]; then
  # Take action if $DIR exists. #
  echo "build exists"
else
  mkdir build
fi

mkdir build/release


cp -rf ./$sel_freerdp/BuildDone/* ./build/release/.


##  os dependency lib collecting

# open264 copy
cp -f /usr/local/lib/libopenh264.so.7 ./build/release/lib/.

## openssl 3 libcrypto libssl   of version 3
#cp -f /lib/x86_64-linux-gnu/libssl.so.3 ./build/release/bin/.
#cp -f /lib/x86_64-linux-gnu/libcrypto.so.3 ./build/release/bin/.

## glibc
#cp -f /lib/x86_64-linux-gnu/libc.so.6 ./build/release/bin/.

## apply patch header
#cd build/Debug
#patch -p1 < ../../build_script/wtypes.h.patch

