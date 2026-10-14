using System.Globalization;
using BIT240049_Mid;
using BIT240049_Mid.Data;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// ---- Da ngon ngu ----
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services
    .AddControllersWithViews(options =>
    {
        options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;

        // Thong bao loi khi model binding that bai (vd: nhap chu vao o so) - dich theo ngon ngu hien tai
        var p = options.ModelBindingMessageProvider;
        p.SetValueMustNotBeNullAccessor(_ => Res.Get("ValueRequired"));
        p.SetValueMustBeANumberAccessor(v => Res.Get("ValueNotNumber", v));
        p.SetValueIsInvalidAccessor(v => Res.Get("ValueInvalid", v));
        p.SetAttemptedValueIsInvalidAccessor((v, _) => Res.Get("ValueInvalid", v));
    })
    .AddViewLocalization()
    .AddDataAnnotationsLocalization(options =>
    {
        // Moi ErrorMessage / Display(Name) tren Model deu duoc tra theo key trong SharedResource
        options.DataAnnotationLocalizerProvider = (type, factory) => factory.Create(typeof(SharedResource));
    });

// Culture (dinh dang so) co dinh en-US de nhap/hien thi so khong bi doi dau . , khi doi ngon ngu;
// UI culture (en / vi) quyet dinh ngon ngu giao dien.
var uiCultures = new[] { new CultureInfo("en"), new CultureInfo("vi") };
var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(culture: "en-US", uiCulture: "en"),
    SupportedCultures = new[] { new CultureInfo("en-US") },
    SupportedUICultures = uiCultures
};
// Chi dung cookie. Bo Accept-Language cua trinh duyet => lan dau truy cap luon la English.
localizationOptions.RequestCultureProviders.Clear();
localizationOptions.RequestCultureProviders.Add(new CookieRequestCultureProvider());

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStatusCodePagesWithReExecute("/Home/HttpStatus", "?code={0}");
app.UseStaticFiles();
app.UseRequestLocalization(localizationOptions);
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dishes}/{action=Index}/{id?}");

app.Run();
