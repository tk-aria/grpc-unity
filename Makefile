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
