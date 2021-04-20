using System;
using System.Collections.Generic;

namespace CQRSJourney.Registration
{
    /// <summary>
    /// Manages the availability of conference seats.
    /// </summary>
    public sealed class SeatsAvailability : Entity, IAggregateRoot
    {
        // Using a private collection field for better encapsulation so new reservations can
        // not be added from "outside" of SeatsAvailability aggregate directly to the collection,
        // but only through the method SeatsAvailability.MakeReservation() which includes behaviour.
        private IDictionary<Guid, int> _pendingReservations = new Dictionary<Guid, int>();

        /// <summary>
        /// Inititializes a new instance of <see cref="SeatsAvailability"/> class.
        /// </summary>
        /// <param name="id">A unique identifier for this <see cref="SeatsAvailability"/> instance.</param>
        public SeatsAvailability(Guid id)
            : base(id) { }

        public int RemainingSeats { get; set; }

        /// <summary>
        /// Increases the number of seats available.
        /// </summary>
        /// <param name="quantity">The number of seats to add.</param>
        public void AddSeats(int quantity)
        {
            RemainingSeats += quantity;
        }

        /// <summary>
        /// Stores the reservation in the data store. This may throw if the data store is unavailable.
        /// </summary>
        /// <param name="reservationId">A unique identifier for the reservation request.</param>
        public void CommitReservation(Guid reservationId)
        {
            var numberOfSeats = _pendingReservations[reservationId];
            _pendingReservations.Remove(reservationId);
        }

        /// <summary>
        /// Requests a reservation for seats.
        /// </summary>
        /// <param name="reservationId">A unique identifier for the reservation request.</param>
        /// <param name="numberOfSeats">The list of seat requirements.</param>
        public void MakeReservation(Guid reservationId, int numberOfSeats)
        {
            if (numberOfSeats > RemainingSeats)
                throw new ArgumentOutOfRangeException(nameof(numberOfSeats));

            _pendingReservations.Add(reservationId, numberOfSeats);
            RemainingSeats -= numberOfSeats;
        }

        public void Expire(Guid reservationId)
        {
            var numberOfSeats = _pendingReservations[reservationId];
            _pendingReservations.Remove(reservationId);
            RemainingSeats += numberOfSeats;
        }
    }
}
