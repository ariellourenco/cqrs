using System;
using System.Collections.Generic;
using System.Linq;
using CQRSJourney.Registration.Events;
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
            base.Handles<AvailableSeatsChanged>(OnSeatsChanged);
            base.Handles<SeatsReservationCancelled>(OnReservationCancelled);
            base.Handles<SeatsReservationCommitted>(OnReservationCommitted);
            base.Handles<SeatsReserved>(OnSeatsReserved);
        }

        /// <summary>
        /// Increases the number of seats available.
        /// </summary>
        /// <param name="seatType">The type of seat to add.</param>
        /// <param name="quantity">The number of seats to add.</param>
        public void AddSeats(Guid seatType, int quantity) =>
            AddEvent(new AvailableSeatsChanged(new[] { new SeatQuantity(seatType, quantity) }));

        /// <summary>
        /// Cancels the reservation operation. If there is no pending reservation, no action is taken.
        /// </summary>
        /// <param name="reservationId">A unique identifier for the reservation request.</param>
        public void CancelReservation(Guid reservationId)
        {
            if (_pendingReservations.TryGetValue(reservationId, out var reservation))
            {
                AddEvent(new SeatsReservationCancelled(
                    id: reservationId,
                    availableSeatsChanged: reservation.Select(x => new SeatQuantity(x.SeatType, x.Quantity)).ToList()));
            }
        }

        /// <summary>
        /// Confirms the reservation and removes it from the pending list.
        /// </summary>
        /// <param name="reservationId">A unique identifier for the reservation request.</param>
        public void CommitReservation(Guid reservationId)
        {
            if (_pendingReservations.ContainsKey(reservationId))
                AddEvent(new SeatsReservationCommitted(reservationId));
        }

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

        /// <summary>
        /// Decreases the number of seats available.
        /// </summary>
        /// <param name="seatType">The type of seat to remove.</param>
        /// <param name="quantity">The number of seats to remove.</param>
        public void RemoveSeats(Guid seatType, int quantity)
        {
            if (_remainingSeats.ContainsKey(seatType))
                AddEvent(new AvailableSeatsChanged(new[] { new SeatQuantity(seatType, -quantity) }));
        }

        private void OnReservationCancelled(SeatsReservationCancelled @event)
        {
            _pendingReservations.Remove(@event.ReservationId);

            foreach (var seat in @event.AvailableSeatsChanged)
                _remainingSeats[seat.SeatType] += seat.Quantity;
        }

        private void OnReservationCommitted(SeatsReservationCommitted @event) =>
            _pendingReservations.Remove(@event.ReservationId);

        private void OnSeatsChanged(AvailableSeatsChanged @event)
        {
            foreach (var seat in @event.Seats)
            {
                var newValue = seat.Quantity;

                if (_remainingSeats.TryGetValue(seat.SeatType, out var quantity))
                    newValue += quantity;

                _remainingSeats[seat.SeatType] = newValue;
            }
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
