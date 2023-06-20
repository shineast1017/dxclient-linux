#!/bin/bash
Cur_Path=$(pwd)
echo $Cur_Path

Target_FilePath=$(find $Cur_Path/Package/ -name "*.deb")
echo $Target_FilePath
Target_FileName=${Target_FilePath##*/}
echo $Target_FileName

Temp="${Target_FileName%%_amd64*}"
Target_FileVision="${Temp#*_}"
echo $Target_FileVision

Final_FilePath="${Cur_Path}/Package/$1_DaaSXpertClient_${Target_FileVision}_$2.deb"

echo $Final_FilePath

mv ${Target_FilePath} ${Final_FilePath}
