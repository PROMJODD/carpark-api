#!/bin/bash

. ./export-dev.bash

curl -s -X GET ${ENDPOINT_GET_USERS} -u ${AUTH_USER}:${AUTH_PASSWORD_GLB}
