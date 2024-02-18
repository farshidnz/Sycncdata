#!/bin/bash

dotnet test --logger:"junit;LogFilePath=/app/testout/{assembly}.xml" test/AccountsSyncData.Tests/AccountsSyncData.Tests.csproj

chown -R $1 /app