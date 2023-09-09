#!/bin/bash

. ./export-dev.bash

ENDPOINT=http://localhost:5186/api/Organization/org/default/action/GetOrganization

curl -s -X GET ${ENDPOINT} -u ${AUTH_USER}:${AUTH_PASSWORD}
