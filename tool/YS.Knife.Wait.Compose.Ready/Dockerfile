FROM alpine

ADD https://github.com/ufoscout/docker-compose-wait/releases/download/2.7.3/wait /wait
RUN chmod +x /wait

ADD ./scripts/ /
RUN chmod +x /*.sh

CMD /wait && /ready.sh