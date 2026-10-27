using SharedKernel;

namespace PaymentDomain;

public static class PaymentErrors
{
	public static DomainException InvalidAmount() => DomainException.Build("PaymentDomain.InvalidAmount", "مبلغ ورودی نامعتبر است");
	public static DomainException NullBookingId() => DomainException.Build("PaymentDomain.NullBookingId", "شناسه سفارش خالی است");
	public static DomainException NullUserId() => DomainException.Build("PaymentDomain.NullUserId", "شناسه کاربر خالی است");
	public static DomainException InvalidToken() => DomainException.Build("PaymentDomain.InvalidToken", "توکن تراکنش نباید خالی باشد");
	public static DomainException InvalidTrackingCode() => DomainException.Build("PaymentDomain.InvalidTrackingCode", "کد رهگیری تراکنش نباید خالی باشد");
	public static DomainException TokenStatusInvalid() => DomainException.Build("PaymentDomain.TokenStatusInvalid", "وضعیت تراکنش برای توکن نامعتبر است");
	public static DomainException TokenMethodInvalid() => DomainException.Build("PaymentDomain.TokenMethodInvalid", "نوع تراکنش برای توکن نامعتبر است");
	public static DomainException TokenDuplicateAssignment() => DomainException.Build("PaymentDomain.TokenDuplicateAssignment", "به این پرداخت قبلا توکن تخصیص داده شده است");
	public static DomainException AmountMismatch() => DomainException.Build("PaymentDomain.AmountMismatch", "مبلغ وارد شده با مبلغ درخواست مغایرت دارد");
	public static DomainException NullReason() => DomainException.Build("PaymentDomain.NullReason", "علت رد تراکنش نمی تواند خالی باشد");
	public static DomainException InvalidTransactionStatus() 
		=> DomainException.Build("PaymentDomain.InvalidTransactionStatus", $"وضعیت تراکنش برای عملیات خواسته شده معتبر نمی باشد");
}
