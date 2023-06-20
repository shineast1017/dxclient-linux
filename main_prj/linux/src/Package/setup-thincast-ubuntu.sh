#!/bin/bash

sudo apt -y install software-properties-common
sudo add-apt-repository ppa:flatpak/stable
sudo apt -y install flatpak ca-certificates

sudo flatpak remote-add flathub https://flathub.org/repo/flathub.flatpakrepo
sudo flatpak install -y flathub com.thincast.client
