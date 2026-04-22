using CarZone.Infastructure.DTOs.Manufacturer;
using CarZone.Infastructure.DTOs.Vehicle;
using CarZone.Infastructure.ResponseClasses;

namespace CarZone.Application.Interfaces
{
    public interface IManufacturerService
    {
        public Task<SettersResponse> CreateManufacturer(ManufacturerCreationDTO manufacturer);
        public Task<SettersResponse> UpdateManufacturer(Guid manufacturerID,ManufacturerUpdateDTO manufacturer);
        public Task<SettersResponse> DeleteManufacturer(Guid manufacturerID);
        public Task<SettersResponse> SellVehicle(Guid manufacturerID,Guid vehicleID);
        public Task<ManufacturerViewDTO?> GetManufacturer(Guid manufacturerID);
        public Task<PagedList<VehicleManufacturerViewDTO>> GetSoldVehicles(Guid manufacturerID, string startDate, string endDate, int page, string sortColumn, string OrderBy, string searchTerm, int pageSize);
        public Task<PagedList<ManufacturerViewDTO>?> GetManufacturers(int page, int pageSize);
    }
}