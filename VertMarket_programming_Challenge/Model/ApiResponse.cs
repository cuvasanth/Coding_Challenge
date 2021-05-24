using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VertMarket_programming_Challenge.Model
{
   public class ApiResponse
    {
        public string token { get; set; }
        
        public bool Success { get; set; }
        
        public string Message { get; set; }
    }
}
