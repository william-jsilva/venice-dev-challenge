#!/bin/bash

echo "🚀 Starting Venice Orders Application..."

# Check if Docker is running
if ! docker info > /dev/null 2>&1; then
    echo "❌ Docker is not running. Please start Docker first."
    exit 1
fi

echo "✅ Docker is running"

# Stop any existing containers
echo "🛑 Stopping existing containers..."
docker-compose down

# Build and start the application
echo "🔨 Building and starting services..."
docker-compose up --build -d

# Wait for services to be ready
echo "⏳ Waiting for services to be ready..."
sleep 30

# Check if the API is responding
echo "🔍 Checking API health..."
max_attempts=30
attempt=0

while [ $attempt -lt $max_attempts ]; do
    attempt=$((attempt + 1))
    if curl -f -s http://localhost:5000/health/live > /dev/null; then
        echo "✅ API is ready!"
        break
    else
        if [ $attempt -eq $max_attempts ]; then
            echo "❌ API is not responding after $max_attempts attempts"
            echo "📋 Checking container logs..."
            docker-compose logs venice-orders-api
            exit 1
        fi
        echo "⏳ Waiting for API to be ready... (attempt $attempt/$max_attempts)"
        sleep 10
    fi
done

echo ""
echo "🎉 Venice Orders Application is ready!"
echo ""
echo "📋 Available endpoints:"
echo "   • API: http://localhost:5000"
echo "   • Swagger: http://localhost:5000/swagger"
echo "   • Health Check: http://localhost:5000/health"
echo "   • RabbitMQ Management: http://localhost:15672 (guest/guest)"
echo ""
echo "🔑 Default JWT Token for testing:"
echo "   Use the login endpoint to get a token"
echo ""
echo "📝 To stop the application, run: docker-compose down"
echo "📝 To view logs, run: docker-compose logs -f"
