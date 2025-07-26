#!/bin/bash

# SpeedType Application Startup Script
echo "🚀 Starting SpeedType Application..."

# Function to clean up processes on exit
cleanup() {
    echo "🛑 Stopping applications..."
    kill $BACKEND_PID $FRONTEND_PID 2>/dev/null
    exit
}

# Set up trap to cleanup on script exit
trap cleanup EXIT INT TERM

# Start Backend API
echo "📡 Starting Backend API on http://localhost:5000..."
cd speedtype.API
dotnet run --urls=http://localhost:5000 &
BACKEND_PID=$!
cd ..

# Wait a moment for backend to start
sleep 3

# Start Frontend
echo "🎨 Starting Frontend on http://localhost:4200..."
cd speedtype-frontend
ng serve --port 4200 --open &
FRONTEND_PID=$!
cd ..

echo "✅ Both applications are starting up!"
echo "📡 Backend API: http://localhost:5000"
echo "🎨 Frontend: http://localhost:4200"
echo "📚 API Documentation: http://localhost:5000"
echo ""
echo "Press Ctrl+C to stop both applications"

# Wait for both processes
wait 