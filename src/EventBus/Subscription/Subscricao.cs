namespace EventBus.Subscription
{
    /// <summary>
    /// Representa uma assinatura de evento. As assinaturas controlam quando ouvimos eventos.
    /// </summary>
    public class Subscricao
    {
        public Type TipoEvento { get; private set; }
        public Type TipoHandler { get; private set; }

        public Subscricao(Type eventType, Type handlerType)
        {
            TipoEvento = eventType;
            TipoHandler = handlerType;
        }
    }

    public class SubscricaoCommand
    {
        public Type TipoComando { get; private set; }
        public Type TipoHandler { get; private set; }

        public SubscricaoCommand( Type handlerType , Type commandType)
        {
            TipoHandler = handlerType;
            TipoComando = commandType;
        }
    }
}
