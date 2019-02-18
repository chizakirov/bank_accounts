using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace bankaccounts.Models
{
  public class LoginUser{

    [Required]
    [EmailAddress]
    public string Email {get;set;}
    
    [Required]
    [MinLength(8, ErrorMessage = "Password must be 8 characters long"), DataType(DataType.Password)]
    public string Password {get;set;}
  }
}