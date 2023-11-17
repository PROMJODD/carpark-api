#!/bin/bash

. ./export-dev.bash

DAT_TEMPLATE=template.json

cat << EOF > ${DAT_TEMPLATE}
{
  "OrgCustomId": "promjodd",
  "OrgName": "PROMJODD",
  "OrgDescription": "Organization for Promjodd"
}
EOF

curl -s -X POST ${ENDPOINT_ADD_ORG} -u ${AUTH_USER}:${AUTH_PASSWORD_GLB} \
    -d "@${DAT_TEMPLATE}" \
    -H "Content-Type: application/json"

#####

cat << EOF > ${DAT_TEMPLATE}
{
  "UserId": "5603e669-ec83-4015-83f8-cb67c3e80954",
  "UserName": "supreeya",
  "RolesList": "OWNER",
  "OrgCustomId": "promjodd"
}
EOF

ENDPOINT_ADD_ORG_USER=http://localhost:5186/api/Organization/org/global/action/AdminAddUserToOrganization
curl -s -X POST ${ENDPOINT_ADD_ORG_USER} -u ${AUTH_USER}:${AUTH_PASSWORD_GLB} \
    -d "@${DAT_TEMPLATE}" \
    -H "Content-Type: application/json"

