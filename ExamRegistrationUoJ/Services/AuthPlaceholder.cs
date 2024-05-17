namespace ExamRegistrationUoJ.Services
{
    public class AuthPlaceholder : AuthInterface
    {
        public async Task<bool> IsAnAdministrator(string? email)
        {
            return email != null && false;
        }

        public async Task<bool> IsACoordinator(string? email, string? nameidentifier)
        {
            return email != null && true;
        }

        public async Task<bool> IsAStudent(string? email, string? nameidentifier)
        {
            return email != null && false;
        }

        public async Task<bool> IsAnAdvisor(string? email)
        {
            return email != null && true;
        }

        public async Task<bool> IsBothAdvisorCoordinator(string? email, string? nameidentifier)
        {
            return await IsAnAdvisor(nameidentifier) || await IsACoordinator(email, nameidentifier);
        }
    }
}