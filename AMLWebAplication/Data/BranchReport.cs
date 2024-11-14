namespace AMLWebAplication.Data
{
    public class BranchReport
    {
        public int BranchId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public BorrowingStats BorrowingStats { get; set; }
        public IEnumerable<PopularMediaItem> PopularItems { get; set; }
    }
}