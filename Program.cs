/*
    https://docs.microsoft.com/en-us/aspnet/core/tutorials/min-web-api?view=aspnetcore-6.0&tabs=visual-studio-code
    DEPLOY:
    https://www.halldorstefans.com/using-sqlite-in-net-core-azure-web-app/

    dotnet publish -c Release -o ./publish
    Copy-Paste your database, into the newly created 'publish' folder.
    Right-click the publish folder and select Deploy to Web App

*/
using fut5.Data;
using fut5.Authentication;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using Microsoft.OpenApi.Models; // necessário se quisermos swagger
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// to publish as openApi so we can add references from vstudio 2022
// https://dotnetthoughts.net/openapi-support-for-aspnetcore-minimal-webapi/
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(setup => setup.SwaggerDoc("v1", new OpenApiInfo()
{
    Description = "Fut5 Minimal Api",
    Title = "fut5 Api",
    Version = "v1",
    Contact = new OpenApiContact()
    {
        Name = "Z",
        Url = new Uri("https://z.primecog.com/MemoryGame/")
    }
}));

// CORS policy
builder.Services.AddCors(options =>
    {
        options.AddPolicy(
            name: "fut5 origins", // Must match app.UseCors("fut5 origins"); bellow
            builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .Build());
    });

// logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Add our services
builder.Services.AddSingleton<dbPathService>();
builder.Services.AddSingleton<ClubesDatabaseService>();
builder.Services.AddSingleton<AtletasDatabaseService>();
builder.Services.AddSingleton<JogosDatabaseService>();

// Authentication
// https://bytelanguage.net/2021/07/30/net-6-jwt-authentication-in-minimal-web-api/
builder.Services.AddSingleton<TokenService>(new TokenService());
builder.Services.AddSingleton<IUserRepositoryService>(new UserRepositoryService(new dbPathService()));
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>
{
    opt.TokenValidationParameters = new()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseAuthentication();
app.UseAuthorization();

// to publish as openApi so we can add references from vstudio 2022
// https://dotnetthoughts.net/openapi-support-for-aspnetcore-minimal-webapi/
app.UseSwagger();

app.MapGet("/", () => "Hello World!");

app.MapGet("/Atletas", [Authorize(Roles = "admin,user")] async (AtletasDatabaseService Atletas) => await Atletas.GetAtletas());
app.MapGet("/AtletaGet/{email}", [Authorize(Roles = "admin,user")] async (string email, AtletasDatabaseService Atletas) => await Atletas.GetAtleta(email));
app.MapPost("/AtletaSave", [Authorize(Roles = "admin,user")] async (Atleta atleta, AtletasDatabaseService Atletas) => await Atletas.AtletaSave(atleta));
app.MapGet("/AtletaDelete/{email}", [Authorize(Roles = "admin,user")] async (string email, AtletasDatabaseService Atletas) => await Atletas.AtletaDELETE(email));
app.MapPost("/AtletaChangePass", [Authorize(Roles = "admin,user")] async (Atleta atleta, AtletasDatabaseService Atletas) => await Atletas.AtletaSavePassword(atleta));

app.MapGet("/Clubes", [Authorize(Roles = "admin")] async (ClubesDatabaseService Clubes) => await Clubes.GetClubes());
app.MapPost("/ClubeSave", [Authorize(Roles = "admin")] async (Clube clube, ClubesDatabaseService Clubes) => await Clubes.AddClube(clube));
app.MapGet("/ClubeDelete/{clube}", [Authorize(Roles = "admin")] async (String clube, ClubesDatabaseService Clubes) => await Clubes.DELETEClube(clube));
app.MapGet("/ClubeMembers/{clubename}", [Authorize(Roles = "admin")] async (string clubename, ClubesDatabaseService Clubes) => await Clubes.GetMembersFor(clubename));
app.MapGet("/ClubesGetFor/{email}", [Authorize(Roles = "admin")] async (string email, ClubesDatabaseService Clubes) => await Clubes.GetClubesFor(email));
app.MapPost("/ClubesSetFor/{email}", [Authorize(Roles = "admin")] async (string email, ClubeMember[] clubMembership, ClubesDatabaseService Clubes) => await Clubes.SetClubesFor(email, clubMembership));

app.MapGet("/Games", [Authorize(Roles = "admin,user")] async (JogosDatabaseService jogosService) => await jogosService.GetJogos());
app.MapGet("/Game/{clube}/{data1}", [Authorize(Roles = "admin,user")] async (string clube, string data1, JogosDatabaseService jogosService) => await jogosService.GetJogo(clube, data1));
app.MapGet("/GamePresences/{clube}/{data1}", [Authorize(Roles = "admin,user")] async (string clube, string data1, JogosDatabaseService jogosService) => await jogosService.GetPresencas(clube, data1));
app.MapGet("/GameCreate/{clube}/{data1}/{data2}", [Authorize(Roles = "admin")] async (string clube, string data1, string data2, JogosDatabaseService jogosService) => await jogosService.JogoSave(clube, data1, data2));
app.MapGet("/GameCreate/{clube}/{data1}/{data2}/{data3}", [Authorize(Roles = "admin")] async (string clube, string data1, string data2, string data3, JogosDatabaseService jogosService) => await jogosService.JogoSave(clube, data1, data2, data3));
app.MapGet("/GameCancel/{clube}/{data1}", [Authorize(Roles = "admin")] async (string clube, string data1, JogosDatabaseService jogosService) => await jogosService.JogoCancel(clube, data1));
app.MapGet("/GamePresenceSave/{clube}/{data1}/{email}/{resposta}", [Authorize(Roles = "admin,user")] async (string clube, string data1, string email, int resposta, JogosDatabaseService jogosService) => await jogosService.SavePresenca(clube, data1, email, resposta));
app.MapGet("/GamesGetFor/{email}", [Authorize(Roles = "admin,user")] async (string email, JogosDatabaseService jogosService) => await jogosService.GetMyGames(email));
app.MapGet("/GamesPurge", [Authorize(Roles = "admin")] async (JogosDatabaseService jogosService) => await jogosService.Purge());

app.MapGet("/GetClaims", ([Authorize(Roles = "admin,user")] (ClaimsPrincipal user) =>
{
    var r = new System.Text.StringBuilder();
    foreach (var item in user.Claims)
    {
        if (item.Type.Contains("/"))
        {
            var value = JsonEncodedText.Encode(item.Value); 
            var t = item.Type.Substring(item.Type.LastIndexOf("/") + 1);
            if (r.Length > 0) r.Append(",");
            r.Append($"\"{t}\":\"{value}\"");
        }
    }
    return "{" + r.ToString() + "}";
}
));

// ##################################################################################################################
app.MapPost("/login", [AllowAnonymous] async (
                                UserModel userModel,
                                TokenService tokenService,
                                IUserRepositoryService userRepositoryService,
                                HttpResponse response) =>
{
    var atleta = userRepositoryService.ValidateCredentials(userModel);
    if (atleta.Email == "")
    {
        response.StatusCode = 401;
        return;
    }
    var token = tokenService.BuildToken(
        builder.Configuration["Jwt:Key"],
        builder.Configuration["Jwt:Issuer"],
        builder.Configuration["Jwt:Audience"],
        atleta);
    await response.WriteAsJsonAsync(new { token = token });
    return;
}).Produces(StatusCodes.Status200OK).WithName("Login").WithTags("Accounts");

// to publish as openApi so we can add references from vstudio 2022
// https://dotnetthoughts.net/openapi-support-for-aspnetcore-minimal-webapi/
app.UseSwaggerUI();

app.UseCors("fut5 origins");

app.Logger.LogInformation("Starting the app.");

app.Run();