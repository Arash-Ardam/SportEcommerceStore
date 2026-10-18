namespace SharedKernel
{
    public abstract record AggregateEvent
    {
        public  DateTimeOffset OccuredAt { get; init; } = DateTimeOffset.UtcNow;
	}
}
