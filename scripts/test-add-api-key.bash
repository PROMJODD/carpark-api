#!/bin/bash

. ./export.bash

APIKEY=demo-api-key
DAT_TEMPLATE=template.json

cat << EOF > ${DAT_TEMPLATE}
{
  "ApiKey": "xxxxxxx",
  "KeyDescription": "API for web uploader",
  "RolesList" : "UPLOADER"
}
EOF

curl -s -X POST ${ENDPOINT_ADD_KEY} -u ${AUTH_USER}:${AUTH_PASSWORD} \
    -d "@${DAT_TEMPLATE}" \
    -H "Content-Type: application/json"
