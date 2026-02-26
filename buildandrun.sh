#!/bin/bash

# Build and run the client and server in development mode

# Start the server
pushd ./api
dotnet run &
SERVER_PID=$!   
popd

# Start the client
pushd ./client
npm install
npm run dev &
CLIENT_PID=$!
popd

trap 'kill $SERVER_PID $CLIENT_PID 2>/dev/null' EXIT INT TERM

until curl -s http://localhost:5173 > /dev/null 2>&1; do
  sleep 0.5
done
xdg-open http://localhost:5173

# Wait for both processes to exit
wait $SERVER_PID
wait $CLIENT_PID

