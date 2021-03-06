using IdentityServer4.Models;
using JPProject.Domain.Core.Commands;

namespace JPProject.Admin.Domain.Commands.Clients
{
    public abstract class ClientCommand : Command
    {
        public Client Client { get; set; }
        public string OldClientId { get; protected set; }
    }
}