#!/bin/bash

. ./export.bash

curl -s -X GET ${ENDPOINT_GET_ORG} -u ${AUTH_USER}:${AUTH_PASSWORD}
