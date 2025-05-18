using System.ComponentModel.DataAnnotations;

namespace UniFlowSn.Models.ViewModels
{
    public class TicketViewModel
    {
        [Display(Name = "Nome do Evento")]
        public string Event { get; set; }

        [Display(Name = "Data de Início")]
        public DateTime? DtStart { get; set; }

        [Display(Name = "Local do Evento")]
        public string? Place { get; set; }

        [Display(Name = "Participante")]
        public string User { get; set; }

        [Display(Name = "E-mail")]
        public string Email { get; set; }

        [Display(Name = "Data da Inscrição")]
        public DateTime CreateDate { get; set; }

        [Display(Name = "Código do Ingresso")]
        public string Id { get; set; }
    }
}
