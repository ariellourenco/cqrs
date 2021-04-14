using System;
using System.Collections.Generic;
using Xunit;

namespace Registration
{
    public sealed class SeatsAvailabilityTests
    {
        private const int AvailableSeats = 10;

        private static readonly Guid ReservationId = Guid.NewGuid();

        public class AddSeatsMethod
        {
            [Theory]
            [InlineData(17)]
            public void CanAddSeats(int seats)
            {
                // Arrange
                var sut = new SeatsAvailability(ReservationId);

                // Act
                sut.AddSeats(AvailableSeats);
                sut.AddSeats(seats);

                // Assert
                Assert.Equal((AvailableSeats + seats), sut.RemainingSeats);
            }
        }

        public class CommitReservationMethod
        {
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
            public void ThrowsExceptionWhenCommittingAnExpiredReservation()
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
            public void ThrowsExceptionWhenCommittingAnInexistantReservation()
            {
                // Arrange
                var requested = 6;
                var sut = new SeatsAvailability(ReservationId);

                sut.AddSeats(AvailableSeats);
                sut.MakeReservation(ReservationId, requested);

                // Act & Assert
                Assert.Throws<KeyNotFoundException>(() => sut.CommitReservation(Guid.NewGuid()));
            }
        }

        public class ExpireMethod
        {
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
            public void ThrowsExceptionWhenExpiringACommittedReservation()
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
            public void ThrowsExceptionWhenExpireAnInexistantReservation()
            {
                // Arrange
                var requested = 6;
                var sut = new SeatsAvailability(ReservationId);

                sut.AddSeats(AvailableSeats);
                sut.MakeReservation(ReservationId, requested);

                // Act & Assert
                Assert.Throws<KeyNotFoundException>(() => sut.Expire(Guid.NewGuid()));
            }
        }

        public class MakeReservationMethod
        {
            [Fact]
            public void ReservesSeatsWhenRequestingLessSeatsThanTotal()
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
            public void ThrowsExceptionWhenReservingMoreSeatsThanTotal()
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
}
