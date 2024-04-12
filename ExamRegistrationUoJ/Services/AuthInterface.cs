namespace ExamRegistrationUoJ.Services
{
    public interface AuthInterface
    {
        Task<bool> IsAnAdministrator(string? nameidentifier);
        Task<bool> IsACoordinator(string? nameidentifier);
        Task<bool> IsAStudent(string? email, string? nameidentifier);
        Task<bool> IsAnAdvisor(string? nameidentifier);
        Task<bool> IsBothAdvisorCoordinator(string? nameidentifier);
    }
}
