using System;
using System.Collections.Generic;
using MediatR;

namespace CQRSJourney.Registration
{
    /// <summary>
    /// Supplies information about a reservation event that is being raised.
    /// </summary>
    public sealed class SeatsReserved : INotification
    {
        /// <summary>
        /// Gets the unique identifier for this reservation.
        /// </summary>
        public Guid ReservationId { get; }

        /// <summary>
        /// Gets the list of seat types and their reserved quantities.
        /// </summary>
        public IReadOnlyCollection<SeatQuantity> Details { get; }

        /// <summary>
        /// Gets the list of seat types and their availability changes.
        /// </summary>
        public IReadOnlyCollection<SeatQuantity> AvailableSeatsChanged { get; }

        /// <summary>
        /// Inititializes a new instance of <see cref="SeatsReserved"/> event.
        /// </summary>
        /// <param name="id">A unique identifier for the reservation request.</param>
        /// <param name="details">A list of seat types and their reserved quantities.</param>
        /// <param name="AvailableSeatsChanged">A list of seats types and their availability changes.</param>
        public SeatsReserved(Guid id, List<SeatQuantity> details, List<SeatQuantity> availableSeatsChanged)
        {
            ReservationId = id;
            Details = details.AsReadOnly();
            AvailableSeatsChanged = availableSeatsChanged.AsReadOnly();
        }
    }
}
