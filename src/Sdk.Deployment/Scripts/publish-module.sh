#!/usr/bin/env bash

#######################################
# Variables
#######################################

# defaults
BACKENDMODULE="${1}.Backend"
CLIENTMODULE="${1}.Client"

BACKENDPROJECT=$(readlink -f "${2}/$BACKENDMODULE/$BACKENDMODULE.csproj")
CLIENTPROJECT=$(readlink -f "${2}/$CLIENTMODULE/$CLIENTMODULE.csproj")

PLATFORM="${3:-linux-x64}"
OUTPUTPATH="${4:-../publish}"
CONFIGURATION="${5:-Release}"

#######################################
# Definitions
########################################

publish_project() {
    # arguments that fit for backend and client
    local publish_args=("--output" "$OUTPUTPATH" "--configuration" "$CONFIGURATION" "--runtime" "$PLATFORM" "--verbosity" "minimal" "--nologo" "--no-self-contained" "--no-restore" "-p" "CompressionEnabled=false" "-p" "SatelliteResourceLanguages=\"en;de\"")

    echo "publish ${1}"
    dotnet publish "${2}" "${publish_args[@]}"
}

main() {
    # publish backend and client into same folder
    if [ -f "$BACKENDPROJECT" ]; then
        publish_project "$BACKENDMODULE" "$BACKENDPROJECT"
    fi

    if [ -f "$CLIENTPROJECT" ]; then
        publish_project "$CLIENTMODULE" "$CLIENTPROJECT"

        local wwwroot_path="${OUTPUTPATH}/wwwroot"
        if [ ! -d "$wwwroot_path" ]; then
            echo "[WARN] ${CLIENTMODULE} has no wwwroot folder"
            exit
        fi

        local replace_pattern="@import '_content\/ViciOne\.((Suite\.Sdk\.Client)|(Ui.Shared)|(Ui.Blazor)|(Ui.MonochromeIcons)).*?bundle.scp.css';"
        local bundle_pattern='^[^@import[^\s].*]'

        for css_bundle in "$wwwroot_path"/*.styles.css; do
            local css
            css=$(cat "$css_bundle")

            # remove @import statements

            # code is adopted from "DeleteCssImportTask" implemented in "Directory.Build.props".
            # do not change without updating "Directory.Build.props" too !!!

            # https://regex101.com/r/lpXZSf/1
            local css_sanitized
            css_sanitized=$(echo "$css" | sed --regexp-extended "s/$replace_pattern//g" | grep --only-matching '[^[:space:]].*[^[:space:]]')

            echo "$css_sanitized" > "$css_bundle"

            local css_bundle_name
            css_bundle_name=$(basename "$css_bundle")

            if [[ "$css_sanitized" =~ $bundle_pattern ]]; then
                echo "updated css bundle $css_bundle_name - styles bundled"
            else
                echo "updated css bundle $css_bundle_name - only imports"
            fi
        done
    fi
}

main
