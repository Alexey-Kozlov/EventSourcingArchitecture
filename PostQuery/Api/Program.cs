using Confluent.Kafka;
using CQRSCore.Consumers;
using CQRSCore.Infrastructure;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using PostQuery.Api.Queries;
using PostQuery.Domain.Entities;
using PostQuery.Domain.Repositories;
using PostQuery.Infrastructure.Consumers;
using PostQuery.Infrastructure.DBContext;
using PostQuery.Infrastructure.Dispathcers;
using PostQuery.Infrastructure.Handlers;
using PostQuery.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);
// builder.Services.AddDbContext<DataContext>(options =>
// {
//     options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
// });
Action<DbContextOptionsBuilder> configureDbContext =
(x => x.UseLazyLoadingProxies().UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDbContext<DataContext>(configureDbContext);
builder.Services.AddSingleton(new DataContextFactory(configureDbContext));

#pragma warning disable ASP0000 // Do not call 'IServiceCollection.BuildServiceProvider' in 'ConfigureServices'
var dataContext = builder.Services.BuildServiceProvider().GetRequiredService<DataContext>();
#pragma warning restore ASP0000 // Do not call 'IServiceCollection.BuildServiceProvider' in 'ConfigureServices'
dataContext.Database.EnsureCreated();

builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IEventHandler, PostQuery.Infrastructure.Handlers.EventHandler>();
builder.Services.Configure<ConsumerConfig>(builder.Configuration.GetSection(nameof(ConsumerConfig)));
builder.Services.AddScoped<IEventConsumer, EventConsumer>();
builder.Services.AddHostedService<ConsumerHostedService>();
builder.Services.AddScoped<IQueryHandler, QueryHandler>();

#pragma warning disable ASP0000 // Do not call 'IServiceCollection.BuildServiceProvider' in 'ConfigureServices'
var queryHandler = builder.Services.BuildServiceProvider().GetRequiredService<IQueryHandler>();
#pragma warning restore ASP0000 // Do not call 'IServiceCollection.BuildServiceProvider' in 'ConfigureServices'
var dispatcher = new QueryDispathcer();
dispatcher.RegisterHandler<FindAllPostsQuery>(queryHandler.HandleAsync);
dispatcher.RegisterHandler<FindPostByIdQuery>(queryHandler.HandleAsync);
dispatcher.RegisterHandler<FindPostsByAuthorQuery>(queryHandler.HandleAsync);
dispatcher.RegisterHandler<FindPostsWithCommentQuery>(queryHandler.HandleAsync);
dispatcher.RegisterHandler<FindPostsWithLikeQuery>(queryHandler.HandleAsync);
builder.Services.AddSingleton<IQueryDispatcher<Post>>(_ => dispatcher);

builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

app.Run();
