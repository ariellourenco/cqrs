using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace CQRSJourney.Registration
{
    public sealed class SeatsAvailabilityTests
    {
        private const int AvailableSeats = 10;

        private static readonly Guid ReservationId = Guid.NewGuid();

        private static readonly Guid SeatTypeId = Guid.NewGuid();


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
            Assert.Equal(ReservationId, ((SeatsReserved)sut.Events.Single()).ReservationId);
            Assert.Equal(SeatTypeId, ((SeatsReserved)sut.Events.Single()).Details.ElementAt(0).SeatType);
            Assert.Equal(wantedSeats, ((SeatsReserved)sut.Events.Single()).Details.ElementAt(0).Quantity);
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
            Assert.Equal(ReservationId, ((SeatsReserved)sut.Events.Single()).ReservationId);
            Assert.Equal(SeatTypeId, ((SeatsReserved)sut.Events.Single()).Details.ElementAt(0).SeatType);
            Assert.Equal(AvailableSeats, ((SeatsReserved)sut.Events.Single()).Details.ElementAt(0).Quantity);
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
            Assert.Equal(SeatTypeId, ((SeatsReserved)sut.Events.Single()).AvailableSeatsChanged.ElementAt(0).SeatType);
            Assert.Equal((-1) * wantedSeats, ((SeatsReserved)sut.Events.Single()).AvailableSeatsChanged.ElementAt(0).Quantity);
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
            Assert.Equal(SeatTypeId, ((SeatsReserved)sut.Events.Single()).AvailableSeatsChanged.ElementAt(0).SeatType);
            Assert.Equal((-1) * AvailableSeats, ((SeatsReserved)sut.Events.Single()).AvailableSeatsChanged.ElementAt(0).Quantity);
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
            Assert.Equal(ReservationId, ((SeatsReserved)sut.Events.Last()).ReservationId);
            Assert.Equal(seatsChanged, ((SeatsReserved)sut.Events.Last()).AvailableSeatsChanged.Single().Quantity);
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
            Assert.Equal(ReservationId, ((SeatsReserved)sut.Events.Last()).ReservationId);
            Assert.Equal(seatsChanged, ((SeatsReserved)sut.Events.Last()).AvailableSeatsChanged.Single().Quantity);
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
            Assert.Equal(ReservationId, ((SeatsReserved)sut.Events.Last()).ReservationId);
            Assert.Equal(seatsChanged, ((SeatsReserved)sut.Events.Last()).AvailableSeatsChanged.Single().Quantity);
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
