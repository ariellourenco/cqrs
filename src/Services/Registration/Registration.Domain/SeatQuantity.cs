using System;

namespace CQRSJourney.Registration
{
    /// <summary>
    ///  Defines the seat types and its quantities.
    /// </summary>
    public sealed record SeatQuantity
    {
        /// <summary>
        /// Gets the quantity of seats.
        /// </summary>
        /// <value>A number that represents the seat quantity.</value>
        public int Quantity { get; }

        /// <summary>
        /// Gets the type of the seat.
        /// </summary>
        /// <value>A unique identifier of the type of the seat.</value>
        public Guid SeatType { get; }

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
