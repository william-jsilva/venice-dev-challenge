#!/usr/bin/env pwsh

Write-Host "🚀 Starting Venice Orders Application..." -ForegroundColor Green

# Check if Docker is running
try {
    docker version | Out-Null
    Write-Host "✅ Docker is running" -ForegroundColor Green
} catch {
    Write-Host "❌ Docker is not running. Please start Docker Desktop first." -ForegroundColor Red
    exit 1
}

# Stop any existing containers
Write-Host "🛑 Stopping existing containers..." -ForegroundColor Yellow
docker-compose down

# Build and start the application
Write-Host "🔨 Building and starting services..." -ForegroundColor Yellow
docker-compose up --build -d

# Wait for services to be ready
Write-Host "⏳ Waiting for services to be ready..." -ForegroundColor Yellow
Start-Sleep -Seconds 30

# Check if the API is responding
Write-Host "🔍 Checking API health..." -ForegroundColor Yellow
$maxAttempts = 30
$attempt = 0

do {
    $attempt++
    try {
        $response = Invoke-RestMethod -Uri "http://localhost:5000/health/live" -Method GET -TimeoutSec 5
        Write-Host "✅ API is ready!" -ForegroundColor Green
        break
    } catch {
        if ($attempt -ge $maxAttempts) {
            Write-Host "❌ API is not responding after $maxAttempts attempts" -ForegroundColor Red
            Write-Host "📋 Checking container logs..." -ForegroundColor Yellow
            docker-compose logs venice-orders-api
            exit 1
        }
        Write-Host "⏳ Waiting for API to be ready... (attempt $attempt/$maxAttempts)" -ForegroundColor Yellow
        Start-Sleep -Seconds 10
    }
} while ($true)

Write-Host ""
Write-Host "🎉 Venice Orders Application is ready!" -ForegroundColor Green
Write-Host ""
Write-Host "📋 Available endpoints:" -ForegroundColor Cyan
Write-Host "   • API: http://localhost:5000" -ForegroundColor White
Write-Host "   • Swagger: http://localhost:5000/swagger" -ForegroundColor White
Write-Host "   • Health Check: http://localhost:5000/health" -ForegroundColor White
Write-Host "   • RabbitMQ Management: http://localhost:15672 (guest/guest)" -ForegroundColor White
Write-Host ""
Write-Host "🔑 Default JWT Token for testing:" -ForegroundColor Cyan
Write-Host "   Use the login endpoint to get a token" -ForegroundColor White
Write-Host ""
Write-Host "📝 To stop the application, run: docker-compose down" -ForegroundColor Yellow
Write-Host "📝 To view logs, run: docker-compose logs -f" -ForegroundColor Yellow
