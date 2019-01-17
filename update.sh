#!/bin/sh
docker-compose -f development.yml -p foodtrucker down --remove-orphans
docker-compose -f development.yml -p foodtrucker pull
docker-compose -f development.yml -p foodtrucker up -d
