FROM node:16

WORKDIR /app

RUN npm install -g truffle @truffle/hdwallet-provider

COPY . .

RUN truffle compile

CMD ["truffle", "migrate", "--network", "holesky"]
