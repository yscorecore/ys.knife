### How to use

```yaml
version: '3.3'

services:
  service1:
    ......
  service2:
    ......
  wait-compose-ready:
    image: ysknife/wait-compose-ready
    volumes:
      - ./tmp:/var/status
    environment:
      STATUS_FILE: ${STATUS_FILE}
      WAIT_HOSTS: service1:80, service2:81
```

