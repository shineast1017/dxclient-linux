sudo apt update
sudo apt install dirmngr gnupg apt-transport-https ca-certificates

sudo apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF
sudo sh -c 'echo "deb https://download.mono-project.com/repo/ubuntu stable-bionic main" > /etc/apt/sources.list.d/mono-official-stable.list'

sudo apt update
sudo apt install mono-complete

sudo apt install monodevelop


sudo apt install monodevelop-nunit

mono --version
#tested 20210215

