using Kawerk.Application.Interfaces;
using Kawerk.Domain;
using Kawerk.Infastructure.Context;
using Kawerk.Infastructure.DTOs.Customer;
using Kawerk.Infastructure.DTOs.Manufacturer;
using Kawerk.Infastructure.DTOs.Notification;
using Kawerk.Infastructure.DTOs.Vehicle;
using Kawerk.Infastructure.Enums;
using Kawerk.Infastructure.ResponseClasses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace Kawerk.Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly DbBase _db;
        private readonly ITokenHandler _tokenHandler;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuthorizationService _authorizationService;

        public CustomerService(DbBase db, ITokenHandler tokenHandler, ICurrentUserService currentUserService, IAuthorizationService authorizationService)
        {
            _db = db;
            _tokenHandler = tokenHandler;
            _currentUserService = currentUserService;
            _authorizationService = authorizationService;
        }


        //        *********** Setters ***********
        public async Task<ResponseToken> CreateCustomer(CustomerCreationDTO customer)
        {
            //Checking customerDTO validity
            if(customer == null)
                return new ResponseToken { status = 0, msg = "Faulty DTO" };

            if (!IsEmailValid(customer.Email))
                return new ResponseToken { status = 0, msg = "Invalid Email" };

            if(!await IsPasswordValid(customer.Password))
                return new ResponseToken { status = 0, msg = "Invalid Password" };

            //Checking if User already exists
            var isCustomerExists = await _db.Customers.AnyAsync(c=>c.Username.ToLower() == customer.Username.ToLower() ||
                                                     c.Email.ToLower() == customer.Email.ToLower());
            //If User exists return
            if (isCustomerExists)
                return new ResponseToken { status = 0, msg = "Customer already exists" };

            //Creating new Customer
            Customer newCustomer = new Customer
            {
                CustomerID = Guid.NewGuid(),
                Name = customer.Name,
                Username = customer.Username,
                Email = customer.Email,
                Password = new PasswordHasher<Customer>().HashPassword(null, customer.Password),
                CreatedAt = DateTime.Now,
                Role = RoleEnum.Customer
            };

            var AccessToken = await _tokenHandler.CreateAccessToken(newCustomer.CustomerID, newCustomer.Name, newCustomer.Email, newCustomer.Role);
            var RefreshToken = await _tokenHandler.CreateRefreshToken(newCustomer.CustomerID);

            //Saving to Database
            await _db.Customers.AddAsync(newCustomer);
            await _db.SaveChangesAsync();
            return new ResponseToken { 
                status = 1,
                AccessToken = AccessToken,
                RefreshToken = RefreshToken,
                msg = "Customer created successfully" };
        }
        public async Task<ResponseToken> CreateAdmin(CustomerCreationDTO customer)
        {
            //Checking customerDTO validity
            if (customer == null)
                return new ResponseToken { status = 0, msg = "Faulty DTO" };

            if (!IsEmailValid(customer.Email))
                return new ResponseToken { status = 0, msg = "Invalid Email" };

            if (!await IsPasswordValid(customer.Password))
                return new ResponseToken { status = 0, msg = "Invalid Password" };

            //Checking if User already exists
            var isCustomerExists = await _db.Customers.AnyAsync(c => c.Username.ToLower() == customer.Username.ToLower() ||
                                                     c.Email.ToLower() == customer.Email.ToLower());
            //If User exists return
            if (isCustomerExists)
                return new ResponseToken { status = 0, msg = "Customer already exists" };

            //Creating new Customer
            Customer newCustomer = new Customer
            {
                CustomerID = Guid.NewGuid(),
                Name = customer.Name,
                Username = customer.Username,
                Email = customer.Email,
                Password = new PasswordHasher<Customer>().HashPassword(null, customer.Password),
                CreatedAt = DateTime.Now,
                Role = RoleEnum.Admin
            };

            var AccessToken = await _tokenHandler.CreateAccessToken(newCustomer.CustomerID, newCustomer.Name, newCustomer.Email, RoleEnum.Admin);
            var RefreshToken = await _tokenHandler.CreateRefreshToken(newCustomer.CustomerID);

            //Saving to Database
            await _db.Customers.AddAsync(newCustomer);
            await _db.SaveChangesAsync();
            return new ResponseToken
            {
                status = 1,
                AccessToken = AccessToken,
                RefreshToken = RefreshToken,
                msg = "Customer created successfully"
            };
        }
        public async Task<ResponseToken> Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                return new ResponseToken { status = 0, msg = "FaultyDTO" };

            var isCustomerExists = await (from c in _db.Customers
                                          where c.Email.ToLower() == email.ToLower()
                                          select c).FirstOrDefaultAsync();

            if(isCustomerExists == null)
                return new ResponseToken { status = 0, msg = "User not found" };
            
            var result = new PasswordHasher<Customer>().VerifyHashedPassword(isCustomerExists,isCustomerExists.Password,password);
            if (result == Microsoft.AspNetCore.Identity.PasswordVerificationResult.Failed)
                return new ResponseToken { status = 0, msg = "Invalid Password" };
            else
            {
                //Creating Tokens
                var AccessToken = await _tokenHandler.CreateAccessToken(isCustomerExists.CustomerID, isCustomerExists.Name, isCustomerExists.Email, isCustomerExists.Role);
                var RefreshToken = await _tokenHandler.RefreshingToken(isCustomerExists.CustomerID);

                //Returning Tokens 
                return new ResponseToken
                {
                    status = 1,
                    AccessToken = AccessToken,
                    RefreshToken = RefreshToken,
                    msg = "Login successful"
                };
            }
        }
        public async Task<SettersResponse> UpdateCustomer(Guid customerID,CustomerUpdateDTO customer)
        {
            //Checking DTO validity
            if (customer == null)
                return new SettersResponse { status = 0, msg = "Faulty DTO" };

            //Checking if User Exists
            var isCustomerExists = await (from c in _db.Customers
                                          where c.CustomerID == customerID
                                          select c).FirstOrDefaultAsync();
            //If User does not exist return
            if (isCustomerExists == null)
                return new SettersResponse { status = 0, msg = "Customer does not exist" };

            //Authorization Check
            var authResult = await _authorizationService.AuthorizeAsync(_currentUserService.User!, customerID, "SameUserAuth");
            if(!authResult.Succeeded)
                return new SettersResponse { status = 1, msg = "Unauthorized to update this customer." };

            // --***Updating***--

            //If the user wants to change their username, we must first check if the username is in use
            if (!string.IsNullOrEmpty(customer.Username))
            {
                //If username is not in use then we change to it
                if (customer.Username.ToLower() == isCustomerExists.Username.ToLower() || !await isUsernameValid(customer.Username))
                    isCustomerExists.Username = customer.Username;
                //If it is in user we return
                else
                    return new SettersResponse { status = 0, msg = "Username is already used" };
            }
            //Updating Phone field
            if(!string.IsNullOrEmpty(customer.Phone))
                isCustomerExists.Phone = customer.Phone;   
            //Updating Address field
            if(!string.IsNullOrEmpty(customer.Address))
                isCustomerExists.Address = customer.Address;
            //Updating City field
            if (!string.IsNullOrEmpty(customer.City)) 
                isCustomerExists.City = customer.City;
            //Updating Country field
            if(!string.IsNullOrEmpty(customer.Country))
                isCustomerExists.Country = customer.Country;
            //Updating Profile picture
            if(!string.IsNullOrEmpty(customer.ProfileUrl))
                isCustomerExists.ProfileUrl = customer.ProfileUrl;

            //Saving to Database
            _db.Customers.Update(isCustomerExists);
            await _db.SaveChangesAsync();
            return new SettersResponse { status = 2, msg = "Updated Successfully" };
        }
        public async Task<SettersResponse> DeleteCustomer(Guid customerID)
        {
            //Checking ID validity
            if (customerID == Guid.Empty)
                return new SettersResponse { status = 0, msg = "Faulty customerID" };

            //Getting curstomer from Database
            var isCustomerExists = await (from  c in _db.Customers
                                          where c.CustomerID == customerID
                                          select c).FirstOrDefaultAsync();
            //If customer not found return
            if (isCustomerExists == null)
                return new SettersResponse { status = 0, msg = "Customer not found" };

            //Authorization Check
            var authResult = await _authorizationService.AuthorizeAsync(_currentUserService.User!, customerID, "SameUserAuth");
            if (!authResult.Succeeded)
                return new SettersResponse { status = 1, msg = "Unauthorized to delete this customer." };

            //Saving to Database
            _db.Customers.Remove(isCustomerExists);
            await _db.SaveChangesAsync();
            return new SettersResponse { status = 2, msg = "Customer Deleted Successfully" };
        }
        public async Task<SettersResponse> BuyVehicle(Guid customerID, Guid vehicleID)
        {
            if (customerID == Guid.Empty || vehicleID == Guid.Empty)
                return new SettersResponse { status = 0, msg = "Invalid identifiers." };

            // Load buyer and vehicle with seller/manufacturer navigations
            var buyer = await _db.Customers.FindAsync(customerID);
            if (buyer == null)
                return new SettersResponse { status = 0, msg = "Buyer not found." };

            //Auth Check
            var authResult = await _authorizationService.AuthorizeAsync(_currentUserService.User!, customerID, "SameUserAuth");
            if (!authResult.Succeeded)
                return new SettersResponse { status = 1, msg = "Unauthorized to buy vehicle for this customer." };

            var vehicle = await _db.Vehicles
                .Include(v => v.Seller)
                .Include(v => v.Manufacturer)
                .FirstOrDefaultAsync(v => v.VehicleID == vehicleID);

            if (vehicle == null)
                return new SettersResponse { status = 0, msg = "Vehicle not found." };

            if (vehicle.BuyerID != null)
                return new SettersResponse { status = 0, msg = "Vehicle already sold." };

            if (vehicle.SellerID == customerID)
                return new SettersResponse { status = 0, msg = "Seller trying to buy his own car" };

            // Use a DB transaction to avoid races
            await using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                // Re-check inside transaction in case of concurrent changes
                var currentVehicle = await _db.Vehicles
                    .AsTracking()
                    .FirstOrDefaultAsync(v => v.VehicleID == vehicleID);

                if (currentVehicle == null)
                    return new SettersResponse { status = 0, msg = "Vehicle not found." };

                if (currentVehicle.BuyerID != null)
                    return new SettersResponse { status = 0, msg = "Vehicle already sold." };

                // Create transaction record
                var transaction = new Transaction
                {
                    TransactionID = Guid.NewGuid(),
                    Amount = currentVehicle.Price,
                    CreatedDate = DateTime.UtcNow,
                    Buyer = buyer,
                    BuyerID = buyer.CustomerID,
                    Vehicle = currentVehicle,
                    VehicleID = currentVehicle.VehicleID,
                    SellerCustomer = currentVehicle.Seller,
                    SellerCustomerID = currentVehicle.SellerID,
                    SellerManufacturer = null,
                    SellerManufacturerID = null
                };
                
                // Update vehicle state
                currentVehicle.BuyerID = buyer.CustomerID;
                currentVehicle.Buyer = buyer;
                currentVehicle.Status = "Sold";
                currentVehicle.Transaction = transaction;

                _db.Transactions.Add(transaction);
                _db.Vehicles.Update(currentVehicle);

                await _db.SaveChangesAsync();
                await tx.CommitAsync();

                return new SettersResponse { status = 2, msg = "Purchase completed successfully." };
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return new SettersResponse { status = 0, msg = $"Purchase failed: {ex.Message}" };
            }
        }
        public async Task<SettersResponse> SellVehicle(Guid sellerID, Guid vehicleID)
        {
            if (sellerID == Guid.Empty || vehicleID == Guid.Empty)
                return new SettersResponse { status = 0, msg = "Invalid identifiers." };

            var seller = await _db.Customers.FindAsync(sellerID);
            if (seller == null)
                return new SettersResponse { status = 0, msg = "Seller not found." };

            //Auth Check
            var authResult = await _authorizationService.AuthorizeAsync(_currentUserService.User!, sellerID, "SameUserAuth");
            if (!authResult.Succeeded)
                return new SettersResponse { status = 1, msg = "Unauthorized to sell vehicle for this customer." };

            var vehicle = await _db.Vehicles.FirstOrDefaultAsync(v => v.VehicleID == vehicleID);
            if (vehicle == null)
                return new SettersResponse { status = 0, msg = "Vehicle not found." };

            if (vehicle.BuyerID != null)
                return new SettersResponse { status = 0, msg = "Vehicle already sold; cannot list." };

            // If already listed by same seller, nothing to change
            if (vehicle.SellerID == sellerID && vehicle.Status == "Available")
                return new SettersResponse { status = 0, msg = "Vehicle is already listed by this seller." };

            // Assign seller and mark as available
            vehicle.SellerID = seller.CustomerID;
            vehicle.Seller = seller;
            vehicle.Status = "Available";
            vehicle.Transaction = null;
            //Checking if the car being sold was brought by the seller
            if (seller.VehiclesBought.Any(v=>v.VehicleID == vehicle.VehicleID))
                seller.VehiclesBought.Remove(vehicle);

            _db.Customers.Update(seller);
            _db.Vehicles.Update(vehicle);
            await _db.SaveChangesAsync();

            return new SettersResponse { status = 2, msg = "Vehicle listed for sale successfully." };
        }
        public async Task<SettersResponse> Subscribe(Guid customerID, Guid manufacturerID)
        {
            var isCustomerExists = await (from c in _db.Customers
                                        where c.CustomerID == customerID
                                        select c).FirstOrDefaultAsync();
            if (isCustomerExists == null)
                return new SettersResponse { status = 0, msg = "Customer does not exist" };

            //Auth Check
            var authResult = await _authorizationService.AuthorizeAsync(_currentUserService.User!, customerID, "SameUserAuth");
            if (!authResult.Succeeded)
                return new SettersResponse { status = 1, msg = "Unauthorized to subscribe for this customer." };

            var isManufacturerExists = await (from m in _db.Manufacturers
                                                where m.ManufacturerID == manufacturerID
                                                select m).FirstOrDefaultAsync();
            if (isManufacturerExists == null)
                return new SettersResponse { status = 0, msg = "Manufacturer does not exist" };
            //Subscription Logic
            isManufacturerExists.Subscribers.Add(isCustomerExists);
            _db.Manufacturers.Update(isManufacturerExists);
            await _db.SaveChangesAsync();
            return new SettersResponse { status = 2, msg = "Subscription successful" };
        }
        //-----------------------------------------------------------------------


        //        *********** Extra Validation Function ***********

        public async Task<bool> isUsernameValid(string username)
        {
            var isUsernameExists = await _db.Customers.AnyAsync(c=>c.Username == username);
            return isUsernameExists;
        } 
        public bool IsEmailValid(string email)
        {
            if (new EmailAddressAttribute().IsValid(email) && email != null)
            {
                return true;
            }
            else return false;
        }
        public async Task<bool> IsPasswordValid(string password)
        {
            var PasswordPolicy = new Microsoft.AspNet.Identity.PasswordValidator
            {
                RequiredLength = 8,
                RequireNonLetterOrDigit = false,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };
            var result = await PasswordPolicy.ValidateAsync(password);
            if (result.Succeeded) 
                return true;
            else
                return false;
        }


        //-----------------------------------------------------------------------

        //        *********** Getters ***********
        public async Task<GetterResponses<CustomerViewDTO>> GetFilteredCustomers(string startDate, string endDate, int page, string sortColumn, string OrderBy, string searchTerm, int pageSize)
        {
            IQueryable<Customer> customerQuery = _db.Customers;
            DateTime validStartDate, validEndDate;
            if (DateTime.TryParse(startDate, out validStartDate))
            {
                customerQuery = customerQuery.Where(u => u.CreatedAt > validStartDate);
            }
            if (DateTime.TryParse(endDate, out validEndDate))
            {
                customerQuery = customerQuery.Where(u => u.CreatedAt < validEndDate);
            }
            if (!string.IsNullOrEmpty(searchTerm))
                customerQuery = customerQuery.Where(u => u.Name.Contains(searchTerm) ||
                u.Username.Contains(searchTerm) || u.Email.Contains(searchTerm));

            if (!string.IsNullOrEmpty(sortColumn))
            {
                Expression<Func<Customer, object>> keySelector = sortColumn.ToLower() switch // throws error when sortColumn is null
                {
                    "name" or "n" => Customer => Customer.Name,
                    "email" or "e" => Customer => Customer.Email,
                    "country" or "co" => Customer => Customer.Country!,
                    "city" or "ci" => Customer => Customer.City!,
                    "createdat" or "ca" => Customer => Customer.CreatedAt,
                    _ => Customer => Customer.CustomerID,
                };
                if (!string.IsNullOrEmpty(OrderBy)) customerQuery = customerQuery.OrderBy(keySelector);
                else customerQuery = customerQuery.OrderBy(keySelector);
            }

            var customerResponse = customerQuery.Select(c => new CustomerViewDTO
            {
                CustomerID = c.CustomerID,
                Name = c.Name,
                Username = c.Username,
                Address = c.Address,
                City = c.City,
                Country = c.Country,
                Phone = c.Phone,
                ProfileUrl = c.ProfileUrl
            });
            var data = await PagedList<CustomerViewDTO>.CreateAsync(customerResponse, page, pageSize);
            return new GetterResponses<CustomerViewDTO>
            {
                status = 1,
                msg = "Customers retrieved successfully",
                Data = data
            };
        }
        public async Task<GetterResponses<VehicleViewDTO>> GetBoughtVehicles(Guid customerID, string startDate, string endDate, int page, string sortColumn, string OrderBy, string searchTerm, int pageSize)
        {
            var vehiclesQuery = (from v in _db.Vehicles
                                 where v.BuyerID == customerID
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
                vehiclesQuery = vehiclesQuery.Where(u =>
                u.Description!.Contains(searchTerm) || u.Type!.Contains(searchTerm) || u.FuelType!.Contains(searchTerm));
            if (!string.IsNullOrEmpty(sortColumn))
            {
                Expression<Func<Vehicle, object>> keySelector = sortColumn.ToLower() switch // throws error when sortColumn is null
                {
                    "price" or "p" => Vehicle => Vehicle.Price,
                    "type" or "t" => Vehicle => Vehicle.Type!,
                    "enginecapacity" or "ec" => Vehicle => Vehicle.EngineCapacity!,
                    "fueltype" or "f" => Vehicle => Vehicle.FuelType!,
                    "seatingcapacity" or "sc" => Vehicle => Vehicle.SeatingCapacity!,
                    "status" or "s" => Vehicle => Vehicle.Status!,
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


            var data = await PagedList<VehicleViewDTO>.CreateAsync(vehiclesResponse, page, pageSize);
            return new GetterResponses<VehicleViewDTO>
            {
                status = 1,
                msg = "Vehicles retrieved successfully",
                Data = data
            };
        }
        public async Task<GetterResponses<VehicleSellerViewDTO>> GetSoldVehicles(Guid customerID, string startDate, string endDate, int page, string sortColumn, string OrderBy, string searchTerm, int pageSize)
        {
            var vehiclesQuery = (from v in _db.Vehicles
                            where v.SellerID == customerID
                            select v);
            DateTime validStartDate, validEndDate;
            if (DateTime.TryParse(startDate, out validStartDate))
            {
                vehiclesQuery = vehiclesQuery.Where(u => u.Transaction.CreatedDate > validStartDate);
            }
            if (DateTime.TryParse(endDate, out validEndDate))
            {
                vehiclesQuery = vehiclesQuery.Where(u => u.Transaction.CreatedDate < validEndDate);
            }
            if (!string.IsNullOrEmpty(searchTerm))
                vehiclesQuery = vehiclesQuery.Where(u => u.Model.Contains(searchTerm) ||
                u.Description.Contains(searchTerm) || u.Type.Contains(searchTerm) || u.FuelType.Contains(searchTerm));
            if (!string.IsNullOrEmpty(sortColumn))
            {
                Expression<Func<Vehicle, object>> keySelector = sortColumn.ToLower() switch // throws error when sortColumn is null
                {
                    "name" or "n" => Vehicle => Vehicle.Model,
                    "price" or "p" => Vehicle => Vehicle.Price,
                    "type" or "t" => Vehicle => Vehicle.Type,
                    "enginecapacity" or "ec" => Vehicle => Vehicle.EngineCapacity,
                    "fueltype" or "f" => Vehicle => Vehicle.FuelType,
                    "seatingcapacity" or "sc" => Vehicle => Vehicle.SeatingCapacity,
                    "status" or "s" => Vehicle => Vehicle.Status,
                    _ => Vehicle => Vehicle.VehicleID,
                };
                if (!string.IsNullOrEmpty(OrderBy)) vehiclesQuery = vehiclesQuery.OrderBy(keySelector);
                else vehiclesQuery = vehiclesQuery.OrderBy(keySelector);
            }
            var vehiclesResponse = vehiclesQuery
                                    .Select(v => new VehicleSellerViewDTO
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


            var data = await PagedList<VehicleSellerViewDTO>.CreateAsync(vehiclesResponse, page, pageSize);
            return new GetterResponses<VehicleSellerViewDTO>
            {
                status = 1,
                msg = "Vehicles retrieved successfully",
                Data = data
            };
        }
        public async Task<CustomerViewDTO?> GetCustomer(Guid customerID)
        {
            //Getting customer from Database and projecting to CustomerDTO
            var customer = await (from c in _db.Customers
                                  where c.CustomerID == customerID
                                  select new CustomerViewDTO
                                  {
                                      CustomerID = customerID,
                                      Name = c.Name,
                                      Username = c.Username,
                                      Address = c.Address,
                                      City = c.City,
                                      Country = c.Country,
                                      Phone = c.Phone,
                                      ProfileUrl = c.ProfileUrl
                                  }).FirstOrDefaultAsync();

            //Returning Customer
            return customer;
        }
        public async Task<GetterResponses<ManufacturerViewDTO>> GetSubscribedManufacturers(Guid customerID, int page, int pageSize)
        {
            var isCustomerExists = await _db.Customers.AnyAsync(c => c.CustomerID == customerID);
            if (!isCustomerExists) 
                return new GetterResponses<ManufacturerViewDTO>
                {
                    status = 0,
                    msg = "Customer not found",
                    Data = null
                };

            var manufacturersQuery = _db.Customers
                                    .Where(c => c.CustomerID == customerID)
                                    .SelectMany(c => c.SubscribedManufacturers) // keep this as an EF query
                                    .Select(m => new ManufacturerViewDTO
                                    {
                                        ManufacturerID = m.ManufacturerID,
                                        Name = m.Name,
                                        Description = m.Description,
                                        Type = m.Type
                                    });

            var data = await PagedList<ManufacturerViewDTO>.CreateAsync(manufacturersQuery, page, pageSize);
            return new GetterResponses<ManufacturerViewDTO>
            {
                status = 1,
                msg = "Subscribed manufacturers retrieved successfully",
                Data = data
            };
        }
        public async Task<GetterResponses<NotificationViewDTO>> GetNotifications(Guid customerID, int page, int pageSize)
        {
            var isCustomerExists = await _db.Customers.AnyAsync(c => c.CustomerID == customerID);
            if (!isCustomerExists)
                return new GetterResponses<NotificationViewDTO>
                {
                    status = 0,
                    msg = "Customer not found",
                    Data = null
                };

            var notificationsQuery = _db.Notifications
                                        .Where(n => n.CustomerID == customerID)
                                        .Select(n => new NotificationViewDTO
                                        {
                                            NotificationID = n.NotificationId,
                                            Title = n.Title,
                                            Message = n.Message,
                                            CreatedAt = n.CreatedAt
                                        });

            var data = await PagedList<NotificationViewDTO>.CreateAsync(notificationsQuery, page, pageSize);
            return new GetterResponses<NotificationViewDTO>
            {
                status = 1,
                msg = "Notifications retrieved successfully",
                Data = data
            };
        }
        public async Task<GetterResponses<CustomerViewDTO>> GetCustomers(int page,int pageSize)
        {
            //Getting customers from Database and projecting to CustomerDTO
            var customersQuery = from c in _db.Customers
                                 select new CustomerViewDTO
                                 {
                                     CustomerID = c.CustomerID,
                                     Name = c.Name,
                                     Username = c.Username,
                                     Address = c.Address,
                                     City = c.City,
                                     Country = c.Country,
                                     Phone = c.Phone,
                                     ProfileUrl = c.ProfileUrl
                                 };

            var data = await PagedList<CustomerViewDTO>.CreateAsync(customersQuery, page, pageSize);
            return new GetterResponses<CustomerViewDTO>
            {
                status = 1,
                msg = "Customers retrieved successfully",
                Data = data
            };
        }
        
    }
}
