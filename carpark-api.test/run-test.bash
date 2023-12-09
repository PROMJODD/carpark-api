#!/bin/bash

COVERAGE_DIR=coverage

rm -rf ${COVERAGE_DIR} coveragereport

dotnet test --collect:"XPlat Code Coverage"  --results-directory":${COVERAGE_DIR}"

COVERAGE_FILE=$(find ${COVERAGE_DIR} -name '*.xml' | head -1)

reportgenerator \
    -reports:"${COVERAGE_FILE}" \
    -targetdir:"coveragereport" \
    -reporttypes:Html
