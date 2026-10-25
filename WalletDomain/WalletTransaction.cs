using SharedKernel;

namespace WalletDomain
{
	public class WalletTransaction : Entity<Guid>
	{
		public WalletTransactionType Type { get; private set; }
		public long Amount { get; private set; }
		public string Reference { get; private set; } = string.Empty;
		public DateTimeOffset OccurredAt { get; private set; }

		private WalletTransaction() { }
		public static WalletTransaction Create(WalletTransactionType type, long amount,string reference,DateTimeOffset now)
		{
			if (amount <= 0)
				throw WalletErrors.InvalidAmount();

			return new WalletTransaction
			{
				Id = Guid.NewGuid(),
				Type = type,
				Amount = amount,
				Reference = reference,
				OccurredAt = now
			};
		}
	}

}
