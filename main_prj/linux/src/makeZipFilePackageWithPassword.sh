#!/bin/bash
Cur_Path=$(pwd)
echo $Cur_Path

echo "1st Param: $1"
echo "2nd Param: $2"
echo "3rd Param: $3"

Target_FilePath=$(find $Cur_Path/Package/ -name "*.deb")
echo $Target_FilePath
Target_FileName=${Target_FilePath##*/}
echo $Target_FileName
o_value=$2
echo "Target OS : ${o_value}"
Temp="${Target_FileName%%.deb}"

echo "Temp : ${Temp}"

ZipFileName="${Temp}.zip"
echo "ZipFileName ${ZipFileName}"

Package_FilePath="${Cur_Path}/Package/"
Final_Zip_FilePath="${Cur_Path}/Package/${ZipFileName}"
cd $Package_FilePath
Final_mono_setup_debian="mono-setup-debian.sh"
Final_mono_setup_ubuntu="mono-setup-ubuntu.sh"

if [[ ${o_value} == "amd64" ]];then
 echo "OS option : ${o_value}"
 zip -P $3 $Final_Zip_FilePath $Target_FileName $Final_mono_setup_debian
elif [[ ${o_value} == "ubuntu18.04" ]];then
 echo "OS option : ${o_value} "
 zip -P $3 $Final_Zip_FilePath $Target_FileName $Final_mono_setup_ubuntu
elif [[ ${o_value} == "debian10" ]];then 
 echo "OS option : ${o_value}"
 zip -P $3 $Final_Zip_FilePath $Target_FileName $Final_mono_setup_debian
fi







