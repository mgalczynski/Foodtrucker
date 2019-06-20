#!/bin/sh

docker build -t docker.miroslawgalczynski.com/dotnet:2.2-sdk-node -f Dockerfile-dotnet-sdk-node .
docker-compose -f manual-test-local.yml -p foodtrucker up -d --build
sleep 60
./init_manual_test_server.sh
