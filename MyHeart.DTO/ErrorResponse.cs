using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.DTO
{
    public class ErrorResponse
    {
        public string Title { get; set; }
        public Dictionary<string,string> Errors { get; set; }
    }
}