using ExamRegistrationUoJ.Components;
using ExamRegistrationUoJ.Services;
using ExamRegistrationUoJ.Services.MySQL;
using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using System.Linq;
using ExamRegistrationUoJ.Services.DBInterfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddControllers();
builder.Services.AddSingleton<AuthInterface, ExamAuth>();
builder.Services.AddSingleton<DBInterface, DBSakilaTest>();
builder.Services.AddSingleton<IDBServiceAdmin1, DBMySQL>();
builder.Services.AddSingleton<IDBServiceCoordinator1, DBMySQL>();
// builder.Services.AddSingleton<IDBServiceStudentHome, DBMySQL>();
// builder.Services.AddSingleton<IDBServiceStudentRegistration, DBMySQL>();

var auth = new AuthPlaceholder();

builder.Services.AddAuthentication("Cookies")
    .AddCookie(opt =>
    {
        opt.Cookie.Name = "AuthCookie";
    })
    .AddMicrosoftAccount(opt =>
    {
        opt.SignInScheme = "Cookies";
        opt.ClientId = "b8129309-d65d-46c5-891b-7bf863174808";
        opt.ClientSecret = "w758Q~EyvTIFA0Ng4Kvzuj~IznX2Q2vF_FSuLb3p";
    });
builder.Services.AddAuthorization(opt => 
{
    opt.AddPolicy("IsAdmin", policy =>
        policy.RequireAssertion(async context =>
        {
            return await auth.IsAnAdministrator((context.User.FindFirst(ClaimTypes.Email) == null)? null: context.User.FindFirst(ClaimTypes.Email).Value);
        }));
    opt.AddPolicy("IsCoordinator", policy =>
        policy.RequireAssertion(async context =>
        {
            return await auth.IsACoordinator((context.User.FindFirst(ClaimTypes.Email) == null) ? null : context.User.FindFirst(ClaimTypes.Email).Value,
                (context.User.FindFirst(ClaimTypes.NameIdentifier) == null) ? null : context.User.FindFirst(ClaimTypes.NameIdentifier).Value);
        }));
    opt.AddPolicy("IsAdvisor", policy =>
        policy.RequireAssertion(async context =>
        {
            return await auth.IsAnAdvisor((context.User.FindFirst(ClaimTypes.Email) == null) ? null : context.User.FindFirst(ClaimTypes.Email).Value);
        }));
    opt.AddPolicy("IsStudent", policy =>
        policy.RequireAssertion(async context =>
        {
            return await auth.IsAStudent((context.User.FindFirst(ClaimTypes.Email) == null) ? null : context.User.FindFirst(ClaimTypes.Email).Value,
                (context.User.FindFirst(ClaimTypes.NameIdentifier) == null) ? null : context.User.FindFirst(ClaimTypes.NameIdentifier).Value);
        }));
    opt.AddPolicy("IsBoth", policy =>
        policy.RequireAssertion(async context =>
        {
            return await auth.IsBothAdvisorCoordinator((context.User.FindFirst(ClaimTypes.Email) == null) ? null : context.User.FindFirst(ClaimTypes.Email).Value,
                (context.User.FindFirst(ClaimTypes.NameIdentifier) == null) ? null : context.User.FindFirst(ClaimTypes.NameIdentifier).Value);
        }));
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
