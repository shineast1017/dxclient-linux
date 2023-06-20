xbuild Build.proj /t:Clean
xbuild Build.proj /t:Restore
xbuild Build.proj /t:Build

xbuild Build.proj /t:Pack

