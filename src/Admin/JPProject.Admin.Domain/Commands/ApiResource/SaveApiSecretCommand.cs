using FluentValidation.Results;
using IdentityModel;
using IdentityServer4.Models;
using JPProject.Admin.Domain.Commands.Clients;
using JPProject.Admin.Domain.Validations.ApiResource;
using System;

namespace JPProject.Admin.Domain.Commands.ApiResource
{
    public class SaveApiSecretCommand : ApiSecretCommand
    {

        public SaveApiSecretCommand(string resourceName, string description, string value, string type, DateTime? expiration,
            int hashType)
        {
            ResourceName = resourceName;
            Description = description;
            Value = value;
            Type = type;
            Expiration = expiration;
            Hash = hashType;
        }
        public override bool IsValid()
        {
            ValidationResult = new SaveApiSecretCommandValidation().Validate(this);
            return ValidationResult.IsValid;
        }

        public string GetValue()
        {
            switch (Hash)
            {
                case 0:
                    return Value.ToSha256();
                case 1:
                    return Value.ToSha512();
                default:
                    throw new ArgumentException(nameof(Hash));
            }
        }

        public Secret ToModel()
        {
            return new Secret
            {
                Description = Description,
                Expiration = Expiration,
                Type = Type,
                Value = GetValue()
            };
        }
    }
}