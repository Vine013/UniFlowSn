using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UniFlowSn.Models.Db;

public partial class User
{
    public int Id { get; set; }

    [Display(Name = "E-mail")]
    public string Email { get; set; } = null!;

    [Display(Name = "Nome")]
    public string FirstName { get; set; } = null!;

    [Display(Name = "Sobrenome")]
    public string LastName { get; set; } = null!;

    [Display(Name = "Telefone")]
    public string Phone { get; set; } = null!;

    [Display(Name = "Endereço")]
    public string Address { get; set; } = null!;

    [Display(Name = "Admin")]
    public bool IsAdmin { get; set; }

    [Display(Name = "Data de Registro")]
    public DateTime? RegisterDate { get; set; }

    [Display(Name = "Token Recuperação")]
    public int? RecoveryCode { get; set; }

    [Display(Name = "Senha")]
    public string Password { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
