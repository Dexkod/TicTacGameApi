
using System.Data.Common;
using System.Text;
using WebApplication1.Entities;
using WebApplication1.SerializationEntities;

namespace WebApplication1
{
    class Program
    {


        public static void Main(string[] args)
        {
            
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options => 
            {
                options.Cookie.Name = ".MyApp.Session";
                options.IdleTimeout = TimeSpan.FromSeconds(3600);
                options.Cookie.IsEssential = true;
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseSession();

            app.MapPost("/api", async (Players game, HttpContext context) =>
            {
                
                context.Response.ContentType = "application/json";

                if (game != null)
                {
                    var newGame = new Game(game.Player1, game.Player2);
                    context.Session.SetString("Game", newGame.Id.ToString());
                    await context.Response.WriteAsJsonAsync(newGame.MessageStartGame());
                }
                else
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsJsonAsync("Неправильные данные");
                }

            });

            app.MapGet("/api", async (HttpContext context) =>
            {
                byte[] gameBytes;
                var game = context.Session.TryGetValue("Game", out gameBytes) ?
                    OrganizerMatch.SearchGame(Encoding.UTF8.GetString(gameBytes)) : null;
                context.Response.Headers.ContentType = "application/json";

                if (game == null)
                {
                    await context.Response.WriteAsJsonAsync("Игра не найдена");
                }
                else
                {
                    await context.Response.WriteAsJsonAsync(game.GetField());
                }
                
            });

            app.MapGet("/api/{column}/{row}", async (string column, string row ,HttpContext context) =>
            {
                int Column = int.Parse(column);
                int Row = int.Parse(row);
                if (Column > 2 || Column < 0 || Row > 2 || Row < 0)
                {
                    await context.Response.WriteAsJsonAsync("Неправильные данные");
                    return;
                }

                byte[] gameBytes;
                var game = context.Session.TryGetValue("Game", out gameBytes) ?
                    OrganizerMatch.SearchGame(Encoding.UTF8.GetString(gameBytes)) : null;

                context.Response.Headers.ContentType = "application/json";

                if (game == null)
                {
                    await context.Response.WriteAsJsonAsync("Игра не найдена");
                }
                else
                {
                    await context.Response.WriteAsJsonAsync(game[Column, Row]);
                }
            });

            app.MapPatch("/api", async(Move move, HttpContext context) => 
            {
                int Column = move.Column;
                int Row = move.Row;
                if (Column > 2 || Column < 0 || Row > 2 || Row < 0)
                {
                    await context.Response.WriteAsJsonAsync("Неправильные данные");
                    return;
                }

                byte[] gameBytes;
                var game = context.Session.TryGetValue("Game", out gameBytes) ?
                    OrganizerMatch.SearchGame(Encoding.UTF8.GetString(gameBytes)) : null;

                context.Response.Headers.ContentType = "application/json";

                if(game == null)
                {
                    await context.Response.WriteAsJsonAsync("Игра не найдена");
                }
                else
                {
                    string message;
                    var field = game.Move(Column, Row, out message);
                    await context.Response.WriteAsJsonAsync(new ResponseObject(field, message));
                }
            });
            app.Run();

        }


    }
}
