using AuthService.Features.DriverRegistration.Command;
using AuthService.Features.DriverRegistration.ViewModel;

namespace AuthService.Features.DriverRegistration.Extension
{
    public static class Mapping
    {
        public static DriverRegistrationCommand ToCommand(this DriverRegistrationViewModel viewModel)
        {
            return new DriverRegistrationCommand(
                viewModel.FullName,
                viewModel.Email,
                viewModel.PhoneNumber,
                viewModel.Gender,
                viewModel.NationalId,
                viewModel.VehicleType,
                viewModel.VehiclePlate,
                viewModel.Password,
                viewModel.ConfirmPassword);
        }
    }
}
