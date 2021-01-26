using Orleans;
using Orleans.Transactions.Abstractions;
using System;
using System.Threading.Tasks;

namespace Actor.Contract
{
    public interface IATMGrain : IGrainWithIntegerKey
    {
        [Transaction(TransactionOption.Create)]
        Task Transfer(string fromAccount, string toAccount, int amountToTransfer);
    }

    public class ATMGrain : Orleans.Grain, IATMGrain
    {
        private readonly IClusterClient _client;
        public ATMGrain(IClusterClient client)
        {
            _client = client;
        }
        Task IATMGrain.Transfer(string fromAccount, string toAccount, int amountToTransfer)
        {
            var deposit = _client.GetGrain<IAccountGrain>(fromAccount).Deposit(amountToTransfer);
            var withdraw = _client.GetGrain<IAccountGrain>(toAccount).Withdraw(amountToTransfer);

            return Task.WhenAll(deposit, withdraw);
        }
    }

    public interface IAccountGrain : IGrainWithStringKey
    {
        [Transaction(TransactionOption.Join)]
        Task Withdraw(int amount);

        [Transaction(TransactionOption.Join)]
        Task Deposit(int amount);

        [Transaction(TransactionOption.CreateOrJoin)]
        Task<int> GetBalance();
    }

    public class Balance
    {
        public int Value { get; set; }
    }

    public class AccountGrain : Grain, IAccountGrain
    {
        private readonly ITransactionalState<Balance> balance;

        public AccountGrain(
            [TransactionalState("balance")]
        ITransactionalState<Balance> balance)
        {
            this.balance = balance ?? throw new ArgumentNullException(nameof(balance));
        }

        Task IAccountGrain.Deposit(int amount)
        {
            return this.balance.PerformUpdate(x => x.Value += amount);
        }

        Task IAccountGrain.Withdraw(int amount)
        {
            return this.balance.PerformUpdate(x => x.Value -= amount);
        }

        Task<int> IAccountGrain.GetBalance()
        {
            return this.balance.PerformRead(x => x.Value);
        }
    }
}
