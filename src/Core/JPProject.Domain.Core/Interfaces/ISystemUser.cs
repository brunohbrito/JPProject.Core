using System.Collections.Generic;
using System.Security.Claims;

namespace JPProject.Domain.Core.Interfaces
{
    public interface ISystemUser
    {
        string Username { get; }
        bool IsAuthenticated();
        bool IsInRole(string role);
        IEnumerable<Claim> GetClaimsIdentity();
        string GetRemoteIpAddress();
        string GetLocalIpAddress();
    }
}
