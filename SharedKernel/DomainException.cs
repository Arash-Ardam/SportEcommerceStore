namespace SharedKernel
{
    public class DomainException : Exception
    {
        public string Code { get; set; }
        public string Description { get; set; }

        public override string Message => $"[{Code}] {Description}";
    }
}
