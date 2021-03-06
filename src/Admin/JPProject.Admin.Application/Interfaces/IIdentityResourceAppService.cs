using IdentityServer4.Models;
using JPProject.Admin.Application.ViewModels.IdentityResourceViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JPProject.Admin.Application.Interfaces
{
    public interface IIdentityResourceAppService : IDisposable
    {
        Task<IEnumerable<IdentityResourceListView>> GetIdentityResources();
        Task<IdentityResource> GetDetails(string name);
        Task Save(IdentityResource model);
        Task Update(string resource, IdentityResource model);
        Task Remove(RemoveIdentityResourceViewModel model);
    }
}