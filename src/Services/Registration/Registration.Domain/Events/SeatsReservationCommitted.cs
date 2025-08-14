using MediatR;

namespace CQRSJourney.Registration.Events;

/// <summary>
/// Supplies information about a reservation committing event that is being raised.
/// </summary>
public sealed class SeatsReservationCommitted : INotification
{
    /// <summary>
    /// Gets the unique identifier for this reservation.
    /// </summary>
    /// <value>The unique identifier for this reservation.</value>
    public Guid ReservationId { get; }

    /// <summary>
    /// Inititializes a new instance of <see cref="SeatsReservationCommitted"/> event.
    /// </summary>
    /// <param name="id">A unique identifier for the reservation request.</param>
    public SeatsReservationCommitted(Guid id) => ReservationId = id;
}
