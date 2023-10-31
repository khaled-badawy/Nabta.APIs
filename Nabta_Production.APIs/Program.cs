using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Nabta_Production.BL;
using Nabta_Production.DAL;
using Nabta_Production.DAL.Data;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

#region Default
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
#endregion

#region Database

var connectionString = builder.Configuration.GetConnectionString("Nabta_test_server");

builder.Services.AddDbContext<ClimateConfNewContext>(options =>
    options.UseSqlServer(connectionString));

#endregion

#region Identity

builder.Services.AddIdentity<ApplicationUser, IdentityRole<int>>(options =>
{
    options.SignIn.RequireConfirmedEmail = true;
    options.SignIn.RequireConfirmedPhoneNumber = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.User.RequireUniqueEmail = true;
    options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;

    // apply following settings to seed old users

    //options.User.RequireUniqueEmail = false;
    //options.Password.RequireUppercase = false;
    //options.Password.RequireLowercase = false;
    //options.Password.RequireDigit = false;
    //options.Password.RequireNonAlphanumeric = false;
    //options.Password.RequiredLength = 3;
    //options.Password.RequiredUniqueChars = 0;
})
    .AddEntityFrameworkStores<ClimateConfNewContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
options.TokenLifespan = TimeSpan.FromDays(7));

#endregion

#region JWT Authentication

builder.Services.Configure<JWT>(builder.Configuration.GetSection("JWT"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.SaveToken = false;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JWT:Issuer"],
            ValidAudience = builder.Configuration["JWT:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["JWT:SecretKey"]!))
        };

    });

#endregion

#region Repos

builder.Services.AddScoped<IEventRepo, EventRepo>();
builder.Services.AddScoped<IAbusesReasons, ReportsRepo>();
builder.Services.AddScoped<INotificationsRepo, NotificationsRepo>();
builder.Services.AddScoped<IPostsRepo, PostsRepo>();
builder.Services.AddScoped<ISideMenuContentRepo, SideMenuContentRepo>();
builder.Services.AddScoped<IMediaRepo, MediaRepo>();
builder.Services.AddScoped<ICommentRepo, CommentRepo>();
builder.Services.AddScoped<IAttachmentRepo, AttachmentRepo>();
builder.Services.AddScoped<IAbusesRepo, AbusesRepo>();
builder.Services.AddScoped<IInitiativesRepo, InitiativesRepo>();
builder.Services.AddScoped<IProjectRepo, ProjectRepo>();
builder.Services.AddScoped<ICompetitionRepo, CompetitionRepoo>();

#endregion

#region Managers

builder.Services.AddScoped<IEventManager, EventManager>();
builder.Services.AddScoped<IReportsManager, ReportsManager>();
builder.Services.AddScoped<INotificationsManager, NotificationsManager>();
builder.Services.AddScoped<IPostsManager, PostsManager>();
builder.Services.AddScoped<ISideMenuManager, SideMenuManager>();
builder.Services.AddScoped<IAuthManager, AuthManager>();
builder.Services.AddScoped<IFileManager, FileManager>();
builder.Services.AddScoped<IMediaManager, MediaManager>();
builder.Services.AddScoped<ICommentManager, CommentManager>();
builder.Services.AddScoped<IAttachmentManager, AttachmentManager>();
builder.Services.AddScoped<IAbusesManager, AbusesManager>();
builder.Services.AddScoped<IInitiativeManager, InitiativeManager>();
builder.Services.AddScoped<IProjectManager, ProjectManager>();
builder.Services.AddScoped<ISocialLoginManager, SocialLoginManager>();
builder.Services.AddScoped<ICompetitionManager, CompetitionManager>();

#endregion

#region Vodafone Configuration

var vodaConfig = builder.Configuration.GetSection("VodafoneConfiguration").Get<VodafoneConfiguration>()!;
builder.Services.AddSingleton(vodaConfig);
builder.Services.AddTransient<ISmsManager, SmsManager>();

//builder.Services.Configure<VodafoneConfiguration>(builder.Configuration.GetSection("VodafoneConfiguration"));

#endregion

#region Email Configuration

var emailConfig = builder.Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>()!;
builder.Services.AddSingleton(emailConfig);
builder.Services.AddTransient<IEmailManager, EmailManager>();

#endregion


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
