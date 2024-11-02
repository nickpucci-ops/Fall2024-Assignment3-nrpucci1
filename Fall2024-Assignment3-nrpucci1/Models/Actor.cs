using System.ComponentModel.DataAnnotations;
using System;

namespace Fall2024_Assignment3_nrpucci1.Models;
public class Actor
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    public string? Gender { get; set; }
    public int Age { get; set; }
    public string? ImdbLink { get; set; }
    public string? PhotoURL   { get; set; }

}

