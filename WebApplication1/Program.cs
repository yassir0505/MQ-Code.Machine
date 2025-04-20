using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;

var builder = WebApplication.CreateBuilder(args);

// ✅ Add SQLite connection
builder.Services.AddDbContext<MQContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("MQConnection")));

builder.Services.AddRazorPages();
builder.Services.AddSession();

var app = builder.Build();

// ✅ Force Development mode temporarily (to see full errors)
if (false) // Normally: if (!app.Environment.IsDevelopment())
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

// ✅ Ensure database and tables are created before any query
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MQContext>();
    db.Database.EnsureCreated();

    var usersWithNulls = db.Users
        .Where(u => u.FirstName == null || u.LastName == null)
        .ToList();

    foreach (var user in usersWithNulls)
    {
        user.FirstName ??= "Default";
        user.LastName ??= "User";
    }

    db.SaveChanges();
}

app.Run();
