using AttendanceAPI.Contracts.Request;
using AttendanceAPI.Contracts.Response;
using AttendanceAPI.Data;
using AttendanceAPI.Entities;
using AttendanceAPI.Interface;
using AttendanceAPI.Utilities;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Spreadsheet;
using FirebaseAdmin.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AttendanceAPI.Services
{
    public class IdentityService : IIdentityService
    {
        // ✅ Removed IMapper - not needed
        private readonly IConfiguration _configuration;
        private readonly NotificationService _notificationService;
        private readonly EmailService _emailService;

        public IdentityService(IConfiguration configuration, NotificationService notificationService, EmailService emailService)
        {
            _configuration = configuration;
            _notificationService = notificationService;
            _emailService = emailService;
        }
        private string GenerateRandomOTP(int length)
        {
            const string validChars = "123789654147852369";
            var random = new Random();
            var passwordChars = new char[length];

            for (int i = 0; i < length; i++)
            {
                passwordChars[i] = validChars[random.Next(0, validChars.Length)];
            }

            return new string(passwordChars);
        }

        private string GenerateRandomPassword(int length)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()-_=+";
            var random = new Random();
            var passwordChars = new char[length];

            for (int i = 0; i < length; i++)
            {
                passwordChars[i] = validChars[random.Next(0, validChars.Length)];
            }

            return new string(passwordChars);
        }

        public async Task<List<Contracts.Request.UserRole>> GetUserRole()
        {
            using (var context = new AppDbContext())
            {
                return await context.RoleTypes.Select(c => new Contracts.Request.UserRole
                {
                    Id = c.Id,
                    Name = c.Title
                }).ToListAsync();
            }
        }

        public async Task<List<StandardHourGroup>> GetStandardHour()
        {
            using (var context = new AppDbContext())
            {
                return await context.StandardHourGroups.ToListAsync();
            }
        }
        public async Task<bool> VerifyOTP(ForgetPasswordRequest user) 
        {
            using (var context = new AppDbContext())
            {
                try
                {

                    var userEmp = context.AppUsers.Where(a => a.Code == user.UserId).FirstOrDefault();
                    var empEmail = context.Employees.FirstOrDefault(a => a.Code == userEmp.EmployeeCode);
                    if (userEmp != null && empEmail != null)
                    {
                        var pass = GenerateRandomOTP(6);
                        var userInfo = $@"
                       Dear {empEmail.Name},<br/><br/>

                       your OTP is {pass} <br/><br/>

                        Best Regards,<br/>
                        IT Support Team
                        ";

                        var verify = new UserVerifyOPT
                        {
                            OTP = pass,
                            Email = empEmail.Email,
                            UserID = userEmp.Code
                        };
                        var isExsitVerifyCode = context.UserVerifyOPTs.FirstOrDefault(a => a.UserID == userEmp.Code);
                        if (isExsitVerifyCode != null)
                        {
                            isExsitVerifyCode.OTP = pass;
                            context.UserVerifyOPTs.Update(isExsitVerifyCode);
                        }
                        else
                        {
                            context.UserVerifyOPTs.Add(verify);
                        }
                        context.SaveChanges();

                        await _emailService.SendEmail(
                                       empEmail.Email,
                                        "Verify OTP",
                                       userInfo);
                        return true;
                    }
                    return false;
                }
                catch (Exception e)
                {
                    Console.WriteLine("An error occurred: " + e.Message);
                    throw;
                }
            }
        }
        public async Task<bool> ResetPassword(ForgetPasswordRequest user)
        {

            using (var context = new AppDbContext())
            {
                var userEmp = context.AppUsers.Where(a => a.Code == user.UserId).FirstOrDefault();
                var empEmail = context.Employees.FirstOrDefault(a => a.Code == userEmp.EmployeeCode);
                var verifyOtp = context.UserVerifyOPTs.FirstOrDefault(a => a.UserID == user.UserId && a.OTP == user.OTP);
                if (user.UserId == userEmp.Code && user.Email == empEmail.Email && verifyOtp != null)
                {


                    var pass = GenerateRandomPassword(5);
                    userEmp.Salt = Hashing.Salt();
                    userEmp.Password = Hashing.HashPassword(pass, userEmp.Salt);

                    var userInfo = $@"
                        Dear {empEmail.Name},<br/><br/>

                        Your password has been successfully reset. Please use the following credentials to log in to your account:<br/><br/>

                        <b>User ID:</b> {userEmp.Code}<br/>
                        <b>Temporary Password:</b> {pass}<br/><br/>

                        Best Regards,<br/>
                        IT Support Team
                        ";
                    try
                    {
                        context.AppUsers.Update(userEmp);
                        await context.SaveChangesAsync();
                        await _emailService.SendEmail(
                                   empEmail.Email,
                                   "Reset Password",
                                   userInfo);
                        return true;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("An error occurred: " + e.Message);
                        throw;

                    }                 
                }                
            }
            return false;
        }    
        public async Task<string> CreateUserAsync(CreateUserRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            await using var context = new AppDbContext();
            await using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                string lastCode = await context.Employees
                    .Select(e => e.Code)
                    .OrderByDescending(c => c)
                    .FirstOrDefaultAsync();

                long nextNumber = 0;
                if (!string.IsNullOrEmpty(lastCode) && long.TryParse(lastCode, out var num))
                    nextNumber = num + 1;

                string newEmployeeCode = nextNumber.ToString("D5");

                var employee = new Employee
                {
                    Code = newEmployeeCode,
                    ZkUserId = request.ZkUserId,
                    Name = request.FirstName + " " + request.LastName,
                    LeadCode = request.ManagerId,
                    CompanyCode = request.CompanyCode,
                    Email = request.Email,
                    Phone = request.Phone,
                    DepartmentCode = request.Department.ToString(),
                    DesignationCode = request.DesignationCode,
                    StandardHourCode = request.StandardHour,
                    CardNumber = request.CardNumber,
                    IsActive = request.IsActive,
                    IsAppUser = request.IsAppUser,
                    JoinDate = request.JoinDate,
                    CreatedAt = DateTime.UtcNow
                };

                await context.Employees.AddAsync(employee);

                string credentialsMessage = string.Empty;

                if (request.IsAppUser)
                {
                    if (string.IsNullOrWhiteSpace(request.Email))
                        throw new InvalidOperationException("Email is required for app users.");

                    string password = GenerateRandomPassword(4);
                    string salt = Hashing.Salt();
                    string hashedPassword = Hashing.HashPassword(password, salt);

                    var appUser = new AppUser
                    {
                        Code = request.Email.Split('@')[0],
                        Name = request.FirstName + " " + request.LastName,
                        EmployeeCode = newEmployeeCode,
                        IsActive = request.IsActive,
                        Password = hashedPassword,
                        Salt = salt
                    };

                    await context.AppUsers.AddAsync(appUser);

                    if (!string.IsNullOrEmpty(request.RoleId))
                    {
                        var userRole = new Entities.UserRole
                        {
                            UserCode = request.Email.Split('@')[0],
                            RoleId = new Guid(request.RoleId),
                        };
                        await context.UserRoles.AddAsync(userRole);
                    }

                    credentialsMessage = $"Username: {appUser.Code}\nPassword: {password}";
                }

                await context.SaveChangesAsync();
                await transaction.CommitAsync();

                return string.IsNullOrEmpty(credentialsMessage)
                    ? "Employee has been created successfully"
                    : $"Employee & App User created successfully.\n{credentialsMessage}";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new ApplicationException("Failed to create employee.", ex);
            }
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    var userInfo = await context.AppUsers
                        .Where(c => c.Code == request.EmployeeCode && c.IsActive)
                        .FirstOrDefaultAsync();

                    if (userInfo == null)
                        return new LoginResponse { Success = false, Message = "User does not exist" };

                    var registeredDevice = await context.UserDevices.FirstOrDefaultAsync(c => c.EmployeeCode == userInfo.EmployeeCode);
                    var validateUser = Hashing.VerifyPassword(request.Password, userInfo.Password, userInfo.Salt);

                    if (!validateUser)
                        return new LoginResponse { Success = false, Message = "Invalid credentials" };

                    bool anyRegisteredDevice = true;

                    if (registeredDevice == null)
                    {
                        bool deviceUsedByAnotherAccount = await context.UserDevices.AnyAsync(c => c.DeviceHash == request.DeviceHash);
                        if (deviceUsedByAnotherAccount)
                        {
                            return new LoginResponse
                            {
                                Success = true,
                                Message = "There is already an account registered on this device.",
                                isAuthDevice = false
                            };
                        }
                        anyRegisteredDevice = false;
                    }else if (registeredDevice.isApproved == false)
                    {
                        return new LoginResponse
                        {
                            Success = true,
                            Message = "Device Registration request is rejected. Contact Admin for the assistence.",
                            isAuthDevice = false
                        };
                    }else if (registeredDevice.DeviceHash != request.DeviceHash)
                    {
                        return new LoginResponse
                        {
                            Success = true,
                            Message = "This account is already registered on another device. Contact Admin to register this new device",
                            isAuthDevice = false
                        };
                    }

                    var accessToken = GenerateJwtToken(userInfo.EmployeeCode, request.DeviceHash);
                    var refreshToken = GenerateRefreshToken();
                    var hashedRefreshToken = HashRefreshToken(refreshToken);

                    var existingToken = await context.RefreshTokens
                        .FirstOrDefaultAsync(r => r.UserCode == userInfo.Code);

                    if (existingToken != null)
                    {
                        existingToken.Token = hashedRefreshToken;
                        existingToken.ExpiresAt = DateTime.UtcNow.AddDays(7);
                        existingToken.CreatedAt = DateTime.UtcNow;

                        context.RefreshTokens.Update(existingToken);
                    }
                    else
                    {
                        await context.RefreshTokens.AddAsync(new RefreshToken
                        {
                            UserCode = userInfo.Code,
                            Token = hashedRefreshToken,
                            ExpiresAt = DateTime.UtcNow.AddDays(7),
                            CreatedAt = DateTime.UtcNow
                        });
                    }

                    await context.SaveChangesAsync();

                    var userRole = await (from a in context.UserRoles
                                          join b in context.RoleTypes on a.RoleId equals b.Id
                                          where a.UserCode == userInfo.Code
                                          select b.Title).ToListAsync();

                    var isImagePending = await context.RegistrationImages.AnyAsync(r =>
                        r.EmployeeCode == userInfo.EmployeeCode &&
                        r.Status == false &&
                        r.HistoryStatus == false);

                    var empInfo = await (from a in context.Employees
                                         join b in context.Departments on a.DepartmentCode equals b.Code
                                         join c in context.Designations on a.DesignationCode equals c.Code
                                         join d in context.FaceImages on a.Code equals d.EmpCode into ad
                                         from d in ad.DefaultIfEmpty()
                                         where a.Code == userInfo.EmployeeCode
                                         select new EmployeeDTO
                                         {
                                             employeeId = a.Code,
                                             employeeCode = userInfo.Code,
                                             name = a.Name,
                                             faceImageBase64 = d.EmpFaceImage ?? "",
                                             email = a.Email,
                                             department = b.Name,
                                             designation = c.Name,
                                             role = userRole,
                                             CompanyCode = a.CompanyCode,
                                             imageApproved = !isImagePending,
                                             isDeviceRegistered = registeredDevice != null && registeredDevice.isApproved == true,
                                             AnyDeviceRegistered = anyRegisteredDevice
                                         }).FirstOrDefaultAsync();


                    return new LoginResponse
                    {
                        Success = true,
                        Employee = empInfo,
                        Message = "Login successful",
                        Token = accessToken,
                        RefreshToken = refreshToken,
                        isAuthDevice = true
                    };
                }
            }
            catch (Exception ex)
            {
                return new LoginResponse { Success = false, Message = "Invalid username and password" };
            }
        }

        public async Task<bool> RegisterFcmTokenAsync(string empId, string token)
        {
            using (var context = new AppDbContext())
            {
                var empInfo = await context.Employees.FirstOrDefaultAsync(c => c.Code == empId);
                if (empInfo == null) return false;

                empInfo.FcmToken = token;
                context.Employees.Update(empInfo);
                await context.SaveChangesAsync();
                return true;
            }
        }

        public async Task<LoginResponse> RefreshAsync(string refreshToken, string employeeCode, string deviceHash)
        {
            using (var context = new AppDbContext())
            {
                var hashedToken = HashRefreshToken(refreshToken);
                var storedToken = await context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == hashedToken);

                if (storedToken == null || storedToken.ExpiresAt < DateTime.UtcNow)
                    return new LoginResponse { Success = false, Message = "Invalid or expired refresh token" };

                var userDevice = await context.UserDevices.Where(c => c.EmployeeCode == storedToken.UserCode)
                    .Select(c => c.DeviceHash).FirstOrDefaultAsync();

                var newAccessToken = GenerateJwtToken(employeeCode, deviceHash);
                var newRefreshToken = GenerateRefreshToken();
                var newHashedRefreshToken = HashRefreshToken(newRefreshToken);

                storedToken.Token = newHashedRefreshToken;
                storedToken.ExpiresAt = DateTime.UtcNow.AddDays(7);
                storedToken.CreatedAt = DateTime.UtcNow;

                await context.SaveChangesAsync();

                var userInfo = await context.AppUsers.FirstOrDefaultAsync(c => c.EmployeeCode == employeeCode);

                var userRole = await (from a in context.UserRoles
                                      join b in context.RoleTypes on a.RoleId equals b.Id
                                      where a.UserCode == userInfo.Code
                                      select b.Title).ToListAsync();


                var empInfo = await (from a in context.Employees
                                     join b in context.Departments on a.DepartmentCode equals b.Code
                                     join c in context.Designations on a.DesignationCode equals c.Code
                                     join d in context.FaceImages on a.Code equals d.EmpCode into ad
                                     from d in ad.DefaultIfEmpty()
                                     where a.Code == userInfo.EmployeeCode
                                     select new EmployeeDTO
                                     {
                                         employeeId = a.Code,
                                         employeeCode = userInfo.Code,
                                         name = a.Name,
                                         faceImageBase64 = d.EmpFaceImage ?? "",
                                         email = a.Email,
                                         department = b.Name,
                                         designation = c.Name,
                                         role = userRole,
                                         CompanyCode = a.CompanyCode,
                                         //imageApproved = !isImagePending,
                                         //isDeviceRegistered = registeredDevice != null && registeredDevice.isApproved == true,
                                         //AnyDeviceRegistered = anyRegisteredDevice
                                     }).FirstOrDefaultAsync();

                return new LoginResponse
                {
                    Success = true,
                    Message = "Token refreshed",
                    Employee = empInfo,
                    Token = newAccessToken,
                    RefreshToken = newRefreshToken,
                    isAuthDevice = true
                };
            }
        }

        private string GenerateJwtToken(string employeeId, string? deviceHash)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new Claim("employeeId", employeeId),
                new Claim(ClaimTypes.NameIdentifier, employeeId)
            };

            if (deviceHash != null) claims.Add(new Claim("deviceHash", deviceHash));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddSeconds(60),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<ChangePasswordResponse> ChangePasswordAsync (ChangePasswordRequest request)
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    var userInfo = await context.AppUsers
                        .Where(c => c.EmployeeCode == request.RequesterCode && c.IsActive)
                        .FirstOrDefaultAsync();

                    if (userInfo == null)
                        return new ChangePasswordResponse()
                        {
                            Success = false,
                            Message = "Can't find user."
                        };

                    var validateUser = Hashing.VerifyPassword(request.OldPassword, userInfo.Password, userInfo.Salt);

                    if (!validateUser)
                        return new ChangePasswordResponse()
                        {
                            Success = false,
                            Message = "Invalid old password."
                        };

                    var newSalt = Hashing.Salt();
                    var newHashPassword = Hashing.HashPassword(request.NewPassword, newSalt);

                    userInfo.Salt = newSalt;
                    userInfo.Password = newHashPassword;

                    context.AppUsers.Update(userInfo);
                    await context.SaveChangesAsync();
                    return new ChangePasswordResponse() { Success = true};
                }
            } catch (Exception e)
            {
                throw;
            }
        }

        private string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }

        private string HashRefreshToken(string refreshToken)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(refreshToken);
            return Convert.ToBase64String(sha256.ComputeHash(bytes));
        }
    }
}