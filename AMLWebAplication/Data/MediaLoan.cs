namespace AMLWebAplication.Data
{
    public class MediaLoan
    {
        public int ID { get; set; }
        public int MediaID { get; set; }
        public int AccountID { get; set; }
        public int BranchID { get; set; }
        public DateTime LoanedDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnedDate { get; set; }
        public string Status { get; set; }
    }

}
