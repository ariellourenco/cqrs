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
            var sut = new SeatsAvailability(ReservationId);

            // Act
            sut.AddSeats(SeatTypeId, AvailableSeats);
            sut.MakeReservation(ReservationId, new[] { new SeatQuantity(SeatTypeId, 8) });

            // Assert
            Assert.Equal(ReservationId, ((SeatsReserved)sut.Events.Single()).ReservationId);
            Assert.Equal(SeatTypeId, ((SeatsReserved)sut.Events.Single()).Details.ElementAt(0).SeatType);
            Assert.Equal(8, ((SeatsReserved)sut.Events.Single()).Details.ElementAt(0).Quantity);
        }

        [Fact]
        public void RequestingMoreSeatsThanTotal_ReservingAllAvailableSeats()
        {
            // Arrange
            var sut = new SeatsAvailability(ReservationId);

            // Act
            sut.AddSeats(SeatTypeId, AvailableSeats);
            sut.MakeReservation(ReservationId, new[] { new SeatQuantity(SeatTypeId, 12) });

            // Assert
            Assert.Equal(ReservationId, ((SeatsReserved)sut.Events.Single()).ReservationId);
            Assert.Equal(SeatTypeId, ((SeatsReserved)sut.Events.Single()).Details.ElementAt(0).SeatType);
            Assert.Equal(AvailableSeats, ((SeatsReserved)sut.Events.Single()).Details.ElementAt(0).Quantity);
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
