#!/bin/bash

. ./export-dev.bash
APIKEY=demo-api-key

curl -s -X GET ${ENDPOINT_VERIFY_KEY}/${APIKEY} -u ${AUTH_USER}:${AUTH_PASSWORD}
