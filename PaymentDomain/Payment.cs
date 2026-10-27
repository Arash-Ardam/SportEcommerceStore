using PaymentDomain.Events;
using SharedKernel;

namespace PaymentDomain
{
	public class Payment : AggregateRoot<Guid>
	{
		public Guid BookingId { get; private set; }
		public string UserId { get; private set; } = string.Empty;
		public long Amount { get; private set; }
		public PaymentMethod Method { get; private set; }
		public PaymentStatus Status { get; private set; }
		public string? GatewayToken { get; private set; }
		public string? TrackingCode { get; private set; }
		public string? FailureReason { get; private set; }
		public DateTimeOffset CreatedAt { get; private set; }
		public DateTimeOffset? CompletedAt { get; private set; }
		
		private Payment() { }

		public static Payment Initiate(Guid bookingId, string userId, long amount, PaymentMethod method, DateTimeOffset now)
		{
			if (amount <= 0) throw PaymentErrors.InvalidAmount();
			if (bookingId == Guid.Empty) throw PaymentErrors.NullBookingId();
			if (string.IsNullOrWhiteSpace(userId)) throw PaymentErrors.NullUserId();

			var newPayment = new Payment
			{
				Id = Guid.NewGuid(),
				BookingId = bookingId,
				UserId = userId.Trim(),
				Amount = amount,
				Method = method,
				CreatedAt = now,
				Status = PaymentStatus.Pending
			};

			newPayment.Raise(new PaymentInitiatedEvent(newPayment.Id,newPayment.BookingId, newPayment.UserId,newPayment.Method, newPayment.CreatedAt));
			return newPayment;
		}

		public void AttachGatewayToken(string token)
		{
			if (string.IsNullOrWhiteSpace(token)) throw PaymentErrors.InvalidToken();
			if (Status != PaymentStatus.Pending) throw PaymentErrors.TokenStatusInvalid();
			if (Method != PaymentMethod.Gateway) throw PaymentErrors.TokenMethodInvalid();
			if (GatewayToken != null) throw PaymentErrors.TokenDuplicateAssignment();

			GatewayToken = token.Trim();
		}

		public void MarkSucceeded(string trackingCode,long verifiedAmount, DateTimeOffset now)
		{
			if (string.IsNullOrWhiteSpace(trackingCode)) throw PaymentErrors.InvalidTrackingCode();
			if (Amount != verifiedAmount) throw PaymentErrors.AmountMismatch();
			if (Status == PaymentStatus.Succeeded && TrackingCode == trackingCode) return;

			if (Status is not (PaymentStatus.Pending or PaymentStatus.Expired))
				throw PaymentErrors.InvalidTransactionStatus();

			Status = PaymentStatus.Succeeded;
			TrackingCode = trackingCode;
			CompletedAt = now;

			Raise(new PaymentSucceededEvent(Id,BookingId,UserId,Amount,trackingCode,now));
		}

		public void MarkFailed(string reason, DateTimeOffset now)
		{
			if (Status != PaymentStatus.Pending) throw PaymentErrors.InvalidTransactionStatus();
			if (string.IsNullOrWhiteSpace(reason)) throw PaymentErrors.NullReason();

			Status = PaymentStatus.Failed;
			CompletedAt = now;
			FailureReason = reason;

			Raise(new PaymentFailedEvent(Id,BookingId, UserId, reason, now));
		}

		public void Expire(DateTimeOffset now)
		{
			if (Status != PaymentStatus.Pending) throw PaymentErrors.InvalidTransactionStatus();
			if ((now - CreatedAt).TotalMinutes > 20)
			{
				Status = PaymentStatus.Expired;
				CompletedAt = now;
				Raise(new PaymentExpiredEvent(Id,BookingId,UserId!,now));
			}
		}
	}
}
