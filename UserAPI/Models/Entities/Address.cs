﻿namespace UserAPI.Models.Entities
{
    public class Address
    {
        public int ID { get; set; }
        public int AccountID { get; set; }
        public required string FirstLine { get; set; }
        public string? SecondLine { get; set; }
        public string? ThirdLine { get; set; }
        public string? FourthLine { get; set; }
        public required string City { get; set; }
        public string? County { get; set; }
        public string? Country { get; set; }
        public required string PostCode { get; set; }
        public required bool IsDefault { get; set; }
    }
}
