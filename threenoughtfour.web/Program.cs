using ThreeZeroFour.Web.Components;
using ThreeZeroFour.Services;
using ThreeZeroFour.Web.Game;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddScoped<IDeckService>(_ => new DeckService(new Random()));
builder.Services.AddScoped<IGameRulesService, GameRulesService>();
builder.Services.AddScoped<IGameConsole, SilentGameConsole>();
builder.Services.AddScoped<IPlayerDecisionService, PlayerDecisionService>();
builder.Services.AddScoped<GameSession>();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
