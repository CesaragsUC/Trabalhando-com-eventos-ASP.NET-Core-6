namespace EventBus.Eventos
{
    //Contrato para manipuladores de eventos.
    //Os manipuladores de eventos são responsáveis por processar eventos quando eles acontecem.
    // "TEvent"  Tipo do Evento
    public interface IEventoHandler<TEvent> where TEvent : class
    {
        Task HandleAsync(TEvent evento);
    }
}
