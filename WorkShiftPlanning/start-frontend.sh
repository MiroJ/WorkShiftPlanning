#!/bin/bash

echo "========================================"
echo "  Work Shift Planning System"
echo "  Frontend-Only Mode"
echo "========================================"
echo ""

cd ClientApp

echo "Installing dependencies..."
npm install

if [ $? -ne 0 ]; then
    echo "Error installing dependencies!"
    exit 1
fi

echo ""
echo "Starting Angular development server..."
echo ""
echo "The application will be available at:"
echo "  http://localhost:4200"
echo ""
echo "Press Ctrl+C to stop the server"
echo ""

npm start
