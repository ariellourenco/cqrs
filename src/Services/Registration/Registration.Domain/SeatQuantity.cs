using System;

namespace CQRSJourney.Registration
{
    /// <summary>
    ///  Defines the seat types and its quantities.
    /// </summary>
    public sealed class SeatQuantity : ValueObject
    {
        /// <summary>
        /// Gets the quantity of seats.
        /// </summary>
        public int Quantity { get; private set; }

        /// <summary>
        /// Gets the type of the seat.
        /// </summary>
        public Guid SeatType { get; private set; }

        /// <summary>
        /// Inititializes a new instance of <see cref="SeatQuantity"/> class.
        /// </summary>
        /// <param name="seatType">The type of the seat.</param>
        /// <param name="quantity">The quantity of seats.</param>
        public SeatQuantity(Guid seatType, int quantity)
        {
            Quantity = quantity;
            SeatType = seatType;
        }
    }
}
