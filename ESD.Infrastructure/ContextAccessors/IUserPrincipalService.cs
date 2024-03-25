namespace ESD.Infrastructure.ContextAccessors
{
    public interface IUserPrincipalService
    {
        bool IsAuthenticated { get; }
        int UserId { get; }
        string UserName { get; }
        string AccessToken { get; }
        string Email { get; }
        string FullName { get; }
        bool IsSuperUser { get; }
        string IpAddress { get; }
        int IDOrgan { get; }
        int IDAgency { get; }
        bool AddUpdateClaim(string key, string value);
    }
}
