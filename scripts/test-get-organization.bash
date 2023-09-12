#!/bin/bash

. ./export-dev.bash

curl -v -X GET ${ENDPOINT_GET_ORG} -u ${AUTH_USER}:${AUTH_PASSWORD}
