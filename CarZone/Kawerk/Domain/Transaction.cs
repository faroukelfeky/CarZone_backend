using System.ComponentModel.DataAnnotations;

namespace Kawerk.Domain
{
    public class Transaction
    {
        public Guid TransactionID { get; set; }
        public int Amount { get; set; }
        public DateTime CreatedDate { get; set; }

        //Relationships
        public required Customer Buyer { get; set; }
        public Guid BuyerID { get; set; }
        public required Vehicle Vehicle { get; set; }
        public Guid VehicleID { get; set; }
        public Customer? SellerCustomer { get; set; }
        public Guid? SellerCustomerID { get; set; }
        public Manufacturer? SellerManufacturer { get; set; }
        public Guid? SellerManufacturerID { get; set;}
    }
}
