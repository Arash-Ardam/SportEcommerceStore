namespace SharedKernel
{
    public abstract class AggregateRoot<TId> : Entity<TId> where TId : notnull
    {
        private readonly List<AggregateEvent> _events = new();

        public IReadOnlyList<AggregateEvent> Events => _events;

        protected AggregateRoot() { }

        protected void Raise(AggregateEvent aggregateEvent) => _events.Add(aggregateEvent);

        public void ClearEvents() => _events.Clear();
    }
}
