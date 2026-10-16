namespace SharedKernel
{
    public abstract record AggregateEvent
    {
        public string Code { get; set; } = string.Empty;
    }
        

}
