using SharedKernel;
using WalletDomain.Events;

namespace WalletDomain
{
	public class WithdrawalRequest : AggregateRoot<Guid>
	{
		public Guid WalletId { get; private set; }
		public long Amount { get; private set; }
		public ShebaNumber ShebaNumber { get; private set; } = null!;
		public WithdrawalOrderStatus Status { get; private set; }
		public DateTimeOffset RequestedAt { get; private set; }
		public string? RejectReason { get; private set; }

		private WithdrawalRequest() { }

		public static WithdrawalRequest Submit(Guid walletId, long amount, ShebaNumber sheba, DateTimeOffset now)
		{
			if (amount <= 0) throw WalletErrors.InvalidAmount();
			if (sheba is null) throw WalletErrors.ShebaNullValue();
			if (walletId == Guid.Empty) throw WalletErrors.WalletIdNullReference();

			var request = new WithdrawalRequest
			{
				Id = Guid.NewGuid(),
				WalletId = walletId,
				Amount = amount,
				ShebaNumber = sheba,
				Status = WithdrawalOrderStatus.Pending,
				RequestedAt = now
			};

			request.Raise(new WithdrawalRequestSubmittedEvent(request.Id,request.WalletId,request.Amount,request.RequestedAt));
			return request;
		}

		public void MarkPaid()
		{
			if (Status != WithdrawalOrderStatus.Pending) throw WalletErrors.WithdrawalNotPending();
			Status = WithdrawalOrderStatus.Paid;

			Raise(new WithdrawalRequestPaidEvent(Id, WalletId, Amount, RequestedAt));
		}

		public void Reject(string reason)
		{
			if (string.IsNullOrWhiteSpace(reason))
				throw WalletErrors.NullReason();

			if (Status != WithdrawalOrderStatus.Pending) throw WalletErrors.WithdrawalNotPending();
			Status = WithdrawalOrderStatus.Rejected;
			RejectReason = reason;

			Raise(new WithdrawalRequestRejectedEvent(Id,WalletId,Amount,RequestedAt,reason));
		}
	}

}
