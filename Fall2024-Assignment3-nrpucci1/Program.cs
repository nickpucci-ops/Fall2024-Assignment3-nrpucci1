using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using Fall2024_Assignment3_nrpucci1.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Fall2024_Assignment3_nrpucci1.Services;
using Azure.AI.OpenAI;
using static System.Net.WebRequestMethods;


var builder = WebApplication.CreateBuilder(args);
//var openAIkey = "";
//var openAIendpoint = "";
//var openAIdeployment = "";

var openAIkey = builder.Configuration["AzureOpenAI:ApiKeySecret"];
var openAIendpoint = "https://fall2024-nrpucci-openai1.openai.azure.com/";
var openAIdeployment = "gpt-35-turbo1";

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
var connectionStringBuilder = new SqlConnectionStringBuilder(connectionString)
{
    Password = builder.Configuration["DbPassword"] // Retrieve the password from Secret Manager
};
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionStringBuilder.ConnectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();
//builder.Services.AddSingleton<AIService>();
builder.Services.AddScoped<AIService>(provider =>
    new AIService(
        openAIkey,
        openAIendpoint,
        openAIdeployment));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
