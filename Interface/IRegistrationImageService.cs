using AttendanceAPI.Contracts.Request;
using AttendanceAPI.Contracts.Response;
using AttendanceAPI.Entities;
namespace AttendanceAPI.Interface
{
    public interface IRegistrationImageService
    {
        Task<RegistrationImage> ApplyRequest(ApplyImageRequest request);
        Task<ApprovedImageResponse> ApproveRequest(RegistrationImageRequest request);
        Task<List<ImageRegistrationResponse>> GetRequest();
    }
}
