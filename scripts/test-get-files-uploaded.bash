#!/bin/bash

. ./export-dev.bash

curl -s -X GET "${ENDPOINT_GET_FILES}" \
    -u ${AUTH_USER}:${AUTH_PASSWORD}

curl -s -X GET "${ENDPOINT_GET_FILES}Count" \
    -u ${AUTH_USER}:${AUTH_PASSWORD}