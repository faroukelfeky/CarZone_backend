using Kawerk.Infastructure.DTOs.Transaction;
using Kawerk.Infastructure.ResponseClasses;

namespace Kawerk.Application.Interfaces
{
    public interface ITransactionService
    {
        public Task<SettersResponse> CreateTransaction(TransactionCreationDTO transaction);
        public Task<SettersResponse> DeleteTransaction(Guid transactionID);
        public Task<TransactionViewDTO?> GetTransaction(Guid transactionID);
        public Task<PagedList<TransactionViewDTO>> GetUserTransactions(Guid userID, int pageNumber, int pageSize);
        public Task<PagedList<TransactionViewDTO>> GetTransactions(int pageNumber, int pageSize);

    }
}
