using Microsoft.EntityFrameworkCore;
using FlowerShopApp.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using FlowerShopApp.Models;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

var app = builder.Build();

// Seed database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    
    // Seed Categories
    if (!context.Categories.Any(c => c.Slug == "guller-v2"))
    {
        // Clear old categories/products if any (re-branding)
        context.Products.RemoveRange(context.Products);
        context.Categories.RemoveRange(context.Categories);
        context.SaveChanges();

        context.Categories.AddRange(
            new Category { Name = "Güller", Slug = "guller-v2" },
            new Category { Name = "Orkideler", Slug = "orkideler" },
            new Category { Name = "Laleler", Slug = "laleler" },
            new Category { Name = "Papatyalar", Slug = "papatyalar" },
            new Category { Name = "Özel Aranjmanlar", Slug = "ozel-aranjmanlar" }
        );
        context.SaveChanges();
    }
    
    if (!context.Products.Any())
    {
        var catGuller = context.Categories.First(c => c.Name == "Güller");
        var catOrkideler = context.Categories.First(c => c.Name == "Orkideler");
        var catLaleler = context.Categories.First(c => c.Name == "Laleler");
        var catPapatyalar = context.Categories.First(c => c.Name == "Papatyalar");
        var catOzel = context.Categories.First(c => c.Name == "Özel Aranjmanlar");

        context.Products.AddRange(
            // Güller
            new Product { CategoryId = catGuller.Id, Name = "Klasik Kırmızı Gül Buketi", Slug = "kirmizi-gul-buketi", Stock=50, Description = "Romantizmin değişmez simgesi 12 adet taze kırmızı gül.", Price = 749.90m, ImageUrl = "https://images.unsplash.com/photo-1548094891-c4ba474eb5c1?w=600&q=80", Badge = "Çok Satan", BadgeColor = "danger" },
            new Product { CategoryId = catGuller.Id, Name = "Beyaz Gül Masumiyeti", Slug = "beyaz-gul-masumiyeti", Stock=15, Description = "Zarafetin ve masumiyetin simgesi 11 adet taze beyaz gül.", Price = 799.00m, ImageUrl = "https://images.unsplash.com/photo-1533616688419-b7a585564566?w=600&q=80", Badge = "", BadgeColor = "" },
            new Product { CategoryId = catGuller.Id, Name = "Pembe Gül Rüyası", Slug = "pembe-gul-ruyasi", Stock=20, Description = "İçinizi ısıtacak capcanlı ve mis kokulu pembe güller.", Price = 699.00m, ImageUrl = "https://images.unsplash.com/photo-1502977249166-824b3a8a4d6d?w=600&q=80", Badge = "Fırsat", BadgeColor = "warning text-dark" },
            
            // Orkideler
            new Product { CategoryId = catOrkideler.Id, Name = "Beyaz Phalaenopsis Orkide", Slug = "beyaz-orkide", Stock=12, Description = "İki dallı, uzun ömürlü ve son derece zarif beyaz orkide.", Price = 1199.50m, ImageUrl = "https://images.unsplash.com/photo-1599388143285-88571431e7da?w=600&q=80", Badge = "Yeni", BadgeColor = "success" },
            new Product { CategoryId = catOrkideler.Id, Name = "Fuşya Orkide Zarafeti", Slug = "fusya-orkide", Stock=8, Description = "Göz alıcı rengiyle büyüleyen çift dallı fuşya orkide.", Price = 1249.00m, ImageUrl = "https://images.unsplash.com/photo-1566417713940-053b5d21ef8c?w=600&q=80", Badge = "", BadgeColor = "" },
            
            // Laleler
            new Product { CategoryId = catLaleler.Id, Name = "Sarı Lale Buketi", Slug = "sari-lale-buketi", Stock=30, Description = "Neşe ve umut saçan taze 15 adet sarı lale.", Price = 549.90m, ImageUrl = "https://images.unsplash.com/photo-1520763185298-1b434c919102?w=600&q=80", Badge = "Popüler", BadgeColor = "info" },
            new Product { CategoryId = catLaleler.Id, Name = "Renkli Lale Karışımı", Slug = "renkli-lale-karisimi", Stock=25, Description = "Baharın tüm renklerini barındıran canlı lale aranjmanı.", Price = 649.00m, ImageUrl = "https://images.unsplash.com/photo-1516706030300-36a5c37efb99?w=600&q=80", Badge = "", BadgeColor = "" },
            
            // Papatyalar
            new Product { CategoryId = catPapatyalar.Id, Name = "Hüsnüyusuf ve Papatya", Slug = "husnuyusuf-papatya", Stock=40, Description = "Kır havasını evinize taşıyan doğal papatya buketi.", Price = 399.50m, ImageUrl = "https://images.unsplash.com/photo-1464335384910-449e37768f6e?w=600&q=80", Badge = "", BadgeColor = "" },
            new Product { CategoryId = catPapatyalar.Id, Name = "Büyük Beyaz Papatyalar", Slug = "buyuk-papatya-buketi", Stock=50, Description = "Samimiyetin en doğal hali, taze büyük papatyalar.", Price = 449.00m, ImageUrl = "https://images.unsplash.com/photo-1560717789-0ac7c58ac90a?w=600&q=80", Badge = "Hızlı Teslimat", BadgeColor = "primary" },
            
            // Özel Aranjmanlar
            new Product { CategoryId = catOzel.Id, Name = "Bahar Esintisi Kutusu", Slug = "bahar-esintisi", Stock=10, Description = "Şık kutu içerisinde lüks bahar çiçekleri aranjmanı.", Price = 1499.90m, ImageUrl = "https://images.unsplash.com/photo-1563241527-3004b7be0ffd?w=600&q=80", Badge = "Vip", BadgeColor = "dark" },
            new Product { CategoryId = catOzel.Id, Name = "Rengarenk Kır Buketi", Slug = "kir-cicekleri-buketi", Stock=30, Description = "Özenle seçilmiş mevsim kır çiçeklerinden dev buket.", Price = 899.50m, ImageUrl = "https://images.unsplash.com/photo-1525310238806-e19703250a85?w=600&q=80", Badge = "", BadgeColor = "" }
        );
        context.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "product_detail",
    pattern: "urun/{slug}",
    defaults: new { controller = "Home", action = "ProductDetail" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
