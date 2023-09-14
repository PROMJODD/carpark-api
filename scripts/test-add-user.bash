#!/bin/bash

. ./export-dev.bash

DAT_TEMPLATE=template.json

cat << EOF > ${DAT_TEMPLATE}
{
  "UserName": "seubpong.mon",
  "UserEmail": "seubpong.mon@napbiotec.io"
}
EOF

curl -s -X POST ${ENDPOINT_ADD_USER} -u ${AUTH_USER}:${AUTH_PASSWORD_GLB} \
    -d "@${DAT_TEMPLATE}" \
    -H "Content-Type: application/json"
