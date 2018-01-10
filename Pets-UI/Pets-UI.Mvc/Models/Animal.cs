using System;

namespace Pets_UI.Mvc.Models
{
    public class Animal
    {
        public Guid Id { get; set; }
        public User User { get; set; }
        public string Name { get; set; }
        public int YearOfBirth { get; set; }
    }
}