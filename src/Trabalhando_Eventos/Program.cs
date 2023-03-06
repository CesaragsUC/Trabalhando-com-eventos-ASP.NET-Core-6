using EventBus.Bus;
using EventBus.Integracao.ComandHandler;
using EventBus.Integracao.EventHandler;
using EventBus.Integracao.Events;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddSingleton<IEventoBus, EventoBus>();
builder.Services.AddSingleton<ICommandBus, CommandBus>();

builder.Services.AddTransient<ProdutoEventHandler>();
builder.Services.AddTransient<ProdutoCommandHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


var eventBus = app.Services.GetRequiredService<IEventoBus>();//registrado como Singlenton

var comandBus = app.Services.GetRequiredService<ICommandBus>();//registrado como Singlenton

// Aqui você adiciona os manipuladores de eventos para cada evento de integração.
eventBus.Inscrever<ProdutoCadastradoEvent, ProdutoEventHandler>();
eventBus.Inscrever<ProdutoAtualizadoEvent, ProdutoEventHandler>();

comandBus.Inscrever<ProdutoCadastroComand, ProdutoCommandHandler>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
