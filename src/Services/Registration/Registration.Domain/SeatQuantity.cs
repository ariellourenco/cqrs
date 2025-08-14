namespace CQRSJourney.Registration;

/// <summary>
/// Defines the seat types and its quantities.
/// </summary>
/// <param name="SeatType">A unique identifier for the seat type.</param>
/// <param name="Quantity">A number that represents the seat quantity.</param>
public sealed record SeatQuantity(Guid SeatType, int Quantity);
