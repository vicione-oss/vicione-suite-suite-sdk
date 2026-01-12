#!/usr/bin/env bash

#######################################
# Variables
#######################################

# defaults
ARTIFACTPATH="${1:-../publish}"
SKIPASSEMBLIES="${2:-false}"
VERBOSEREMOVE="${3:-false}"

RESOLVEDPATH=$(realpath --strip "$ARTIFACTPATH")

#######################################
# Definitions
########################################

get_folder_size() {
   du --summarize "$ARTIFACTPATH" | grep --only-matching --extended-regexp '^[0-9]+'
}

remove_assemblies() {
    if [ "$SKIPASSEMBLIES" == false ]; then
        local filepath="${RESOLVEDPATH}/${1}"
        filepath=${filepath//"//"/"/"}

        if [ -f "$filepath" ]; then
            if [ "$VERBOSEREMOVE" == true ]; then
                echo "remove file: $filepath"
            fi

            rm "$filepath"
        fi

        if [ -d "$filepath" ]; then
            if [ "$VERBOSEREMOVE" == true ]; then
                echo "remove directory: $filepath"
            fi

            rm --recursive --force "$filepath"
        fi
    fi
}

main() {
    if [ ! -d "$RESOLVEDPATH" ]; then
        echo -e "\033[0;31m artifact Path: $ARTIFACTPATH does not exist\033[0m"
        return
    fi

    local size_before
    local source_file

    size_before=$(get_folder_size "$RESOLVEDPATH")
    source_file=$(readlink --canonicalize "$(dirname "$0")/suite-libraries.txt")

    if [ "$SKIPASSEMBLIES" == true ]; then
        echo "skip assembly cleanup"
    fi

    # remove all files and folders listed in suite-libraries
    while read -r line
    do
        if echo "$line" | grep --quiet '#SDK:*' ; then
            local sdk_version=${line//"#SDK:"/}
            echo "using $sdk_version"
            continue
        fi

        # Publish module uses SatelliteResourceLanguages flag. Language line just need to be omitted
        if echo "$line" | grep --quiet '#LANGUAGES:*' ; then
            local languages=${line//"#LANGUAGES:"/}
            echo "language support $languages"
            continue
        fi

        # don't remove assemblies for core or ui hosts
        remove_assemblies "$line"

    done < "$source_file"

    local size_after
    size_after=$(get_folder_size "$RESOLVEDPATH")

    if [[ "$size_before" -gt 0 && $size_after -gt 0 ]]; then
        local sum
        sum=$((("$size_before" - "$size_after") / 1024))
        local perc=$((100 - ("$size_after" * 100 / "$size_before")))
        echo "totally removed $sum mb ($perc %)"
    fi
}

main
