#!/bin/bash

. ./export-dev.bash

BASE64_AUTH_JWT=$(echo ${AUTH_JWT} | base64 -w0)

curl -v -X GET "${ENDPOINT_GET_FILES}" \
    -H "Authorization: Bearer ${BASE64_AUTH_JWT}"
    #-u ${AUTH_USER}:${AUTH_PASSWORD}

#curl -s -X GET "${ENDPOINT_GET_FILES}Count" \
#    -H "Authorization: Bearer ${BASE64_AUTH_JWT}"
#    #-u ${AUTH_USER}:${AUTH_PASSWORD}
