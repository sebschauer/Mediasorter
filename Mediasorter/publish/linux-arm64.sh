#!/bin/bash

dotnet publish ../Mediasorter.csproj --configuration Release --self-contained --runtime linux-arm64

if [ "$1" == "--install" ]
then
	path=/usr/local/bin
	filename=mediasorter
	
	if [ -f "$path/$filename" ]
	then
		echo "Removing old version of $filename from $path. Need superuser rights for that."
		sudo rm "$path/$filename"
	fi
	
	echo "Copying $filename to $path. Need superuser rights for that."
	sudo cp ../bin/Release/net6.0/linux-arm64/publish/Mediasorter "$path/$filename"
	echo "Done."
fi
