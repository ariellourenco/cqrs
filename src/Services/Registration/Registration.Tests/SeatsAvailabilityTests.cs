using System;
using System.Linq;
using CQRSJourney.Registration.Events;
using Xunit;

namespace CQRSJourney.Registration
{
    public sealed class SeatsAvailabilityTests
    {
        private const int AvailableSeats = 10;

        private static readonly Guid ReservationId = Guid.NewGuid();

        private static readonly Guid SeatTypeId = Guid.NewGuid();

        [Fact]
        public void AddSeats_ForNonExistingSeatType_ChangesSeatsAvailability()
        {
            // Arrange
            var sut = new SeatsAvailability(ReservationId);

            // Act
            sut.AddSeats(SeatTypeId, AvailableSeats);

            // Assert
            Assert.Equal(SeatTypeId, ((AvailableSeatsChanged)sut.Events.Single()).Seats.ElementAt(0).SeatType);
            Assert.Equal(AvailableSeats, ((AvailableSeatsChanged)sut.Events.Single()).Seats.ElementAt(0).Quantity);
        }

        [Fact]
        public void AddSeats_ForExistingSeatType_IncreaseSeatsAvailability()
        {
            // Arrange
            var quantity = 10;
            var sut = new SeatsAvailability(ReservationId);

            // Act
            sut.AddSeats(SeatTypeId, AvailableSeats);
            sut.AddSeats(SeatTypeId, quantity);

            // Assert
            Assert.Equal(SeatTypeId, ((AvailableSeatsChanged)sut.Events.LastOrDefault()).Seats.ElementAt(0).SeatType);
            Assert.Equal(quantity, ((AvailableSeatsChanged)sut.Events.LastOrDefault()).Seats.ElementAt(0).Quantity);
        }

        [Fact]
        public void RemoveSeats_ForExistingSeatType_ChangesSeatsAvailability()
        {
            // Arrange
            var quantity = 5;
            var sut = new SeatsAvailability(ReservationId);

            // Act
            sut.AddSeats(SeatTypeId, AvailableSeats);
            sut.RemoveSeats(SeatTypeId, quantity);

            // Assert
            Assert.Equal(SeatTypeId, ((AvailableSeatsChanged)sut.Events.LastOrDefault()).Seats.ElementAt(0).SeatType);
            Assert.Equal(-quantity, ((AvailableSeatsChanged)sut.Events.LastOrDefault()).Seats.ElementAt(0).Quantity);
        }

        [Fact]
        public void RemoveSeats_ForNonExistingSeatType_DoNothing()
        {
            // Arrange
            var unknownSeatType = Guid.NewGuid();
            var sut = new SeatsAvailability(ReservationId);

            // Act
            sut.AddSeats(SeatTypeId, AvailableSeats);
            sut.RemoveSeats(unknownSeatType, 5);

            // Assert
            Assert.DoesNotContain(sut.Events,
                @event => (@event is AvailableSeatsChanged e && e.Seats.ElementAt(0).SeatType == unknownSeatType));
        }

        [Fact]
        public void CancelReservationChangesSeatsAvailability()
        {
            // Arrange
            var wantedSeats = 5;
            var sut = new SeatsAvailability(ReservationId);

            // Act
            sut.AddSeats(SeatTypeId, AvailableSeats);
            sut.MakeReservation(ReservationId, new[] { new SeatQuantity(SeatTypeId, wantedSeats) });
            sut.CancelReservation(ReservationId);

            // Assert
            Assert.Equal(ReservationId, ((SeatsReservationCancelled)sut.Events.LastOrDefault()).ReservationId);
            Assert.Equal(SeatTypeId, ((SeatsReservationCancelled)sut.Events.LastOrDefault()).AvailableSeatsChanged.ElementAt(0).SeatType);
            Assert.Equal(wantedSeats, ((SeatsReservationCancelled)sut.Events.LastOrDefault()).AvailableSeatsChanged.ElementAt(0).Quantity);
        }

        [Fact]
        public void CancelReservation_ForNonExistingReservation_DoNothing()
        {
            // Arrange
            var sut = new SeatsAvailability(ReservationId);

            // Act
            sut.AddSeats(SeatTypeId, AvailableSeats);
            sut.MakeReservation(ReservationId, new[] { new SeatQuantity(SeatTypeId, 4) });
            sut.CancelReservation(Guid.NewGuid());

            // Assert
            Assert.DoesNotContain(sut.Events, e => (e is SeatsReservationCancelled));
        }

        [Fact]
        public void CanCommitReservation()
        {
            // Arrange
            var sut = new SeatsAvailability(ReservationId);

            // Act
            sut.AddSeats(SeatTypeId, AvailableSeats);
            sut.MakeReservation(ReservationId, new[] { new SeatQuantity(SeatTypeId, 3) });
            sut.CommitReservation(ReservationId);

            // Assert
            Assert.Equal(ReservationId, ((SeatsReservationCommitted)sut.Events.LastOrDefault()).ReservationId);
        }

        [Fact]
        public void CommittingNonExistingReservation_DoNothing()
        {
            // Arrange
            var sut = new SeatsAvailability(ReservationId);

            // Act
            sut.AddSeats(SeatTypeId, AvailableSeats);
            sut.MakeReservation(ReservationId, new[] { new SeatQuantity(SeatTypeId, 3) });
            sut.CommitReservation(Guid.NewGuid());

            // Assert
            Assert.DoesNotContain(sut.Events, e => e.GetType() == typeof(SeatsReservationCommitted));
        }

        [Fact]
        public void RequestingLessSeatsThanTotal_ReservesAllRequestedSeats()
        {
            // Arrange
            var wantedSeats = 8;
            var sut = new SeatsAvailability(ReservationId);

            // Act
            sut.AddSeats(SeatTypeId, AvailableSeats);
            sut.MakeReservation(ReservationId, new[] { new SeatQuantity(SeatTypeId, wantedSeats) });

            // Assert
            Assert.Equal(ReservationId, ((SeatsReserved)sut.Events.LastOrDefault()).ReservationId);
            Assert.Equal(SeatTypeId, ((SeatsReserved)sut.Events.LastOrDefault()).Details.ElementAt(0).SeatType);
            Assert.Equal(wantedSeats, ((SeatsReserved)sut.Events.LastOrDefault()).Details.ElementAt(0).Quantity);
        }

        [Fact]
        public void RequestingMoreSeatsThanTotal_ReservingAllAvailableSeats()
        {
            // Arrange
            var wantedSeats = 12;
            var sut = new SeatsAvailability(ReservationId);

            // Act
            sut.AddSeats(SeatTypeId, AvailableSeats);
            sut.MakeReservation(ReservationId, new[] { new SeatQuantity(SeatTypeId, wantedSeats) });

            // Assert
            Assert.Equal(ReservationId, ((SeatsReserved)sut.Events.LastOrDefault()).ReservationId);
            Assert.Equal(SeatTypeId, ((SeatsReserved)sut.Events.LastOrDefault()).Details.ElementAt(0).SeatType);
            Assert.Equal(AvailableSeats, ((SeatsReserved)sut.Events.LastOrDefault()).Details.ElementAt(0).Quantity);
        }

        [Fact]
        public void RequestingLessSeatsThanTotal_ReducesRemainingSeats()
        {
            // Arrange
            var wantedSeats = 8;
            var sut = new SeatsAvailability(ReservationId);

            // Act
            sut.AddSeats(SeatTypeId, AvailableSeats);
            sut.MakeReservation(ReservationId, new[] { new SeatQuantity(SeatTypeId, wantedSeats) });

            // Assert
            Assert.Equal(SeatTypeId, ((SeatsReserved)sut.Events.LastOrDefault()).AvailableSeatsChanged.ElementAt(0).SeatType);
            Assert.Equal((-1) * wantedSeats, ((SeatsReserved)sut.Events.LastOrDefault()).AvailableSeatsChanged.ElementAt(0).Quantity);
        }

        [Fact]
        public void RequestingMoreSeatsThanTotal_ReducesAllAvailableSeats()
        {
            // Arrange
            var wantedSeats = 12;
            var sut = new SeatsAvailability(ReservationId);

            // Act
            sut.AddSeats(SeatTypeId, AvailableSeats);
            sut.MakeReservation(ReservationId, new[] { new SeatQuantity(SeatTypeId, wantedSeats) });

            // Assert
            Assert.Equal(SeatTypeId, ((SeatsReserved)sut.Events.LastOrDefault()).AvailableSeatsChanged.ElementAt(0).SeatType);
            Assert.Equal((-1) * AvailableSeats, ((SeatsReserved)sut.Events.LastOrDefault()).AvailableSeatsChanged.ElementAt(0).Quantity);
        }

        [Fact]
        public void UpdateReservationWithLessSeats_IncreasesAvailableSeats()
        {
            // Arrange
            var seatsChanged = 4;
            var sut = new SeatsAvailability(ReservationId);

            // Act
            sut.AddSeats(SeatTypeId, AvailableSeats);
            sut.MakeReservation(ReservationId, new[] { new SeatQuantity(SeatTypeId, 8) });
            sut.MakeReservation(ReservationId, new[] { new SeatQuantity(SeatTypeId, 4) });

            // Assert
            Assert.Equal(ReservationId, ((SeatsReserved)sut.Events.LastOrDefault()).ReservationId);
            Assert.Equal(seatsChanged, ((SeatsReserved)sut.Events.LastOrDefault()).AvailableSeatsChanged.Single().Quantity);
        }

        [Fact]
        public void UpdateReservationWithMoreSeats_DecreasesAvailableSeats()
        {
            // Arrange
            var seatsChanged = -4;
            var sut = new SeatsAvailability(ReservationId);

            // Act
            sut.AddSeats(SeatTypeId, AvailableSeats);
            sut.MakeReservation(ReservationId, new[] { new SeatQuantity(SeatTypeId, 4) });
            sut.MakeReservation(ReservationId, new[] { new SeatQuantity(SeatTypeId, 8) });

            // Assert
            Assert.Equal(ReservationId, ((SeatsReserved)sut.Events.LastOrDefault()).ReservationId);
            Assert.Equal(seatsChanged, ((SeatsReserved)sut.Events.LastOrDefault()).AvailableSeatsChanged.Single().Quantity);
        }

        [Fact]
        public void UpdateReservationWithMoreSeatsThanRemaining_DecreasesAllAvailableSeats()
        {
            // Arrange
            var seatsChanged = -6;
            var sut = new SeatsAvailability(ReservationId);

            // Act
            sut.AddSeats(SeatTypeId, AvailableSeats);
            sut.MakeReservation(ReservationId, new[] { new SeatQuantity(SeatTypeId, 4) });
            sut.MakeReservation(ReservationId, new[] { new SeatQuantity(SeatTypeId, 12) });

            // Assert
            Assert.Equal(ReservationId, ((SeatsReserved)sut.Events.LastOrDefault()).ReservationId);
            Assert.Equal(seatsChanged, ((SeatsReserved)sut.Events.LastOrDefault()).AvailableSeatsChanged.Single().Quantity);
        }

        [Fact]
        public void RequestingAnInexistantSeatType_Throws()
        {
            // Arrange
            var sut = new SeatsAvailability(ReservationId);

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                sut.MakeReservation(ReservationId, new[] { new SeatQuantity(Guid.NewGuid(), 7) }));
        }
    }
}
