using System.Collections.Generic;
using MediatR;

namespace CQRSJourney.Registration.Events
{
    /// <summary>
    /// Supplies information about changes on seats availability.
    /// </summary>
    public sealed class AvailableSeatsChanged : INotification
    {
        /// <summary>
        /// Gets a collection containing the seat types and their quantities.
        /// </summary>
        /// <value>A collection containing the seat types and their quantities.</value>
        public IReadOnlyCollection<SeatQuantity> Seats { get; }

        /// <summary>
        /// Inititializes a new instance of <see cref="AvailableSeatsChanged"/> event.
        /// </summary>
        /// <param name="seats">A collection containing the seats types and their changes.</param>
        public AvailableSeatsChanged(IReadOnlyCollection<SeatQuantity> seats)
        {
            Seats = seats;
        }
    }
}
