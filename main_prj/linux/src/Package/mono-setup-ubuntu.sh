#!/bin/bash

version=$(lsb_release -sr)
echo "$version"

# user message popup function  "yes or no keep install"
function checkNotSupportOS {
#  echo "Call CheckNotSupportOS"
  if zenity --question  --width=300 --height=150 --text="\
    <big>Current Not Support OS Version\n\n You Want Keep Going?</big>";
  then
    #yes
    # notify-send "Keep going Install"
    echo "run keep going"
  else    
    #no
    # notify-send "Cancle Install"
    # cancle install 
    exit 1
  fi


}


FREERDP_PKG_NAME=freerdp2-x11

sudo apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF

if [[ $version == '20.04' ]]
then
    #echo "Version of ubuntu is 20.04 running script"
	echo "deb [arch=amd64] https://download.mono-project.com/repo/ubuntu stable-focal main" | sudo tee /etc/apt/sources.list.d/mono-official-stable.list
elif [[ $version == '22.04' ]]
then
	#echo "Version of unbuntu is 22.04 running script"
	echo "deb [arch=amd64] https://download.mono-project.com/repo/ubuntu stable-facal main" | sudo tee /etc/apt/sources.list.d/mono-official-stable.list
elif [[ $version == "18.04" ]]
then
    	echo "Version of ubuntu is 18.04 running script"
	echo "deb https://download.mono-project.com/repo/ubuntu stable-bionic main" | sudo tee /etc/apt/sources.list.d/mono-official-stable.list
elif [[ $version == '16.04' ]]
then
    #echo "Version of ubuntu is 16.04 running script"
	sudo apt install apt-transport-https
	echo "deb https://download.mono-project.com/repo/ubuntu stable-xenial main" | sudo tee /etc/apt/sources.list.d/mono-official-stable.list
	FREERDP_PKG_NAME=freerdp-x11
else
        # call check user yes or not message popup window, keep install
        checkNotSupportOS

        #echo "Version of ubuntu is 12.04 running script"
	sudo apt install apt-transport-https
	echo "deb https://download.mono-project.com/repo/ubuntu stable-trusty main" | sudo tee /etc/apt/sources.list.d/mono-official-stable.list
	FREERDP_PKG_NAME=freerdp-x11
fi

#sudo dpkg --configure -a
sudo apt update

#echo "Install mono-complete freerdp virt-viewer"
#sudo apt-get install -y dpkg debconf debhelper fakeroot rsync dos2unix lintian mono-complete ${FREERDP_PKG_NAME} virt-viewer
exit 0


