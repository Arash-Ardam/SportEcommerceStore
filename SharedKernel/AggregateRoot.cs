namespace SharedKernel
{
    public abstract class AggregateRoot<TId>  
    {
        public TId Id { get; protected set; }
        public List<AggregateEvent> Events { get; protected set; } = new();

        protected AggregateRoot()
        {
            Id = default!;
        }

        public void Raise(AggregateEvent aggregateEvent)
        {
            Events.Add(aggregateEvent);
        }

        public void ClearEvents()
        {
            Events.Clear();
        }
    }
}
