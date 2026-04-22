using Kawerk.Application.Interfaces;
using Kawerk.Domain;
using Kawerk.Infastructure.Context;
using Kawerk.Infastructure.DTOs.Branch;
using Kawerk.Infastructure.DTOs.Salesman;
using Kawerk.Infastructure.ResponseClasses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Kawerk.Application.Services
{
    public class BranchService : IBranchSevice
    {
        private readonly DbBase _db;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuthorizationService _authorizationService;
        public BranchService(DbBase db, ICurrentUserService currentUserService, IAuthorizationService authorizationService)
        {
            _db = db;
            _currentUserService = currentUserService;
            _authorizationService = authorizationService;
        }

        //        *********** Setters ***********
        public async Task<SettersResponse> CreateBranch(BranchCreationDTO branch)
        {
            //Checking DTO validation
            if (branch == null)
                return new SettersResponse { status = 0, msg = "Faulty DTO" };

            //Checking name and location uniqueness
            if(await isNameValid(branch.Name) || await isLocationValid(branch.Location))
                return new SettersResponse { status = 0, msg = "Name or location already in use" };

            //Creating new Branch
            Branches newBranch = new Branches
            {
                BranchID = Guid.NewGuid(),
                Name = branch.Name,
                Description = branch.Description,
                Location = branch.Location,
                CreatedAt = DateTime.UtcNow,
            };

            //Saving to Database
            await _db.Branches.AddAsync(newBranch);
            await _db.SaveChangesAsync();
            return new SettersResponse { status = 1, msg = "Branch created successfully" };
        }
        public async Task<SettersResponse> UpdateBranch(Guid branchID, BranchUpdateDTO branch)
        {
            //Checking DTO & ID validity
            if (branchID == Guid.Empty || branch == null)
                return new SettersResponse { status = 0, msg = "Faulty DTO or ID" };

            //Getting branch from Database
            var isBranchExisting = await (from b in _db.Branches
                                          where b.BranchID == branchID
                                          select b).FirstOrDefaultAsync();
            //If Branch not found return
            if (isBranchExisting == null)
                return new SettersResponse { status = 0, msg = "Branch not found" };
            
            // --***Updating***--

            //If they want to change the name of the branch we have to check if the new name is in use or not
            if (!string.IsNullOrEmpty(branch.Name))
            {
                //if not in use update to new name
                if (!await isNameValid(branch.Name))
                    isBranchExisting.Name = branch.Name;
                //if in use return
                else
                    return new SettersResponse { status = 0, msg = "Name is already in use" };
            }
            //If they want to change the location of the branch we have to check if the new location is in use or not
            if (!string.IsNullOrEmpty(branch.Location))
            {
                //if not in use update to new location
                if (!await isLocationValid(branch.Location))
                    isBranchExisting.Location = branch.Location;
                //if in use return
                else
                    return new SettersResponse { status = 0, msg = "Location is already in use" };
            }
            //Updating Description
            if(!string.IsNullOrEmpty(branch.Description))
                isBranchExisting.Description = branch.Description;
            //Updating Warranty
            if(!string.IsNullOrEmpty(branch.Warranty))
                isBranchExisting.Warranty = branch.Warranty;

            //Saving to Database
            _db.Branches.Update(isBranchExisting);
            await _db.SaveChangesAsync();
            return new SettersResponse { status = 1, msg = "Branch updated successfully" };

        }
        public async Task<SettersResponse> DeleteBranch(Guid branchID)
        {
            //Checking ID validity
            if (branchID == Guid.Empty)
                return new SettersResponse { status = 0, msg = "Faulty ID" };

            //Getting branch from Database
            var isBranchExisting = await (from b in _db.Branches
                                          where b.BranchID == branchID
                                          select b).FirstOrDefaultAsync();
            //if branch not found return
            if(isBranchExisting == null) 
                return new SettersResponse { status = 0, msg = "Branch not found" };

            //Saving to Database
            _db.Branches.Remove(isBranchExisting);
            await _db.SaveChangesAsync();
            return new SettersResponse { status = 1, msg = "Branch deleted successfully" };
        }
        public async Task<SettersResponse> AssignManager(Guid branchID, Guid customerID)
        {
            //Checking ID validity
            if (branchID == Guid.Empty || customerID == Guid.Empty)
                return new SettersResponse { status = 0, msg = "Faulty ID" };
            //Getting branch from Database
            var isBranchExisting = await (from b in _db.Branches.Include(b => b.BranchManager)
                                          where b.BranchID == branchID
                                          select b).FirstOrDefaultAsync();
            //if branch not found return
            if (isBranchExisting == null)
                return new SettersResponse { status = 0, msg = "Branch not found" };
            //Getting customer from Database
            var isCustomerExisting = await (from c in _db.Customers
                                            where c.CustomerID == customerID
                                            select c).FirstOrDefaultAsync();
            //if customer not found return
            if (isCustomerExisting == null)
                return new SettersResponse { status = 0, msg = "Customer not found" };

            bool hasManager = false;
            //Checking if branch already has a manager
            if (isBranchExisting.BranchManager != null)
            {
               hasManager = true;
            }
            //Assigning manager to branch
            isBranchExisting.BranchManager = isCustomerExisting;
            _db.Branches.Update(isBranchExisting);
            await _db.SaveChangesAsync();
            if(isCustomerExisting.Role != "BranchManager")
            {
                isCustomerExisting.Role = "BranchManager";
                _db.Customers.Update(isCustomerExisting);
                await _db.SaveChangesAsync();
            }
            if(hasManager)
                return new SettersResponse { status = 2, msg = "Branch manager reassigned successfully" };
            else
                return new SettersResponse { status = 2, msg = "Manager assigned to branch successfully" };
        }
        public async Task<SettersResponse> AddSalesman(Guid branchID, Guid salesmanID)
        {
            //Checking ID validity
            if (branchID == Guid.Empty || salesmanID == Guid.Empty)
                return new SettersResponse { status = 0, msg = "Faulty ID" };

            //Getting branch from Database
            var isBranchExisting = await (from b in _db.Branches.Include(b => b.Salesmen).Include(b=>b.BranchManager)
                                          where b.BranchID == branchID
                                          select b).FirstOrDefaultAsync();
            //if branch not found return
            if (isBranchExisting == null)
                return new SettersResponse { status = 0, msg = "Branch not found" };

            //Checking if branch has a manager assigned
            if (isBranchExisting.BranchManager == null)
                return new SettersResponse { status = 0, msg = "Branch has no manager assigned" };
            
            //Authorization Check
            var accessingUser = _currentUserService.User;
            var authorizationResult = await _authorizationService.AuthorizeAsync(accessingUser!, isBranchExisting.BranchManager.CustomerID, "SameUserAuth");
            if(!authorizationResult.Succeeded)
                return new SettersResponse { status = 1, msg = "You are not authorized to add salesmen to this branch" };
            //Getting salesman from Database
            var isSalesmanExisting = await (from s in _db.Salesman
                                            where s.SalesmanID == salesmanID
                                            select s).FirstOrDefaultAsync();
            //if salesman not found return
            if (isSalesmanExisting == null)
                return new SettersResponse { status = 0, msg = "Salesman not found" };

            //Checking if salesman is already assigned to this branch
            if (isBranchExisting.Salesmen.Any(s => s.SalesmanID == salesmanID))
                return new SettersResponse { status = 0, msg = "Salesman already assigned to this branch" };

            //Adding salesman to branch
            isBranchExisting.Salesmen.Add(isSalesmanExisting);
            _db.Branches.Update(isBranchExisting);
            await _db.SaveChangesAsync();
            return new SettersResponse { status = 2, msg = "Salesman added to branch successfully" };
        }
        public async Task<SettersResponse> RemoveSalesman(Guid branchID, Guid salesmanID)
        {
            //Checking ID validity
            if (branchID == Guid.Empty || salesmanID == Guid.Empty)
                return new SettersResponse { status = 0, msg = "Faulty ID" };

            //Getting branch from Database
            var isBranchExisting = await (from b in _db.Branches.Include(b => b.Salesmen).Include(b => b.BranchManager)
                                          where b.BranchID == branchID
                                          select b).FirstOrDefaultAsync();
            //if branch not found return
            if (isBranchExisting == null)
                return new SettersResponse { status = 0, msg = "Branch not found" };

            if(isBranchExisting.BranchManager == null)
                return new SettersResponse { status = 0, msg = "Branch has no manager assigned" };
            //Authorization Check
            var accessingUser = _currentUserService.User;
            var authorizationResult = await _authorizationService.AuthorizeAsync(accessingUser!, isBranchExisting.BranchManager.CustomerID, "SameUserAuth");
            if (!authorizationResult.Succeeded)
                return new SettersResponse { status = 1, msg = "You are not authorized to remove salesmen from this branch" };

            //Getting salesman from Database
            var isSalesmanExisting = await (from s in _db.Salesman
                                            where s.SalesmanID == salesmanID
                                            select s).FirstOrDefaultAsync();
            //if salesman not found return
            if (isSalesmanExisting == null)
                return new SettersResponse { status = 0, msg = "Salesman not found" };

            //Checking if salesman is assigned to this branch
            if (!isBranchExisting.Salesmen.Any(s => s.SalesmanID == salesmanID))
                return new SettersResponse { status = 0, msg = "Salesman not assigned to this branch" };

            //Removing salesman from branch
            isBranchExisting.Salesmen.Remove(isSalesmanExisting);
            _db.Branches.Update(isBranchExisting);
            await _db.SaveChangesAsync();
            return new SettersResponse { status = 1, msg = "Salesman removed from branch successfully" };
        }
        //-----------------------------------------------------------------------


        //        *********** Extra Validation Function ***********

        public async Task<bool> isNameValid(string name)
        {
            var isNameExists = await _db.Branches.AnyAsync(b=>b.Name == name);
            return isNameExists;
        }
        public async Task<bool> isLocationValid(string location)
        {
            var isLocationValid = await _db.Branches.AnyAsync(b=>b.Location == location);
            return isLocationValid;
        }

        //-----------------------------------------------------------------------

        //        *********** Getters ***********

        public async Task<BranchViewDTO?> GetBranch(Guid branchID)
        {
            //Checking ID validity
            if (branchID == Guid.Empty)
                return null;

            //Getting branch from Database and projecting to BranchDTO 
            var isBranchExisting = await (from b in _db.Branches
                                          where b.BranchID == branchID
                                          select new BranchViewDTO
                                          {
                                              BranchID = b.BranchID,
                                              Name = b.Name,
                                              Description = b.Description,
                                              Location = b.Location,
                                              CreatedAt = b.CreatedAt
                                          }).FirstOrDefaultAsync();
            //returning result
            return isBranchExisting;
        }
        public async Task<PagedList<SalesmanViewDTO>?> GetBranchSalesmen(Guid branchID, string startDate, string endDate, int page, string sortColumn, string OrderBy, string searchTerm, int pageSize = 5)
        {
            if(branchID == Guid.Empty)
                return null;
            //Getting branch salesmen from Database and projecting to SalesmanDTO
            var salesmenQuery = (from b in _db.Branches
                                 where b.BranchID == branchID
                                 from s in b.Salesmen
                                 select s);

            DateTime validStartDate, validEndDate;
            if (DateTime.TryParse(startDate, out validStartDate))
            {
                salesmenQuery = salesmenQuery.Where(u => u.CreatedAt > validStartDate);
            }
            if (DateTime.TryParse(endDate, out validEndDate))
            {
                salesmenQuery = salesmenQuery.Where(u => u.CreatedAt < validEndDate);
            }
            if (!string.IsNullOrEmpty(searchTerm)) salesmenQuery = salesmenQuery.Where(u => u.Name.Contains(searchTerm) || u.Email.Contains(searchTerm));

            if (!string.IsNullOrEmpty(sortColumn))
            {
                Expression<Func<Salesman, object>> keySelector = sortColumn.ToLower() switch // throws error when sortColumn is null
                {
                    "name" or "n" => Salesman => Salesman.Name,
                    "email" or "e" => Salesman => Salesman.Email,
                    "country" or "co" => Salesman => Salesman.Country,
                    "city" or "ci" => Salesman => Salesman.City,
                    "address" or "a" => Salesman => Salesman.Address,
                    "createdat" or "ca" => Salesman => Salesman.CreatedAt,
                    _ => Salesman => Salesman.SalesmanID,
                };
                if (!string.IsNullOrEmpty(OrderBy)) salesmenQuery = salesmenQuery.OrderBy(keySelector);
                else salesmenQuery = salesmenQuery.OrderBy(keySelector);
            }
            var salesmanResponse = salesmenQuery
                                .Select(u => new SalesmanViewDTO
                                {
                                    SalesmanID = u.SalesmanID,
                                    Name = u.Name,
                                    Email = u.Email,
                                    Address = u.Address,
                                    Phone = u.Phone,
                                    Salary = u.Salary,
                                    City = u.City,
                                    Country = u.Country,
                                    CreatedAt = u.CreatedAt
                                });
            return await PagedList<SalesmanViewDTO>.CreateAsync(salesmanResponse, page, pageSize);
        }
        public async Task<PagedList<BranchViewDTO>?> GetBranches(int page,int pageSize)
        {
            //Getting branches from Database and projecting to BranchDTO
            var branchQuery = (from b in _db.Branches
                               select new BranchViewDTO
                               {
                                             BranchID = b.BranchID,
                                   Name = b.Name,
                                   Description = b.Description,
                                   Location = b.Location,
                                   CreatedAt = b.CreatedAt
                               });
            //returning result
            return await PagedList<BranchViewDTO>.CreateAsync(branchQuery, page, pageSize);
        }
    }
}
