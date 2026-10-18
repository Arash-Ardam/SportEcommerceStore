namespace SharedKernel
{
    public class DomainException : Exception
    {
        public string Code { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;

		private DomainException()
		{
			
		}

		public static DomainException Build(string code, string description)
			=> new DomainException
			{
				Code = code,
				Description = description
			};

		public override string Message => $"[{Code}] {Description}";
    }
}
