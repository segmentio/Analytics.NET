#!/bin/sh
set -ex
if [ "$RUN_E2E_TESTS" != "true" ]; then
  echo "Skipping end to end tests."
else
  echo "Running end to end tests..."
  wget https://github.com/segmentio/library-e2e-tester/releases/download/0.2.1/tester_linux_amd64 -O tester
  chmod +x tester

  # Hide system environment variable values
  echo "./tester -path='./analytics.sh'"
  { ./tester -path='./analytics.sh' -segment-write-key="$SEGMENT_WRITE_KEY" -webhook-auth-username="$WEBHOOK_AUTH_USERNAME" -webhook-bucket="$WEBHOOK_BUCKET"; } 2> /dev/null

  # Run code coverage
  wget https://codecov.io/bash -O codecov
  chmod +x codecov
  ./codecov

  echo "End to end tests completed!"
fi