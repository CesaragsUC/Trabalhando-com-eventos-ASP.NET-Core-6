using EventBus.Comandos;
using EventBus.Command;
using EventBus.Eventos;
using EventBus.Subscription;
using Microsoft.Extensions.Logging;

namespace EventBus.Bus
{
    public class CommandBus : ICommandBus
    {
        private readonly Dictionary<string, List<SubscricaoCommand>> _handlers = new Dictionary<string, List<SubscricaoCommand>>();
        private readonly List<Type> _tiposComandos = new List<Type>();
        private readonly List<Type> _tiposEventos = new List<Type>();
        public event EventHandler<string> CommandRemovido;


        private readonly ILogger<EventoBus> _logger;
        private readonly IServiceProvider _serviceProvider;

        public CommandBus(ILogger<EventoBus> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            ConfiguraMessageEventos();
        }

        public void Inscrever<TRequest, TCommandHandler>()
                where TRequest : Comand
                where TCommandHandler : ICommandHandler<TRequest>
        {

            var commandName = ObterIdentificacaoRequest<TRequest>();
            var handlerName = typeof(TCommandHandler).Name;

            AdicionarSubscricao<TRequest, TCommandHandler>();

            _logger.LogInformation("Escutando o evento {RequestName} com {CommandHandler}.", commandName, handlerName);

        }

        public async Task Publicar<TRequest>(TRequest command) where TRequest : Comand
        {
            var subscricoes = ObterHandlersPorRequest(command.GetType().Name);
            foreach (var subscricao in subscricoes)
            {
                var handler = _serviceProvider.GetService(subscricao.TipoHandler);
                if (handler == null)
                {
                    _logger.LogWarning("Não há manipuladores para o seguinte evento: {EventName}", nameof(command));
                    continue;
                }


                var eventoTipo = ObterTipoComandoPeloNome(command.GetType().Name);

                var eventoHandlerTipo = typeof(ICommandHandler<>).MakeGenericType(eventoTipo);

                // Faz com que o método retorne imediatamente
                await Task.Yield();

                // Este código vai ser executado no futuro
                await (Task)eventoHandlerTipo.GetMethod(nameof(ICommandHandler<Comand>.HandleAsync)).Invoke(handler, new object[] { command });

                await Task.CompletedTask;
            }
        }

        public void AdicionarSubscricao<TRequest, TCommandHandler>()
                where TRequest : Comand
                where TCommandHandler : ICommandHandler<TRequest>
        {
            var commandName = ObterIdentificacaoRequest<TRequest>();

            FazSubscricao(typeof(TRequest), typeof(TCommandHandler), commandName);

            if (!_tiposComandos.Contains(typeof(TRequest)))
                _tiposComandos.Add(typeof(TRequest));
        }

        public void Desinscrever<TRequest, TCommandHandler>()
                where TRequest : Comand
                where TCommandHandler : ICommandHandler<TRequest>
        {

            var eventoName = ObterIdentificacaoRequest<TRequest>();

            _logger.LogInformation("Cancelando inscrição no evento {EventName}...", eventoName);

            RemoverSubscricao<TRequest, TCommandHandler>();

            _logger.LogInformation("Inscrição cancelada no evento {EventName}.", eventoName);
        }

        private void RemoverSubscricao<TRequest, TCommandHandler>()
            where TRequest : Comand
            where TCommandHandler : ICommandHandler<TRequest>
        {
            var handlerToRemove = EncontraSubscricaoParaRemover<TRequest, TCommandHandler>();
            var commandName = ObterIdentificacaoRequest<TRequest>();
            FazRemocaoHandler(commandName, handlerToRemove);

        }
        public Type ObterTipoComandoPeloNome(string comando)
        {
            return _tiposComandos.SingleOrDefault(t => t.Name == comando);
        }
        public IEnumerable<SubscricaoCommand> ObterHandlersPorRequest(string nomeRequest)
        {
            return _handlers[nomeRequest];
        }
        public string ObterIdentificacaoRequest<TRequest>()
        {
            return typeof(TRequest).Name;
        }
        private void FazSubscricao(Type tipoRequest, Type tipoHandler, string commandName)
        {
            if (!PossuiSubscricaoParaRequest(commandName))
                _handlers.Add(commandName, new List<SubscricaoCommand>());

            if (_handlers[commandName].Any(x => x.TipoHandler == tipoHandler))
                throw new ArgumentException($"O Handler {tipoHandler.Name} já foi registrado para o {commandName}");

            _handlers[commandName].Add(new SubscricaoCommand(tipoHandler,tipoRequest));
        }

        public bool PossuiSubscricaoParaRequest(string eventoNome)
        {
            return _handlers.ContainsKey(eventoNome);
        }

        private void FazRemocaoHandler(string eventName, SubscricaoCommand subscriptionToRemove)
        {
            if (subscriptionToRemove == null)
            {
                return;
            }

            _handlers[eventName].Remove(subscriptionToRemove);
            if (_handlers[eventName].Any())
            {
                return;
            }

            _handlers.Remove(eventName);
            var eventType = _tiposComandos.SingleOrDefault(e => e.Name == eventName);
            if (eventType != null)
            {
                _tiposComandos.Remove(eventType);
            }

            OnComandSubscricaoRemovido(eventName);
        }
        private void ConfiguraMessageEventos()
        {
            CommandRemovido += AvisoSubscricaoCommandRemovido;
        }
        private void AvisoSubscricaoCommandRemovido(object sender, string command)
        {
            _logger.LogWarning($"Foi removido a subscricao para o evento {command}");
        }

        private void OnComandSubscricaoRemovido(string eventName)
        {
            var handler = CommandRemovido;
            handler?.Invoke(this, eventName);
        }

        private SubscricaoCommand EncontraSubscricaoParaRemover<TRequest, TCommandHandler>()
              where TRequest : Comand
              where TCommandHandler : ICommandHandler<TRequest>
        {
            var eventName = ObterIdentificacaoECommand<TRequest>();
            return ProcuraEventoParaRemover(eventName, typeof(TCommandHandler));
        }

        private SubscricaoCommand ProcuraEventoParaRemover(string commandName, Type tipoHandler)
        {
            if (!PossuiSubscricaoParaRequest(commandName))
                return null;

            return _handlers[commandName].SingleOrDefault(s => s.TipoHandler == tipoHandler);

        }

        public string ObterIdentificacaoECommand<TRequest>()
        {
            return typeof(TRequest).Name;
        }

        public void Limpar()
        {
            _handlers.Clear();
            _tiposComandos.Clear();
        }
    }
}
