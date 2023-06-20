#!/bin/bash

t_value="default"
m_value="static"

while getopts "t:m:" option
do 
  case $option in
	\?)
	 echo "Usage:autobuild_debian10.sh [-t {target}][-m {mode}] | -t {default;MND;MOIS}:custom site -m {static;repo}:mode " 1>&2 
	 exit 1
	 ;;
	t)
	 t_value=$OPTARG
	 echo "Found the option -t, parameter $OPTARG"
	 ;;
	m)
	 m_value=$OPTARG
	 echo "Found the option -m, parameter $OPTARG"
	 ;;
  esac
done

   


sudo apt-get update
echo "-------------------------------"
echo "Git clone dxClient-Linux"
sudo apt install -y git make gcc cmake g++
git clone https://gitlab.com/daasxpert/daasxpert-client/DaaSXpert-Client-Linux.git


if [[ ${m_value} == "static" ]];then 
 echo "-------------------------------"
 echo "Preparing before freerdp-build"
 sudo apt install -y build-essential libssl-dev libx11-dev 
 sudo apt install -y libxext-dev libxinerama-dev libxcursor-dev libxkbfile-dev
 sudo apt install -y libxv-dev libxi-dev libxdamage-dev libxrender-dev libxrandr-dev
 sudo apt install -y libasound2-dev libcups2-dev libpulse-dev libavutil-dev libavcodec-dev
 sudo apt install -y libusb-1.0.0-dev

 echo "-------------------------------"
 echo "buinding OPENCL...."
 sudo apt-get install ocl-icd-opencl-dev -y
 sudo apt-get install clinfo -y

 echo "-------------------------------"
 echo "buinding OPENH264...."
 sudo apt-get install -y nasm
 git clone https://github.com/cisco/openh264
 cd openh264
 make & sudo make install

 cd ../DaaSXpert-Client-Linux

 echo "-------------------------------"
 echo "git clone freerdp...."
 bash ./main_prj/init_submodule.sh
 cd sub_prj

 echo "-------------------------------"
 echo "building freerdp...."
 bash ./build_script/build_freerdp.release.sh

 cd ../main_prj/linux/src
else
 cd ./DaaSXpert-Client-Linux/main_prj/linux/src
fi

echo $PWD


echo "-------------------------------"
echo "Preparing before dxClient package build...."
sudo apt-get install -y dpkg debconf debhelper fakeroot rsync dos2unix lintian nuget

echo "-------------------------------"
echo "Preparing before mono-develop debian package"
 
sudo apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF
echo "deb https://download.mono-project.com/repo/debian vs-buster main" | sudo tee /etc/apt/sources.list.d/mono-official-vs.list

sudo apt update
sudo apt install -y dirmngr gnupg apt-transport-https ca-certificates
sudo apt install -y apt-transport-https dirmngr
sudo apt install -y mono-complete
sudo apt install -y monodevelop
sudo apt install -y monodevelop-nunit

echo "-------------------------------"
echo "Copy nuget package setting"
cp ../../../../NuGet.Config ~/.config/NuGet/NuGet.Config

echo "-------------------------------"
echo "prebuild using msbuild"

msbuild Build.proj /t:Restore
msbuild Build.proj /t:build

echo "-------------------------------"
echo "building dxClient package"

echo "${m_value} install : ${m_value}"

if [[ ${m_value} == "static" ]];then 
 bash ./oneStepBuild_staticInstall.sh -t ${t_value} -o debian10
else
 bash ./oneStepBuild_repoInstall.sh -t ${t_value} -o debian10
fi







