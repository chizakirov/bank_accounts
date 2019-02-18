using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace bankaccounts.Models
{
  public class Transaction
  {
    [Key]
    public int TransactionId {get;set;}

    [Required]
    public decimal Amount {get; set;}

    public DateTime CreatedAt {get;set;} = DateTime.Now;
    public DateTime UpdatedAt {get;set;} = DateTime.Now;

    [ForeignKey("UserId")]
    public int UserId {get;set;}
    public User Owner {get;set;}
    public Transaction(){
    }

  }
}