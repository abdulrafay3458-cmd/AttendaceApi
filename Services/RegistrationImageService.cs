using AttendanceAPI.Contracts.Request;
using AttendanceAPI.Contracts.Response;
using AttendanceAPI.Data;
using AttendanceAPI.Entities;
using AttendanceAPI.Interface;
using AttendanceAPI.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Identity.Client.Extensions.Msal;
using static OpenCvSharp.Stitcher;

namespace AttendanceAPI.Services
{
    public class RegistrationImageService : IRegistrationImageService
    {
        private readonly ApiClass apiClass;

        public RegistrationImageService(ApiClass apiClass)
        {
            this.apiClass = apiClass;
        }

        public async Task<RegistrationImage> ApplyRequest(ApplyImageRequest request)
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    var isEmpAlreadyExists = await context.Employees.FirstOrDefaultAsync(a => a.Code == request.EmployeeCode);
                    var admin = await context.RoleTypes.FirstOrDefaultAsync(a => a.Title == "Admin");
                    var userAdmin = await context.UserRoles.FirstOrDefaultAsync(a => a.RoleId == admin.Id);
                    var user = await context.AppUsers.FirstOrDefaultAsync(a => a.Code == userAdmin.UserCode);

                    if (isEmpAlreadyExists == null) return null;

                    var newEmp = new RegistrationImage()
                    {
                        Code = Guid.NewGuid(),
                        EmployeeCode = request.EmployeeCode,
                        AdminCode = user.EmployeeCode,
                        RequestDate = DateTime.Now,
                        FaceImage = request.FaceImage,
                        HistoryStatus = false,
                        Status = false,
                        CompanyCode = request.CompanyCode
                    };

                    await context.RegistrationImages.AddAsync(newEmp);
                    await context.SaveChangesAsync();
                    return newEmp;
                }
            }
            catch (Exception ex) { return null; }
        }

        public async Task<List<ImageRegistrationResponse>> GetRequest()
        {
            using (var context = new AppDbContext())
            {
                //var regList = await context.RegistrationImages.Where(e => e.Status == false && e.RejectionDate== null).ToListAsync();
                var regList =( from e in context.Employees join i in context.RegistrationImages on e.Code equals i.EmployeeCode
                              where i.Status == false && i.RejectionDate == null
                    select new ImageRegistrationResponse
                    {
                        Code = i.Code,
                        EmployeeCode = i.EmployeeCode,
                        EmployeeName =e.Name,
                        AdminCode = i.AdminCode,
                        RequestDate = i.RequestDate,
                        ApproveDate = i.ApproveDate,
                        RejectionDate = i.RejectionDate,
                        RejectionReason = i.RejectionReason,
                        FaceImage = i.FaceImage,
                        Status = i.Status,
                        //HistoryStatus = i.HistoryStatus,
                        CompanyCode = i.CompanyCode
                    }).ToListAsync();
                return await regList;
            }
        }

        public async Task<ApprovedImageResponse> ApproveRequest(RegistrationImageRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            await using var context = new AppDbContext();

            try
            {
                var historyImages = await context.RegistrationImages
                    .Where(x => x.EmployeeCode == request.EmployeeCode && x.HistoryStatus)
                    .ToListAsync();

                foreach (var img in historyImages)
                {
                    img.HistoryStatus = false;
                  //  img.Status = true;
                }

                var registration = await context.RegistrationImages
                    .FirstOrDefaultAsync(x => x.Code == request.Code);

                if (registration == null)
                     throw new ApplicationException("Request does not exist.");

                var employee = await context.Employees.FirstOrDefaultAsync(e => e.Code == request.EmployeeCode);
                var EmpImage = await context.FaceImages.FirstOrDefaultAsync(x => x.EmpCode == request.EmployeeCode);
                string message;

                if (request.Status)
                {
                    registration.Status = true;
                    registration.ApproveDate = DateTime.Now;
                    registration.HistoryStatus = true;


                    if (employee == null)
                        throw new ApplicationException("Employee not found.");

                    message = "approved";

                    var imageBytesList = new List<byte[]>();

                    byte[] imageBytes = string.IsNullOrEmpty(registration.FaceImage)
                                     ? null
                                     : Convert.FromBase64String(registration.FaceImage);
                    imageBytesList.Add(imageBytes);

                    var response = await apiClass.GetMeanFacialAsync(imageBytesList);

                    //employee.FaceImage = registration.FaceImage;
                    //context.Employees.Update(employee);

                    //employee.FaceImage = string.IsNullOrEmpty(response.Result)
                    //                 ? null
                    //                 : response.Result;

                    

                    if (EmpImage == null)
                    {
                        await context.FaceImages.AddAsync(new FaceImage()
                        {
                            Id = Guid.NewGuid(),
                            EmpCode = request.EmployeeCode,
                            EmpFaceImage = registration.FaceImage
                        });
                    } else
                    {
                        EmpImage.EmpFaceImage = registration.FaceImage;
                        context.FaceImages.Update(EmpImage);
                    }

                    employee.FaceImage = response.ToString();

                    context.Employees.Update(employee);
                    //var rec = new EmployeeImages()
                    //{
                    //    EmpId = employee.Code,
                    //    EmpImage = string.IsNullOrEmpty(response.Result)
                    //                 ? null
                    //                 : Convert.FromBase64String(response.Result)
                    //};

                    //await jsonStorageService.SaveImageBytesAsync(rec);

                    //var saved = await _storage.RegisterFace(imageBytes,employee.Name,employee.Code);
                }
                else
                {
                    registration.Status = true;
                    registration.RejectionDate = DateTime.Now;
                    registration.RejectionReason = request.RejectionReason;

                    message = "rejected";
                }
                context.RegistrationImages.Update(registration);
                await context.SaveChangesAsync();
                return new ApprovedImageResponse { EmpCode = employee.Code,Status = registration.Status,Image = EmpImage != null ? EmpImage.EmpFaceImage : "" };
            }
            catch (Exception ex)
            {
                // TODO: log exception (ILogger / Serilog)
                throw new ApplicationException("Failed to process registration request.", ex);
            }
        }


    }
}
