FROM alpine

ADD https://github.com/ufoscout/docker-compose-wait/releases/download/2.7.3/wait /wait
RUN chmod +x /wait

ADD ./ready.sh /ready.sh
RUN chmod +x /ready.sh

CMD /wait && /ready.sh