namespace SharedKernel
{
    public abstract class Entity<TId>
    {
        public TId Id { get; protected set; }
        protected Entity() { }
    }
}
