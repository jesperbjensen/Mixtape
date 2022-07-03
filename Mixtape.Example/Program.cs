using System.Reflection;
using Mixtape;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.MapMixtape("Mixtape.Example.Pages");

app.Run();