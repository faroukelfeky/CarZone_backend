using Kawerk.Application.Interfaces;
using Kawerk.Domain;
using Kawerk.Infastructure.Context;
using Kawerk.Infastructure.DTOs.Manufacturer;
using Kawerk.Infastructure.DTOs.Vehicle;
using Kawerk.Infastructure.ResponseClasses;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Kawerk.Application.Services
{
    public class ManufacturerService : IManufacturerService
    {
        private readonly DbBase _db;
        public ManufacturerService(DbBase db)
        {
            _db = db;
        }


        //        *********** Setters ***********
        public async Task<SettersResponse> CreateManufacturer(ManufacturerCreationDTO manufacturer)//0 == Faulty DTO || 1 == Name is already in use || 2 == Successful
        {
            //Checking DTO validity
            if (manufacturer == null)
                return new SettersResponse { status = 0, msg = "Faulty DTO" };

            //Checking if name is already in use or not
            if(await isNameValid(manufacturer.Name))
                return new SettersResponse { status = 0, msg = "Name is already in use" };

            //Making the new Manufacturer
            Manufacturer newManufacturer = new Manufacturer
            {
                Name = manufacturer.Name,
                Description = manufacturer.Description,
                Type = manufacturer.Type,
                CreatedAt = DateTime.UtcNow,
            };

            //Saving to Database
            await _db.Manufacturers.AddAsync(newManufacturer);
            await _db.SaveChangesAsync();
            return new SettersResponse { status = 1, msg = "Manufacturer created successfully" };
        }
        public async Task<SettersResponse> UpdateManufacturer(Guid manufacturerID, ManufacturerUpdateDTO manufacturer)//0 == Faulty DTO || 1 == Manufacturer not found || 2 == Name is already in use || 3 == Successfull
        {
            //Checking DTO validity
            if (manufacturer == null)
                return new SettersResponse { status = 0, msg = "Faulty DTO" };

            //Getting Manufacturer from Database
            var isManufacturerExists = await (from m in _db.Manufacturers
                                              where m.ManufacturerID == manufacturerID
                                              select m).FirstOrDefaultAsync();
            //If Manufacturer not found return
            if (isManufacturerExists == null)
                return new SettersResponse { status = 0, msg = "Manufacturer not found" };

            //If they want to change the name we should check if the new name is in use or not
            if (!string.IsNullOrEmpty(isManufacturerExists.Name))
            {
                //If not in use update the name and continue
                if (isManufacturerExists.Name.ToLower() == manufacturer.Name.ToLower() || !await isNameValid(manufacturer.Name))
                    isManufacturerExists.Name = manufacturer.Name;
                //if in use return
                else
                    return new SettersResponse { status = 0, msg = "Name is already in use" };
            }
            //Updating Description
            if(!string.IsNullOrEmpty(manufacturer.Description))
                isManufacturerExists.Description = manufacturer.Description;
            //Updating Type
            if(!string.IsNullOrEmpty(manufacturer.Type))
                isManufacturerExists.Type = manufacturer.Type;
            //Updating Warranty
            if(!string.IsNullOrEmpty(manufacturer.Warranty))
                isManufacturerExists.Warranty = manufacturer.Warranty;

            //Saving to Database
            _db.Manufacturers.Update(isManufacturerExists);
            await _db.SaveChangesAsync();
            return new SettersResponse { status = 1, msg = "Manufacturer updated successfully" };
        }
        public async Task<SettersResponse> DeleteManufacturer(Guid manufacturerID)//0 == Faulty ID || 1 == Manufacturer not found || 2 == Succcessfull
        {
            //Checking ID validity
            if (manufacturerID == Guid.Empty)
                return new SettersResponse { status = 0, msg = "Faulty ID" };

            //Getting Manufacturer from Database
            var isManufacturerExists = await(from m in _db.Manufacturers
                                             where m.ManufacturerID == manufacturerID
                                             select m).FirstOrDefaultAsync();
            //If manufacturer not found return
            if(isManufacturerExists == null)
                return new SettersResponse { status = 0, msg = "Manufacturer not found" };

            //Saving to Database
            _db.Manufacturers.Remove(isManufacturerExists);
            await _db.SaveChangesAsync();
            return new SettersResponse { status = 1, msg = "Manufacturer deleted successfully" };   
        }
        public async Task<SettersResponse> SellVehicle(Guid manufacturerID, Guid vehicleID)//0 == Faulty IDs || 1 == Manufacturer not found || 2 == Vehicle not found || 3 == Vehicle already sold by this manufacturer || 4 == Successfull
        {
            //Checking ID validity
            if (manufacturerID == Guid.Empty || vehicleID == Guid.Empty)
                return new SettersResponse { status = 0, msg = "Faulty IDs" };
            //Getting Manufacturer from Database
            var isManufacturerExists = await (from m in _db.Manufacturers
                                              where m.ManufacturerID == manufacturerID
                                              select m).FirstOrDefaultAsync();
            //If Manufacturer not found return
            if (isManufacturerExists == null)
                return new SettersResponse { status = 0, msg = "Manufacturer not found" };
            
            //Getting Vehicle from Database
            var isVehicleExists = await (from v in _db.Vehicles
                                         where v.VehicleID == vehicleID
                                         select v).FirstOrDefaultAsync();
            //If Vehicle not found return
            if (isVehicleExists == null)
                return new SettersResponse { status = 0, msg = "Vehicle not found" };

            //Checking if Vehicle is already sold by this manufacturer
            if (isVehicleExists.ManufacturerID == manufacturerID)
                return new SettersResponse { status = 0, msg = "Vehicle already sold by this manufacturer" };

            //Checking if Vehicle is already being sold
            if (isVehicleExists.SellerID != null || isVehicleExists.BuyerID != null)
                return new SettersResponse { status = 0, msg = "Vehicle is already being sold" };

            //Selling Vehicle
            isVehicleExists.ManufacturerID = manufacturerID;

            //--------------------------------------------

            //Observer Design Pattern Implementation

            //Notifying Subscribers
            var manufacturerSubscribers = await (from c in _db.Customers
                                              where c.SubscribedManufacturers.Any(sm => sm.ManufacturerID == manufacturerID)
                                              select c).ToListAsync();
            if (manufacturerSubscribers.Count != 0)
            {
                // Notify subscribers
                foreach (var subscriber in manufacturerSubscribers)
                {
                    // Send notification to subscriber
                    Notification newNotification = new Notification
                    {
                        NotificationId = Guid.NewGuid(),
                        Title = "New Vehicle Sold",
                        Message = $"A new vehicle '{isVehicleExists.Model}' has been sold by manufacturer '{isManufacturerExists.Name}'.",
                        CreatedAt = DateTime.UtcNow,
                        CustomerID = subscriber.CustomerID,
                        Customer = subscriber
                    };
                    await _db.Notifications.AddAsync(newNotification);
                }
            }
            //--------------------------------------------

            _db.Vehicles.Update(isVehicleExists);
            await _db.SaveChangesAsync();
            return new SettersResponse { status = 1, msg = "Vehicle sold successfully" };
        }
        //--------------------------------------------

        //        *********** Extra Validation Function ***********

        public async Task<bool> isNameValid(string name)//returns true if name is in user
        {
            var result = await _db.Manufacturers.AnyAsync(x => x.Name == name);
            return result;
        }

        //--------------------------------------------

        //        *********** Getters ***********
        public async Task<ManufacturerViewDTO?> GetManufacturer(Guid manufacturerID)
        {
            //Checcking ID validity
            if (manufacturerID == Guid.Empty)
                return null;

            //Getting manufacturer from Database
            var isManufacturerExists = await (from m in  _db.Manufacturers
                                              where m.ManufacturerID == manufacturerID
                                              select new ManufacturerViewDTO
                                              {
                                                  ManufacturerID = manufacturerID,
                                                  Name = m.Name,
                                                  Description = m.Description,
                                                  Type = m.Type,
                                              }).FirstOrDefaultAsync();

            //Returning the result
            return isManufacturerExists;
        }
        public async Task<PagedList<VehicleManufacturerViewDTO>> GetSoldVehicles(Guid manufacturerID, string startDate, string endDate, int page, string sortColumn, string OrderBy, string searchTerm, int pageSize)
        {
            //Checcking ID validity
            if (manufacturerID == Guid.Empty)
                return null;
            //Getting Vehicle from Database
            var vehiclesQuery = (from v in _db.Vehicles
                                   where v.ManufacturerID == manufacturerID
                                   select v);

            DateTime validStartDate, validEndDate;
            if (DateTime.TryParse(startDate, out validStartDate))
            {
                vehiclesQuery = vehiclesQuery.Where(u => u.Transaction!.CreatedDate > validStartDate);
            }
            if (DateTime.TryParse(endDate, out validEndDate))
            {
                vehiclesQuery = vehiclesQuery.Where(u => u.Transaction!.CreatedDate < validEndDate);
            }
            if (!string.IsNullOrEmpty(searchTerm))
                vehiclesQuery = vehiclesQuery.Where(u => u.Model!.Contains(searchTerm) ||
                u.Description!.Contains(searchTerm) || u.Type!.Contains(searchTerm) || u.FuelType!.Contains(searchTerm));
            if (!string.IsNullOrEmpty(sortColumn))
            {
                Expression<Func<Vehicle, object>> keySelector = sortColumn.ToLower() switch // throws error when sortColumn is null
                {
                    "name" or "n" => Vehicle => Vehicle.Model!,
                    "price" or "p" => Vehicle => Vehicle.Price,
                    "type" or "t" => Vehicle => Vehicle.Type!,
                    "enginecapacity" or "ec" => Vehicle => Vehicle.EngineCapacity!,
                    "fueltype" or "f" => Vehicle => Vehicle.FuelType!,
                    "seatingcapacity" or "sc" => Vehicle => Vehicle.SeatingCapacity,
                    "status" or "s" => Vehicle => Vehicle.Status!,
                    _ => Vehicle => Vehicle.VehicleID,
                };
                if (!string.IsNullOrEmpty(OrderBy)) vehiclesQuery = vehiclesQuery.OrderBy(keySelector);
                else vehiclesQuery = vehiclesQuery.OrderBy(keySelector);
            }
            var vehiclesResponse = vehiclesQuery
                                    .Select(v => new VehicleManufacturerViewDTO
                                    {
                                        ManufacturerID = v.ManufacturerID,
                                        VehicleID = v.VehicleID,
                                        Model = v.Model!,
                                        ManufacturerName = v.ManufacturerName,
                                        Description = v.Description,
                                        Price = v.Price,
                                        Type = v.Type,
                                        Transmission = v.Transmission,
                                        FuelType = v.FuelType,
                                        EngineCapacity = v.EngineCapacity,
                                        SeatingCapacity = v.SeatingCapacity,
                                        Status = v.Status,
                                        HorsePower = v.HorsePower,
                                        DaysOnMarket = v.DaysOnMarket,
                                        ConditionScore = v.ConditionScore,
                                        Year = v.Year,
                                    });


            return await PagedList<VehicleManufacturerViewDTO>.CreateAsync(vehiclesResponse, page, pageSize);
        }
        public async Task<PagedList<ManufacturerViewDTO>?> GetManufacturers(int page, int pageSize)
        {
            //Getting Manufacturer from Database
            var manufacturerQuery = (from m in _db.Manufacturers
                                     select new ManufacturerViewDTO
                                     {
                                        ManufacturerID = m.ManufacturerID,
                                        Name = m.Name,
                                        Description = m.Description,
                                        Type = m.Type,
                                     });

            //Returning the result
            return await PagedList<ManufacturerViewDTO>.CreateAsync(manufacturerQuery, page, pageSize);
        }

    }
}
