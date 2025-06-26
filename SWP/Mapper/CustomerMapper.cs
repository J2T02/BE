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
                AccCus = customer.Acc != null ? new AccountDetailResponeDto
                {
                    FullName = customer.Acc.FullName,
                    Phone = customer.Acc.Phone,
                    Mail = customer.Acc.Mail
                } : null
            };
        }
        public static UpdateCustomerResponseDto ToCustomer(this Customer customer)
        {
           return new UpdateCustomerResponseDto
            {
                HusName = customer.HusName,
                WifeName = customer.WifeName,
                HusYob = (DateOnly)customer.HusYob,
                WifeYob = (DateOnly)customer.WifeYob
           };

        }
    }
}
