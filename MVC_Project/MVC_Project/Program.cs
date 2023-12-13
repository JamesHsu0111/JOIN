using Microsoft.EntityFrameworkCore;
using MVC_Project.Models;
using SmartBreadcrumbs.Extensions;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Unicode;

var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

// Add services to the container.
builder.Services.AddControllersWithViews();

//連接
builder.Services.AddDbContext<ProjectXContext>(
      options => options.UseSqlServer(builder.Configuration.GetConnectionString("ProjectXconnection")));

// 連接到第二個資料庫

//重載(目前好像沒用到，但之後應該會需要就先放著了)
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

//JSON
builder.Services
    .AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;    //讓property不要變小寫
        options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);  //編碼全放
        options.JsonSerializerOptions.WriteIndented = true;  //自動排版JSON輸出
    });

// CORS連線
builder.Services.AddCors(options => {
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy => {
                          policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                      });
});

//-----麵包屑-----
builder.Services.AddBreadcrumbs(Assembly.GetExecutingAssembly());

builder.Services.AddBreadcrumbs(Assembly.GetExecutingAssembly(), options =>
{
	options.TagName = "nav";
	options.TagClasses = "mt-3 fs-5";
	options.OlClasses = "breadcrumb";
	options.LiClasses = "breadcrumb-item";
	options.ActiveLiClasses = "breadcrumb-item";
    options.SeparatorElement = "<li class=\"separator\">&nbsp<i class=\"fa-solid fa-chevron-right\"></i>&nbsp</li>";
});
//-----麵包屑-----

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".AdventureWorks.Session";
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.IsEssential = true;
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

//CORS
app.UseCors(MyAllowSpecificOrigins);

app.UseRouting();

app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=MyActivity}/{action=Homepage}/{id?}/{account?}");

app.Run();
