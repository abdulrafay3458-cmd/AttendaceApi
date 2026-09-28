using AttendanceAPI.Contracts.Request;
using AttendanceAPI.Data;
using AttendanceAPI.Entities;
using AttendanceAPI.Interface;
using AttendanceAPI.Middleware;
using AttendanceAPI.Services;
using AttendanceAPI.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateBootstrapLogger();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5238);
});
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer {token}'"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] {}
        }
    });
});


builder.Services.Configure<SmtpSettings>(
    builder.Configuration.GetSection("SmtpSettings"));
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
    .RequireAuthenticatedUser()
    .Build();
});

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateActor = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])
            )

        };
    });


// Add SignalR
builder.Services.AddSignalR();

builder.Services.AddDbContext<AppDbContext>(opt => 
    opt.UseSqlServer(builder.Configuration.GetConnectionString("conn")));

// Register services
builder.Services.AddHttpClient<AddressGeoService>();
builder.Services.AddScoped<ZKTecoService>();
builder.Services.AddScoped<AttendanceService>();
builder.Services.AddScoped<ITaskOvertimeService,TaskOvertimeService>();
builder.Services.AddScoped<DeviceService>();
builder.Services.AddScoped<ApiClass>();
builder.Services.AddScoped<NotificationService>();  // NEW
builder.Services.AddHostedService<ZKTecoMonitorService>();
builder.Services.AddScoped<IReportService , ReportService>();
builder.Services.AddScoped<IIdentityService, IdentityService>();
builder.Services.AddScoped<IEntityService, EntityService>();
builder.Services.AddScoped<ILeaveService, LeaveService>();
builder.Services.AddScoped<IHolidayService, HolidayService>();
builder.Services.AddScoped<ITaskService,TaskService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IDesignationService, DesignationService>();
builder.Services.AddScoped<ICompanyService,CompanyService>();
builder.Services.AddScoped<IDepartmentService,DepartmentService>();
builder.Services.AddScoped<IRegistrationImageService,RegistrationImageService>();
builder.Services.AddScoped<EmailService>();


//builder.Services.AddSingleton<FaceRecognitionService>(sp =>
//{
//    var config = builder.Configuration.GetSection("FaceRecognition");
//    string modelsPath = config["ModelsPath"];

//    return new FaceRecognitionService(
//        cascadePath: Path.Combine(modelsPath, "haarcascade_frontalface_default.xml"),
//        shapePredictorPath: Path.Combine(modelsPath, "shape_predictor_68_face_landmarks.dat"),
//        faceRecognizerPath: Path.Combine(modelsPath, "dlib_face_recognition_resnet_model_v1.dat"),
//        jsonStoragePath: config["JsonStoragePath"],
//        photosPath: config["PhotosPath"]
//    );
//});

//return new FaceRecognitionService(
//    cascadePath: Path.Combine(modelsPath, "haarcascade_frontalface_default.xml"),
//    shapePredictorPath: Path.Combine(modelsPath, "shape_predictor_68_face_landmarks.dat"),
//    faceRecognizerPath: Path.Combine(modelsPath, "dlib_face_recognition_resnet_model_v1.dat"),
//    jsonStoragePath: config["JsonStoragePath"],
//    photosPath: config["PhotosPath"]
//);
//});

//builder.Services.AddSingleton<EmployeeService>();

//builder.Services.AddSingleton<TaskService>();
builder.Services.AddSingleton<IUserIdProvider, EmployeeUserIdProvider>();

FirebaseInitializer.Initialize(builder.Environment);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<Middleware>();
app.MapControllers();

app.MapHub<AttendanceHub>("/attendanceHub");

//var zkService = app.Services.GetRequiredService<ZKTecoService>();
//_ = Task.Run(() => zkService.ConnectToDevices());

using (var scope = app.Services.CreateScope())
{
    var zkService = scope.ServiceProvider.GetRequiredService<ZKTecoService>();
    _ = Task.Run(() => zkService.ConnectToDevices());
}

app.Run();