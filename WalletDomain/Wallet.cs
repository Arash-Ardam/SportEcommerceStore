using SharedKernel;
using WalletDomain.Events;

namespace WalletDomain
{
	public class Wallet : AggregateRoot<Guid>
	{
		public string OwnerId { get; private set; } = string.Empty;
		public long Balance { get; private set; }
		public long BlockedBalance { get; private set; }
		public long AvailableAmount => Balance - BlockedBalance;
		private readonly List<WalletTransaction> _transactions = [];
		public IReadOnlyList<WalletTransaction> Transactions => _transactions;
		private readonly List<WithdrawalHold> _holds = [];
		public IReadOnlyList<WithdrawalHold> Holds => _holds;
		private Wallet() { }

		public static Wallet Open(string ownerId)
		{
			if (string.IsNullOrWhiteSpace(ownerId))
				throw WalletErrors.InvalidOwnerId();

			var newWallet = new Wallet
			{
				Id = Guid.NewGuid(),
				OwnerId = ownerId.Trim(),
				Balance = 0,
				BlockedBalance = 0
			};

			newWallet.Raise(new WalletOpenedEvent(newWallet.Id, newWallet.OwnerId));
			return newWallet;
		}

		public void Credit(long amount , string reference,DateTimeOffset now)
		{
			if (string.IsNullOrWhiteSpace(reference))
				throw WalletErrors.NullReference();

			if (amount <= 0)
				throw WalletErrors.InvalidAmount();

			if (Transactions.Any(trx => trx.Reference == reference && trx.Type == WalletTransactionType.Refund))
				throw WalletErrors.DuplicateReference();

			Balance += amount;
			_transactions.Add(WalletTransaction.Create(WalletTransactionType.Refund,amount,reference,now));

			Raise(new WalletCreditedEvent(this.Id, reference, amount));
		}

		public void Debit(long amount, string reference, DateTimeOffset now)
		{
			if (string.IsNullOrWhiteSpace(reference))
				throw WalletErrors.NullReference();

			if (amount <= 0)
				throw WalletErrors.InvalidAmount();

			if (Transactions.Any(trx => trx.Reference == reference && trx.Type == WalletTransactionType.Purchase))
				throw WalletErrors.DuplicateReference();

			if (AvailableAmount < amount)
				throw WalletErrors.NotAvailableAmount();

			Balance -= amount;
			_transactions.Add(WalletTransaction.Create(WalletTransactionType.Purchase, amount, reference, now));

			Raise(new WalletDebitedEvent(this.Id, reference, amount));
		}

		public void BlockForWithdrawal(Guid requestId, long amount)
		{
			if (requestId == Guid.Empty)
				throw WalletErrors.NullReference();
			if (amount <= 0)
				throw WalletErrors.InvalidAmount();
			if (_holds.Count != 0)                                   
				throw WalletErrors.PendingWithdrawalExists();
			if (AvailableAmount < amount)
				throw WalletErrors.NotAvailableAmount();

			_holds.Add(new WithdrawalHold(requestId, amount));
			BlockedBalance += amount;
		}

		public void ReleaseBlock(Guid requestId)               
		{
			var hold = _holds.FirstOrDefault(h => h.RequestId == requestId)
				?? throw WalletErrors.HoldNotFound();

			_holds.Remove(hold);
			BlockedBalance -= hold.Amount;
		}

		public void SettleWithdrawal(Guid requestId, DateTimeOffset now)   
		{
			var hold = _holds.FirstOrDefault(h => h.RequestId == requestId)
				?? throw WalletErrors.HoldNotFound();

			_holds.Remove(hold);
			BlockedBalance -= hold.Amount;
			Balance -= hold.Amount;
			_transactions.Add(WalletTransaction.Create(
				WalletTransactionType.Withdrawal, hold.Amount, requestId.ToString(), now));
		}
	}

}
