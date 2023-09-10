#!/bin/bash

. ./export-dev.bash
KEY_ID="b19aa9a3-0277-4871-81c7-2a3cb6622ce2"

curl -s -X DELETE ${ENDPOINT_DELETE_KEY}/${KEY_ID} -u ${AUTH_USER}:${AUTH_PASSWORD}
