namespace AMLWebAplication.Data
{
    public class MediaTransfer
    {
        public int ID { get; set; }
        public int MediaID { get; set; }
        public int OriginBranchID { get; set; }
        public string DestinationBranchID { get; set; }
        public string AccountID { get; set; }
        public string Approved { get; set; }
        public string Created { get; set; }
        public string Completed { get; set; }
    }

}
