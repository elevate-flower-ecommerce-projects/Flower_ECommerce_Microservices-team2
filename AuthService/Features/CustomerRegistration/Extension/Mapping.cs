
using AuthService.Features.CustomerRegistration.Command;
using AuthService.Features.CustomerRegistration.ViewModel;

namespace AuthService.Features.CustomerRegistration.Extension
{

    public static class Mapping
    {
        public static CustomerRegistrationCommand ToCommand(this CustomerRegistrationViewModel viewModel)
        {
            return new CustomerRegistrationCommand(
                viewModel.FullName,
                viewModel.Email,
                viewModel.PhoneNumber,
                viewModel.Gender,
                viewModel.Password,
                viewModel.ConfirmPassword);
        }
    }
}
