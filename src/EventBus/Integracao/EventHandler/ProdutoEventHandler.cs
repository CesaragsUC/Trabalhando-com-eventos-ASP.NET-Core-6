using EventBus.Eventos;
using EventBus.Integracao.ComandHandler;
using EventBus.Integracao.Events;
using Microsoft.Extensions.Logging;

namespace EventBus.Integracao.EventHandler
{
    public class ProdutoEventHandler :
        IEventoHandler<ProdutoCadastradoEvent>,
        IEventoHandler<ProdutoAtualizadoEvent>
    {
        private readonly ILogger<ProdutoEventHandler> _logger;
        public ProdutoEventHandler(ILogger<ProdutoEventHandler> logger)
        {
            _logger = logger;
        }

        public async Task HandleAsync(ProdutoAtualizadoEvent evento)
        {
            _logger.LogWarning("Um produto foi atualizado");
            await Task.CompletedTask;
        }

        public async Task HandleAsync(ProdutoCadastradoEvent evento)
        {
            _logger.LogWarning("Um produto foi cadastrado");
            await Task.CompletedTask;
        }
    }
}
