using System;
using System.ComponentModel.DataAnnotations;

namespace Pets_UI.Mvc.Models
{
    public class AnimalToCare
    {
        public Guid Id { get; set; }
        public Animal Animal { get; set; }

        [DataType(DataType.Date, ErrorMessage="Date only")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date from")]
        public DateTime DateFrom { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Date only")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date to")]
        public DateTime DateTo { get; set; }

        [Display(Name = "Is taken")]
        public bool IsTaken { get; set; }
    }
}