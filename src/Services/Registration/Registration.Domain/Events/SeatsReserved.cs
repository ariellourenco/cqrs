using System;
using MediatR;

namespace CQRSJourney.Registration
{
    /// <summary>
    /// A domain event that is fired after a reservation has made successfully.
    /// </summary>
    public sealed class SeatsReserved : INotification
    {
        /// <summary>
        /// Gets the unique identifier for this reservation.
        /// </summary>
        public Guid ReservationId { get; }

        /// <summary>
        /// Inititializes a new instance of <see cref="SeatsReserved"/> event.
        /// </summary>
        /// <param name="reservationId">A unique identifier for the reservation request.</param>
        public SeatsReserved(Guid reservationId)
        {
            ReservationId = reservationId;
        }
    }
}
