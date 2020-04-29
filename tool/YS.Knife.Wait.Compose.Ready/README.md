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
    environment:
      REPORT_TO_HOST_PORT: ${REPORT_TO_HOST_PORT:-8901}
      WAIT_HOSTS: service1:80, service2:81
```

