// const HDWalletProvider = require("@truffle/hdwallet-provider");

// module.exports = {
//     networks: {
//         holesky: {
//             provider: () => new HDWalletProvider(
//                 "tourist position nest stuff assist suspect emotion early hungry any earn impulse", // Frazy odzyskiwania z MetaMask
//                 "https://1rpc.io/holesky"
//             ),
//             network_id: 17000,
//             gas: 5000000,
//             gasPrice: 20000000000 // 20 Gwei
//         }
//     },
//     compilers: {
//         solc: {
//             version: "0.8.20"
//         }
//     }
// };

//const HDWalletProvider = require("@truffle/hdwallet-provider");

module.exports = {
    networks: {
        devel: {
            host: "ganache",        
            port: 7545,
            network_id: 123456,    
            gas: 3000000,          
            gasPrice: 20000000000, 
        }
    },
    compilers: {
        solc: {
            version: "0.8.20", // Wersja kompilatora Solidity
        }
    }
};