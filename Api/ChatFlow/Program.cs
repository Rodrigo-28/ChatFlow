using ChatFlow.Application.Extensions;
using ChatFlow.Application.Mappings;
using ChatFlow.Extensions;
using ChatFlow.Infrastructure.Contexts;
using ChatFlow.Infrastructure.Extensions;
using ChatFlow.Middleware;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllers();


//Swagger extension
builder.Services.AddCustomSwagger();

//services infrastructure
builder.Services.AddInfrastructureServices();
//
builder.Services.AddApplicationServices();


//Validator service
builder.Services.AddCustomValidators();

builder.Services.AddHttpClient();

//Add applicationDbContext service
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

//Add authorization and authentication
builder.Services.AddCustomAuth(builder.Configuration);

//mapping
builder.Services.AddAutoMapper(typeof(UserProfile));



// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();




// Configure CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();
app.UseCors("AllowAllOrigins");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

}

app.UseMiddleware<ExceptionMiddleware>();

var webSocketOptions = new WebSocketOptions()
{
    KeepAliveInterval = TimeSpan.FromMinutes(2),

};

app.UseWebSockets(webSocketOptions);
app.UseMiddleware<WebSocketMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
