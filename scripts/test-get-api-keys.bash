#!/bin/bash

. ./export.bash

curl -s -X POST ${ENDPOINT_GET_KEYS} -u ${AUTH_USER}:${AUTH_PASSWORD}