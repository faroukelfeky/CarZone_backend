using Kawerk.Infastructure.DTOs.Salesman;
using Kawerk.Infastructure.ResponseClasses;

namespace Kawerk.Application.Interfaces
{
    public interface ISalesmanService
    {
        public Task<SettersResponse> CreateSalesman(SalesmanCreationDTO salesman);
        public Task<SettersResponse> UpdateSalesman(Guid salesmanID, SalesmanUpdateDTO salesman);
        public Task<SettersResponse> DeleteSalesman(Guid salesmanID);
        public Task<SalesmanViewDTO?> GetSalesman(Guid salesmanID);
        public Task<PagedList<SalesmanViewDTO>?> GetSalesmen(int page, int pageSize);
    }
}
