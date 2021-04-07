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
        public void WhenReservingLessSeatsThanTotalThenSucceeds()
        {
            var sut = GivenAvailableSeats();
            sut.MakeReservation(Guid.NewGuid(), 4);
        }

        [Fact]
        public void WhenReservingLessSeatsThanRemainingThenSucceeds()
        {
            var sut = GivenSomeAvilableSeatsAndSomeTaken();
            sut.MakeReservation(Guid.NewGuid(), 4);
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
        public void WhenReservationExpiresThenSucceeds()
        {
            var sut = GivenSomeAvilableSeatsAndSomeTaken();
            sut.Expires(ReservationId);
        }

        [Fact]
        public void WhenReservationExpiresThenCanReuseSeats()
        {
            var sut = GivenSomeAvilableSeatsAndSomeTaken();
            sut.Expires(ReservationId);
            sut.MakeReservation(ReservationId, 8);
        }

        [Fact]
        public void WhenAnInexistantReservationExpiresThenFails()
        {
            var sut = GivenSomeAvilableSeatsAndSomeTaken();
            Assert.Throws<KeyNotFoundException>(() => sut.Expires(Guid.NewGuid()));
        }
    }
}
