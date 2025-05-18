using System.ComponentModel.DataAnnotations;

namespace UniFlowSn.Models.ViewModels
{
    public class RecoveryPasswordViewModel
    {
        [Display(Name = "E-mail")]
        [Required(ErrorMessage = "Campo obrigatório")]
        public string Email { get; set; }
    }
}
