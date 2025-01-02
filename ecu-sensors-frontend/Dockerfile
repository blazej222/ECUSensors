# Użyj odpowiedniej wersji Node.js
FROM node:18-alpine

# Ustaw katalog roboczy
WORKDIR /app

# Kopiuj pliki package.json i package-lock.json (jeśli istnieje)
COPY package.json ./
COPY package-lock.json ./

# Ustaw flagę środowiska, aby zminimalizować rozmiar logów
ENV CI=true

# Zainstaluj zależności z --legacy-peer-deps, aby ominąć problemy z peer dependencies
RUN npm install --legacy-peer-deps

# Kopiuj resztę aplikacji do kontenera
COPY . ./

# Buduj aplikację
RUN npm run build

# Ekspozycja portu
EXPOSE 3000

# Uruchom serwer aplikacji
CMD ["npm", "start"]