namespace CQRSJourney.Registration
{
    /// <summary>
    /// Interface implemented by objects that can provide access to the underlying entity that implements <see cref="IAggregateRoot"/>.
    /// </summary>
    /// <typeparam name="TEntity">The root entity, also known as Aggregate Root.</typeparam>
    public interface IRepository<TEntity> where TEntity : class, IAggregateRoot
    {
        /// <summary>
        /// Gets the <see cref="IUnitOfWork"/> associated with the underlying repository.
        /// </summary>
        IUnitOfWork UnitOfWork { get; }
    }
}