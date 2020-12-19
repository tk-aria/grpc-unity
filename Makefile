# Makefile
# help:
#    all)
#    make --always-make
.DEFAULT_GOAL := help
MAKEFILE_DIR := $(dir $(lastword $(MAKEFILE_LIST)))

.PHONY: help
help:
	@echo "\n>> help [ command list ]"
	@echo 'Usage: make [target]'
	@echo ''
	@echo 'Targets:'
	@grep -E '^.PHONY: [a-zA-Z_-]+.*?##' $(MAKEFILE_LIST) | awk 'BEGIN {FS = " "}; {printf "\033[35m%-30s\033[32m %s\n", $$2, $$4}'
	@echo ""

.PHONY: start_unity ## [category]`description`.
start_unity:
	open ./Assets/Demo/Demo.unity

.PHONY: delete_all_csproj ## [category]`description`.
delete_all_csproj:
	rm *.csproj
	rm *.sln

.PHONY: dotnet_test ## [category]`description`.
dotnet_test:
	@echo 'hello world'

.PHONY: update_mono_darwin ## [category]`description`.
update_mono_darwin:
	curl -L -O https://download.mono-project.com/archive/mdk-latest-stable.pkg
	open mdk-latest-stable.pkg

.PHONY: dl_donetcore_script ## [category]`description`.
dl_donet_script:
	curl -L -O https://dot.net/v1/dotnet-install.sh
	chmod 744 dotnet-install.sh
	curl -L -O https://dot.net/v1/dotnet-install.ps1

.PHONY: publish_npmjs ## [category]`description`.
publish_npmjs:
	npm publish Assets/GrpcUnity

.PHONY: update_protobuf ## [category]`description`.
update_protobuf:
	# <error>
	# Error: Could not load signature of Google.Protobuf.ByteString:get_Span due to: Could not load file or assembly 'System.Memory, Version=4.0.1.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51' or one of its dependencies. assembly:System.Memory, Version=4.0.1.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51 type:<unknown type> member:(null) signature:<none>
	# Unloading broken assembly Assets/Plugins/Google.Protobuf/lib/net45/Google.Protobuf.dll, this assembly can cause crashes in the runtime
	cd .third-party/protobuf/csharp/src/Google.Protobuf && \
		dotnet publish -c Release -f net45
	cp -f ./.third-party/protobuf/csharp/src/Google.Protobuf/bin/Release/net45/publish/Google.Protobuf.dll ./Assets/GrpcUnity/Plugins/Google.Protobuf/lib/net45/
