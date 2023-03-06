using EventBus.Bus;
using EventBus.Integracao.ComandHandler;
using EventBus.Integracao.EventHandler;
using EventBus.Integracao.Events;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Trabalhando_Eventos.DTO;
using Trabalhando_Eventos.Entidade;

namespace Trabalhando_Eventos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutoController : ControllerBase
    {

        private readonly IEventoBus _eventoBus;
        private readonly ICommandBus _comandBus;
        public ProdutoController(IEventoBus eventobus,
            ICommandBus comandBus)
        {
            _eventoBus = eventobus;
            _comandBus = comandBus;
        }

        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> Add(ProdutoAddDTO model)
        {
            var produto = new Produto(model.Nome, model.Preco, model.Quantidade);

            //publica comando
            var command = new ProdutoCadastroComand(produto.Id, produto.Nome, produto.Preco, produto.Quantidade);
            await _comandBus.Publicar(command);

            //caso querira remover a subscricao do evento
            //_comandBus.Desinscrever<ProdutoCadastroComand,ProdutoCommandHandler>();

            //publica evento ( algo que ocorreu)
            var evento = new ProdutoCadastradoEvent(produto.Id, produto.Nome, produto.Preco, produto.Quantidade);
            await _eventoBus.Publicar(evento);

            _eventoBus.Desinscrever<ProdutoCadastradoEvent, ProdutoEventHandler>();

            return Ok(produto);
        }

        [HttpPut]
        [Route("update")]
        public async Task<IActionResult> Update(ProdutoUpdateDTO model)
        {
            var produto = new Produto(model.Nome, model.Preco, model.Quantidade);

            return Ok(produto);
        }
    }
}
