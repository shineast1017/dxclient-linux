
s_flag=0 # secure Define
b_flag=0 # xbuild or msbuild
t_value="default" # custom site
o_value="amd64" # package name 

while getopts "sbt:o:" option
do 
  case $option in
	s)
	 s_flag=1
	 ;;
	b)
	 b_flag=1
	 ;;
  	\?)
	 echo "Usage:oneStepBuild_staticInstall.sh [-s] [-t {target}][-o {OS}] | -s:secure Define -t {default;MND;MOIS}:custom site -o {debian10;ubuntu18.04}:target OS" 1>&2 # -s:secure Define
	 exit 1
	 ;;
	t)
	 t_value=$OPTARG
	 echo "Found the option -t, parameter $OPTARG"
	 ;;
	o)
	 o_value=$OPTARG
	 echo "Found the option -o, parameter $OPTARG"
	 ;;
  esac
done

select_build_type_str="static_install"

echo "secure Define: ${s_flag}"
echo "custom site : ${t_value}"
echo "Build Type : ${select_build_type_str}"

if [[ ${t_value} != "default" ]];then
 echo "config init option : ${t_value}"
 ./config_initSetting.sh ${t_value}
else
 echo "config init option None "
 ./config_initSetting.sh
fi




if [ ${b_flag} -eq 0 ];then
 echo "---!!!! xbuild Package !!!---" 
else
 echo "---!!!! msbuild Package !!!---"
fi

#echo "---!!!! xbuild Package !!!---"

echo "---!!!! Select Package Install Type static install "
rm -rf  ./Package/deb

echo "---!!!! setup collection lib "

echo "---!!!! libopenh264.so.7 "
cp -rf  ../../../sub_prj/build/release/lib/libopenh264.so.7 ./Package/deb_preset/static_install/deb/usr/local/lib/.

echo "---!!!! copy deb preset "
cp -rf  ./Package/deb_preset/static_install/deb ./Package/.

echo "---!!!! Setting Package Install Type static install  end"


if [ ${b_flag} -eq 0 ];then
 xbuild Build.proj /t:Clean
 xbuild Build.proj /t:Restore
 xbuild Build.proj /t:Version
 xbuild Build.proj /t:Build
 xbuild Build.proj /t:Pack
  
else
 msbuild Build.proj /t:Clean
 msbuild Build.proj /t:Restore
 msbuild Build.proj /t:Version
 msbuild Build.proj /t:Build
 msbuild Build.proj /t:PackEx
fi

#xbuild Build.proj /t:Clean
#xbuild Build.proj /t:Restore
#xbuild Build.proj /t:Version
#xbuild Build.proj /t:Build

#if [ ${s_flag} -eq 0 ];then
# echo "Build pure option" 
# xbuild Build.proj /t:Build
#else
# echo "Build Secure option"
# msbuild Build.proj /t:Build /p:DefineConstants=SECURE_MESSAGE
#fi

#xbuild Build.proj /t:Pack


echo "---!!!! Make dpkg deb pack end"

if [[ ${t_value} != "default" ]];then
 echo "Rename Package File Target  OS: ${o_value}"
 ./renameInstallPackage.sh ${t_value} ${o_value}
else
 echo "Rename Package File General"
 ./renameInstallPackage.sh General ${o_value}
 t_value="General"
fi

echo "---!!!! Compress with password ****(1234)"
password=1234
./makeZipFilePackageWithPassword.sh ${t_value} ${o_value} ${password}

