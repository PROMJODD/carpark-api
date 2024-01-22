#!/bin/bash

. ./export-dev.bash

# Must use echo -n to remove new line !!!
BASE64_AUTH_JWT=$(echo -n ${AUTH_JWT} | base64 -w0)

curl -s -X GET "${ENDPOINT_GET_FILES}" \
    -H "Authorization: Bearer ${BASE64_AUTH_JWT}"
    #-u ${AUTH_USER}:${AUTH_PASSWORD}

curl -s -X GET "${ENDPOINT_GET_FILES}Count" \
    -H "Authorization: Bearer ${BASE64_AUTH_JWT}"
    #-u ${AUTH_USER}:${AUTH_PASSWORD}
