#!/bin/bash

. ./export-dev.bash

DAT_TEMPLATE=template.json

cat << EOF > ${DAT_TEMPLATE}
{
  "UserId": "f30b956d-d44b-4111-997c-fc291a654026",
  "UserName": "pjamenaja",
  "RolesList": "OWNER"
}
EOF

curl -s -X POST ${ENDPOINT_ADD_ORG_USER} -u ${AUTH_USER}:${AUTH_PASSWORD} \
    -d "@${DAT_TEMPLATE}" \
    -H "Content-Type: application/json"
