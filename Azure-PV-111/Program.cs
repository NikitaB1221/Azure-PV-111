using Azure_PV_111.Cron;
using Azure_PV_111.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<DataMiddleware>();

builder.Configuration.AddJsonFile("azuresettings.json");

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(10);
    options.Cookie.HttpOnly = true;
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

CronTask.Add(() => System.Console.WriteLine("CroneTask5"), 5);
CronTask.Add(() => System.Console.WriteLine("CroneTask10"), 10);
CronTask.Add(
    action: DataMiddleware.RemoveExpired,
    seconds: DataMiddleware.LifeTime
    );
CronTask.Start();

app.UseRouting();

app.UseAuthorization();

app.UseMiddleware<DataMiddleware>();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
