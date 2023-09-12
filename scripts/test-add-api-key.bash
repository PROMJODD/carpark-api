#!/bin/bash

. ./export.bash

APIKEY=demo-api-key
DAT_TEMPLATE=template.json

cat << EOF > ${DAT_TEMPLATE}
{
  "ApiKey": "secretxAIdboww0jKeykks98SwOe",
  "KeyDescription": "API for CCTV file upload",
  "RolesList" : "UPLOADER"
}
EOF

curl -s -X POST ${ENDPOINT_ADD_KEY} -u ${AUTH_USER}:${AUTH_PASSWORD} \
    -d "@${DAT_TEMPLATE}" \
    -H "Content-Type: application/json"
