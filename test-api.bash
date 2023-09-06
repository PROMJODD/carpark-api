#!/bin/bash

. ./export.bash
FILE_TO_UPLOAD="/c/Users/User/Desktop/TICKET-001.20230829_1236.jpg"

curl -s -X POST ${UPLOAD_ENDPOINT} -u ${AUTH_USER}:${AUTH_PASSWORD} -F "image=@${FILE_TO_UPLOAD}"
