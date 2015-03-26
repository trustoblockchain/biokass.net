using System;
using System.ComponentModel.DataAnnotations;

namespace fingerprints_service.Models
{
    public class TokenForm
    {
        [Required]
        public string tokenid { get; set; }
    }
}
