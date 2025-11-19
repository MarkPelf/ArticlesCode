FROM nginx

# Copy nginx configuration
COPY nginx.conf /etc/nginx/nginx.conf

# Create directory for SSL certificates
RUN mkdir -p /etc/nginx/ssl

# Copy SSL certificates
COPY ssl/nginx.crt /etc/nginx/ssl/nginx.crt
COPY ssl/nginx.key /etc/nginx/ssl/nginx.key

EXPOSE 80 443

CMD ["nginx", "-g", "daemon off;"]
