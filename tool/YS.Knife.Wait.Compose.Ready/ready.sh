STATUS_FILE_PATH=/var/status/${STATUS_FILE-__docker_compose_ready__}

echo 'all dependencies ready at ' > $STATUS_FILE_PATH 

echo "docker compose ready, create status flag file '$STATUS_FILE_PATH'."