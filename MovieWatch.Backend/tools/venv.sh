#!/usr/bin/env bash
cd "$(dirname "$0")"
export PIPENV_VENV_IN_PROJECT=1
pipenv install
unset PIPENV_VENV_IN_PROJECT