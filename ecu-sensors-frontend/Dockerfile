# Etap budowania aplikacji
FROM node:18 AS build

# Ustawienie katalogu roboczego
WORKDIR /app

# Skopiowanie plików konfiguracyjnych aplikacji
COPY package*.json ./

# Instalacja zależności
RUN npm install

# Skopiowanie całej aplikacji
COPY . .

# Budowa aplikacji
RUN npm run build

# Etap serwowania aplikacji
FROM nginx:1.23

# Skopiowanie zbudowanej aplikacji do katalogu Nginx
COPY --from=build /app/build /usr/share/nginx/html

# Domyślny port Nginx
EXPOSE 80

# Uruchomienie Nginx
CMD ["nginx", "-g", "daemon off;"]