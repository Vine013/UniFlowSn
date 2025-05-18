using System.ComponentModel.DataAnnotations;

namespace UniFlowSn.Models.ViewModels
{
    public class InscriptionViewModel
    {
        [Required(ErrorMessage = "O ID do evento é obrigatório.")]
        public int EventId { get; set; }

        public int? UserId { get; set; }

        [Required(ErrorMessage = "O Nome completo é obrigatório.")]
        [Display(Name = "Nome Completo")]
        public string Name { get; set; }

        [Required(ErrorMessage = "O E-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "Informe um E-mail válido.")]
        [Display(Name = "E-mail")]
        public string Email { get; set; }

        [Display(Name = "Telefone")]
        public string? Phone { get; set; }

        [Display(Name = "Endereço")]
        public string? Address { get; set; }
    }
}
