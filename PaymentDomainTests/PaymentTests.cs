using PaymentDomain;
using PaymentDomain.Events;
using SharedKernel;
using System.Security.Cryptography.X509Certificates;
using Xunit.Sdk;

namespace PaymentDomainTests
{
	public class PaymentTests
	{
		[Fact]
		public void Should_Initiate_With_Valid_Data()
		{
			// Arrange
			var bookingId = Guid.NewGuid();
			var userId = "test-user-1";
			var amount = 30000;
			var method = PaymentMethod.Wallet;
			var now = DateTimeOffset.UtcNow;

			// Act
			var payment = Payment.Initiate(bookingId, userId, amount, method, now);
			var initaiteEvent = payment.Events.OfType<PaymentInitiatedEvent>().FirstOrDefault(e => e.BookingId == bookingId && e.Method == method);

			//Assert
			Assert.NotNull(initaiteEvent);
			Assert.Equal(bookingId, payment.BookingId);
			Assert.Equal(userId, payment.UserId);
			Assert.Equal(amount, payment.Amount);
			Assert.Equal(method, payment.Method);
			Assert.Equal(now, payment.CreatedAt);
		}

		[Fact]
		public void Should_Throws_Exception_On_Initiate_With_Invalid_Amount()
		{
			// Arrange
			var bookingId = Guid.NewGuid();
			var userId = "test-user-1";
			var amount = -1500;
			var method = PaymentMethod.Wallet;
			var now = DateTimeOffset.UtcNow;

			// Act & Assert
			var exception = Assert.Throws<DomainException>(() => Payment.Initiate(bookingId, userId, amount, method, now));
			Assert.Equal("PaymentDomain.InvalidAmount",exception.Code);
		}

		[Fact]
		public void Should_Throws_Exception_On_Initiate_With_Null_BookingId()
		{
			// Arrange
			var bookingId = Guid.Empty;
			var userId = "test-user-1";
			var amount = 50000;
			var method = PaymentMethod.Wallet;
			var now = DateTimeOffset.UtcNow;

			// Act & Assert
			var exception = Assert.Throws<DomainException>(() => Payment.Initiate(bookingId, userId, amount, method, now));
			Assert.Equal("PaymentDomain.NullBookingId", exception.Code);
		}

		[Fact]
		public void Should_Throws_Exception_On_Initiate_With_Null_UserId()
		{
			// Arrange
			var bookingId = Guid.NewGuid();
			var userId = "  ";
			var amount = 50000;
			var method = PaymentMethod.Wallet;
			var now = DateTimeOffset.UtcNow;

			// Act & Assert
			var exception = Assert.Throws<DomainException>(() => Payment.Initiate(bookingId, userId, amount, method, now));
			Assert.Equal("PaymentDomain.NullUserId", exception.Code);
		}

		[Fact]
		public void Should_Attach_GatewayToken_With_Valid_Data()
		{
			// Arrange
			var bookingId = Guid.NewGuid();
			var userId = "test-user-1";
			var amount = 30000;
			var method = PaymentMethod.Gateway;
			var now = DateTimeOffset.UtcNow;
			var payment = Payment.Initiate(bookingId, userId, amount, method, now);
			var token = "test-token";

			// Act
			payment.AttachGatewayToken(token);

			// Assert
			Assert.Equal(token,payment.GatewayToken);
		}

		[Fact]
		public void Should_Throws_Exception_On_AttachToken_With_Invalid_Token()
		{
			// Arrange
			var bookingId = Guid.NewGuid();
			var userId = "test-user-1";
			var amount = 30000;
			var method = PaymentMethod.Gateway;
			var now = DateTimeOffset.UtcNow;
			var payment = Payment.Initiate(bookingId, userId, amount, method, now);
			var token = " ";

			// Act & Assert
			var exception = Assert.Throws<DomainException>(() => payment.AttachGatewayToken(token));
			Assert.Equal("PaymentDomain.InvalidToken", exception.Code);
		}

		[Fact]
		public void Should_Throws_Exception_On_AttachToken_With_Invalid_Status()
		{
			// Arrange
			var bookingId = Guid.NewGuid();
			var userId = "test-user-1";
			var amount = 30000;
			var method = PaymentMethod.Gateway;
			var now = DateTimeOffset.UtcNow;
			var payment = Payment.Initiate(bookingId, userId, amount, method, now);
			payment.MarkSucceeded("test-tracking-code", 30000, now);
			var token = "test-token";

			// Act & Assert
			var exception = Assert.Throws<DomainException>(() => payment.AttachGatewayToken(token));
			Assert.Equal("PaymentDomain.TokenStatusInvalid", exception.Code);
		}

		[Fact]
		public void Should_Throws_Exception_On_AttachToken_With_Invalid_Method()
		{
			// Arrange
			var bookingId = Guid.NewGuid();
			var userId = "test-user-1";
			var amount = 30000;
			var method = PaymentMethod.Wallet;
			var now = DateTimeOffset.UtcNow;
			var payment = Payment.Initiate(bookingId, userId, amount, method, now);
			var token = "test-token";

			// Act & Assert
			var exception = Assert.Throws<DomainException>(() => payment.AttachGatewayToken(token));
			Assert.Equal("PaymentDomain.TokenMethodInvalid", exception.Code);
		}

		[Fact]
		public void Should_Throws_Exception_On_AttachToken_By_Duplicate_Attachment()
		{
			// Arrange
			var bookingId = Guid.NewGuid();
			var userId = "test-user-1";
			var amount = 30000;
			var method = PaymentMethod.Gateway;
			var now = DateTimeOffset.UtcNow;
			var payment = Payment.Initiate(bookingId, userId, amount, method, now);
			var token = "test-token";
			payment.AttachGatewayToken(token);

			// Act & Assert
			var exception = Assert.Throws<DomainException>(() => payment.AttachGatewayToken("token2"));
			Assert.Equal("PaymentDomain.TokenDuplicateAssignment", exception.Code);
		}

		[Fact]
		public void Should_Mark_Succeeded_With_Valid_Data()
		{
			// Arrange
			var bookingId = Guid.NewGuid();
			var userId = "test-user-1";
			var amount = 30000;
			var method = PaymentMethod.Wallet;
			var now = DateTimeOffset.UtcNow;
			var payment = Payment.Initiate(bookingId, userId, amount, method, now);
			var trackingCode = "test-tracking-code";

			// Act 
			payment.MarkSucceeded(trackingCode, amount, now);
			var successEvent = payment.Events.OfType<PaymentSucceededEvent>().FirstOrDefault(e => e.PaymentId == payment.Id);

			// Assert
			Assert.Equal(PaymentStatus.Succeeded, payment.Status);
			Assert.Equal(trackingCode, payment.TrackingCode);
			Assert.Equal(now, payment.CompletedAt);
			Assert.NotNull(successEvent);
		}

		[Theory]
		[InlineData(" ")]
		[InlineData("   ")]
		public void Should_Throws_Exception_On_MarkSucceeded_With_Invalid_TrackingCode(string trackingCode)
		{
			// Arrange
			var bookingId = Guid.NewGuid();
			var userId = "test-user-1";
			var amount = 30000;
			var method = PaymentMethod.Wallet;
			var now = DateTimeOffset.UtcNow;
			var payment = Payment.Initiate(bookingId, userId, amount, method, now);

			// Act & Assert
			var exception = Assert.Throws<DomainException>(() => payment.MarkSucceeded(trackingCode,amount,now));
			Assert.Equal("PaymentDomain.InvalidTrackingCode", exception.Code);
		}

		[Theory]
		[InlineData(100)]
		[InlineData(60000)]
		public void Should_Throws_Exception_On_MarkSucceeded_With_Invalid_Amount(long amount)
		{
			// Arrange
			var bookingId = Guid.NewGuid();
			var userId = "test-user-1";
			var method = PaymentMethod.Wallet;
			var now = DateTimeOffset.UtcNow;
			var initAmount = 5000;
			var payment = Payment.Initiate(bookingId, userId, initAmount, method, now);
			var trackingCode = "test-tracking-code";

			// Act & Assert
			var exception = Assert.Throws<DomainException>(() => payment.MarkSucceeded(trackingCode, amount, now));
			Assert.Equal("PaymentDomain.AmountMismatch", exception.Code);
		}

		[Fact]
		public void Should_Throws_Exception_On_MarkSucceeded_With_Invalid_State()
		{
			// Arrange
			var bookingId = Guid.NewGuid();
			var userId = "test-user-1";
			var amount = 30000;
			var method = PaymentMethod.Wallet;
			var now = DateTimeOffset.UtcNow;
			var payment = Payment.Initiate(bookingId, userId, amount, method, now);
			var trackingCode = "test-tracking-code";
			payment.MarkFailed("test-reason", now);

			// Act & Assert
			var exception = Assert.Throws<DomainException>(() => payment.MarkSucceeded(trackingCode, amount, now));
			Assert.Equal("PaymentDomain.InvalidTransactionStatus", exception.Code);
		}

		[Fact]
		public void Should_Do_Nothing_On_MarkSucceeded_If_Status_And_TrackingCode_Are_Success_And_Same()
		{
			// Arrange
			var bookingId = Guid.NewGuid();
			var userId = "test-user-1";
			var amount = 30000;
			var method = PaymentMethod.Wallet;
			var now = DateTimeOffset.UtcNow;
			var payment = Payment.Initiate(bookingId, userId, amount, method, now);
			var trackingCode = "test-tracking-code";
			payment.MarkSucceeded(trackingCode, amount, now);

			// Act
			payment.MarkSucceeded(trackingCode, amount, now + TimeSpan.FromMinutes(10));

			// Assert
			Assert.Equal(PaymentStatus.Succeeded, payment.Status);
			Assert.Equal(trackingCode, payment.TrackingCode);
			Assert.Equal(now, payment.CompletedAt);
		}

		[Fact]
		public void Should_Mark_Failed_With_Valid_Data()
		{
			// Arrange
			var bookingId = Guid.NewGuid();
			var userId = "test-user-1";
			var amount = 30000;
			var method = PaymentMethod.Wallet;
			var now = DateTimeOffset.UtcNow;
			var payment = Payment.Initiate(bookingId, userId, amount, method, now);
			var reason = "failed-test-reason";

			// Act
			payment.MarkFailed(reason, now);
			var failedEvent = payment.Events.OfType<PaymentFailedEvent>().FirstOrDefault(e => e.PaymentId == payment.Id);

			// Assert
			Assert.Equal(PaymentStatus.Failed, payment.Status);
			Assert.Equal(now, payment.CompletedAt);
			Assert.Equal(reason, payment.FailureReason);
			Assert.NotNull(failedEvent);
		}

		[Fact]
		public void Should_Throws_Exception_On_MarkFailed_With_Invalid_State()
		{
			// Arrange
			var bookingId = Guid.NewGuid();
			var userId = "test-user-1";
			var amount = 30000;
			var method = PaymentMethod.Wallet;
			var now = DateTimeOffset.UtcNow;
			var payment = Payment.Initiate(bookingId, userId, amount, method, now);
			var reason = "failed-test-reason";
			payment.MarkSucceeded("test-tracking-code", amount, now);

			// Act & Assert
			var exception = Assert.Throws<DomainException>(() => payment.MarkFailed(reason,now));
			Assert.Equal("PaymentDomain.InvalidTransactionStatus", exception.Code);
		}

		[Theory]
		[InlineData(" ")]
		[InlineData("   ")]
		public void Should_Throws_Exception_On_MarkFailed_With_Null_Reason(string reason)
		{
			// Arrange
			var bookingId = Guid.NewGuid();
			var userId = "test-user-1";
			var amount = 30000;
			var method = PaymentMethod.Wallet;
			var now = DateTimeOffset.UtcNow;
			var payment = Payment.Initiate(bookingId, userId, amount, method, now);

			// Act & Assert
			var exception = Assert.Throws<DomainException>(() => payment.MarkFailed(reason, now));
			Assert.Equal("PaymentDomain.NullReason", exception.Code);
		}

		[Fact]
		public void Should_Expire_With_Valid_Time()
		{
			// Arrange
			var bookingId = Guid.NewGuid();
			var userId = "test-user-1";
			var amount = 30000;
			var method = PaymentMethod.Wallet;
			var createdAt = DateTimeOffset.UtcNow;
			var now = createdAt.AddMinutes(25);
			var payment = Payment.Initiate(bookingId, userId, amount, method, createdAt);

			// Act
			payment.Expire(now);
			var expireEvent = payment.Events.OfType<PaymentExpiredEvent>().FirstOrDefault(e => e.PaymentId == payment.Id);

			// Assert
			Assert.Equal(PaymentStatus.Expired, payment.Status);
			Assert.Equal(now, payment.CompletedAt);
			Assert.NotNull(expireEvent);
		}

		[Fact]
		public void Should_Not_Expire_With_Valid_Time()
		{
			// Arrange
			var bookingId = Guid.NewGuid();
			var userId = "test-user-1";
			var amount = 30000;
			var method = PaymentMethod.Wallet;
			var createdAt = DateTimeOffset.UtcNow;
			var now = createdAt.AddMinutes(10);
			var payment = Payment.Initiate(bookingId, userId, amount, method, createdAt);

			// Act
			payment.Expire(now);
			var expireEvent = payment.Events.OfType<PaymentExpiredEvent>().FirstOrDefault(e => e.PaymentId == payment.Id);

			// Assert
			Assert.Equal(PaymentStatus.Pending, payment.Status);
			Assert.Null(expireEvent);
		}

		[Fact]
		public void Should_Throws_Exception_On_Expire_With_Invalid_State()
		{
			// Arrange
			var bookingId = Guid.NewGuid();
			var userId = "test-user-1";
			var amount = 30000;
			var method = PaymentMethod.Wallet;
			var createdAt = DateTimeOffset.UtcNow;
			var now = createdAt.AddMinutes(25);
			var payment = Payment.Initiate(bookingId, userId, amount, method, createdAt);
			payment.MarkSucceeded("test-tracking-code", amount, createdAt);

			// Act & Assert
			var exception = Assert.Throws<DomainException>(() => payment.Expire(now));
			Assert.Equal("PaymentDomain.InvalidTransactionStatus", exception.Code);
		}
	}
}

