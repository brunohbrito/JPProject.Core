using JPProject.Admin.Domain.Commands.Clients;

namespace JPProject.Admin.Domain.Validations.Client
{
    public class SaveClientClaimCommandValidation : ClientClaimValidation<SaveClientClaimCommand>
    {
        public SaveClientClaimCommandValidation()
        {
            ValidateClientId();
            ValidateKey();
            ValidateValue();
        }
    }
}