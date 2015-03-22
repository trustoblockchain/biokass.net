using System;
using System.ComponentModel.DataAnnotations;

namespace fingerprints_service.Models
{
    public class HardwareSessionForm
    {
        [Required]
        public string hsid { get; set; }
    }
}
