using EventBus.Comandos;
using EventBus.Command;
using EventBus.Eventos;
using EventBus.Subscription;

namespace EventBus.Bus
{
    public interface ICommandBus
    {
        Type ObterTipoComandoPeloNome(string comando);
        IEnumerable<SubscricaoCommand> ObterHandlersPorRequest(string nomeRequest);
        string ObterIdentificacaoRequest<TRequest>();
        bool PossuiSubscricaoParaRequest(string nomeRequest);
        event EventHandler<string> CommandRemovido;
        public string ObterIdentificacaoECommand<TRequest>();
        void Limpar();

        /// <summary>
        /// Se increve em um evento para ficar escutando.
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <param name="evento"></param>
        void Inscrever<TRequest, TCommandHandler>()
            where TRequest : Comand
            where TCommandHandler : ICommandHandler<TRequest>;

        Task Publicar<TRequest>(TRequest command)
            where TRequest : Comand;


        void Desinscrever<TRequest, TCommandHandler>()
            where TRequest : Comand
            where TCommandHandler : ICommandHandler<TRequest>;
    }
}
