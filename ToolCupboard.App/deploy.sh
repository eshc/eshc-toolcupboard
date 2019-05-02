#!/bin/bash

FRAMEWORK="netcoreapp2.2"
RUNTIME="linux-arm"

dotnet publish -f "$FRAMEWORK" -r "$RUNTIME"

if [ "$?" -eq 0 ]; then
  echo "Successfully compiled the applicatino."

  if [ "$1" = "full" ]; then
    echo "Performing a full copy to device."
    scp -r bin/Debug/$FRAMEWORK/$RUNTIME/publish/* rpi:~/tc
  else
    echo "Performing a minimal copy to device."
	scp bin/Debug/$FRAMEWORK/$RUNTIME/publish/ToolCupboard* rpi:~/tc
  fi
fi
