using EventBus.Command;
using EventBus.Integracao.Events;
using Microsoft.Extensions.Logging;

namespace EventBus.Integracao.ComandHandler
{
    public class ProdutoCommandHandler :
        ICommandHandler<ProdutoCadastroComand>
    {
        private readonly ILogger<ProdutoCommandHandler> _logger;
        public ProdutoCommandHandler(ILogger<ProdutoCommandHandler> logger)
        {
            _logger = logger;
        }

        public async Task<bool> HandleAsync(ProdutoCadastroComand command)
        {
            //aplicar alguma logica aqui.

            _logger.LogWarning("Foi realizado um comand");
            return true;
        }
    }
}
