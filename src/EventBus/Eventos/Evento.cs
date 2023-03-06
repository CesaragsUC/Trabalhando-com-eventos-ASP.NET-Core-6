namespace EventBus.Eventos
{
    //Representa um evento de integração. 
    public abstract class Evento
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
