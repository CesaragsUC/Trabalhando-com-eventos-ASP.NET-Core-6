using EventBus.Eventos;

namespace EventBus.Command
{
    public interface ICommandHandler<TRequest> where TRequest : class
    {
        Task<bool> HandleAsync(TRequest command);
    }
}
