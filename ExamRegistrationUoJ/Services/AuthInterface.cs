namespace ExamRegistrationUoJ.Services
{
    public interface AuthInterface
    {
        Task<bool> IsAnAdministrator(string? email);
        Task<bool> IsACoordinator(string? email, string? nameidentifier);
        Task<bool> IsAStudent(string? email, string? nameidentifier);
        Task<bool> IsAnAdvisor(string? email);
        Task<bool> IsBothAdvisorCoordinator(string? email, string? nameidentifier);
    }
}
