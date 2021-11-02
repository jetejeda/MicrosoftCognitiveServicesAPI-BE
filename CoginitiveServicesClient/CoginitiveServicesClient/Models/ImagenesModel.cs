using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CoginitiveServicesClient.Models
{
    public class ImagenesModel
    {
        public int Id { get; set; }
        public byte[] ImageData { get; set; }
    }
}
