using EventBus.Eventos;
using EventBus.Subscription;

namespace EventBus.Bus
{
    public interface IEventoBus
    {
        event EventHandler<string> EventoRemovido;
        string ObterIdentificacaoEvento<TEvent>();
        bool PossuiSubscricaoParaEvento(string eventoNome);
        IEnumerable<Subscricao> ObterHandlersPorEvento(string nomeEvento);

        Type ObterTipoEventoPeloNome(string nomeEvento);

        void Limpar();

        /// <summary>
        /// Publica um evento 
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="evento"></param>
        Task Publicar<TEvent>(TEvent eventoNome)
            where TEvent : Eventos.Evento;

        /// <summary>
        /// Se increve em um evento para ficar escutando.
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="evento"></param>
        void Inscrever<TEvent,TEventhandler>()
            where TEvent : Eventos.Evento
            where TEventhandler : IEventoHandler<TEvent>;

        /// <summary>
        /// Remove a inscrição de um evento.
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="evento"></param>
        void Desinscrever<TEvent, TEventhandler>()
             where TEvent : Eventos.Evento
             where TEventhandler : IEventoHandler<TEvent>;


    }
}
