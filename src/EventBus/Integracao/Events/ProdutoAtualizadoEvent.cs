using EventBus.Eventos;

namespace EventBus.Integracao.Events
{
    public class ProdutoAtualizadoEvent : Evento
    {
        public Guid Id { get; private set; }
        public string Nome { get; private set; }

        public ProdutoAtualizadoEvent(Guid id, string nome)
        {
            Nome = nome;
            Id = id;
        }
    }
}
