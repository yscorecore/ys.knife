
### How it works

Use [docker-compose-wait](https://github.com/ufoscout/docker-compose-wait) to report the container port status to a special file(status.txt), and we mapping a host text file to it, then we can get the status about the container ports.  


### How to use
```yaml
version: '3.3'

services:
  mongo:
    image: mongo:3.4
    hostname: mongo
    ports:
      - "17017:27017"

  postgres:
    image: "postgres:9.4"
    hostname: postgres
    ports:
      - "15432:5432"

  mysql:
    image: "mysql:5.7"
    hostname: mysql
    ports:
      - "13306:3306"

  wait-compose-ready:
    image: ysknife/wait-compose-ready
    volumes:
    - <%your status file in host%>:/status.txt
    environment:
      WAIT_HOSTS: mongo:27017, mysql:3306, postgres:5432
```
**Remember that the port in `WAIT_HOSTS` is container port, not the host port.**
