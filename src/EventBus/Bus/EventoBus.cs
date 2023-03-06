using EventBus.Eventos;
using EventBus.Subscription;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;

namespace EventBus.Bus
{
    public class EventoBus : IEventoBus
    {
        private readonly ILogger<EventoBus> _logger;
        private readonly IServiceProvider _serviceProvider;

        private readonly Dictionary<string, List<Subscricao>> _handlers = new Dictionary<string, List<Subscricao>>();
        private readonly List<Type> _tiposEventos = new List<Type>();
        public event EventHandler<string> EventoRemovido;


        public EventoBus(ILogger<EventoBus> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            ConfiguracaoMessageEvents();
        }

        public async Task Publicar<TEvent>(TEvent evento) where TEvent : Eventos.Evento
        {
            var subscricoes = ObterHandlersPorEvento(evento.GetType().Name);
            foreach (var subscricao in subscricoes)
            {
                var handler = _serviceProvider.GetService(subscricao.TipoHandler);
                if (handler == null)
                {
                    _logger.LogWarning("Não há manipuladores para o seguinte evento: {EventName}", nameof(evento));
                    continue;
                }


                var eventoTipo = ObterTipoEventoPeloNome(evento.GetType().Name);

                var eventoHandlerTipo = typeof(IEventoHandler<>).MakeGenericType(eventoTipo);

                // Faz com que o método retorne imediatamente
                 await Task.Yield();

                // Este código vai ser executado no futuro
                await(Task)eventoHandlerTipo.GetMethod(nameof(IEventoHandler<Eventos.Evento>.HandleAsync)).Invoke(handler, new object[] { evento });

                await Task.CompletedTask;
            }
        }

        public void Inscrever<TEvent, TEventhandler>()
                where TEvent : Eventos.Evento
                where TEventhandler : IEventoHandler<TEvent>
        {

            var eventoNome = ObterIdentificacaoEvento<TEvent>();
            var eventoHandlerName = typeof(TEventhandler).Name;
            AdicionarSubscricao<TEvent, TEventhandler>();
            _logger.LogInformation("Escutando o evento {EventName} com {EvenHandler}.", eventoNome, eventoHandlerName);

        }

        public void Desinscrever<TEvent, TEventhandler>()
                where TEvent : Evento
                where TEventhandler : IEventoHandler<TEvent>
        {

            var eventoName = ObterIdentificacaoEvento<TEvent>();

            _logger.LogInformation("Cancelando inscrição no evento {EventName}...", eventoName);

            RemoverSubscricao<TEvent, TEventhandler>();

            _logger.LogInformation("Inscrição cancelada no evento {EventName}.", eventoName);
        }



        private void AdicionarSubscricao<TEvent, TEventHandler>()
            where TEvent : Evento
            where TEventHandler : IEventoHandler<TEvent>
        {
            var eventoNome = ObterIdentificacaoEvento<TEvent>();

            FazSubscricao(typeof(TEvent), typeof(TEventHandler), eventoNome);

            if (!_tiposEventos.Contains(typeof(TEvent)))
                _tiposEventos.Add(typeof(TEvent));
        }

        private void RemoverSubscricao<TEvent, TEventHandler>()
            where TEvent : Evento
            where TEventHandler : IEventoHandler<TEvent>
        {
            var handlerToRemove = EncontraSubscricaoParaRemover<TEvent, TEventHandler>();
            var eventName = ObterIdentificacaoEvento<TEvent>();
            FazRemocaoHandler(eventName, handlerToRemove);
        }



        public Type ObterTipoEventoPeloNome(string nomeEvento)
        {
            return _tiposEventos.SingleOrDefault(t => t.Name == nomeEvento);
        }

        public IEnumerable<Subscricao> ObterHandlersPorEvento(string nomeEvento)
        {
            return _handlers[nomeEvento];
        }

        public string ObterIdentificacaoEvento<TEvent>()
        {
            return typeof(TEvent).Name;
        }

        public bool PossuiSubscricaoParaEvento(string eventoNome)
        {
            return _handlers.ContainsKey(eventoNome);
        }

        private Subscricao EncontraSubscricaoParaRemover<TEvent, TEventHandler>()
              where TEvent : Eventos.Evento
             where TEventHandler : IEventoHandler<TEvent>
        {
            var eventName = ObterIdentificacaoEvento<TEvent>();
            return ProcuraEventoParaRemover(eventName, typeof(TEventHandler));
 
        }

        private Subscricao ProcuraEventoParaRemover(string eventoName, Type tipoHandler)
        {
            if (!PossuiSubscricaoParaEvento(eventoName))
                return null;

            return _handlers[eventoName].SingleOrDefault(s => s.TipoHandler == tipoHandler);
        }
        private void FazSubscricao(Type tipoEvento, Type tipoHandler, string eventoNome)
        {
            if (!PossuiSubscricaoParaEvento(eventoNome))
                _handlers.Add(eventoNome, new List<Subscricao>());

            if (_handlers[eventoNome].Any(x => x.TipoHandler == tipoHandler))
                throw new ArgumentException($"O Handler {tipoHandler.Name} já foi registrado para o {eventoNome}");

            _handlers[eventoNome].Add(new Subscricao(tipoEvento, tipoHandler));
        }
        private void OnEventoRemovido(string eventName)
        {
            var handler = EventoRemovido;
            handler?.Invoke(this, eventName);
        }

        private void ConfiguracaoMessageEvents()
        {
            EventoRemovido += AvisoRemovidoSubscricao;
        }
        private void AvisoRemovidoSubscricao(object sender, string evento)
        {
            _logger.LogWarning($"Foi removido a subscricao para o evento {evento}");
        }
        private void FazRemocaoHandler(string eventName, Subscricao subscriptionToRemove)
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

            var eventType = _tiposEventos.SingleOrDefault(e => e.Name == eventName);
            if (eventType != null)
            {
                _tiposEventos.Remove(eventType);
            }

            OnEventoRemovido(eventName);
        }

        public void Limpar()
        {
            _handlers.Clear();
            _tiposEventos.Clear();
        }

    }
}
