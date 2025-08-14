using MediatR;

namespace CQRSJourney.Registration;

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
    /// <param name="id">A unique identifier for this <see cref="Entity"/> instance.</param>
    public Entity(Guid id) => Id = id;
}

/// <summary>
/// Represents an entity in the domain model.
/// </summary>
/// <typeparam name="TKey">The type used for the identity for the entity.</typeparam>
public abstract class Entity<TKey> where TKey : IEquatable<TKey>
{
    private int? _requestedHashCode;

    private readonly List<INotification> _events = [];

    private readonly Dictionary<Type, Action<INotification>> _handlers = [];

    /// <summary>
    /// A unique identifier for this <see cref="Entity{TKey}"/> instance.
    /// </summary>
    public TKey? Id { get; protected set; }

    /// <summary>
    /// Gets a list of domain events that can be subscribed to.
    /// </summary>
    public IReadOnlyCollection<INotification> Events => _events.AsReadOnly();

    /// <summary>
    /// Adds the specified domain event to the events list.
    /// </summary>
    /// <remarks>
    /// This method is not intended for adding many events over a long period of time,
    /// because the event objects are stored in memory and adding too many events to
    /// the same Entity object can impact app performance.
    /// </remarks>
    /// <param name="event">The domain event to add.</param>
    public void AddEvent(INotification @event)
    {
        _events.Add(@event);

        // If we have a handler registered for the event then invoke it.
        if (_handlers.TryGetValue(@event.GetType(), out var handler))
            handler.Invoke(@event);
    }

    /// <summary>
    /// Removes all domain events from the events list.
    /// </summary>
    public void ClearEvents() => _events.Clear();

    /// <summary>
    /// Registers a handler that will be invoked when the specified domain event is fired.
    /// </summary>
    /// <typeparam name="TEvent">The type of the domain event handled.</typeparam>
    /// <param name="handler">A delegate to handle the domain event.</param>
    protected void Handles<TEvent>(Action<TEvent> handler) where TEvent : INotification
        => _handlers.Add(typeof(TEvent), @event => handler((TEvent)@event));

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        // Check for null and compare whether the runtime types are not
        // exactly the same.
        if ((obj == null) || !GetType().Equals(obj.GetType()))
            return false;

        // Optimization for a common success case.
        if (object.ReferenceEquals(this, obj))
            return true;

        return ((Entity<TKey>)obj).Id?.Equals(Id) ?? false;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        int hashCodeMultiplier = 397;

        unchecked
        {
            // We want to compare entity instances by its identity.
            // XOR for random distribution (https://ericlippert.com/2011/02/28/guidelines-and-rules-for-gethashcode/)
            if (!_requestedHashCode.HasValue)
                _requestedHashCode = ((Id?.GetHashCode() ?? 0) * hashCodeMultiplier) ^ 31;

            return _requestedHashCode.Value;
        }
    }

    /// <summary>
    /// Compares two <see cref="Entity"/> values for equality.
    /// See the type documentation for a description of equality semantics.
    /// </summary>
    /// <param name="left">The first value to compare</param>
    /// <param name="right">The second value to compare</param>
    /// <returns><see langword="true"/> if the two <see cref="Entity"/> values are the same; <see langword="false"/> otherwise.</returns>
    public static bool operator ==(Entity<TKey> left, Entity<TKey> right)
        => Equals(left, null) ? Equals(right, null) : left.Equals(right);

    /// <summary>
    /// Compares two <see cref="Entity"/> values for inequality.
    /// See the type documentation for a description of equality semantics.
    /// </summary>
    /// <param name="left">The first value to compare</param>
    /// <param name="right">The second value to compare</param>
    /// <returns><see langword="false"/> if the two <see cref="Entity"/> values are the same; <see langword="true"/> otherwise.</returns>
    public static bool operator !=(Entity<TKey> left, Entity<TKey> right) => !(left == right);
}
