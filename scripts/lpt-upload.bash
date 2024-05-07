#!/bin/bash

FILE_TO_UPLOAD="/d/LPR/Feb_2_2020/2020-02-18/0715327008_1R2005.jpg"

. ./export-dev.bash

if [[ ${FILE_TO_UPLOAD} == "" ]]; 
then
    FILE_TO_UPLOAD="/c/Users/User/Desktop/TICKET-001.20230829_1236.jpg"
fi

curl -v -H "Authorization: Bearer ${LPR_TOKEN}" -F "image=@${FILE_TO_UPLOAD}" ${LPR_ENDPOINT}

