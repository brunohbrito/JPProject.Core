using JPProject.Admin.Domain.Commands.Clients;
using JPProject.Admin.Domain.Events.Client;
using JPProject.Admin.Domain.Interfaces;
using JPProject.Domain.Core.Bus;
using JPProject.Domain.Core.Commands;
using JPProject.Domain.Core.Interfaces;
using JPProject.Domain.Core.Notifications;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JPProject.Admin.Domain.CommandHandlers
{
    public class ClientCommandHandler : CommandHandler,
        IRequestHandler<RemoveClientCommand, bool>,
        IRequestHandler<UpdateClientCommand, bool>,
        IRequestHandler<RemoveClientSecretCommand, bool>,
        IRequestHandler<SaveClientSecretCommand, bool>,
        IRequestHandler<RemovePropertyCommand, bool>,
        IRequestHandler<SaveClientPropertyCommand, bool>,
        IRequestHandler<RemoveClientClaimCommand, bool>,
        IRequestHandler<SaveClientClaimCommand, bool>,
        IRequestHandler<SaveClientCommand, bool>,
        IRequestHandler<CopyClientCommand, bool>
    {
        private readonly IClientRepository _clientRepository;

        public ClientCommandHandler(
            IUnitOfWork uow,
            IMediatorHandler bus,
            INotificationHandler<DomainNotification> notifications,
            IClientRepository clientRepository) : base(uow, bus, notifications)
        {
            _clientRepository = clientRepository;
        }


        public async Task<bool> Handle(RemoveClientCommand request, CancellationToken cancellationToken)
        {
            if (!request.IsValid())
            {
                NotifyValidationErrors(request);
                return false; ;
            }

            var savedClient = await _clientRepository.GetByClientId(request.Client.ClientId);
            if (savedClient == null)
            {
                await Bus.RaiseEvent(new DomainNotification("Client", "Client not found"));
                return false;
            }
            _clientRepository.Remove(savedClient);
            if (await Commit())
            {
                await Bus.RaiseEvent(new ClientRemovedEvent(request.Client.ClientId));
                return true;
            }
            return false;
        }

        public async Task<bool> Handle(UpdateClientCommand request, CancellationToken cancellationToken)
        {
            if (!request.IsValid())
            {
                NotifyValidationErrors(request);
                return false;
            }

            var savedClient = await _clientRepository.GetByClientId(request.OldClientId);
            if (savedClient == null)
            {
                await Bus.RaiseEvent(new DomainNotification("Client", "Client not found"));
                return false;
            }

            await _clientRepository.UpdateWithChildrens(request.OldClientId, request.Client);

            if (await Commit())
            {
                await Bus.RaiseEvent(new ClientUpdatedEvent(request));
                return true;
            }
            return false;

        }

        public async Task<bool> Handle(RemoveClientSecretCommand request, CancellationToken cancellationToken)
        {
            if (!request.IsValid())
            {
                NotifyValidationErrors(request);
                return false;
            }

            var savedClient = await _clientRepository.GetClient(request.ClientId);
            if (savedClient == null)
            {
                await Bus.RaiseEvent(new DomainNotification("Client", "Client not found"));
                return false;
            }

            if (!savedClient.ClientSecrets.Any(f => f.Type == request.Type && f.Value == request.Value))
            {
                await Bus.RaiseEvent(new DomainNotification("Client Secret", "Invalid secret"));
                return false;
            }

            await _clientRepository.RemoveSecret(request.ClientId, request.ToModel());

            if (await Commit())
            {
                await Bus.RaiseEvent(new ClientSecretRemovedEvent(request.Type, request.ClientId));
                return true;
            }
            return false;
        }

        public async Task<bool> Handle(SaveClientSecretCommand request, CancellationToken cancellationToken)
        {
            if (!request.IsValid())
            {
                NotifyValidationErrors(request);
                return false;
            }

            var savedClient = await _clientRepository.GetByClientId(request.ClientId);
            if (savedClient == null)
            {
                await Bus.RaiseEvent(new DomainNotification("Client", "Client not found"));
                return false;
            }

            var secret = request.ToEntity();

            await _clientRepository.AddSecret(request.ClientId, secret);

            if (await Commit())
            {
                await Bus.RaiseEvent(new NewClientSecretEvent(request.ClientId, secret.Type, secret.Description));
                return true;
            }
            return false;
        }

        public async Task<bool> Handle(RemovePropertyCommand request, CancellationToken cancellationToken)
        {
            if (!request.IsValid())
            {
                NotifyValidationErrors(request);
                return false;
            }

            var savedClient = await _clientRepository.GetClient(request.ClientId);
            if (savedClient == null)
            {
                await Bus.RaiseEvent(new DomainNotification("Client", "Client not found"));
                return false;
            }

            if (savedClient.Properties.All(f => f.Key != request.Key))
            {
                await Bus.RaiseEvent(new DomainNotification("Client Properties", "Invalid Property"));
                return false;
            }

            await _clientRepository.RemoveProperty(request.ClientId, request.Key, request.Value);

            if (await Commit())
            {
                await Bus.RaiseEvent(new ClientPropertyRemovedEvent(request.Key, request.Value, request.ClientId));
                return true;
            }
            return false;
        }

        public async Task<bool> Handle(SaveClientPropertyCommand request, CancellationToken cancellationToken)
        {
            if (!request.IsValid())
            {
                NotifyValidationErrors(request);
                return false;
            }

            var savedClient = await _clientRepository.GetByClientId(request.ClientId);
            if (savedClient == null)
            {
                await Bus.RaiseEvent(new DomainNotification("Client", "Client not found"));
                return false;
            }

            await _clientRepository.AddProperty(savedClient.ClientId, request.Key, request.Value);

            if (await Commit())
            {
                await Bus.RaiseEvent(new NewClientPropertyEvent(request.ClientId, request.Key, request.Value));
                return true;
            }
            return false;
        }


        public async Task<bool> Handle(RemoveClientClaimCommand request, CancellationToken cancellationToken)
        {
            if (!request.IsValid())
            {
                NotifyValidationErrors(request);
                return false;
            }

            var savedClient = await _clientRepository.GetClient(request.ClientId);
            if (savedClient == null)
            {
                await Bus.RaiseEvent(new DomainNotification("Client", "Client not found"));
                return false;
            }

            if (savedClient.Claims.All(f => f.Type != request.Type))
            {
                await Bus.RaiseEvent(new DomainNotification("Client Claims", "Invalid Claim"));
                return false;
            }

            if (request.ByType())
                await _clientRepository.RemoveClaim(request.ClientId, request.Type);
            else
                await _clientRepository.RemoveClaim(request.ClientId, request.Type, request.Value);

            if (await Commit())
            {
                await Bus.RaiseEvent(new ClientClaimRemovedEvent(request.Type, request.Value, request.ClientId));
                return true;
            }
            return false;
        }

        public async Task<bool> Handle(SaveClientClaimCommand request, CancellationToken cancellationToken)
        {
            if (!request.IsValid())
            {
                NotifyValidationErrors(request);
                return false;
            }

            var savedClient = await _clientRepository.GetByClientId(request.ClientId);
            if (savedClient == null)
            {
                await Bus.RaiseEvent(new DomainNotification("Client", "Client not found"));
                return false;
            }

            var claim = request.ToEntity();

            await _clientRepository.AddClaim(request.ClientId, claim);

            if (await Commit())
            {
                await Bus.RaiseEvent(new NewClientClaimEvent(request.ClientId, claim.Type, claim.Value));
                return true;
            }
            return false;
        }

        public async Task<bool> Handle(SaveClientCommand request, CancellationToken cancellationToken)
        {
            if (!request.IsValid())
            {
                NotifyValidationErrors(request);
                return false;
            }

            var savedClient = await _clientRepository.GetByClientId(request.Client.ClientId);
            if (savedClient != null)
            {
                await Bus.RaiseEvent(new DomainNotification("Client", "Client already exists"));
                return false;
            }

            _clientRepository.Add(request.ToModel());

            if (await Commit())
            {
                await Bus.RaiseEvent(new NewClientEvent(request.Client));
                return true;
            }

            return false;
        }


        public async Task<bool> Handle(CopyClientCommand request, CancellationToken cancellationToken)
        {
            if (!request.IsValid())
            {
                NotifyValidationErrors(request);
                return false;
            }

            var savedClient = await _clientRepository.GetClient(request.Client.ClientId);
            if (savedClient == null)
            {
                await Bus.RaiseEvent(new DomainNotification("Client", "Client not found"));
                return false;
            }

            var copyOf = savedClient;

            copyOf.ClientId = $"copy-of-{copyOf.ClientId}-{Guid.NewGuid().ToString().Replace("-", string.Empty)}";
            copyOf.ClientSecrets = new List<IdentityServer4.Models.Secret>();
            copyOf.ClientName = "Copy of " + copyOf.ClientName;

            _clientRepository.Add(copyOf);

            if (await Commit())
            {
                await Bus.RaiseEvent(new ClientClonedEvent(request.Client.ClientId, copyOf.ClientId));
                await Bus.RaiseEvent(new NewClientEvent(copyOf));
                return true;
            }

            return false;
        }
    }


}