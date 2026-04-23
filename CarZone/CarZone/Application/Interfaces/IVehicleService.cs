using Kawerk.Infastructure.DTOs.Vehicle;
using Kawerk.Infastructure.ResponseClasses;
using Microsoft.AspNetCore.Http;

namespace Kawerk.Application.Interfaces
{
    public interface IVehicleService
    {
        public Task<SettersResponse> CreateVehicle(VehicleViewDTO vehicle);
        public Task<SettersResponse> UpdateVehicle(Guid vehicleID, VehicleViewDTO vehicle);
        public Task<SettersResponse> DeleteVehicle(Guid vehicleID);
        public Task<VehicleViewDTO?> GetVehicle(Guid vehicleID);
        public Task<PagedList<VehicleViewDTO>?> GetFilteredVehicles(string startDate, string endDate, int minimumPrice, int maximumPrice, int page, string sortColumn, string OrderBy, string searchTerm, int pageSize);
        public Task<PagedList<VehicleViewDTO>?> GetVehicles(int pageNumber, int pageSize);
        public Task<SettersResponse> ImportVehiclesFromCsv();
        public Task<SettersResponse> DeleteAllData();
    }
}
