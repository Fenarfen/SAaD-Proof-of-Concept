using System;
using System.Collections.Generic;

namespace AMLWebAplication.DTOs
{
    public class BorrowingStatsDto
    {
        public int TotalLoans { get; set; }
        public int UniqueUsers { get; set; }
        public int OverdueLoans { get; set; }
    }
}
