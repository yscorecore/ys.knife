echo "docker compose ready, report to host."
wget "http://host.docker.internal:${REPORT_TO_HOST_PORT:-8901}"