using System;
using System.Collections.Generic;
using Xunit;

namespace CQRSJourney.Registration
{
    public sealed class SeatsAvailabilityTests
    {
        private const int AvailableSeats = 10;

        private static readonly Guid ReservationId = Guid.NewGuid();

        [Fact]
        public void CanAddSeats()
        {
            // Arrange
            var seats = 10;
            var sut = new SeatsAvailability(ReservationId);

            // Act
            sut.AddSeats(AvailableSeats);
            sut.AddSeats(seats);

            // Assert
            Assert.Equal((AvailableSeats + seats), sut.RemainingSeats);
        }

        [Fact]
        public void CanCommitsReservation()
        {
            // Arrange
            var requested = 6;
            var remaining = 4;
            var sut = new SeatsAvailability(ReservationId);

            sut.AddSeats(AvailableSeats);
            sut.MakeReservation(ReservationId, requested);

            // Act
            sut.CommitReservation(ReservationId);

            // Assert
            Assert.Equal(remaining, sut.RemainingSeats);
        }

        [Fact]
        public void CommitReservation_WhenReservationIsExpired_Throws()
        {
            // Arrange
            var requested = 6;
            var sut = new SeatsAvailability(ReservationId);

            sut.AddSeats(AvailableSeats);
            sut.MakeReservation(ReservationId, requested);
            sut.Expire(ReservationId);

            // Act & Assert
            Assert.Throws<KeyNotFoundException>(() => sut.CommitReservation(Guid.NewGuid()));
        }

        [Fact]
        public void CommitReservation_WhenReservationIsInexistant_Throws()
        {
            // Arrange
            var requested = 6;
            var sut = new SeatsAvailability(ReservationId);

            sut.AddSeats(AvailableSeats);
            sut.MakeReservation(ReservationId, requested);

            // Act & Assert
            Assert.Throws<KeyNotFoundException>(() => sut.CommitReservation(Guid.NewGuid()));
        }

        [Fact]
        public void CanExpireReservation()
        {
            // Arrange
            var requested = 6;
            var sut = new SeatsAvailability(ReservationId);

            sut.AddSeats(AvailableSeats);
            sut.MakeReservation(ReservationId, requested);

            // Act
            sut.Expire(ReservationId);

            // Assert
            Assert.Equal(AvailableSeats, sut.RemainingSeats);
        }

        [Fact]
        public void Expire_WhenReservationIsCommitted_Throws()
        {
            // Arrange
            var requested = 6;
            var sut = new SeatsAvailability(ReservationId);

            sut.AddSeats(AvailableSeats);
            sut.MakeReservation(ReservationId, requested);
            sut.CommitReservation(ReservationId);

            // Act & Assert
            Assert.Throws<KeyNotFoundException>(() => sut.Expire(ReservationId));
        }

        [Fact]
        public void Expire_WhenReservationIsInexistant_Throws()
        {
            // Arrange
            var requested = 6;
            var sut = new SeatsAvailability(ReservationId);

            sut.AddSeats(AvailableSeats);
            sut.MakeReservation(ReservationId, requested);

            // Act & Assert
            Assert.Throws<KeyNotFoundException>(() => sut.Expire(Guid.NewGuid()));
        }

        [Fact]
        public void MakeReservation_WhenRequestingLessSeatsThanTotal()
        {
            // Arrange
            var requested = 6;
            var remaining = 4;
            var sut = new SeatsAvailability(ReservationId);

            // Act
            sut.AddSeats(AvailableSeats);
            sut.MakeReservation(ReservationId, requested);

            // Assert
            Assert.Equal(remaining, sut.RemainingSeats);
        }

        [Fact]
        public void MakeReservation_WhenReservingMoreSeatsThanTotal_Throws()
        {
            // Arrange
            var requested = 11;
            var sut = new SeatsAvailability(ReservationId);

            sut.AddSeats(AvailableSeats);

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => sut.MakeReservation(ReservationId, requested));
        }
    }
}
