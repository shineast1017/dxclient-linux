# Ubuntu Mono Client Packaging



## 1. Prepare Packaging Environment

Installation of the following programs are needed to build Debian/Ubuntu-flavour linux packages:

```sh
$ sudo apt install dpkg debconf debhelper fakeroot rsync dos2unix lintian
```



| Program  | Description                                                  |
| -------- | ------------------------------------------------------------ |
| fakeroot | It runs a command in an environment wherein it appears to have root privileges for file manipulation. It will be used for p |
| rsync    | A file copying tool. It will be used to copy files into package folder structure |
| dos2unix | A converter from DOS to UNIX text file format. This will be used for version numbering |
| lintian  | This programs helps to identify bugs in Debian package.      |



## 2. Prepare Package Folder

```sh
$ cd DaaSXpert-Client/ubuntu/src/
$ mkdir -p Package/deb/DEBIAN
$ mkdir -p Package/usr/bin
$ mkdir -p Package/usr/lib/HCVKAService
$ mkdir -p Package/usr/share/HCVKAService
```


```sh
Package/
├── deb
    ├── DEBIAN
    │   ├── conffiles
    │   ├── control
    │   ├── postinst
    │   └── preinst
    └── usr
        ├── bin
        │   └── DaaSXpertClient
        ├── lib
        └── share
            ├── applications
            │   └── DaaSXpertClient.desktop
            └── DaaSXpertClient
                └── HCTeam.png
```



- Folder **deb** is placed just inside the Package folder. This is **package root**. There are more folders inside the package root. Folder DEBIAN contains files that are used while installation.

- And another folder in **my** package root is **usr**. This is because I want to install my program to **/usr** folder of Linux operating system.

While installation all folders from package root will be copied to a  Linux OS root. Thus folder structure must be exactly the same as we want  it to be after installation. I will install my program to **/usr/lib/HCVKAService**. HCVKAService is the name of application. Thus I have **/deb/usr/lib/HCVKAService** in my package folder structure.



## 3. Create MSBuilx Building Script File

Packaging is a part of a application building process. I control  building process through a single MSBuild project file. Building file  contains  the following steps (*targets* in MSBuild terminology):

- Clean project
- Restore NuGet packages
- Assign version to the assembly
- Build binaries
- Packaging with following substeps:
- Set version number to deb-package in DEBIAN/control
- Copy binaries to package root
- Start packaging process



**../Build.proj**

```xml
﻿<?xml version="1.0" encoding="utf-8"?>
<Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003" DefaultTargets="Run">
  <PropertyGroup>
    <Today>$([System.DateTime]::Now.ToString("yyyy.MM.dd"))</Today>
    <!--
    <Configuration>Release</Configuration>
    -->
    <Configuration  Condition=" '$(Configuration)' == '' ">Release</Configuration>
    <Platform Condition=" '$(Platform)' == '' ">AnyCPU</Platform>
    <PackageName>DaaSXpertClient</PackageName>
    <ProjectName>client</ProjectName>
    <ProjectLibraryName>HCVKSLibrary</ProjectLibraryName>
    <BinaryFolder>$(ProjectName)/bin/$(Configuration)</BinaryFolder>
    <MainExecutable>$(BinaryFolder)/$(ProjectName).exe</MainExecutable>
  </PropertyGroup>
  
   <Target Name="Run">
    <CallTarget Targets="Clean" />
    <CallTarget Targets="Restore" />
    <CallTarget Targets="Version" />
    <CallTarget Targets="Build" />
    <CallTarget Targets="Pack" />
  </Target>

  <!--
    Clean Solution
  -->
  <Target Name="Clean">
    <Message Text="Clean" />
    <RemoveDir Directories="$(ProjectName)/bin/$(Configuration); slibrary/bin/$(Configuration);" ContinueOnError="False"/>
    <RemoveDir Directories="$(ProjectName)/obj/$(Configuration); slibrary/obj/$(Configuration);" ContinueOnError="False"/>
  </Target>

  <!--
    Restore NuGet Packages
  -->
  <Target Name="Restore">
    <Message Text="Restore NuGet packages" />
    <Exec Command="nuget restore" ContinueOnError="False"/>
  </Target>

  <!--
    Update the version of the assemblies
  -->
  <UsingTask AssemblyFile="packages/MSBuild.Extension.Pack.1.9.1/tools/net40/MSBuild.ExtensionPack.dll" TaskName="AssemblyInfo" />
  <Target Name="Version">
    <Message Text="Versioning Assemblies" />
    <ItemGroup>
      <AssemblyInfoFiles Include="**\AssemblyInfo.cs" />
    </ItemGroup>
    <AssemblyInfo
            AssemblyInfoFiles="@(AssemblyInfoFiles)"
            
            AssemblyMajorVersion="$(MajorVersion)"
            AssemblyMinorVersion="$(MinorVersion)"
            AssemblyBuildNumberType="DateString"
            AssemblyBuildNumberFormat="MMdd"
            AssemblyRevisionType="AutoIncrement"
            AssemblyRevisionFormat="000"
          
            AssemblyFileMajorVersion="$(MajorVersion)"
            AssemblyFileMinorVersion="$(MinorVersion)"
            AssemblyFileBuildNumberType="DateString"
            AssemblyFileBuildNumberFormat="MMdd"
            AssemblyFileRevisionType="AutoIncrement"
            AssemblyFileRevisionFormat="000"
    />
  </Target>
  
  <!--
    Build Solution
  -->
  <Target Name="Build">
    <Message Text="Build $(Configuration)" />
    <MSBuild Projects="slibrary/$(ProjectLibraryName).csproj" Properties="Configuration=$(Configuration)" ContinueOnError="False"/>
    <MSBuild Projects="$(ProjectName)/$(ProjectName).csproj" Properties="Configuration=$(Configuration)" ContinueOnError="False"/>
  </Target>
 
  <!--
    Pack Solution Binaries to Debian package
  -->
    <UsingTask AssemblyFile="packages/MSBuild.Extension.Pack.1.9.1/tools/net40/MSBuild.ExtensionPack.dll" TaskName="Assembly"/>
    <UsingTask AssemblyFile="packages/MSBuildTasks.1.5.0.235/tools/MSBuild.Community.Tasks.dll" TaskName="FileUpdate"/>
    <Target Name="Pack" Condition=" '$(OS)' == 'Unix'" >
      <Message Text="Pack binaries to deb package" />
      <PropertyGroup>
        <PackageFolder>Package</PackageFolder>
        <TempFolder>temp</TempFolder>
        <PackageDebFolder>$(PackageFolder)/deb</PackageDebFolder>
        <PackageTempFolder>$(PackageFolder)/$(TempFolder)</PackageTempFolder>
      </PropertyGroup>
    
    <MakeDir Directories="$(PackageDebFolder)/usr/share/locale/ko/LC_MESSAGES"/>
    <MakeDir Directories="$(PackageDebFolder)/usr/share/locale/en/LC_MESSAGES"/>
    <Exec Command="msgfmt localization/ko.po -o $(PackageDebFolder)/usr/share/locale/ko/LC_MESSAGES/DaaSXpertClient.mo"/>
    <Exec Command="msgfmt localization/en.po -o $(PackageDebFolder)/usr/share/locale/en/LC_MESSAGES/DaaSXpertClient.mo"/>

    <RemoveDir Directories="$(PackageTempFolder)" />    
    <ItemGroup>
      <FilesToDeleteInPackageFolder Include="$(PackageFolder)/*.deb"/>
    </ItemGroup>    
    <Delete Files="@(FilesToDeleteInPackageFolder)"/>
  
    <MakeDir Directories="$(PackageTempFolder)"/>
    
    <Exec Command="rsync -r --delete $(PackageDebFolder)/* $(PackageTempFolder)"/>
  
    <Assembly TaskAction="GetInfo" NetAssembly="$(MainExecutable)"> 
      <Output TaskParameter="OutputItems" ItemName="Info"/> 
    </Assembly>
    <Message Text="AssemblyVersion: %(Info.AssemblyVersion)" />
  
    <FileUpdate Files="$(PackageTempFolder)/DEBIAN/control"
          Regex="{xxx}"
          ReplacementText="%(Info.AssemblyVersion)" />    
  
    <Exec Command="dos2unix $(PackageTempFolder)/DEBIAN/control"/>
    
    <ItemGroup>
      <BinaryFiles Include="$(BinaryFolder)/**/*.dll;$(BinaryFolder)/**/*.exe;$(BinaryFolder)/**/*.config;"/>
    </ItemGroup>
       <Copy SourceFiles="@(BinaryFiles)"
                DestinationFiles="@(BinaryFiles->'$(PackageTempFolder)/usr/lib/$(PackageName)\%(RecursiveDir)%(Filename)%(Extension)')" />  
    <Exec Command="fakeroot dpkg-deb -v --build $(PackageTempFolder)"/>
    <Copy 
      SourceFiles="$(PackageFolder)/$(TempFolder).deb" 
      DestinationFiles="$(PackageFolder)/$(PackageName)_%(Info.AssemblyVersion)_amd64.deb"/>
    <Delete Files="$(PackageFolder)/$(TempFolder).deb"/>
  
    <RemoveDir Directories="$(PackageTempFolder)" />
  </Target>
</Project>
```



## 4. Run the build script from command line



```sh
$ cd ~/Projects/DaaSXpert-Client/ubuntu/src
$ sudo xbuild Build.proj /t:Clean
$ sudo xbuild Build.proj /t:Restore
$ sudo xbuild Build.proj /t:Version
$ sudo xbuild Build.proj /t:Build
```



** Generate a package**

```sh
$ sudo xbuild Build.proj /t:Pack
Build Engine Version 14.0
Mono, Version 5.14.0.177
Copyright (C) 2005-2013 Various Mono authors

Build started 9/4/2018 1:26:18 PM.
__________________________________________________
Project "/home/knight47/Projects/DaaSXpert-Client/ubuntu/src/agent/Build.proj" (Pack target(s)):
	Target Pack:
		Pack binaries to deb package
		Created directory "Package/temp"
		Executing: rsync -r --delete Package/deb/* Package/temp
		Loading Assembly: /home/knight47/Projects/DaaSXpert-Client/ubuntu/src/agent/HCVKAService/bin/Debug/HCVKAService.exe
		AssemblyVersion: 1.0.903.5
		Updating File "Package/temp/DEBIAN/control".
		Executing: dos2unix Package/temp/DEBIAN/control
		dos2unix: converting file Package/temp/DEBIAN/control to Unix format ...
		Copying file from '/home/knight47/Projects/DaaSXpert-Client/ubuntu/src/agent/HCVKAService/bin/Debug/en-US/HCVKAService.resources.dll' to '/home/knight47/Projects/DaaSXpert-Client/ubuntu/src/agent/Package/temp/usr/lib/HCVKAService/HCVKAService.resources.dll'
		Copying file from '/home/knight47/Projects/DaaSXpert-Client/ubuntu/src/agent/HCVKAService/bin/Debug/en-US/HCVKAService.resources.dll' to '/home/knight47/Projects/DaaSXpert-Client/ubuntu/src/agent/Package/temp/usr/lib/HCVKAService/HCVKAService.resources.dll'
		Copying file from '/home/knight47/Projects/DaaSXpert-Client/ubuntu/src/agent/HCVKAService/bin/Debug/HCVKSLibrary.dll' to '/home/knight47/Projects/DaaSXpert-Client/ubuntu/src/agent/Package/temp/usr/lib/HCVKAService/HCVKSLibrary.dll'
		Copying file from '/home/knight47/Projects/DaaSXpert-Client/ubuntu/src/agent/HCVKAService/bin/Debug/INIFileParser.dll' to '/home/knight47/Projects/DaaSXpert-Client/ubuntu/src/agent/Package/temp/usr/lib/HCVKAService/INIFileParser.dll'
		Copying file from '/home/knight47/Projects/DaaSXpert-Client/ubuntu/src/agent/HCVKAService/bin/Debug/Newtonsoft.Json.dll' to '/home/knight47/Projects/DaaSXpert-Client/ubuntu/src/agent/Package/temp/usr/lib/HCVKAService/Newtonsoft.Json.dll'
		Copying file from '/home/knight47/Projects/DaaSXpert-Client/ubuntu/src/agent/HCVKAService/bin/Debug/System.Management.Automation.dll' to '/home/knight47/Projects/DaaSXpert-Client/ubuntu/src/agent/Package/temp/usr/lib/HCVKAService/System.Management.Automation.dll'
		Copying file from '/home/knight47/Projects/DaaSXpert-Client/ubuntu/src/agent/HCVKAService/bin/Debug/log4net.dll' to '/home/knight47/Projects/DaaSXpert-Client/ubuntu/src/agent/Package/temp/usr/lib/HCVKAService/log4net.dll'
		Copying file from '/home/knight47/Projects/DaaSXpert-Client/ubuntu/src/agent/HCVKAService/bin/Debug/nunit.framework.dll' to '/home/knight47/Projects/DaaSXpert-Client/ubuntu/src/agent/Package/temp/usr/lib/HCVKAService/nunit.framework.dll'
		Copying file from '/home/knight47/Projects/DaaSXpert-Client/ubuntu/src/agent/HCVKAService/bin/Debug/HCVKAService.exe' to '/home/knight47/Projects/DaaSXpert-Client/ubuntu/src/agent/Package/temp/usr/lib/HCVKAService/HCVKAService.exe'
		Copying file from '/home/knight47/Projects/DaaSXpert-Client/ubuntu/src/agent/HCVKAService/bin/Debug/HCVKAService.exe.config' to '/home/knight47/Projects/DaaSXpert-Client/ubuntu/src/agent/Package/temp/usr/lib/HCVKAService/HCVKAService.exe.config'
		Copying file from '/home/knight47/Projects/DaaSXpert-Client/ubuntu/src/agent/HCVKAService/bin/Debug/HCVKAService.pfx' to '/home/knight47/Projects/DaaSXpert-Client/ubuntu/src/agent/Package/temp/usr/lib/HCVKAService/HCVKAService.pfx'
		Executing: fakeroot dpkg-deb -v --build Package/temp
		dpkg-deb: building package 'hcvkaservice' in 'Package/temp.deb'.
		Copying file from '/home/knight47/Projects/DaaSXpert-Client/ubuntu/src/agent/Package/temp.deb' to '/home/knight47/Projects/DaaSXpert-Client/ubuntu/src/agent/Package/HCVKAService_1.0.903.5_amd64.deb'
		Deleting file '/home/knight47/Projects/DaaSXpert-Client/ubuntu/src/agent/Package/temp.deb'
Done building project "/home/knight47/Projects/DaaSXpert-Client/ubuntu/src/agent/Build.proj".

Build succeeded.
	 0 Warning(s)
	 0 Error(s)

Time Elapsed 00:00:03.4282520
```



## 5. Install the package

```sh
$ sudo apt install ./DaaSXpertClient_
```

