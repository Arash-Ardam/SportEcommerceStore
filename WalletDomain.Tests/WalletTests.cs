using SharedKernel;
using WalletDomain.Events;

namespace WalletDomain.Tests
{
	public class WalletTests
	{
		[Fact]
		public void Should_Open_With_ValidData()
		{
			// Arrange
			var ownerId = "test-user-1";

			// Act 
			var wallet = Wallet.Open(ownerId);

			// Assert
			Assert.NotNull(wallet);
			Assert.Equal(ownerId, wallet.OwnerId);
			Assert.IsType<WalletOpenedEvent>(Assert.Single(wallet.Events));
		}

		[Fact]
		public void Should_Throw_Error_On_Empty_UserId_While_Open_Wallet()
		{
			var exception = Assert.Throws<DomainException>(() => Wallet.Open(string.Empty));
			Assert.Equal("WalletDomain.InvalidOwnerId", exception.Code);
		}

		[Fact]
		public void Should_Credit_With_Valid_Amount()
		{
			// Arrange
			var wallet = Wallet.Open("test-user-1");
			var amount = 10000;
			var reference = "test-credit-reference1";
			var now = DateTimeOffset.UtcNow;

			// Act
			wallet.Credit(amount, reference, now);
			var creditEvent = wallet.Events.OfType<WalletCreditedEvent>().FirstOrDefault(e => e.Reference == reference);
			var creaditedTransaction = wallet.Transactions.FirstOrDefault(trx => trx.Reference == reference);

			// Assert
			Assert.Equal(amount, wallet.Balance);
			Assert.Equal(amount, wallet.AvailableAmount);
			Assert.Equal(0, wallet.BlockedBalance);
			Assert.NotNull(creaditedTransaction);
			Assert.Equal(WalletTransactionType.Refund, creaditedTransaction.Type);
			Assert.NotNull(creditEvent);
			Assert.Equal(reference, creditEvent.Reference);
			Assert.Equal(amount, creditEvent.Amount);
		}

		[Fact]
		public void Should_Throw_Exception_In_Credit_With_Invalid_Reference()
		{
			// Arrange
			var wallet = Wallet.Open("test-user-1");

			// Act & Assert
			var exception = Assert.Throws<DomainException>(() => wallet.Credit(1000,string.Empty,DateTimeOffset.UtcNow));
			Assert.Equal("WalletDomain.NullReference", exception.Code);
		}

		[Fact]
		public void Should_Throw_Exception_In_Credit_With_Invalid_Amount()
		{
			// Arrange
			var wallet = Wallet.Open("test-user-1");

			// Act & Assert
			var exception = Assert.Throws<DomainException>(() => wallet.Credit(0,"test-user-1", DateTimeOffset.UtcNow));
			Assert.Equal("WalletDomain.InvalidAmount", exception.Code);
		}

		[Fact]
		public void Should_Throw_Exception_In_Credit_With_Duplicate_Reference()
		{
			// Arrange
			var wallet = Wallet.Open("test-user-1");
			var amount = 10000;
			var reference = "test-credit-reference1";
			var now = DateTimeOffset.UtcNow;
			wallet.Credit(amount, reference, now);

			// Act & Assert
			var exception = Assert.Throws<DomainException>(() => wallet.Credit(12000, reference, DateTimeOffset.UtcNow));
			Assert.Equal("WalletDomain.DuplicateReference", exception.Code);
		}

		[Fact]
		public void Should_Debit_With_Valid_Amount()
		{
			// Arrange
			var wallet = Wallet.Open("test-user-1");
			var amount = 10000;
			var reference = "test-debit-reference1";
			var now = DateTimeOffset.UtcNow;
			wallet.Credit(amount, "test-credit-reference", now);

			// Act
			wallet.Debit(5000, reference, now);
			var debitEvent = wallet.Events.OfType<WalletDebitedEvent>().FirstOrDefault(e => e.Reference == reference);
			var debittedTransaction = wallet.Transactions.FirstOrDefault(trx => trx.Reference == reference);

			// Assert
			Assert.Equal(5000, wallet.Balance);
			Assert.Equal(5000, wallet.AvailableAmount);
			Assert.Equal(0, wallet.BlockedBalance);
			Assert.NotNull(debittedTransaction);
			Assert.Equal(WalletTransactionType.Purchase, debittedTransaction.Type);
			Assert.NotNull(debitEvent);
			Assert.Equal(reference, debitEvent.Reference);
			Assert.Equal(5000, debitEvent.Amount);
		}

		[Fact]
		public void Should_Throw_Exception_In_debit_With_Invalid_Reference()
		{
			// Arrange
			var wallet = Wallet.Open("test-user-1");

			// Act & Assert
			var exception = Assert.Throws<DomainException>(() => wallet.Debit(1000, string.Empty, DateTimeOffset.UtcNow));
			Assert.Equal("WalletDomain.NullReference", exception.Code);
		}

		[Fact]
		public void Should_Throw_Exception_In_Debit_With_Invalid_Amount()
		{
			// Arrange
			var wallet = Wallet.Open("test-user-1");

			// Act & Assert
			var exception = Assert.Throws<DomainException>(() => wallet.Debit(0, "test-user-1", DateTimeOffset.UtcNow));
			Assert.Equal("WalletDomain.InvalidAmount", exception.Code);
		}

		[Fact]
		public void Should_Throw_Exception_In_Debit_With_Duplicate_Reference()
		{
			// Arrange
			var wallet = Wallet.Open("test-user-1");
			var amount = 10000;
			var reference = "test-debit-reference1";
			var now = DateTimeOffset.UtcNow;
			wallet.Credit(amount, "test-ref", now);
			wallet.Debit(5000, reference, now);

			// Act & Assert
			var exception = Assert.Throws<DomainException>(() => wallet.Debit(3000, reference, DateTimeOffset.UtcNow));
			Assert.Equal("WalletDomain.DuplicateReference", exception.Code);
		}

		[Fact]
		public void Should_Throw_Exception_In_Debit_With_NotAvailable_Amount()
		{
			// Arrange
			var wallet = Wallet.Open("test-user-1");
			var amount = 10000;
			var reference = "test-debit-reference1";
			var now = DateTimeOffset.UtcNow;
			wallet.Credit(amount, "test-ref", now);
			
			// Act & Assert
			var exception = Assert.Throws<DomainException>(() => wallet.Debit(500000, reference, DateTimeOffset.UtcNow));
			Assert.Equal("WalletDomain.NotAvailableAmount", exception.Code);
		}
		[Fact]
		public void Should_Throw_Exception_In_Debit_With_NotAvailable_Amount_After_Block()
		{
			// Arrange
			var wallet = Wallet.Open("test-user-1");
			var amount = 10000;
			var reference = "test-debit-reference1";
			var now = DateTimeOffset.UtcNow;
			wallet.Credit(amount, "test-ref", now);
			wallet.BlockForWithdrawal(Guid.NewGuid(), 6000);

			// Act & Assert
			var exception = Assert.Throws<DomainException>(() => wallet.Debit(5000, reference, DateTimeOffset.UtcNow));
			Assert.Equal("WalletDomain.NotAvailableAmount", exception.Code);
		}

		[Fact]
		public void Should_BlockWithdrawal_With_Valid_Data()
		{
			// Arrange
			var wallet = Wallet.Open("test-user-id");
			wallet.Credit(10000, "test-credit-ref", DateTimeOffset.UtcNow);
			var requestId = Guid.NewGuid();

			// Act
			wallet.BlockForWithdrawal(requestId, 5000);
			var holdedRequest = wallet.Holds.FirstOrDefault(h => h.RequestId == requestId);

			// Assert
			Assert.NotNull(holdedRequest);
			Assert.Equal(5000, wallet.BlockedBalance);
		}

		[Fact]
		public void Should_Throw_Exception_On_BlockWithdrawal_With_Invalid_Reference()
		{
			// Arrange
			var wallet = Wallet.Open("test-user-id");
			wallet.Credit(10000, "test-credit-ref", DateTimeOffset.UtcNow);
			var requestId = Guid.Empty;

			// Act & Assert
			var exception = Assert.Throws<DomainException>(() => wallet.BlockForWithdrawal(requestId, 1000));
			Assert.Equal("WalletDomain.NullReference", exception.Code);
		}

		[Fact]
		public void Should_Throw_Exception_On_BlockWithdrawal_With_Invalid_Amount()
		{
			// Arrange
			var wallet = Wallet.Open("test-user-id");
			wallet.Credit(10000, "test-credit-ref", DateTimeOffset.UtcNow);
			var requestId = Guid.NewGuid();

			// Act & Assert
			var exception = Assert.Throws<DomainException>(() => wallet.BlockForWithdrawal(requestId, 0));
			Assert.Equal("WalletDomain.InvalidAmount", exception.Code);
		}

		[Fact]
		public void Should_Throw_Exception_On_BlockWithdrawal_With_PendingReuqest()
		{
			// Arrange
			var wallet = Wallet.Open("test-user-id");
			wallet.Credit(10000, "test-credit-ref", DateTimeOffset.UtcNow);
			var requestId = Guid.NewGuid();
			wallet.BlockForWithdrawal(requestId, 5000);

			// Act & Assert
			var exception = Assert.Throws<DomainException>(() => wallet.BlockForWithdrawal(Guid.NewGuid(), 3000));
			Assert.Equal("WalletDomain.PendingWithdrawalExists", exception.Code);
		}

		[Fact]
		public void Should_Throw_Exception_On_BlockWithdrawal_With_NotAvailable_Amount()
		{
			// Arrange
			var wallet = Wallet.Open("test-user-id");
			wallet.Credit(10000, "test-credit-ref", DateTimeOffset.UtcNow);
			var requestId = Guid.NewGuid();

			// Act & Assert
			var exception = Assert.Throws<DomainException>(() => wallet.BlockForWithdrawal(requestId, 50000000));
			Assert.Equal("WalletDomain.NotAvailableAmount", exception.Code);
		}

		[Fact]
		public void Should_ReleaseBlock_With_Valida_Data()
		{
			// Arrange
			var wallet = Wallet.Open("test-user-id");
			wallet.Credit(10000, "test-credit-ref", DateTimeOffset.UtcNow);
			var requestId = Guid.NewGuid();
			wallet.BlockForWithdrawal(requestId, 5000);

			// Act
			wallet.ReleaseBlock(requestId);

			// Assert
			Assert.Empty(wallet.Holds);
			Assert.Equal(0, wallet.BlockedBalance);
		}

		[Fact]
		public void Should_Throws_Exception_On_ReleaseBlock_With_Invalid_RequestId()
		{
			// Arrange
			var wallet = Wallet.Open("test-user-id");
			wallet.Credit(10000, "test-credit-ref", DateTimeOffset.UtcNow);
			var requestId = Guid.NewGuid();
			wallet.BlockForWithdrawal(requestId, 5000);

			// Act & Assert
			var exception = Assert.Throws<DomainException>(() => wallet.ReleaseBlock(Guid.NewGuid()));
			Assert.Equal("WalletDomain.HoldNotFound", exception.Code);
		}

		[Fact]
		public void Should_SettleWithdrawal_With_Valid_Data()
		{
			// Arrange
			var wallet = Wallet.Open("test-user-id");
			wallet.Credit(10000, "test-credit-ref", DateTimeOffset.UtcNow);
			var requestId = Guid.NewGuid();
			wallet.BlockForWithdrawal(requestId, 5000);

			// Act
			wallet.SettleWithdrawal(requestId, DateTimeOffset.UtcNow);
			var transaction = wallet.Transactions.FirstOrDefault(trx => trx.Reference == requestId.ToString());

			// Assert
			Assert.NotNull(transaction);
			Assert.Equal(0, wallet.BlockedBalance);
			Assert.Equal(5000, wallet.Balance);
			Assert.Equal(5000, wallet.AvailableAmount);
			Assert.Equal(WalletTransactionType.Withdrawal, transaction.Type);
		}

		[Fact]
		public void Should_Throws_Exception_On_SettleWithdrawal_With_Invalid_RequestId()
		{
			// Arrange
			var wallet = Wallet.Open("test-user-id");
			wallet.Credit(10000, "test-credit-ref", DateTimeOffset.UtcNow);
			var requestId = Guid.NewGuid();
			wallet.BlockForWithdrawal(requestId, 5000);

			// Act & Assert
			var exception = Assert.Throws<DomainException>(() => wallet.SettleWithdrawal(Guid.NewGuid(), DateTimeOffset.UtcNow));
			Assert.Equal("WalletDomain.HoldNotFound", exception.Code);
		}

		[Fact]
		public void Should_Credit_Refund_For_A_Booking_Paid_With_Wallet()
		{
			// Arrange: بلیت booking-B با کیف پول خریده شده
			var wallet = Wallet.Open("test-user-1");
			var now = DateTimeOffset.UtcNow;
			wallet.Credit(10000, "test-credit-ref", now);
			wallet.Debit(6000, "booking-B", now);

			// Act: لغو همان بلیت؛ برگشت پول با همان مرجع
			wallet.Credit(6000, "booking-B", now);

			// Assert
			Assert.Equal(10000, wallet.Balance);
			Assert.Equal(2, wallet.Transactions.Count(trx => trx.Reference == "booking-B"));
		}
	}
}
