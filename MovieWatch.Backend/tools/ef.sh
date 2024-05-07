#!/usr/bin/env bash
cd "$(dirname "$0")"
~/.dotnet/tools/dotnet-ef "$@" --project ../MovieWatch.Data --startup-project ../MovieWatch.Api
