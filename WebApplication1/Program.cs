using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MQContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("MQConnection")));

builder.Services.AddRazorPages();
builder.Services.AddSession();

var app = builder.Build();

// ✅ Production error handling
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

// ✅ Apply migrations instead of EnsureCreated
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MQContext>();
    db.Database.Migrate(); // <-- ضروري هادي

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
