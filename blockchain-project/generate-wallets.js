const { Wallet } = require("ethers");
const fs = require("fs");

const generateWallets = (count) => {
    const wallets = [];
    for (let i = 0; i < count; i++) {
        const wallet = Wallet.createRandom();
        wallets.push({
            SensorId: `Sensor-${i + 1}`,
            Address: wallet.address,
            PrivateKey: wallet.privateKey
        });
        console.log(`Generated wallet for Sensor-${i + 1}: ${wallet.address}`);
    }
    return wallets;
};

const saveWalletsToFile = (wallets, fileName) => {
    fs.writeFileSync(fileName, JSON.stringify(wallets, null, 2));
    console.log(`Wallets saved to ${fileName}`);
};

const walletCount = 16; // Liczba portfeli
const wallets = generateWallets(walletCount);
saveWalletsToFile(wallets, "sensors.json");
