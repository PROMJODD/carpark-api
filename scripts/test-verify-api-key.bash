#!/bin/bash

. ./export-dev.bash
APIKEY=secretxAseKeykks9803SeeeubXX

curl -s -X GET ${ENDPOINT_VERIFY_KEY}/${APIKEY} -u ${AUTH_USER}:${AUTH_PASSWORD}
