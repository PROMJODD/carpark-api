#!/bin/bash

. ./export.bash

TOKEN=eyJhbGciOiJSUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICI4cEd1bk9aMC1vLUdCV3FTUTM1U05XcUNXT1BMOG1fN200V2lhR2xwYUxVIn0.eyJleHAiOjE2OTQ3NTQ1NDQsImlhdCI6MTY5NDc1NDI0NCwiYXV0aF90aW1lIjoxNjk0NzU0MTYwLCJqdGkiOiIwNzRhMmQ2YS04OTViLTQyY2YtOTI0NS03NjI0MzVmZGUxYmEiLCJpc3MiOiJodHRwczovL2tleWNsb2FrLnByb21pZC5wcm9tLmNvLnRoL2F1dGgvcmVhbG1zL3Byb21pZCIsImF1ZCI6ImFjY291bnQiLCJzdWIiOiI0NWNjY2E2Ni02ZDBlLTQ0MTUtOWM3Yy0xODA4ZjNjZGE1MzEiLCJ0eXAiOiJCZWFyZXIiLCJhenAiOiJwcm9tbHByIiwibm9uY2UiOiIxNjk0NzQwMTk3OTM3LTZONSIsInNlc3Npb25fc3RhdGUiOiIyZWNlM2Y1ZC1mNmE3LTRlZGItODcwNi1kMzY1MTg0ZTMzYzAiLCJhY3IiOiIwIiwicmVhbG1fYWNjZXNzIjp7InJvbGVzIjpbIm9mZmxpbmVfYWNjZXNzIiwidW1hX2F1dGhvcml6YXRpb24iLCJkZWZhdWx0LXJvbGVzLXByb21pZCJdfSwicmVzb3VyY2VfYWNjZXNzIjp7ImFjY291bnQiOnsicm9sZXMiOlsibWFuYWdlLWFjY291bnQiLCJtYW5hZ2UtYWNjb3VudC1saW5rcyIsInZpZXctcHJvZmlsZSJdfX0sInNjb3BlIjoib3BlbmlkIGVtYWlsIHByb2ZpbGUiLCJzaWQiOiIyZWNlM2Y1ZC1mNmE3LTRlZGItODcwNi1kMzY1MTg0ZTMzYzAiLCJlbWFpbF92ZXJpZmllZCI6ZmFsc2UsIm5hbWUiOiJTZXVicG9uZyBNb25zYXIiLCJwcmVmZXJyZWRfdXNlcm5hbWUiOiJwamFtZW5hamEiLCJnaXZlbl9uYW1lIjoiU2V1YnBvbmciLCJmYW1pbHlfbmFtZSI6Ik1vbnNhciIsImVtYWlsIjoicGphbWUuZmJAZ21haWwuY29tIn0.ZOkHfXmq9uGQUrkvM4W1JfDerBewPy8B6gqON2jEuzDoZAmRnmY9YA25Bi47eylztVfDBrQ-a920jWUW_8qdWlK91PRX9Rnu3vcwFRWHEXkkhDPrP6JGxtB55aZTS-s-PZxhG2B5r2YLqEXjNtkDDwKx7VFNEEY85LMZjAznHlcm-5Q5zgeRrZTghHyROI4e5puFUHCmYdP7jl-tY0-ExzTPhdyn-rxcgeSxyICNF3JIPyrxNDC_55xQOLW3sR57KThr-bPRKj--63DD4OgcjxdcOWDgYmtzFDdf7scozVsqwEep1vDTlwiRCleAi5K7ABpHMtCaZ0PJ6iDQIqZxlg
TOKEN_BASE64=$(echo -n ${TOKEN} | base64 -w0 )

curl -s -X GET ${ENDPOINT_GET_USERS} -u ${AUTH_USER}:${AUTH_PASSWORD_GLB}

#-H "Authorization: Bearer ${TOKEN_BASE64}"
# -u ${AUTH_USER}:${AUTH_PASSWORD_GLB}