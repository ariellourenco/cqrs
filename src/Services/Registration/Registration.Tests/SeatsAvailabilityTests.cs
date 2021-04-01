using System;
using Xunit;

namespace Registration
{
    public sealed class SeatsAvailabilityTests
    {
        private static readonly Guid TicketTypeId = Guid.NewGuid();

        public SeatsAvailability GivenAvailableSeats()
        {
            var sut = new SeatsAvailability();
            sut.AddSeats(10, TicketTypeId);

            return sut;
        }

        [Fact]
        public void WhenReservingLessSeatsThanAvailableThenSucceeds()
        {
            var sut = GivenAvailableSeats();
            sut.MakeReservation(Guid.NewGuid(), 4, TicketTypeId);
        }
    }
}
