namespace ReportAPI.Models
{    
    public class Media
    {
        public int ID { get; set; }
        public int BranchID { get; set; }
        public string Title { get; set; }
        public string Released { get; set; }
        public string Author { get; set; }
        public string Type { get; set; }
    }
}