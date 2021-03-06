using System.ComponentModel.DataAnnotations;

namespace JPProject.Admin.Application.ViewModels.ClientsViewModels
{
    public class RemoveClientClaimViewModel
    {
        public RemoveClientClaimViewModel(string clientId, string type, string value)
        {
            ClientId = clientId;
            Type = type;
            Value = value;
        }

        [Required]
        public string ClientId { get; set; }

        [Required]
        public string Type { get; set; }
        public string Value { get; }
    }
}