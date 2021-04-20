using System.Threading.Tasks;

namespace CQRSJourney.Infrastructure.EventBus.Abstractions
{
    /// <summary>
    /// Provides a way for systems to communicate without knowing about each other.
    /// </summary>
    public interface IEventBus
    {
        /// <summary>
        /// Publish an event asynchronously.
        /// </summary>
        /// <param name="event">Event for communicating information between systems.</param>
        Task PublishAsync(object @event);
    }
}
