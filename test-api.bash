#!/bin/bash

. ./export.bash

curl -s -X POST ${UPLOAD_ENDPOINT} -u ${AUTH_USER}:${AUTH_PASSWORD}
