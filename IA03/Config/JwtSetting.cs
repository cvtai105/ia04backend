using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IA03.Config
{
    public class JwtSettings
    {
        public string? Secret { get; set; } 
        public string? Issuer { get; set; }
        public string? Audience { get; set; }
    }
}