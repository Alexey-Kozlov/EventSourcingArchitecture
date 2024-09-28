using CQRSCore.Domain;
using CQRSCore.Handlers;
using CQRSCore.Infrastructure;
using PostCmd.Api.Commands;
using PostCmd.Domain.Aggregates;
using PostCmd.Infrastructure;
using PostCmd.Infrastructure.Config;
using PostCmd.Infrastructure.Handlers;
using PostCmd.Infrastructure.Repositories;
using PostCmd.Infrastructure.Stores;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MongoDbConfig>(builder.Configuration.GetSection(nameof(MongoDbConfig)));
builder.Services.AddScoped<IEventStoreRepository, EventStoreRepository>();
builder.Services.AddScoped<IEventStore, EventStore>();
builder.Services.AddScoped<IEventSourceHandler<PostAggregates>, EventSourceHandler>();
builder.Services.AddScoped<ICommandHandler, CommandHandler>();
builder.Services.AddControllers();

//предупреждение - что будут задублированы все синглтоны-сервисы. В нашем случае не актуально,
//т.к. у нас все ранее объявленные сервисы - Scoped. Предупреждение отключено
#pragma warning disable ASP0000 // Do not call 'IServiceCollection.BuildServiceProvider' in 'ConfigureServices'
var commandHandler = builder.Services.BuildServiceProvider().GetRequiredService<ICommandHandler>();
#pragma warning restore ASP0000 // Do not call 'IServiceCollection.BuildServiceProvider' in 'ConfigureServices'
var dispatcher = new CommandDispatcher();
dispatcher.RegisterHandler<NewPostCommand>(commandHandler.HandlerAsync);
dispatcher.RegisterHandler<EditMessageCommand>(commandHandler.HandlerAsync);
dispatcher.RegisterHandler<LikePostCommand>(commandHandler.HandlerAsync);
dispatcher.RegisterHandler<AddCommentCommand>(commandHandler.HandlerAsync);
dispatcher.RegisterHandler<EditCommentCommand>(commandHandler.HandlerAsync);
dispatcher.RegisterHandler<DeleteCommentCommand>(commandHandler.HandlerAsync);
dispatcher.RegisterHandler<DeletePostCommand>(commandHandler.HandlerAsync);
builder.Services.AddSingleton<ICommandDispatcher>(_ => dispatcher);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
