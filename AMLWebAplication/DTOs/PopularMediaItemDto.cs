using System;
using System.Collections.Generic;

namespace AMLWebAplication.DTOs
{
    public class PopularMediaItemDto
    {
        public int MediaID { get; set; }
        public string Title { get; set; }
        public int LoanCount { get; set; }
    }
}
