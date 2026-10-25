using SharedKernel;

namespace WalletDomain
{
	public static class WalletErrors
	{
		public static DomainException InvalidOwnerId() => DomainException.Build("WalletDomain.InvalidOwnerId", "شناسه صاحب کیف پول نا معتبر است");
		public static DomainException InvalidAmount() => DomainException.Build("WalletDomain.InvalidAmount", "مبلغ وارد شده نامعتبر است");
		public static DomainException NotAvailableAmount() => DomainException.Build("WalletDomain.NotAvailableAmount", "موجودی در دسترس کافی نمی باشد");
		public static DomainException DuplicateReference() => DomainException.Build("WalletDomain.DuplicateReference", "شماره مرجع وارد شده تکراری است");
		public static DomainException NullReference() => DomainException.Build("WalletDomain.NullReference", "شماره مرجع خالی است");
		public static DomainException NullReason() => DomainException.Build("WalletDomain.NullReason", "علت رد تراکنش خالی است");
		public static DomainException WalletIdNullReference() => DomainException.Build("WalletDomain.WalletIdNullReference", "شناسه کیف پول خالی است");
		public static DomainException PendingWithdrawalExists() => DomainException.Build("WalletDomain.PendingWithdrawalExists", "به دلیل وجود درخواست برداشت پول در انتظار امکان درخواست جدید وجود ندارد");
		public static DomainException HoldNotFound() => DomainException.Build("WalletDomain.HoldNotFound", "درخواست وجه در انتظاری یافت نشد");
		public static DomainException ShebaInvalidFormat() => DomainException.Build("WalletDomain.ShebaInvalidFormat", "شماره شبای وارد شده صحیح نمی باشد.");
		public static DomainException ShebaNullValue() => DomainException.Build("WalletDomain.ShebaNullValue", "شماره شبای الزامی می باشد.");
		public static DomainException WithdrawalNotPending() => DomainException.Build("WalletDomain.WithdrawalNotPending", "فقط درخواست در حال انتظار قابل پردازش است");
	}
}
