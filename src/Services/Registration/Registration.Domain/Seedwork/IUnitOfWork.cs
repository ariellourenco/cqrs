using System;
using System.Threading;
using System.Threading.Tasks;

namespace CQRSJourney.Registration
{
    /// <summary>
    /// A thin wrapper interface for <see cref="DbContext"/> intended to encapsulate the infrastructure persistence layer
    /// so it is decoupled from the application and domain-model layers.
    /// https://docs.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/infrastructure-persistence-layer-implementation-entity-framework-core
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Asynchronously saves all changes made to the underlying database.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// A <see cref="Task{int}"/> that represents the asynchronous save operation, containing the the number of state entries written
        /// to the underlying database. This can include state entries for entities and/or relationships.
        /// </returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
