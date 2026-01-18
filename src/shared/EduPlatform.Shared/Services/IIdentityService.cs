namespace EduPlatform.Shared.Services;

public interface IIdentityService
{
    public Guid UserId { get;  }
    public string UserName { get; }
    public List<string> Roles { get; }
}