using AuthService.Features.UserManagement.LoginFeature.Orchestrator;
using AuthService.Features.UserManagement.LoginFeature.ViewModel;

namespace AuthService.Features.UserManagement.LoginFeature.Extension
{
    public static class Mapping
    {
        public static GenerateTokenOrchestrator ToCommand(this LoginRequestVm viewModel)
        {
            return new GenerateTokenOrchestrator(viewModel.Email, viewModel.Password);
        }
    }
}
