#!/bin/bash

dotnet publish ../Mediasorter.csproj --configuration Release --self-contained --runtime linux-x64

if [ "$1" == "--install" ]
then
	echo "Copying mediasorter to /usr/local/bin. Need superuser rights for that."
	sudo rm /usr/local/bin/mediasorter
	sudo cp ../bin/Release/net6.0/linux-x64/publish/Mediasorter /usr/local/bin/mediasorter
	echo "Done."
fi
