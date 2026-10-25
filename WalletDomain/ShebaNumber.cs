namespace WalletDomain
{
	public record ShebaNumber
	{
		public string Value { get; } = string.Empty;
		private ShebaNumber(string value) => Value = value;

		public static ShebaNumber Create(string value)
		{
			var v = (value ?? "").Replace(" ", "").ToUpperInvariant();
			if (v.Length != 26 || !v.StartsWith("IR") || !v[2..].All(char.IsAsciiDigit))
				throw WalletErrors.ShebaInvalidFormat();
			return new ShebaNumber(v);
		}

	}

}
