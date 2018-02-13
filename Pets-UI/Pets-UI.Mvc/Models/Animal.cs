using System;
using System.ComponentModel.DataAnnotations;

namespace Pets_UI.Mvc.Models
{
    public class Animal
    {
        public Guid Id { get; set; }
        public User User { get; set; }

        [Display(Name = "Animal name")]
        public string Name { get; set; }

        [Display(Name = "Year of birth")]
        public int YearOfBirth { get; set; }
    }
}