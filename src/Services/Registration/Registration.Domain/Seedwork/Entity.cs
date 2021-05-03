using System;
using System.Collections.Generic;
using MediatR;

namespace CQRSJourney.Registration
{
    /// <summary>
    /// The default implementation of <see cref="Entity{TKey}"/> which uses a <see cref="Guid"/>
    /// as an identity.
    /// </summary>
    public class Entity : Entity<Guid>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="Entity"/>.
        /// </summary>
        /// <remarks>
        /// The Id property is initialized to form a new GUID value.
        /// </remarks>
        public Entity() => Id = Guid.NewGuid();

        /// <summary>
        /// Initializes a new instance of <see cref="Entity"/>.
        /// </summary>
        /// <param name="id">A unique identifier for this <see cref="Entity"/> instance.
        public Entity(Guid id) => Id = id;
    }

    /// <summary>
    /// Represents an entity in the domain model.
    /// </summary>
    /// <typeparam name="TKey">The type used for the identity for the entity.</typeparam>
    public abstract class Entity<TKey> where TKey : IEquatable<TKey>
    {
        private int? _requestedHashCode;

        private List<INotification> _events;

        /// <summary>
        /// A unique identifier for this <see cref="Entity{TKey}"/> instance.
        /// </summary>
        public TKey Id { get; protected set; }

        /// <summary>
        /// Gets a list of domain events that can be subscribed to.
        /// </summary>
        public IReadOnlyCollection<INotification> Events => _events?.AsReadOnly();

        /// <summary>
        /// Adds the specified domain event to the events list.
        /// </summary>
        /// <param name="event">The domain event to add.</param>
        /// <remarks>
        /// AddEvent is not intended for adding many events over a long period of time, because the event objects
        /// are stored in memory. Adding too many events to the same Entity object can impact app performance.
        /// </remarks>
        public void AddEvent(INotification @event)
        {
            _events = _events ?? new List<INotification>();
            _events.Add(@event);
        }

        /// <summary>
        /// Removes all domain events from the events list.
        /// </summary>
        public void ClearEvents()
        {
            _events?.Clear();
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            // Check for null and compare run-time types.
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
                return false;

            return ((Entity<TKey>)obj).Id.Equals(this.Id);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                // We want to compare entity instances by its identity.
                // XOR for random distribution (https://ericlippert.com/2011/02/28/guidelines-and-rules-for-gethashcode/)
                if (!_requestedHashCode.HasValue)
                    _requestedHashCode = (Id.GetHashCode() * 397) ^ 31;

                return _requestedHashCode.Value;
            }
        }

        public static bool operator ==(Entity<TKey> left, Entity<TKey> right)
        {
            if (object.Equals(left, null))
                return object.Equals(right, null) ? true : false;

            return left.Equals(right);
        }

        public static bool operator !=(Entity<TKey> left, Entity<TKey> right)
        {
            return !(left == right);
        }
    }
}
