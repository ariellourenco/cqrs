namespace CQRSJourney.Registration
{
    /// <summary>
    /// This is an empty interface, sometimes called a marker interface, that has no methods or constants inside it.
    /// It provides run-time type information about objects to indicate that a given entity class is also an aggregate root.
    /// </summary>
    /// <remarks>
    /// Though marker interfaces are still in use, they very likely point to a code smell and should be used carefully.
    /// The main reason for this is that they blur the lines about what an interface represents
    /// since markers don't define any behavior.
    /// </remarks>
    public interface IAggregateRoot { }
}
