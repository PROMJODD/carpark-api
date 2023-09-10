#!/bin/bash

. ./export-dev.bash

APIKEY=demo-api-key
DAT_TEMPLATE=template.json

cat << EOF > ${DAT_TEMPLATE}
{
  "ApiKey": "secretxAseKeykks9803See",
  "KeyDescription": "API for CCTV file upload"
}
EOF

curl -s -X POST ${ENDPOINT_ADD_KEY} -u ${AUTH_USER}:${AUTH_PASSWORD} \
    -d "@${DAT_TEMPLATE}" \
    -H "Content-Type: application/json"
