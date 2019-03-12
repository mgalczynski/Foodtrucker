#!/bin/sh
docker-compose -f development.yml -p foodtrucker down --remove-orphans --rmi all -v
docker-compose -f development.yml -p foodtrucker pull
docker-compose -f development.yml -p foodtrucker up -d
