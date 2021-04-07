using System;
using System.Collections.Generic;

namespace Registration
{
    public sealed class SeatsAvailability
    {
        private readonly Guid _id;

        // Using a private collection field for better encapsulation so new reservations can
        // not be added from "outside" of SeatsAvailability aggregate directly to the collection,
        // but only through the method SeatsAvailability.MakeReservation() which includes behaviour.
        private IDictionary<Guid, int> _pendingReservations;

        public SeatsAvailability(Guid id)
        {
            _id = id;
            _pendingReservations = new Dictionary<Guid, int>();
        }

        private int RemainingSeats { get; set; }

        public void AddSeats(int additionalSeats)
        {
            RemainingSeats += additionalSeats;
        }

        public void MakeReservation(Guid reservationId, int numberOfSeats)
        {
            if (numberOfSeats > RemainingSeats)
                throw new ArgumentOutOfRangeException(nameof(numberOfSeats));

            _pendingReservations.Add(reservationId, numberOfSeats);
            RemainingSeats -= numberOfSeats;
        }

        public void Expires(Guid reservationId)
        {
            var numberOfSeats = _pendingReservations[reservationId];
            _pendingReservations.Remove(reservationId);
            RemainingSeats += numberOfSeats;
        }
    }
}
