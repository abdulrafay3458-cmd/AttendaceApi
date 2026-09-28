using AttendanceAPI.Contracts.Request;
using AttendanceAPI.Contracts.Response;
using AttendanceAPI.Data;
using AttendanceAPI.Entities;
using AttendanceAPI.Interface;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Net.Sockets;
using System.Security.Principal;

namespace AttendanceAPI.Services
{
    public class EntityService : IEntityService
    {
        private readonly ILogger<TaskService> _logger;
       
        private readonly AddressGeoService addressGeo;

        public EntityService( ILogger<TaskService> logger, AddressGeoService addressGeo)
        {
            _logger = logger;
            this.addressGeo = addressGeo;
        }

        public async Task<Entity> AddEntityAsync(EntityRequest request)
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    var address = BuildAddress(
                        request.entitySuitNo,
                        request.entitySteetNO,
                        request.entityTown,
                        request.entityPostalCode,
                        request.entityProvince,
                        request.entityCountry
                    );

                    var nominatimAddress = $"{request.entitySteetNO} {request.entityTown}, {request.entityPostalCode}, {request.entityProvince}, {request.entityCountry}";

                    var (lat, lng) = await addressGeo.GetLatLngFromAddress(nominatimAddress);

                    //var address = request.entitySuitNo + " " + request.entitySteetNO + " " + request.entityPostalCode + " " + request.entityTown + " " + request.entityProvince + " " + request.entityCountry;
                    var entity = new Entity();
                    string logmsg;
                    var isEntityExist = await context.Entities.AnyAsync(b => b.Code == request.entityCode);
                    if (!isEntityExist)
                    {
                        var addEntity = new Entity
                        {
                            Code = request.entityCode,
                            Name = request.entityName,
                            LegalName = request.entityLegalName,
                            //Address = "601 Beaumont Plaza 7876 Karachi Sindh Pakistan",
                            Address = address,
                            AllowedRadiusMeters = 150,
                            Email = request.entityEmail,
                            Phone = request.entityPhone,
                            IndustryCode = request.entityIndustryCode,
                            Fax = request.entityFax,
                            ShortName = request.entityShortName,
                            CountryCode = request.entityCountryCode,
                            GlCode = request.entityGlCode,
                            FiscalYear = request.entityFiscalYear,
                            ParentCode = request.entityParentCode,
                            ChildCode = request.entityChildCode,
                            Status = request.entityStatus,
                            ClientVendor = request.entityVendor,
                            latitude = (double) lat,
                            longitude = (double) lng,
                        };

                        entity = addEntity.Adapt<Entity>();

                        await context.Entities.AddAsync(addEntity);
                        logmsg = "Add New Entity";
                    }
                    else
                    {
                        var updEntity = new Entity
                        {
                            Code = request.entityCode,
                            Name = request.entityName,
                            LegalName = request.entityLegalName,
                            //Address = "601 Beaumont Plaza 7876 Karachi Sindh Pakistan",
                            Address = address,
                            Email = request.entityEmail,
                            AllowedRadiusMeters = 150,
                            Phone = request.entityPhone,
                            IndustryCode = request.entityIndustryCode,
                            Fax = request.entityFax,
                            ShortName = request.entityShortName,
                            CountryCode = request.entityCountryCode,
                            GlCode = request.entityGlCode,
                            FiscalYear = request.entityFiscalYear,
                            ParentCode = request.entityParentCode,
                            ChildCode = request.entityChildCode,
                            Status = request.entityStatus,
                            ClientVendor = request.entityVendor,
                            latitude = (double)lat,
                            longitude = (double)lng,
                        };
                        entity = updEntity.Adapt<Entity>();

                        context.Entities.Update(updEntity);

                        logmsg = "Update Entity";
                    }
                        await context.SaveChangesAsync();

                    //_logger.LogInformation($"✅ New Entity Added: {addEntity.entityName}");
                    Log.Information($"{logmsg} {entity.Name}");

                    return entity;
                }
            }
            catch (DbUpdateException ex)
            {

                throw ex;
            }
           
           
        }

        private static string BuildAddress(params string?[] parts)
        {
            return string.Join(", ",
                parts
                    .Where(p => !string.IsNullOrWhiteSpace(p))
                    .Select(p => p!.Trim())
            );
        }

        public async Task<List<EntityResponse>> GetEntityAsync()
        {
            using (var context = new AppDbContext())
            {
                var allEntity = await context.Entities.Select(a => new EntityResponse
                {

                    Code = a.Code,
                    Name = a.Name,
                    Email = a.Email ?? "",
                    Phone = a.Phone ?? "",
                    IndustryCode = a.IndustryCode,
                    Fax = a.Fax,
                    ShortName = a.ShortName,
                    CountryCode = a.CountryCode,
                    GlCode = a.GlCode,
                    FiscalYear = a.FiscalYear,
                    ParentCode = a.ParentCode,
                    LegalName = a.LegalName,
                    ChildCode = a.ChildCode,
                    Status = a.Status,
                    Vendor = a.ClientVendor,
                    SuitNo = GetSuitNo(a.Address ?? ""),
                    SteetNO = GetStreetNo(a.Address ?? ""),
                    Town = GetTown(a.Address ?? ""),
                    PostalCode = GetPostal(a.Address ?? ""),
                    Province = GetProvince(a.Address ?? ""),
                    Country = GetCountry(a.Address ?? ""),
                }).ToListAsync();

                return allEntity;
            }
        }

        private static string GetSuitNo(string address)
        {
            if (!string.IsNullOrEmpty(address))
            {
                var parts = (address ?? string.Empty)
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(p => p.Trim())
                    .ToList();
                var suitNo = parts.ElementAtOrDefault(0);
                return suitNo ?? "";
            } else
            {
                return string.Empty;
            }
        }

        private static string GetStreetNo(string address)
        {
            var parts = (address ?? string.Empty)
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim())
                .ToList();
            var street = parts.ElementAtOrDefault(1);
            return street ?? "";
        }

        private static string GetTown(string address)
        {
            var parts = (address ?? string.Empty)
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim())
                .ToList();
            var town = parts.ElementAtOrDefault(2);
            return town ?? "";
        }

        private static string GetPostal(string address)
        {
            var parts = (address ?? string.Empty)
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim())
                .ToList();
            var postal = parts.ElementAtOrDefault(3);
            return postal ?? "";
        }

        private static string GetProvince(string address)
        {
            var parts = (address ?? string.Empty)
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim())
                .ToList();
            var province = parts.ElementAtOrDefault(4);
            return province ?? "";
        }

        private static string GetCountry(string address)
        {
            var parts = (address ?? string.Empty)
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim())
                .ToList();
            var country = parts.ElementAtOrDefault(5);
            return country ?? "";
        }

        public async Task<Entity> EditEntityAsync(EntityRequest request)
        {
            using (var context = new AppDbContext())
            {
                var entity = await context.Entities.FirstOrDefaultAsync(e => e.Code == request.entityCode);

                if(entity == null)
                {
                    //_logger.LogWarning("Entity with code {EntityCode} not found", request.entityCode);
                    Log.Warning("Entity with code {EntityCode} not found", request.entityCode);
                    throw new KeyNotFoundException("Entity not found");
                }

                var address = request.entitySuitNo + " " + request.entitySteetNO + " " + request.entityPostalCode + " " + request.entityTown + " " + request.entityProvince + " " + request.entityCountry;

                entity.Name = request.entityName;
                entity.LegalName = request.entityLegalName;
                entity.Address = address;
                entity.Email = request.entityEmail;
                entity.Phone = request.entityPhone;
                entity.IndustryCode = request.entityIndustryCode;
                entity.Fax = request.entityFax;
                entity.ShortName = request.entityShortName;
                entity.CountryCode = request.entityCountryCode;
                entity.GlCode = request.entityGlCode;
                entity.FiscalYear = request.entityFiscalYear;
                entity.ParentCode = request.entityParentCode;
                entity.ChildCode = request.entityChildCode;
                entity.Status = request.entityStatus;

                await context.SaveChangesAsync();

                //_logger.LogInformation($"Entity: {entity.entityCode} updated successfully!");
                Log.Information($"Entity: {entity.Code} updated successfully!");

                return entity;
            }
            
        }
    }
}