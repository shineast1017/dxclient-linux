echo "---!!!! xbuild Package(repo) !!!---"

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
         echo "Usage:oneStepBuild_repoInstall.sh [-s] [-t {target}] [-o {OS}] [-b] | -s:secure Define -t {default;MND;MOIS;ShinHan;KOSTA}:Custom site -o {debian10;ubuntu18.04}: Target OS -b: msbuild " 1>&2 # -s:secure Define
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

select_build_type_str="repo_install"

echo "secure Define: ${s_flag}"
echo "custom site : ${t_value}"
echo "Build Type : ${select_build_type_str}"

if [[ ${t_value} != "default" ]];then
 echo "config init option : ${t_value}"
 ./config_initSetting.sh ${t_value} 
else
 echo "config init option None"
 ./config_initSetting.sh 
fi

echo "---!!!! Select Package Install Type repo install "
rm -rf  ./Package/deb
cp -rf  ./Package/deb_preset/repo_install/deb ./Package/.
echo "---!!!! copy deb preset "
echo "---!!!! Setting Package Install Type repo install  end"


if [ ${b_flag} -eq 0 ];then
 xbuild Build.proj /t:Clean
 xbuild Build.proj /t:Restore
 xbuild Build.proj /t:Version
 if [ ${s_flag} -eq 0 ];then
  echo "Build pure option"
  xbuild Build.proj /t:Build
 else
  echo "Build Secure option"
  xbuild Build.proj /t:Build /p:DefineConstants=SECURE_HTTP_MESSAGE
 fi
 xbuild Build.proj /t:Pack
  
else
 msbuild Build.proj /t:Clean
 msbuild Build.proj /t:Restore
 msbuild Build.proj /t:Version
 msbuild Build.proj /t:Build
 msbuild Build.proj /t:PackEx
fi


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

