#!/bin/bash

echo "Waiting for SQL Server to be ready..."
until /opt/mssql-tools/bin/sqlcmd -S sqlserver -U sa -P YourStrong@Passw0rd -Q "SELECT 1" &> /dev/null
do
  echo "SQL Server is not ready yet. Waiting..."
  sleep 2
done
echo "SQL Server is ready!"

echo "Waiting for MongoDB to be ready..."
until mongosh --host mongodb --port 27017 --eval "db.adminCommand('ping')" &> /dev/null
do
  echo "MongoDB is not ready yet. Waiting..."
  sleep 2
done
echo "MongoDB is ready!"

echo "Waiting for Redis to be ready..."
until redis-cli -h redis ping &> /dev/null
do
  echo "Redis is not ready yet. Waiting..."
  sleep 2
done
echo "Redis is ready!"

echo "Waiting for RabbitMQ to be ready..."
until curl -s -u guest:guest http://rabbitmq:15672/api/overview &> /dev/null
do
  echo "RabbitMQ is not ready yet. Waiting..."
  sleep 2
done
echo "RabbitMQ is ready!"

echo "All services are ready! Starting application..."
