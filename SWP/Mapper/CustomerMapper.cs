using SWP.Dtos.Customer;
using SWP.Models;

namespace SWP.Mapper
{
    public static class CustomerMapper
    {
        public static CustomerDto ToCustomerDto(this Customer customer)
        {
           return new CustomerDto
            {
                CusId = customer.CusId,
                AccId = customer.AccId,
                HusName = customer.HusName,
                WifeName = customer.WifeName,
                HusYob = customer.HusYob,
                WifeYob = customer.WifeYob,
                Phone = customer.Phone,
                Mail = customer.Mail
            };
        }
       
    }
}
