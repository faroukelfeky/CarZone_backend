using Kawerk.Application.Interfaces;
using Kawerk.Domain;
using Kawerk.Infastructure.Context;
using Kawerk.Infastructure.DTOs.Vehicle;
using Kawerk.Infastructure.ResponseClasses;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Text;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace Kawerk.Application.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly DbBase _db;
        private readonly IWebHostEnvironment _env;
        public VehicleService(DbBase db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        //
        //        *********** Setters ***********
        public async Task<SettersResponse> CreateVehicle(VehicleViewDTO vehicle)//0 == Faulty DTO || 1 == Successful
        {
            if (vehicle == null)
                return new SettersResponse { status = 0, msg = "Faulty DTO" };

            Vehicle newVehicle = new Vehicle
            {
                VehicleID = Guid.NewGuid(),
                Model = vehicle.Model!,
                ManufacturerName = vehicle.ManufacturerName,
                Description = vehicle.Description,
                Price = vehicle.Price,
                Type = vehicle.Type,
                Transmission = vehicle.Transmission,
                FuelType = vehicle.FuelType,
                EngineCapacity = vehicle.EngineCapacity,
                SeatingCapacity = vehicle.SeatingCapacity,
                Status = vehicle.Status,
                HorsePower = vehicle.HorsePower,
                DaysOnMarket = vehicle.DaysOnMarket,
                ConditionScore = vehicle.ConditionScore,
                Year = vehicle.Year,
            };

            await _db.Vehicles.AddAsync(newVehicle);
            await _db.SaveChangesAsync();
            return new SettersResponse { status = 1, msg = "Vehicle created successfully" };
        }
        public async Task<SettersResponse> UpdateVehicle(Guid vehicleID, VehicleViewDTO vehicle)//0 == Faulty DTO || 1 == Vehicle not found || 2 == Successful
        {
            //Checking DTO validity
            if (vehicle == null)
                return new SettersResponse { status = 0, msg = "Faulty DTO" };

            //Getting Vehicle from Database
            var isVehicleExists = await (from v in _db.Vehicles
                                         where v.VehicleID == vehicleID
                                         select v).FirstOrDefaultAsync();
            //If vehicle not found return
            if (isVehicleExists == null)
                return new SettersResponse { status = 0, msg = "Vehicle not found" };

            // --***Updating***--
            
            if(!string.IsNullOrEmpty(vehicle.Model))
                isVehicleExists.Model = vehicle.Model; 

            if(!string.IsNullOrEmpty(vehicle.Description))
                isVehicleExists.Description = vehicle.Description;

            if(!string.IsNullOrEmpty(vehicle.Transmission))
                isVehicleExists.Transmission = vehicle.Transmission;    

            if(!string.IsNullOrEmpty(vehicle.EngineCapacity))
                isVehicleExists.EngineCapacity = vehicle.EngineCapacity;

            if(!string.IsNullOrEmpty(vehicle.FuelType))
                isVehicleExists.FuelType = vehicle.FuelType;

            if(vehicle.Price != 0)
                isVehicleExists.Price = vehicle.Price;

            if(vehicle.SeatingCapacity != 0)
                isVehicleExists.SeatingCapacity = vehicle.SeatingCapacity;

            if(!string.IsNullOrEmpty(vehicle.Status))
                isVehicleExists.Status = vehicle.Status;

            if(!string.IsNullOrEmpty(vehicle.Type))
                isVehicleExists.Type = vehicle.Type;

            //Saving to Database
            _db.Vehicles.Update(isVehicleExists);
            await _db.SaveChangesAsync();
            return new SettersResponse { status = 1, msg = "Vehicle updated successfully" };
        }
        public async Task<SettersResponse> DeleteVehicle(Guid vehicleID)//0 == Faulty ID || 1 == Vehicle not found || 2 == Successful
        {
            //Checking ID validity
            if (vehicleID == Guid.Empty)
                return new SettersResponse { status = 0, msg = "Faulty ID" };

            //Getting vehicle from Database
            var isVehicleExists = await (from v in _db.Vehicles
                                         where v.VehicleID == vehicleID
                                         select v).FirstOrDefaultAsync();
            //If vehicle not found return
            if (isVehicleExists == null)
                return new SettersResponse { status = 0, msg = "Vehicle not found" };

            //Saving to Database
            _db.Vehicles.Remove(isVehicleExists);
            await _db.SaveChangesAsync();
            return new SettersResponse { status = 1, msg = "Vehicle deleted successfully" };
        }

        //--------------------------------------------

        //        *********** Getters ***********
        public async Task<VehicleViewDTO?> GetVehicle(Guid vehicleID)
        {
            //Checking ID validity
            if (vehicleID == Guid.Empty)
                return null;

            //Getting the vehicle from the Database
            var isVehicleExists = await(from v in _db.Vehicles
                                        where v.VehicleID == vehicleID
                                        select new VehicleViewDTO
                                        {
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
                                        }).FirstOrDefaultAsync();
            //Returning the result
            return isVehicleExists;
        }
        public async Task<PagedList<VehicleViewDTO>?> GetFilteredVehicles(string startDate, string endDate, int minimumPrice, int maximumPrice, int page, string sortColumn, string OrderBy, string searchTerm, int pageSize)
        {
            IQueryable<Vehicle> vehiclesQuery = _db.Vehicles;
            DateTime validStartDate, validEndDate;
            if (DateTime.TryParse(startDate, out validStartDate))
            {
                vehiclesQuery = vehiclesQuery.Where(u => u.Transaction.CreatedDate > validStartDate);
            }
            if (DateTime.TryParse(endDate, out validEndDate))
            {
                vehiclesQuery = vehiclesQuery.Where(u => u.Transaction.CreatedDate < validEndDate);
            }

            if (minimumPrice > 0)
                vehiclesQuery = vehiclesQuery.Where(u => u.Price >= minimumPrice);

            if (maximumPrice > 0)
                vehiclesQuery = vehiclesQuery.Where(u => u.Price <= maximumPrice);

            if (!string.IsNullOrEmpty(searchTerm))
                vehiclesQuery = vehiclesQuery.Where(u => u.Model!.Contains(searchTerm) ||
                u.Description!  .Contains(searchTerm) || u.Type!.Contains(searchTerm) || u.FuelType!.Contains(searchTerm) || u.ManufacturerName!.Contains(searchTerm));
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
                    "condition" or "c" or "score" => Vehicle => Vehicle.ConditionScore!,
                    "horsepower" or "horse" or "h" or "speed" => Vehicle => Vehicle.HorsePower!,
                    _ => Vehicle => Vehicle.VehicleID,
                };
                if (!string.IsNullOrEmpty(OrderBy)) vehiclesQuery = vehiclesQuery.OrderBy(keySelector);
                else vehiclesQuery = vehiclesQuery.OrderBy(keySelector);
            }
            var vehiclesResponse = vehiclesQuery
                                    .Select(v => new VehicleViewDTO
                                    {
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


            return await PagedList<VehicleViewDTO>.CreateAsync(vehiclesResponse, page, pageSize);
        }
        public async Task<PagedList<VehicleViewDTO>?> GetVehicles(int pageNumber, int pageSize)
        {
            //Getting vehicles from the Database
            var vehicleQuery = (from v in _db.Vehicles
                                select new VehicleViewDTO
                                {
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
            //Returning the result
            return await PagedList<VehicleViewDTO>.CreateAsync(vehicleQuery, pageNumber, pageSize);
        }

        // New: import vehicles from CSV (will read uploaded file when provided; otherwise reads the dataset file path)
        public async Task<SettersResponse> ImportVehiclesFromCsv()//Adding only first hundred thousand rows
        {
            _db.Database.SetCommandTimeout(150);
            string defaultPath = Path.Combine(_env.ContentRootPath,"Infastructure","Datasets","used_cars_data_250k.csv");

            const int batchSize = 100;

            Stream stream = null!;
            try
            {
                if (File.Exists(defaultPath))
                    stream = File.OpenRead(defaultPath);
                else
                    return new SettersResponse { status = 0, msg = $"No uploaded file and default dataset not found at '{defaultPath}'." };

                var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true,
                    BadDataFound = null,
                    MissingFieldFound = null,
                    HeaderValidated = null,
                    IgnoreBlankLines = true,
                    TrimOptions = TrimOptions.Trim
                };

                var vehiclesAdded = 0;
                var skipped = 0;
                var errors = 0;
                var batch = new List<Vehicle>(batchSize);

                using (stream)
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                using (var csv = new CsvReader(reader, csvConfig))
                {
                    // Read header and normalize
                    if (!await csv.ReadAsync() || !csv.ReadHeader())
                        return new SettersResponse { status = 0, msg = "CSV file has no header or is empty." };

                    // helper to get the first matching field among alternatives
                    static string GetFieldOrEmpty(CsvReader csvReader, params string[] alternatives)
                    {
                        foreach (var name in alternatives)
                        {
                            try
                            {
                                if (csvReader.HeaderRecord != null && csvReader.HeaderRecord.Any(h => string.Equals(h, name, StringComparison.OrdinalIgnoreCase)))
                                {
                                    var val = csvReader.GetField<string>(name);
                                    if (!string.IsNullOrWhiteSpace(val))
                                        return val.Trim();
                                    // return empty so alternatives can continue
                                }
                            }
                            catch
                            {
                                // ignore and try next
                            }
                        }
                        // fallback to try names transformed (underscore vs camel)
                        foreach (var name in alternatives)
                        {
                            var alt = name.Replace("_", "").ToLowerInvariant();
                            var match = csvReader.HeaderRecord.FirstOrDefault(h => h.Replace("_", "").ToLowerInvariant() == alt);
                            if (match != null)
                            {
                                try
                                {
                                    var val = csvReader.GetField<string>(match);
                                    if (!string.IsNullOrWhiteSpace(val))
                                        return val.Trim();
                                }
                                catch { }
                            }
                        }
                        return string.Empty;
                    }

                    while (await csv.ReadAsync())
                    {
                        try
                        {
                            // Map fields using likely header names from dataset
                            var name = GetFieldOrEmpty(csv, "name", "title","model");
                            if (string.IsNullOrEmpty(name))
                            {
                                skipped++;
                                continue;
                            }

                            var description = GetFieldOrEmpty(csv, "description", "desc");
                            var country = GetFieldOrEmpty(csv, "country");
                            var city = GetFieldOrEmpty(csv, "city");
                            var priceStr = GetFieldOrEmpty(csv, "price_usd", "price");
                            var type = GetFieldOrEmpty(csv, "type");
                            var transmission = GetFieldOrEmpty(csv, "transmission");
                            var fuelType = GetFieldOrEmpty(csv, "fuel_type", "fuel");
                            var engineCapacity = GetFieldOrEmpty(csv, "engine_capacity", "enginecapacity", "engine_size");
                            var seatingStr = GetFieldOrEmpty(csv, "seatingcapacity", "seats", "seating");
                            var status = GetFieldOrEmpty(csv, "status");
                            var year = GetFieldOrEmpty(csv, "year", "model_year");
                            var createdAtStr = GetFieldOrEmpty(csv, "created_at", "createdat", "listed_at");
                            var doorsStr = GetFieldOrEmpty(csv, "doors");
                            var model = GetFieldOrEmpty(csv, "model");
                            var brand = GetFieldOrEmpty(csv, "brand");
                            var horsePowerStr = GetFieldOrEmpty(csv, "horsepower", "hp");
                            var conditionScoreStr = GetFieldOrEmpty(csv, "condition_score", "condition");
                            var daysOnMarketStr = GetFieldOrEmpty(csv, "days_on_market", "days_on_market");
                            var color = GetFieldOrEmpty(csv, "color");

                            // parse numeric fields safely
                            int price = 0;
                            int seating = 0;
                            int doors = 0;
                            int horsePower = 0;
                            float conditionScore = 0;
                            int daysOnMarket = 0;
                            if (!string.IsNullOrEmpty(priceStr))
                                int.TryParse(priceStr.Replace(",", ""), NumberStyles.Integer, CultureInfo.InvariantCulture, out price);
                            if (!string.IsNullOrEmpty(seatingStr))
                                int.TryParse(seatingStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out seating);
                            if (!string.IsNullOrEmpty(doorsStr))
                                int.TryParse(doorsStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out doors);
                            if (!string.IsNullOrEmpty(horsePowerStr))
                                int.TryParse(horsePowerStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out horsePower);
                            if (!string.IsNullOrEmpty(conditionScoreStr))
                                float.TryParse(conditionScoreStr, NumberStyles.Float | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out conditionScore);
                            if (!string.IsNullOrEmpty(daysOnMarketStr))
                                int.TryParse(daysOnMarketStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out daysOnMarket);

                            DateTime createdAt = DateTime.UtcNow;
                            if (!string.IsNullOrEmpty(createdAtStr) &&
                                DateTime.TryParse(createdAtStr, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var dt))
                                createdAt = dt;

                            var vehicle = new Vehicle
                            {
                                VehicleID = Guid.NewGuid(),
                                Description = string.IsNullOrEmpty(description) ? null : description,
                                Price = price,
                                Type = string.IsNullOrEmpty(type) ? null : type,
                                Transmission = string.IsNullOrEmpty(transmission) ? null : transmission,
                                FuelType = string.IsNullOrEmpty(fuelType) ? null : fuelType,
                                EngineCapacity = string.IsNullOrEmpty(engineCapacity) ? null : engineCapacity,
                                CreatedAt = createdAt,
                                SeatingCapacity = seating,
                                Status = string.IsNullOrEmpty(status) ? null : status,
                                Year = string.IsNullOrEmpty(year) ? null : year,
                                Country = string.IsNullOrEmpty(country) ? null : country,
                                Doors = doors,
                                Model = string.IsNullOrEmpty(model) ? null : model,
                                City = string.IsNullOrEmpty(city) ? null : city,
                                HorsePower = horsePower == 0 ? null : horsePower,
                                ConditionScore = conditionScore == 0 ? null : conditionScore,
                                DaysOnMarket = daysOnMarket == 0 ? null : daysOnMarket,
                                Color = string.IsNullOrEmpty(color) ? null : color,
                                ManufacturerName = brand
                            };

                            batch.Add(vehicle);

                            if (batch.Count >= batchSize)
                            {
                                await _db.Vehicles.AddRangeAsync(batch);
                                await _db.SaveChangesAsync();
                                _db.ChangeTracker.Clear();
                                vehiclesAdded += batch.Count;
                                batch.Clear();
                            }
                        }
                        catch
                        {
                            errors++;
                            // continue processing remaining rows
                        }
                    }

                    // write remaining batch
                    if (batch.Count > 0)
                    {
                        await _db.Vehicles.AddRangeAsync(batch);
                        await _db.SaveChangesAsync();
                        _db.ChangeTracker.Clear();
                        vehiclesAdded += batch.Count;
                        batch.Clear();
                    }
                    _db.Database.SetCommandTimeout(30);
                    return new SettersResponse
                    {
                        status = 2,
                        msg = $"Import completed. Imported: {vehiclesAdded}, Skipped: {skipped}, Errors: {errors}"
                    };
                }
            }
            catch (Exception ex)
            {
                _db.Database.SetCommandTimeout(30);
                return new SettersResponse { status = 0, msg = $"Import failed: {ex.Message}" };
            }
        }
        public async Task<SettersResponse> DeleteAllData()
        {
            try
            {
                _db.Vehicles.RemoveRange(_db.Vehicles.ToList());
                await _db.SaveChangesAsync();
                return new SettersResponse { status = 2, msg = "Successful" };
            }
            catch 
            {
                return new SettersResponse { status = 0 ,msg = "Error happened"};
            }
            
        }
    }
}
