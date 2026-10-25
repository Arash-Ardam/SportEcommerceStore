using SharedKernel;
using WalletDomain.Events;

namespace WalletDomain.Tests;

public class WithdrawalRequestTests
{
	[Fact]
	public void Should_Submit_With_Valid_Data()
	{
		// Arrange
		var walletId = Guid.NewGuid();
		var amount = 5000;
		var sheba = ShebaNumber.Create("IR890160000000000568058750");
		var now = DateTimeOffset.UtcNow;

		// Act
		var withdrawalRequest = WithdrawalRequest.Submit(walletId, amount, sheba, now);
		var requestEvent = withdrawalRequest.Events.OfType<WithdrawalRequestSubmittedEvent>().FirstOrDefault(e => e.WalletId == walletId);

		// Assert
		Assert.Equal(WithdrawalOrderStatus.Pending,withdrawalRequest.Status);
		Assert.Equal(walletId, withdrawalRequest.WalletId);
		Assert.Equal(amount, withdrawalRequest.Amount);
		Assert.Equal("IR890160000000000568058750", sheba.Value);
		Assert.NotNull(requestEvent);
	}

	[Fact]
	public void Should_Throws_Exception_In_Submit_With_Invalid_Amount()
	{
		// Arrange
		var walletId = Guid.NewGuid();
		var amount = -12;
		var sheba = ShebaNumber.Create("IR890160000000000568058750");
		var now = DateTimeOffset.UtcNow;

		// Act & Assert
		var exception = Assert.Throws<DomainException>(() => WithdrawalRequest.Submit(walletId, amount, sheba, now));
		Assert.Equal("WalletDomain.InvalidAmount", exception.Code);
	}

	[Fact]
	public void Should_Throws_Exception_In_Submit_With_Invalid_Wallet()
	{
		// Arrange
		var walletId = Guid.Empty;
		var amount = 5000;
		var sheba = ShebaNumber.Create("IR890160000000000568058750");
		var now = DateTimeOffset.UtcNow;

		// Act & Assert
		var exception = Assert.Throws<DomainException>(() => WithdrawalRequest.Submit(walletId, amount, sheba, now));
		Assert.Equal("WalletDomain.WalletIdNullReference", exception.Code);
	}

	[Fact]
	public void Should_Throws_Exception_In_Submit_With_Null_Sheba()
	{
		// Arrange
		var walletId = Guid.NewGuid();
		var amount = 5000;
		var now = DateTimeOffset.UtcNow;

		// Act & Assert
		var exception = Assert.Throws<DomainException>(() => WithdrawalRequest.Submit(walletId, amount, null!, now));
		Assert.Equal("WalletDomain.ShebaNullValue", exception.Code);
	}

	[Fact]
	public void Should_Throws_Exception_In_Submit_With_Invalid_Sheba()
	{
		// Act & Assert
		var exception = Assert.Throws<DomainException>(() => ShebaNumber.Create("123456"));
		Assert.Equal("WalletDomain.ShebaInvalidFormat",exception.Code);
	}

	[Fact]
	public void Should_MarkPaid_With_Valid_State()
	{
		// Arrange
		var walletId = Guid.NewGuid();
		var amount = 5000;
		var sheba = ShebaNumber.Create("IR890160000000000568058750");
		var now = DateTimeOffset.UtcNow;
		var withdrawalRequest = WithdrawalRequest.Submit(walletId, amount, sheba, now);

		// Act
		withdrawalRequest.MarkPaid();
		var requestEvent = withdrawalRequest.Events.OfType<WithdrawalRequestPaidEvent>().FirstOrDefault(e => e.WalletId == walletId);

		// Assert
		Assert.Equal(WithdrawalOrderStatus.Paid, withdrawalRequest.Status);
		Assert.NotNull(requestEvent);
	}

	[Fact]
	public void Should_Throws_Exception_On_MarkPaid_With_InValid_State()
	{
		// Arrange
		var walletId = Guid.NewGuid();
		var amount = 5000;
		var sheba = ShebaNumber.Create("IR890160000000000568058750");
		var now = DateTimeOffset.UtcNow;
		var withdrawalRequest = WithdrawalRequest.Submit(walletId, amount, sheba, now);
		withdrawalRequest.MarkPaid();

		// Act & Assert
		var exceprion = Assert.Throws<DomainException>(() => withdrawalRequest.MarkPaid());
		Assert.Equal("WalletDomain.WithdrawalNotPending", exceprion.Code);
	}

	[Fact]
	public void Should_Reject_With_Valid_State()
	{
		// Arrange
		var walletId = Guid.NewGuid();
		var amount = 5000;
		var sheba = ShebaNumber.Create("IR890160000000000568058750");
		var now = DateTimeOffset.UtcNow;
		var withdrawalRequest = WithdrawalRequest.Submit(walletId, amount, sheba, now);

		// Act
		withdrawalRequest.Reject("test failed payment");
		var requestEvent = withdrawalRequest.Events.OfType<WithdrawalRequestRejectedEvent>().FirstOrDefault(e => e.WalletId == walletId);

		// Assert
		Assert.Equal(WithdrawalOrderStatus.Rejected, withdrawalRequest.Status);
		Assert.Equal("test failed payment",withdrawalRequest.RejectReason);
		Assert.NotNull(requestEvent);
	}

	[Fact]
	public void Should_Throws_Exception_On_Reject_With_InValid_State()
	{
		// Arrange
		var walletId = Guid.NewGuid();
		var amount = 5000;
		var sheba = ShebaNumber.Create("IR890160000000000568058750");
		var now = DateTimeOffset.UtcNow;
		var withdrawalRequest = WithdrawalRequest.Submit(walletId, amount, sheba, now);
		withdrawalRequest.Reject("test failed payment");

		// Act & Assert
		var exceprion = Assert.Throws<DomainException>(() => withdrawalRequest.Reject("test failed payment"));
		Assert.Equal("WalletDomain.WithdrawalNotPending", exceprion.Code);
	}

	[Fact]
	public void Should_Throws_Exception_On_Reject_With_Null_Reason()
	{
		// Arrange
		var walletId = Guid.NewGuid();
		var amount = 5000;
		var sheba = ShebaNumber.Create("IR890160000000000568058750");
		var now = DateTimeOffset.UtcNow;
		var withdrawalRequest = WithdrawalRequest.Submit(walletId, amount, sheba, now);

		// Act & Assert
		var exceprion = Assert.Throws<DomainException>(() => withdrawalRequest.Reject(string.Empty));
		Assert.Equal("WalletDomain.NullReason", exceprion.Code);
	}
}
