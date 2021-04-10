using System;
using System.Collections.Generic;
using Xunit;

namespace Registration
{
    public sealed class SeatsAvailabilityTests
    {
        private static readonly Guid ReservationId = Guid.NewGuid();

        public SeatsAvailability GivenAvailableSeats()
        {
            var sut = new SeatsAvailability(ReservationId);
            sut.AddSeats(10);

            return sut;
        }

        public SeatsAvailability GivenSomeAvilableSeatsAndSomeTaken()
        {
            var sut = GivenAvailableSeats();
            sut.MakeReservation(ReservationId, 6);

            return sut;
        }

        [Fact]
        public void WhenReservingLessSeatsThanTotalThenSeatsBecomeUnavailable()
        {
            // Arrange
            var sut = GivenAvailableSeats();

            // Act
            sut.MakeReservation(Guid.NewGuid(), 4);

            // Assert
            Assert.Equal(6, sut.RemainingSeats);
        }

        [Fact]
        public void WhenReservingLessSeatsThanRemainingThenSeatsBecomeUnavailable()
        {
            // Arrange
            var sut = GivenSomeAvilableSeatsAndSomeTaken();

            // Act
            sut.MakeReservation(Guid.NewGuid(), 4);

            // Assert
            Assert.Equal(0, sut.RemainingSeats);
        }

        [Fact]
        public void WhenReservingMoreSeatsThanTotalThenFails()
        {
            var sut = GivenAvailableSeats();
            Assert.Throws<ArgumentOutOfRangeException>(() => sut.MakeReservation(Guid.NewGuid(), 11));
        }

        [Fact]
        public void WhenReservingMoreSeatsThanRemainingThenFails()
        {
            var sut = GivenSomeAvilableSeatsAndSomeTaken();
            Assert.Throws<ArgumentOutOfRangeException>(() => sut.MakeReservation(Guid.NewGuid(), 5));
        }

        [Fact]
        public void WhenReservationExpiresThenSeatsBecomeAvailable()
        {
            // Arrange
            var sut = GivenSomeAvilableSeatsAndSomeTaken();

            // Act
            sut.Expire(ReservationId);

            // Assert
            Assert.Equal(10, sut.RemainingSeats);
        }

        [Fact]
        public void WhenReservationExpiresThenCanReuseSeats()
        {
            var sut = GivenSomeAvilableSeatsAndSomeTaken();
            sut.Expire(ReservationId);
            sut.MakeReservation(ReservationId, 8);
        }

        [Fact]
        public void WhenAnInexistantReservationExpiresThenFails()
        {
            var sut = GivenSomeAvilableSeatsAndSomeTaken();
            Assert.Throws<KeyNotFoundException>(() => sut.Expire(Guid.NewGuid()));
        }

        [Fact]
        public void WhenReservationIsCommittedThenRemainingSeatsAreNotModified()
        {
            // Arrange
            var sut = GivenSomeAvilableSeatsAndSomeTaken();
            var remaining = sut.RemainingSeats;

            // Act
            sut.CommitReservation(ReservationId);

            Assert.Equal(remaining, sut.RemainingSeats);
        }

        [Fact]
        public void WhenAnInexistantReservationIsCommittedThenFails()
        {
            var sut = GivenSomeAvilableSeatsAndSomeTaken();
            Assert.Throws<KeyNotFoundException>(() => sut.CommitReservation(Guid.NewGuid()));
        }

        [Fact]
        public void WhenReservationIsCommittedThenCannotExpireIt()
        {
            var sut = GivenSomeAvilableSeatsAndSomeTaken();
            sut.CommitReservation(ReservationId);
            Assert.Throws<KeyNotFoundException>(() => sut.Expire(ReservationId));
        }
    }
}
