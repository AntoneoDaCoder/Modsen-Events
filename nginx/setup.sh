#!/bin/bash
cp /certs/cert.crt /usr/local/share/ca-certificates/cert.crt
cp /certs/key.key /etc/ssl/private/key.key
update-ca-certificates

nginx -g "daemon off;"