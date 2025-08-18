using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTourGuide.Domain.Entities
{
    public class Photo
    {
        public int Id { get; set; }
        public string Url { get; set; } = default!;
        public int PlaceId { get; set; }
        public Place Place { get; set; } = default!;
    }
}

