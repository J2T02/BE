using SWP.Dtos.Services;

namespace SWP.Mapper
{
    public static class ServiceMapper
    {
        public static ServiceDto ToServiceDto (this SWP.Models.Service service)
        {
            return new ServiceDto
            {
                SerId = service.SerId,
                SerName = service.SerName,
                Description = service.Description,
                Price = service.Price,
                FilePath = service.FilePath
            };
        }
    }
}
