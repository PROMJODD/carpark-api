#!/bin/bash

. ./export-dev.bash

USER=tpthemes@gmail.com #bancherd@cybertracx.com

curl -X GET ${ENDPOINT_GET_USER_ALLOWED_ORG}/${USER} -u ${AUTH_ADMIN_USER}:${AUTH_ADMIN_PASSWORD}
