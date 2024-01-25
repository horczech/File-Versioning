using FluentValidation;
using MediatR;
using FileVersioning.Server;
using FileVersioning.Server.Application;
using FileVersioning.Server.Application.Validators;
using FileVersioning.Server.Storage;
using FileVersioning.Server.Validators;

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
