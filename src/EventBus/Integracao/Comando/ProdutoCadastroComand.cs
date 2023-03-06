using EventBus.Comandos;

namespace EventBus.Integracao.Events
{
    public class ProdutoCadastroComand : Comand
    {
        public Guid Id { get; private set; }
        public string Nome { get; private set; }
        public decimal Preco { get; private set; }
        public int Quantidade { get; private set; }
        public DateTime DataCadastro { get; private set; }

        public ProdutoCadastroComand(Guid id, string nome, decimal preco, int quantidade)
        {
            Nome = nome;
            Preco = preco;
            Quantidade = quantidade;
            DataCadastro = DateTime.Now;
            Id = id;
        }
    }
}
