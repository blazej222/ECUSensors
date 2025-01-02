
namespace ECUBackend.Services
{
    using Nethereum.Web3;
    using Nethereum.Web3.Accounts;
    using System.Numerics;
    using System.Threading.Tasks;

    public class BlockchainService
    {
        private readonly string _rpcUrl;
        private readonly string _contractAddress;
        private readonly string _abi;

        public BlockchainService(string rpcUrl, string contractAddress, string abi)
        {
            _rpcUrl = rpcUrl;
            _contractAddress = contractAddress;
            _abi = abi;
        }

        public async Task RewardSensor(string adminPrivateKey, string sensorAddress, BigInteger amount)
        {
            // Tworzenie konta na podstawie klucza prywatnego
            var account = new Account(adminPrivateKey);

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

            var receipt = await rewardFunction.SendTransactionAndWaitForReceiptAsync(
                from: account.Address,
                gas: new Nethereum.Hex.HexTypes.HexBigInteger(5000000),
                value: null,
                functionInput: new object[] { sensorAddress, amount }
            );

            // Wyświetlenie hashu transakcji
            Console.WriteLine($"Transaction successful! Hash: {receipt.TransactionHash}");
        }
    }


}
