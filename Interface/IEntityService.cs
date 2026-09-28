using AttendanceAPI.Contracts.Request;
using AttendanceAPI.Contracts.Response;
using AttendanceAPI.Entities;


namespace AttendanceAPI.Interface
{
    public interface IEntityService
    {
        Task<Entity> AddEntityAsync(EntityRequest request);
        Task<List<EntityResponse>> GetEntityAsync();
        Task<Entity> EditEntityAsync(EntityRequest request);
    }
}
