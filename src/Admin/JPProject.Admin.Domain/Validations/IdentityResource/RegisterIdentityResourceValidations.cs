using JPProject.Admin.Domain.Commands.IdentityResource;

namespace JPProject.Admin.Domain.Validations.IdentityResource
{
    public class RegisterIdentityResourceCommandValidation : IdentityResourceValidation<RegisterIdentityResourceCommand>
    {
        public RegisterIdentityResourceCommandValidation()
        {
            ValidateName();
        }
    }
}