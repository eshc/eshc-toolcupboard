#!/bin/bash

dotnet publish -f netcoreapp2.2 -r linux-arm

if [ "$?" -eq 0 ]; then
  echo "Successfully compiled the applicatino."

  if [ "$1" = "full" ]; then
    echo "Performing a full copy to device."
    scp -r bin/Debug/netcoreapp2.1/linux-arm/publish/* rpi:~/guiapp
  else
    echo "Performing a minimal copy to device."
	scp bin/Debug/netcoreapp2.1/linux-arm/publish/MyApp* rpi:~/guiapp
  fi
fi
