using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;

var builder = WebApplication.CreateBuilder(args);

// Add SQLite connection
builder.Services.AddDbContext<MQContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("MQConnection")));

builder.Services.AddRazorPages();
builder.Services.AddSession();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<MQContext>();

    var usersWithNulls = context.Users
        .Where(u => u.FirstName == null || u.LastName == null)
        .ToList();

    foreach (var user in usersWithNulls)
    {
        user.FirstName ??= "Default";
        user.LastName ??= "User";
    }

    context.SaveChanges();
}
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MQContext>();
    db.Database.EnsureCreated();
}


app.Run();
