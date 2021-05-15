using System;
using System.Collections.Generic;
using System.Linq;
using CQRSJourney.Registration.Extensions;

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
        private IDictionary<Guid, List<SeatQuantity>> _pendingReservations = new Dictionary<Guid, List<SeatQuantity>>();

        private IDictionary<Guid, int> _remainingSeats = new Dictionary<Guid, int>();

        /// <summary>
        /// Inititializes a new instance of <see cref="SeatsAvailability"/> class.
        /// </summary>
        /// <param name="id">A unique identifier for this <see cref="SeatsAvailability"/> instance.</param>
        public SeatsAvailability(Guid id) : base(id)
        {
            base.Handles<SeatsReserved>(OnSeatsReserved);
        }

        /// <summary>
        /// Increases the number of seats available.
        /// </summary>
        /// <param name="seatType">The type of seat to add.</param>
        /// <param name="quantity">The number of seats to add.</param>
        public void AddSeats(Guid seatType, int quantity)
        {
            _remainingSeats.Add(seatType, quantity);
        }

        /// <summary>
        /// Stores the reservation in the data store. This may throw if the data store is unavailable.
        /// </summary>
        /// <param name="reservationId">A unique identifier for the reservation request.</param>
        // public void CommitReservation(Guid reservationId)
        // {
        //     var numberOfSeats = _pendingReservations[reservationId];
        //     _pendingReservations.Remove(reservationId);
        // }

        /// <summary>
        /// Requests a reservation for seats.
        /// </summary>
        /// <param name="reservationId">A unique identifier for the reservation request.</param>
        /// <param name="details">The list of seat requirements.</param>
        public void MakeReservation(Guid reservationId, IEnumerable<SeatQuantity> details)
        {
            if (details.Any(item => !_remainingSeats.ContainsKey(item.SeatType)))
                throw new ArgumentOutOfRangeException(nameof(details));

            var reservation = new Dictionary<Guid, Reservation>();

            // Add the requested seats to the reservation list.
            foreach (var item in details)
            {
                var entry = reservation.GetOrAdd(item.SeatType);
                entry.Wanted = item.Quantity;
                entry.Remaining = _remainingSeats[item.SeatType];
            }

            // When a reservation request hasn't been completed yet it is added to the peding list.
            // Therefore, before make a new reservation we need to check this list first.
            if (_pendingReservations.TryGetValue(reservationId, out var pending))
            {
                // Updates a reservation made previously.
                foreach (var item in pending)
                    reservation.GetOrAdd(item.SeatType).Pending = item.Quantity;
            }

            // Add the SeatsReserved to the domain events collection to be raised/dispatched
            // when comitting changes into the Database.
            AddEvent(new SeatsReserved(
                id: reservationId,
                details: reservation.Select(x => new SeatQuantity(x.Key, x.Value.Actual)).Where(x =>x.Quantity != 0).ToList(),
                availableSeatsChanged: reservation.Select(x => new SeatQuantity(x.Key, -x.Value.Difference)).Where(x => x.Quantity != 0).ToList()));
        }

        private void OnSeatsReserved(SeatsReserved @event)
        {
            _pendingReservations[@event.ReservationId] = @event.Details.ToList();

            foreach (var seat in @event.AvailableSeatsChanged)
                _remainingSeats[seat.SeatType] += seat.Quantity;
        }

        /// <summary>
        /// Represents the current result of seats availability and in-flight reservations
        /// for a seat type.
        /// </summary>
        private sealed class Reservation
        {
            /// <summary>
            /// Gets or sets the number of seats that is pending reservation.
            /// </summary>
            public int Pending { get; set; }

            /// <summary>
            /// Gets or sets the number of remaning seats.
            /// </summary>
            public int Remaining { get; set; }

            /// <summary>
            /// Gets or sets the number of seats wanted.
            /// </summary>
            public int Wanted { get; set; }

            /// <summary>
            /// Gets the actual number of seats that can be reserved.
            /// </summary>
            public int Actual
            {
                // If the number of wanted seats is greater than available we reserve all
                // available seats.
                get { return Math.Min(Wanted, (Math.Max(Remaining, 0) + Pending)); }
            }

            /// <summary>
            /// Gets the difference of seats availability since last reservation.
            /// </summary>
            public int Difference
            {
                get { return (Actual - Pending); }
            }
        }
    }
}
