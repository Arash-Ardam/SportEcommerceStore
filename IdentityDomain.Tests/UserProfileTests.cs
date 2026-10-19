using SharedKernel;

namespace IdentityDomain.Tests
{
	public class UserProfileTests
	{
		[Fact]
		public void Should_Register_With_Valid_Data()
		{
			//Arrange
			var userId = "test-user-1";
			var fullName = "John Doe";
			var gender = UserProfileGender.Male;
			var nationalId = "0123456789";
			var birthdate = new DateOnly(2002, 11, 02);

			// Act
			var user = UserProfile.Register(userId,fullName, gender, nationalId, birthdate, DateTime.Now);

			// Assert
			Assert.NotNull(user);
			Assert.Equal(fullName, user.FullName);
			Assert.Equal(gender, user.Gender);
			Assert.Equal(nationalId, user.NationalId);
			Assert.Equal(birthdate, user.BirthDate.Value);
		}

		[Theory]
		[InlineData("")]
		[InlineData("  ")]
		public void Should_Throw_Exception_When_Registering_With_Invalid_UserId(string userId)
		{
			//Arrange
			var fullName = "John Doe";
			var gender = UserProfileGender.Male;
			var nationalId = "0123456789";
			var birthdate = new DateOnly(2002, 11, 02);

			// Act & Assert
			Assert.Throws<DomainException>(() => UserProfile.Register(userId,fullName, gender, nationalId, birthdate, DateTime.Now));
		}

		[Fact]
		public void Should_Throw_Exception_When_Registering_With_Invalid_BirthDate()
		{
			//Arrange
			var userId = "test-user-1";
			var fullName = "John Wick";
			var gender = UserProfileGender.Male;
			var nationalId = "0602912156";
			var birthdate = DateOnly.MaxValue;

			// Act & Assert
			var ex = Assert.Throws<DomainException>(() => UserProfile.Register(userId,fullName, gender, nationalId, birthdate, DateTime.Now));
			Assert.Equal("Identity.UserProfile.InvalidBirthdate", ex.Code);
		}

		[Fact]
		public void Should_Throw_Exception_When_Registering_With_TooOld_BirthDate()
		{
			//Arrange
			var userId = "test-user-1";
			var fullName = "John Wick";
			var gender = UserProfileGender.Male;
			var nationalId = "0602912156";
			var birthdate = DateOnly.MinValue;

			// Act & Assert
			var ex = Assert.Throws<DomainException>(() => UserProfile.Register(userId,fullName, gender, nationalId, birthdate, DateTime.Now));
			Assert.Equal("Identity.UserProfile.InvalidBirthdate", ex.Code);
		}


		[Fact]
		public void Should_Update_Profile_With_Valid_Data()
		{
			//Arrange
			var userId = "test-user-1";
			var fullName = "John Doe";
			var gender = UserProfileGender.Male;
			var nationalId = "0123456789";
			var birthdate = new DateOnly(2002, 11, 02);
			var user = UserProfile.Register(userId,fullName, gender, nationalId, birthdate, DateTime.Now);
			// Act
			var newFullName = "Jon Doe";
			user.ChangeName(newFullName);
			// Assert
			Assert.Equal(newFullName, user.FullName);
		}

		[Fact]
		public void Should_Throw_Exception_When_Updating_Profile_With_Invalid_Data()
		{
			//Arrange
			var userId = "test-user-1";
			var fullName = "John Doe";
			var gender = UserProfileGender.Male;
			var nationalId = "0123456789";
			var birthdate = new DateOnly(2002, 11, 02);
			var user = UserProfile.Register(userId,fullName, gender, nationalId, birthdate, DateTime.Now);

			// Act & Assert
			Assert.Throws<DomainException>(() => user.ChangeName(new string('B',51)));
		}

		[Fact]
		public void Should_Correct_IdentityInfo_With_Valid_Data()
		{
			// Arrange
			var userId = "test-user-1";
			var fullName = "John Doe";
			var gender = UserProfileGender.Male;
			var nationalId = "0123456789";
			var birthdate = new DateOnly(2002, 11, 02);
			var today = DateTime.Now;
			var user = UserProfile.Register(userId,fullName, gender, nationalId, birthdate, today);

			// Act
			user.CorrectIdentityInfo(UserProfileGender.Male, "0602912156", new DateOnly(2003, 11, 02), today);

			// Assert
			Assert.Equal(UserProfileGender.Male, user.Gender);
			Assert.Equal("0602912156", user.NationalId);
			Assert.Equal(new DateOnly(2003, 11, 02), user.BirthDate.Value);
		}

		[Fact]
		public void Should_Throw_Exception_On_Correct_IdentityInfo_With_Invalid_Data()
		{
			// Arrange
			var userId = "test-user-1";
			var fullName = "John Doe";
			var gender = UserProfileGender.Male;
			var nationalId = "0123456789";
			var birthdate = new DateOnly(2002, 11, 02);
			var today = DateTime.Now;
			var user = UserProfile.Register(userId,fullName, gender, nationalId, birthdate, today);

			// Act & Assert
			Assert.Throws<DomainException>(() => user.CorrectIdentityInfo(UserProfileGender.Male, "0602912", new DateOnly(2003, 11, 02) , today));
		}

		[Fact]
		public void Should_Return_True_Age_At_A_Day_Before_BirthDate()
		{
			// Arrange
			var userId = "test-user-1";
			var fullName = "John Doe";
			var gender = UserProfileGender.Male;
			var nationalId = "0110165853";
			var birthdate = new DateOnly(2002, 11, 02);
			var user = UserProfile.Register(userId,fullName, gender, nationalId, birthdate, DateTime.Now);

			// Act
			var age = user.BirthDate.AgeAt(new(2026,11,01));

			// Assert
			Assert.Equal(23, age);
		}


		[Fact]
		public void Should_Return_True_Age_At_BirthDate()
		{
			// Arrange
			var userId = "test-user-1";
			var fullName = "John Doe";
			var gender = UserProfileGender.Male;
			var nationalId = "0110165853";
			var birthdate = new DateOnly(2002, 11, 02);
			var user = UserProfile.Register(userId,fullName, gender, nationalId, birthdate, DateTime.Now);

			// Act
			var age = user.BirthDate.AgeAt(new(2026, 11, 02));

			// Assert
			Assert.Equal(24, age);
		}
	}
}
