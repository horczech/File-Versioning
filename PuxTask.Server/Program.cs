using FluentValidation;
using MediatR;
using PuxTask.Server;
using PuxTask.Server.Application;
using PuxTask.Server.Application.Validators;
using PuxTask.Server.Storage;
using PuxTask.Server.Validators;

// Add services to the container.
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(typeof(FolderAnalyzer).Assembly);
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));

});
builder.Services.AddValidatorsFromAssembly(typeof(FolderAnalyzerValidator).Assembly);
builder.Services.AddSingleton<IFileStateStorage, FileStateStorage>();

builder.Services.AddControllers(options =>
{
    options.Filters.Add(new ValidationExceptionFilter());
});

//Add Middleware
var app = builder.Build();

app.UseDeveloperExceptionPage(); 
app.UseDefaultFiles();
app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
