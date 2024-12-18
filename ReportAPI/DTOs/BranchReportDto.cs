namespace ReportAPI.DTOs
{
    public class BranchReportDto
    {
        public int BranchId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public BorrowingStatsDto BorrowingStats { get; set; }
        public IEnumerable<PopularMediaItemDto> PopularItems { get; set; }
    }
}
