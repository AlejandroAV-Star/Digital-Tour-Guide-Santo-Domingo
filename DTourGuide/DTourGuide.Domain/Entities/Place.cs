using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace DTourGuide.Domain.Entities
{
    public class Place
    {
        public int Id { get; set; }
        [Required, MaxLength(120)]
        public string Name { get; set; } = default!;
        [MaxLength(2000)] public string? Description { get; set; }
        public string? Address { get; set; }
        [MaxLength(200)] public string? OpeningHours { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public Category Category { get; set; }

        public ICollection<Photo> Photos { get; set; } = new List<Photo>();
    }
}

