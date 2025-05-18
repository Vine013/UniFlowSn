using System.ComponentModel.DataAnnotations;

namespace UniFlowSn.Models.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Display(Name = "E-mail")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Código de Recuperação")]
        public int? RecoveryCode { get; set; }

        [Required]
        [Display(Name = "Nova Senha")]
        public string NewPassword { get; set; }
    }
}
