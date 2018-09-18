#!/bin/sh
 set -ex
 if [ "$RUN_E2E_TESTS" != "true" ]; then
  echo "Skipping end to end tests."
else
  echo "Running end to end tests..."
  wget https://github.com/segmentio/library-e2e-tester/releases/download/0.2.1/tester_linux_amd64 -O tester
  chmod +x tester
  ./tester -path='dotnet run -- '

  # Run code coverage
  curl https://codecov.io/bash --output codecov
  chmod 777 codecov
  ./codecov

  echo "End to end tests completed!"
fi