using Kawerk.Application.Interfaces;
using Kawerk.Domain;
using Kawerk.Infastructure.Context;
using Kawerk.Infastructure.DTOs.Transaction;
using Kawerk.Infastructure.ResponseClasses;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace Kawerk.Application.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly DbBase _db;
        public TransactionService(DbBase db)
        {
            _db = db;
        }

        //        *********** Setters ***********
        public async Task<SettersResponse> CreateTransaction(TransactionCreationDTO transaction)//0 == Faulty DTO || 1 == emtpy SellerID and ManufacturerID || 2 == Seller not found || 3 == Manufacturer not found || 4 == Customer not found || 5 == Vehicle not found || 6 == Successful
        {
            if (transaction == null)
                return new SettersResponse { status = 0, msg = "Faulty DTO" };

            if (transaction.SellerCustomerID != Guid.Empty)
            {
                if (isSameUserID(transaction.BuyerID, transaction.SellerCustomerID))
                    return new SettersResponse { status = 0, msg = "Buyer and Seller cannot be the same" };

                var isSellerExists = await (from c in _db.Customers
                                              where c.CustomerID == transaction.SellerCustomerID
                                              select c).FirstOrDefaultAsync();
                if (isSellerExists == null)
                    return new SettersResponse { status = 0, msg = "Seller not found" };
                var isCustomerExists = await (from c in _db.Customers
                                                where c.CustomerID == transaction.BuyerID
                                                select c).FirstOrDefaultAsync();
                if(isCustomerExists == null)
                    return new SettersResponse { status = 0, msg = "Customer not found" };

                var isVehicleExists = await (from v in _db.Vehicles
                                             where v.VehicleID == transaction.VehicleID
                                             select v).FirstOrDefaultAsync();
                if (isVehicleExists == null)
                    return new SettersResponse { status = 0, msg = "Vehicle not found" };

                Domain.Transaction newTransaction = new Domain.Transaction
                {
                    TransactionID = Guid.NewGuid(),
                    Buyer = isCustomerExists,
                    SellerCustomer = isSellerExists,
                    Amount = transaction.Amount,
                    CreatedDate = DateTime.Now,
                    Vehicle = isVehicleExists
                };
                await _db.Transactions.AddAsync(newTransaction);
                await _db.SaveChangesAsync();
                return new SettersResponse { status = 1, msg = "Successful" };
            }
            else if (transaction.SellerManufacturerID != Guid.Empty)
            {
                

                var isManufacturerExists = await (from m in _db.Manufacturers
                                            where m.ManufacturerID == transaction.SellerManufacturerID
                                            select m).FirstOrDefaultAsync();
                if (isManufacturerExists == null)
                    return new SettersResponse { status = 0, msg = "Manufacturer not found" };

                var isCustomerExists = await (from c in _db.Customers
                                              where c.CustomerID == transaction.BuyerID
                                              select c).FirstOrDefaultAsync();
                if (isCustomerExists == null)
                    return new SettersResponse { status = 0, msg = "Customer not found" };

                var isVehicleExists = await (from v in _db.Vehicles
                                             where v.VehicleID == transaction.VehicleID
                                             select v).FirstOrDefaultAsync();
                if (isVehicleExists == null)
                    return new SettersResponse { status = 5, msg = "Faulty DTO" };

                Domain.Transaction newTransaction = new Domain.Transaction
                {
                    TransactionID = Guid.NewGuid(),
                    Buyer = isCustomerExists,
                    SellerManufacturer = isManufacturerExists,
                    Amount = transaction.Amount,
                    CreatedDate = DateTime.Now,
                    Vehicle = isVehicleExists
                };
                await _db.Transactions.AddAsync(newTransaction);
                await _db.SaveChangesAsync();
                return new SettersResponse { status = 1, msg = "Successful" };
            }
            else
                return new SettersResponse { status = 0, msg = "Faulty DTO" };
        }
        public async Task<SettersResponse> DeleteTransaction(Guid transactionID)//0 == Invalid ID || 1 == Transaction not found || 2 == Deleted Successfully
        {
            if (transactionID == Guid.Empty)
                return new SettersResponse { status = 0, msg = "Invalid ID" };
            var isTransactionExists = await (from t in _db.Transactions
                                     where t.TransactionID == transactionID
                                     select t).FirstOrDefaultAsync();

            if (isTransactionExists == null)
                return new SettersResponse { status = 0, msg = "Transaction not found" };

            _db.Transactions.Remove(isTransactionExists);
            await _db.SaveChangesAsync();
            return new SettersResponse { status = 1, msg = "Deleted transaction Successfully" };
        }

        public bool isSameUserID(Guid buyerID, Guid? sellerID)
        {
            return buyerID == sellerID;
        }

        //        *********** Getters ***********
        public async Task<TransactionViewDTO?> GetTransaction(Guid transactionID)
        {
            var isTransactionExist = await(from t in _db.Transactions
                                          where t.TransactionID == transactionID
                                          select new TransactionViewDTO
                                          {
                                              TransactionID = t.TransactionID,
                                              Amount = t.Amount,
                                              BuyerID = t.Buyer.CustomerID,
                                              VehicleID = t.Vehicle.VehicleID,
                                              SellerCustomerID = t.SellerCustomer != null ? t.SellerCustomer.CustomerID : null,
                                              SellerManufacturerID = t.SellerManufacturer != null ? t.SellerManufacturer.ManufacturerID : null,
                                              CreatedDate = t.CreatedDate
                                          }).FirstOrDefaultAsync();
            return isTransactionExist;
        }

        public Task<PagedList<TransactionViewDTO>> GetUserTransactions(Guid userID, int pageNumber, int pageSize)
        {
            var transationQuerry = from t in _db.Transactions
                                   where t.Buyer.CustomerID == userID || (t.SellerCustomer != null && t.SellerCustomer.CustomerID == userID)
                                   select new TransactionViewDTO
                                   {
                                       TransactionID = t.TransactionID,
                                       Amount = t.Amount,
                                       BuyerID = t.Buyer.CustomerID,
                                       VehicleID = t.Vehicle.VehicleID,
                                       SellerCustomerID = t.SellerCustomer != null ? t.SellerCustomer.CustomerID : null,
                                       SellerManufacturerID = t.SellerManufacturer != null ? t.SellerManufacturer.ManufacturerID : null,
                                       CreatedDate = t.CreatedDate
                                   };
            return PagedList<TransactionViewDTO>.CreateAsync(transationQuerry, pageNumber, pageSize);
        }

        public async Task<PagedList<TransactionViewDTO>> GetTransactions(int page,int pageSize)
        {
            var transactionQuerry = (from t in _db.Transactions
                                      select new TransactionViewDTO
                                      {
                                        TransactionID = t.TransactionID,
                                        Amount = t.Amount,
                                        BuyerID = t.Buyer.CustomerID,
                                        VehicleID = t.Vehicle.VehicleID,
                                        SellerCustomerID = t.SellerCustomer != null ? t.SellerCustomer.CustomerID : null,
                                        SellerManufacturerID = t.SellerManufacturer != null ? t.SellerManufacturer.ManufacturerID : null,
                                        CreatedDate = t.CreatedDate
                                      });
            return await PagedList<TransactionViewDTO>.CreateAsync(transactionQuerry, page, pageSize);
        }
    }
}
