# Build stage
FROM node:24-alpine AS build

WORKDIR /app

ARG NG_APP_API_URL=http://localhost:5008
ENV NG_APP_API_URL=$NG_APP_API_URL

COPY package*.json ./
RUN npm ci

COPY . .

RUN npm run build -- --configuration production \
    && mkdir -p /app/www \
    && cp -r "$(find dist -type f -name index.html -exec dirname {} \; | head -n 1)/." /app/www


# Runtime stage
FROM nginx:1.29-alpine

COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/www/ /usr/share/nginx/html/

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
