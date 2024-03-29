#!/bin/bash

. ./export-dev.bash
FILE_TO_UPLOAD="/d/LPR/Feb_2_2020/2020-02-18/0715327008_1R2005.jpg"

REF_ID=$(date +"%s")
UPLOADED_PATH=${FILE_TO_UPLOAD}
COMPANY_ID=001
BRANCH_ID=AB001
EQUIPMENT_ID=CCTV-001
UPLOADED_SIZE=$(wc -c < ${FILE_TO_UPLOAD})

curl -v -X POST ${UPLOAD_ENDPOINT} -u ${AUTH_USER}:${AUTH_PASSWORD_UPLOADER} -F "image=@${FILE_TO_UPLOAD}" \
   -H "Prom-Ref-ID: ${REF_ID}" \
   -H "Prom-Upload-Path: ${UPLOADED_PATH}" \
   -H "Prom-Upload-Size: ${UPLOADED_SIZE}" \
   -H "Prom-Company: ${COMPANY_ID}" \
   -H "Prom-Branch: ${BRANCH_ID}" \
   -H "Prom-Equipment-ID: ${EQUIPMENT_ID}" \
   -H "Prom-Upload-User: ${AUTH_USER}" \
