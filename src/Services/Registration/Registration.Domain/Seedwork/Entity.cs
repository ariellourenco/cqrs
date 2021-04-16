using System;

namespace Registration
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

        /// <summary>
        /// A unique identifier for this <see cref="Entity{TKey}"/> instance.
        /// </summary>
        public TKey Id { get; protected set; }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            // Check for null and compare run-time types.
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            Entity<TKey> item = (Entity<TKey>)obj;

            return item.Id.Equals(this.Id);
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
            if (Object.Equals(left, null))
                return Object.Equals(right, null) ? true : false;

            return left.Equals(right);
        }

        public static bool operator !=(Entity<TKey> left, Entity<TKey> right)
        {
            return !(left == right);
        }
    }
}
