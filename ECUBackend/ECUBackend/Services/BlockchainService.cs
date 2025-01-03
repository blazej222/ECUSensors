
namespace ECUBackend.Services
{
    using Nethereum.Web3;
    using Nethereum.Web3.Accounts;
    using System.Numerics;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;

    public class BlockchainService
    {
        private readonly string _rpcUrl;
        private readonly string _contractAddress = "none";
        private readonly string _adminPrivateKey = "none";
        private readonly string _abi;

        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public BlockchainService()///string rpcUrl, string contractAddress, string abi)
        {
            _rpcUrl = Environment.GetEnvironmentVariable("RPC_URL") ?? "http://host.docker.internal:7545"; //"http://host.docker.internal:7545";
            _abi = File.ReadAllText("Resources/SensorToken.abi");
            //The Docker container is supposed to crash if the keys are not found.
#if DEBUG
            _adminPrivateKey = "set it";
            _contractAddress = "set it";
#else
            var content = File.ReadAllText("../ganache-data/ganache-output.log");
            var privateKeyRegex = new Regex(@"\((\d+)\)\s+(0x[a-fA-F0-9]{64})");
            var contractRegex = new Regex(@"Contract created:\s+(0x[a-fA-F0-9]{40})");


            var privateKeys = privateKeyRegex.Matches(content)
                .Select(m => m.Groups[2].Value)
                .ToList();

            var contracts = contractRegex.Matches(content)
                .Select(c => c.Groups[1].Value)
                .ToList();


            _adminPrivateKey = privateKeys[0];
            _contractAddress = contracts[0];
#endif
            Console.WriteLine($"Admin private key: {_adminPrivateKey}");
            Console.WriteLine($"Contract address: {_contractAddress}");
        }


        public async Task RewardSensor(string sensorAddress, BigInteger amount)
        {
            // Tworzenie konta na podstawie klucza prywatnego
            var account = new Account(_adminPrivateKey);

            // Tworzenie instancji Web3 z kontem
            var web3 = new Web3(account, _rpcUrl);

            // Pobieranie kontraktu na podstawie ABI i adresu
            var contract = web3.Eth.GetContract(_abi, _contractAddress);

            // Wywołanie funkcji "rewardSensor"
            var rewardFunction = contract.GetFunction("rewardSensor");

            //var transactionInput = rewardFunction.CreateTransactionInput(adminPrivateKey, sensorAddress, amount);

            //var receipt = await rewardFunction.SendTransactionAndWaitForReceiptAsync(account.Address, sensorAccount, amount);
            //var receipt = await web3.Eth.TransactionManager.SendTransactionAsync(transactionInput);
            //var receipt = await rewardFunction.SendTransactionAndWaitForReceiptAsync(transactionInput);


            Console.WriteLine(sensorAddress);
            await _semaphore.WaitAsync();

            var receipt = await rewardFunction.SendTransactionAndWaitForReceiptAsync(
                from: account.Address,
                gas: new Nethereum.Hex.HexTypes.HexBigInteger(5000000), // gas limit
                value: null,
                functionInput: new object[] { sensorAddress, amount }
            );

            _semaphore.Release();

            // Wyświetlenie hashu transakcji
            Console.WriteLine($"Transaction successful! Hash: {receipt.TransactionHash}");
        }

        public async Task GetBalance(string accountAddress)
        {
            // Tworzenie instancji Web3
            var web3 = new Web3(_rpcUrl);

            // Pobieranie kontraktu na podstawie ABI i adresu
            var contract = web3.Eth.GetContract(_abi, _contractAddress);

            // Pobieranie funkcji "balanceOf"
            var balanceOfFunction = contract.GetFunction("balanceOf");

            // Wywołanie funkcji "balanceOf" dla określonego adresu
            var balance = await balanceOfFunction.CallAsync<BigInteger>(accountAddress);

            // Wyświetlenie balansu na konsoli
            Console.WriteLine($"Balance of {accountAddress}: {balance} tokens");
        }
    }


}
