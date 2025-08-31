namespace NeUrokDBManager.Core.DTOs
{
    public class ClientDTO
    {
        public int UserId { get; set; }
        public string? StudentName { get; set; }
        public DateTime? Birthday { get; set; }
        public DateTime RegistrationDate { get; set; }
        public int? Class { get; set; }
        public string? Courses { get; set; }
        public string? ParentName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? AnotherPhoneNumber { get; set; }
        public string? Comments { get; set; }
    }
}
