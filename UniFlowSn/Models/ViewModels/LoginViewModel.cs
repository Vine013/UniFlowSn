using System.ComponentModel.DataAnnotations;

namespace UniFlowSn.Models.ViewModels
{
    public class LoginViewModel
    {
        [Display(Name = "E-mail")]
        [Required(ErrorMessage = "Campo obrigatório")]
        public string Email { get; set; }

        [Display(Name = "Senha")]
        [Required(ErrorMessage = "Campo obrigatório")]
        public string Password { get; set; }
    }
}
