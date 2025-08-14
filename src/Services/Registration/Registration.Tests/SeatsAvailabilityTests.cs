using CQRSJourney.Registration.Events;
using Xunit;

namespace CQRSJourney.Registration;

public sealed class SeatsAvailabilityTests
{
    private const int AvailableSeats = 10;

    private static readonly Guid _reservationId = Guid.NewGuid();

    private static readonly Guid _seatTypeId = Guid.NewGuid();

    [Fact]
    public void AddSeats_ForNonExistingSeatType_ChangesSeatsAvailability()
    {
        // Arrange
        var sut = new SeatsAvailability(_reservationId);

        // Act
        sut.AddSeats(_seatTypeId, AvailableSeats);

        // Assert
        Assert.Equal(_seatTypeId, ((AvailableSeatsChanged)sut.Events.Single()).Seats.ElementAt(0).SeatType);
        Assert.Equal(AvailableSeats, ((AvailableSeatsChanged)sut.Events.Single()).Seats.ElementAt(0).Quantity);
    }

    [Fact]
    public void AddSeats_ForExistingSeatType_IncreaseSeatsAvailability()
    {
        // Arrange
        var quantity = 10;
        var sut = new SeatsAvailability(_reservationId);

        // Act
        sut.AddSeats(_seatTypeId, AvailableSeats);
        sut.AddSeats(_seatTypeId, quantity);

        // Assert
        Assert.Equal(_seatTypeId, ((AvailableSeatsChanged)sut.Events.Last()).Seats.ElementAt(0).SeatType);
        Assert.Equal(quantity, ((AvailableSeatsChanged)sut.Events.Last()).Seats.ElementAt(0).Quantity);
    }

    [Fact]
    public void RemoveSeats_ForExistingSeatType_ChangesSeatsAvailability()
    {
        // Arrange
        var quantity = 5;
        var sut = new SeatsAvailability(_reservationId);

        // Act
        sut.AddSeats(_seatTypeId, AvailableSeats);
        sut.RemoveSeats(_seatTypeId, quantity);

        // Assert
        Assert.Equal(_seatTypeId, ((AvailableSeatsChanged)sut.Events.Last()).Seats.ElementAt(0).SeatType);
        Assert.Equal(-quantity, ((AvailableSeatsChanged)sut.Events.Last()).Seats.ElementAt(0).Quantity);
    }

    [Fact]
    public void RemoveSeats_ForNonExistingSeatType_DoNothing()
    {
        // Arrange
        var unknownSeatType = Guid.NewGuid();
        var sut = new SeatsAvailability(_reservationId);

        // Act
        sut.AddSeats(_seatTypeId, AvailableSeats);
        sut.RemoveSeats(unknownSeatType, 5);

        // Assert
        Assert.DoesNotContain(sut.Events,
            @event => @event is AvailableSeatsChanged e && e.Seats.ElementAt(0).SeatType == unknownSeatType);
    }

    [Fact]
    public void CancelReservationChangesSeatsAvailability()
    {
        // Arrange
        var wantedSeats = 5;
        var sut = new SeatsAvailability(_reservationId);

        // Act
        sut.AddSeats(_seatTypeId, AvailableSeats);
        sut.MakeReservation(_reservationId, [new SeatQuantity(_seatTypeId, wantedSeats)]);
        sut.CancelReservation(_reservationId);

        // Assert
        Assert.Equal(_reservationId, ((SeatsReservationCancelled)sut.Events.Last()).ReservationId);
        Assert.Equal(_seatTypeId, ((SeatsReservationCancelled)sut.Events.Last()).AvailableSeatsChanged.ElementAt(0).SeatType);
        Assert.Equal(wantedSeats, ((SeatsReservationCancelled)sut.Events.Last()).AvailableSeatsChanged.ElementAt(0).Quantity);
    }

    [Fact]
    public void CancelReservation_ForNonExistingReservation_DoNothing()
    {
        // Arrange
        var sut = new SeatsAvailability(_reservationId);

        // Act
        sut.AddSeats(_seatTypeId, AvailableSeats);
        sut.MakeReservation(_reservationId, [new SeatQuantity(_seatTypeId, 4)]);
        sut.CancelReservation(Guid.NewGuid());

        // Assert
        Assert.DoesNotContain(sut.Events, e => e is SeatsReservationCancelled);
    }

    [Fact]
    public void CanCommitReservation()
    {
        // Arrange
        var sut = new SeatsAvailability(_reservationId);

        // Act
        sut.AddSeats(_seatTypeId, AvailableSeats);
        sut.MakeReservation(_reservationId, [new SeatQuantity(_seatTypeId, 3)]);
        sut.CommitReservation(_reservationId);

        // Assert
        Assert.Equal(_reservationId, ((SeatsReservationCommitted)sut.Events.Last()).ReservationId);
    }

    [Fact]
    public void CommittingNonExistingReservation_DoNothing()
    {
        // Arrange
        var sut = new SeatsAvailability(_reservationId);

        // Act
        sut.AddSeats(_seatTypeId, AvailableSeats);
        sut.MakeReservation(_reservationId, [new SeatQuantity(_seatTypeId, 3)]);
        sut.CommitReservation(Guid.NewGuid());

        // Assert
        Assert.DoesNotContain(sut.Events, e => e.GetType() == typeof(SeatsReservationCommitted));
    }

    [Fact]
    public void RequestingLessSeatsThanTotal_ReservesAllRequestedSeats()
    {
        // Arrange
        var wantedSeats = 8;
        var sut = new SeatsAvailability(_reservationId);

        // Act
        sut.AddSeats(_seatTypeId, AvailableSeats);
        sut.MakeReservation(_reservationId, [new SeatQuantity(_seatTypeId, wantedSeats)]);

        // Assert
        Assert.Equal(_reservationId, ((SeatsReserved)sut.Events.Last()).ReservationId);
        Assert.Equal(_seatTypeId, ((SeatsReserved)sut.Events.Last()).Details.ElementAt(0).SeatType);
        Assert.Equal(wantedSeats, ((SeatsReserved)sut.Events.Last()).Details.ElementAt(0).Quantity);
    }

    [Fact]
    public void RequestingMoreSeatsThanTotal_ReservingAllAvailableSeats()
    {
        // Arrange
        var wantedSeats = 12;
        var sut = new SeatsAvailability(_reservationId);

        // Act
        sut.AddSeats(_seatTypeId, AvailableSeats);
        sut.MakeReservation(_reservationId, [new SeatQuantity(_seatTypeId, wantedSeats)]);

        // Assert
        Assert.Equal(_reservationId, ((SeatsReserved)sut.Events.Last()).ReservationId);
        Assert.Equal(_seatTypeId, ((SeatsReserved)sut.Events.Last()).Details.ElementAt(0).SeatType);
        Assert.Equal(AvailableSeats, ((SeatsReserved)sut.Events.Last()).Details.ElementAt(0).Quantity);
    }

    [Fact]
    public void RequestingLessSeatsThanTotal_ReducesRemainingSeats()
    {
        // Arrange
        var wantedSeats = 8;
        var sut = new SeatsAvailability(_reservationId);

        // Act
        sut.AddSeats(_seatTypeId, AvailableSeats);
        sut.MakeReservation(_reservationId, [new SeatQuantity(_seatTypeId, wantedSeats)]);

        // Assert
        Assert.Equal(_seatTypeId, ((SeatsReserved)sut.Events.Last()).AvailableSeatsChanged.ElementAt(0).SeatType);
        Assert.Equal((-1) * wantedSeats, ((SeatsReserved)sut.Events.Last()).AvailableSeatsChanged.ElementAt(0).Quantity);
    }

    [Fact]
    public void RequestingMoreSeatsThanTotal_ReducesAllAvailableSeats()
    {
        // Arrange
        var wantedSeats = 12;
        var sut = new SeatsAvailability(_reservationId);

        // Act
        sut.AddSeats(_seatTypeId, AvailableSeats);
        sut.MakeReservation(_reservationId, [new SeatQuantity(_seatTypeId, wantedSeats)]);

        // Assert
        Assert.Equal(_seatTypeId, ((SeatsReserved)sut.Events.Last()).AvailableSeatsChanged.ElementAt(0).SeatType);
        Assert.Equal((-1) * AvailableSeats, ((SeatsReserved)sut.Events.Last()).AvailableSeatsChanged.ElementAt(0).Quantity);
    }

    [Fact]
    public void UpdateReservationWithLessSeats_IncreasesAvailableSeats()
    {
        // Arrange
        var seatsChanged = 4;
        var sut = new SeatsAvailability(_reservationId);

        // Act
        sut.AddSeats(_seatTypeId, AvailableSeats);
        sut.MakeReservation(_reservationId, [new SeatQuantity(_seatTypeId, 8)]);
        sut.MakeReservation(_reservationId, [new SeatQuantity(_seatTypeId, 4)]);

        // Assert
        Assert.Equal(_reservationId, ((SeatsReserved)sut.Events.Last()).ReservationId);
        Assert.Equal(seatsChanged, ((SeatsReserved)sut.Events.Last()).AvailableSeatsChanged.Single().Quantity);
    }

    [Fact]
    public void UpdateReservationWithMoreSeats_DecreasesAvailableSeats()
    {
        // Arrange
        var seatsChanged = -4;
        var sut = new SeatsAvailability(_reservationId);

        // Act
        sut.AddSeats(_seatTypeId, AvailableSeats);
        sut.MakeReservation(_reservationId, [new SeatQuantity(_seatTypeId, 4)]);
        sut.MakeReservation(_reservationId, [new SeatQuantity(_seatTypeId, 8)]);

        // Assert
        Assert.Equal(_reservationId, ((SeatsReserved)sut.Events.Last()).ReservationId);
        Assert.Equal(seatsChanged, ((SeatsReserved)sut.Events.Last()).AvailableSeatsChanged.Single().Quantity);
    }

    [Fact]
    public void UpdateReservationWithMoreSeatsThanRemaining_DecreasesAllAvailableSeats()
    {
        // Arrange
        var seatsChanged = -6;
        var sut = new SeatsAvailability(_reservationId);

        // Act
        sut.AddSeats(_seatTypeId, AvailableSeats);
        sut.MakeReservation(_reservationId, [new SeatQuantity(_seatTypeId, 4)]);
        sut.MakeReservation(_reservationId, [new SeatQuantity(_seatTypeId, 12)]);

        // Assert
        Assert.Equal(_reservationId, ((SeatsReserved)sut.Events.Last()).ReservationId);
        Assert.Equal(seatsChanged, ((SeatsReserved)sut.Events.Last()).AvailableSeatsChanged.Single().Quantity);
    }

    [Fact]
    public void RequestingAnInexistantSeatType_Throws()
    {
        // Arrange
        var sut = new SeatsAvailability(_reservationId);

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            sut.MakeReservation(_reservationId, [new SeatQuantity(Guid.NewGuid(), 7)]));
    }
}
