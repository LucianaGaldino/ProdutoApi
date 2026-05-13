using System.Reflection;
using ProdutoAPI.Database;
using ProdutoAPI.Repositories;
using ProdutoAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// ── Controllers ──────────────────────────────────────────
builder.Services.AddControllers();

// ── Swagger / OpenAPI ─────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "Produto API",
        Version = "v1",
        Description = "API de gerenciamento de produtos com arquitetura em camadas (Controller → Service → Repository)."
    });

    // Incluir comentários XML do código-fonte na documentação Swagger
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        options.IncludeXmlComments(xmlPath);
});

// ── Dependências da aplicação ─────────────────────────────
builder.Services.AddSingleton<DatabaseConnection>();
builder.Services.AddScoped<ProdutoRepository>();
builder.Services.AddScoped<ProdutoService>();

var app = builder.Build();

// ── Middleware ────────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Produto API v1");
        options.RoutePrefix = "swagger"; // Swagger em "/swagger"
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
