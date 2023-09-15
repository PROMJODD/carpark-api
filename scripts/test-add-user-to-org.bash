#!/bin/bash

. ./export-dev.bash

DAT_TEMPLATE=template.json

cat << EOF > ${DAT_TEMPLATE}
{
  "UserId": "5603e669-ec83-4015-83f8-cb67c3e80954",
  "UserName": "supreeya",
  "RolesList": "OWNER"
}
EOF

curl -s -X POST ${ENDPOINT_ADD_ORG_USER} -u ${AUTH_USER}:${AUTH_PASSWORD} \
    -d "@${DAT_TEMPLATE}" \
    -H "Content-Type: application/json"
