using System;
using System.Collections.Generic;
using MediatR;

namespace CQRSJourney.Registration.Events
{
    /// <summary>
    /// Supplies information about a reservation cancellation event that is being raised.
    /// </summary>
    public sealed class SeatsReservationCancelled : INotification
    {
        /// <summary>
        /// Gets the unique identifier for this reservation.
        /// </summary>
        /// <value>The unique identifier for this reservation.</value>
        public Guid ReservationId { get; }

        /// <summary>
        /// Gets a collection containing the seat types and their quantities.
        /// </summary>
        /// <value>A collection containing the seat types and their quantities.</value>
        public IReadOnlyCollection<SeatQuantity> AvailableSeatsChanged { get; }

        /// <summary>
        /// Inititializes a new instance of <see cref="SeatsReservationCancelled"/> event.
        /// </summary>
        /// <param name="id">A unique identifier for the reservation request.</param>
        /// <param name="availableSeatsChanged">A collection containing the seats types and their changes.</param>
        public SeatsReservationCancelled(Guid id, IReadOnlyCollection<SeatQuantity> availableSeatsChanged)
        {
            ReservationId = id;
            AvailableSeatsChanged = availableSeatsChanged;
        }
    }
}