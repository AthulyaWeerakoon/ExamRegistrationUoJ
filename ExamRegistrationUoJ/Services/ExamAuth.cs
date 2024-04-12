
namespace ExamRegistrationUoJ.Services
{
    public class ExamAuth : AuthInterface
    {
        public async Task<bool> IsAnAdministrator(string? nameidentifier)
        {
            return nameidentifier != null && false;
        }

        public async Task<bool> IsACoordinator(string? nameidentifier)
        {
            return nameidentifier != null && false;
        }

        public async Task<bool> IsAStudent(string? email, string? nameidentifier)
        {
            return nameidentifier != null && false;
        }

        public async Task<bool> IsAnAdvisor(string? nameidentifier)
        {
            return nameidentifier != null && false;
        }

        public async Task<bool> IsBothAdvisorCoordinator(string? nameidentifier)
        {
            return await IsAnAdvisor(nameidentifier) || await IsACoordinator(nameidentifier);
        }
    }
}
