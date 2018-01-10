using System;

namespace Pets_UI.Mvc.Models
{
    public class AnimalToCare
    {
        public Guid Id { get; set; }
        public Animal Animal { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public bool IsTaken { get; set; }
    }
}