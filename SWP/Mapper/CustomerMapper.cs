using SWP.Dtos.Account;
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


                HusName = customer.HusName,
                HusYob = customer.HusYob,
                WifeName = customer.WifeName,
                WifeYob = customer.WifeYob,
            };
        }

    }
}
