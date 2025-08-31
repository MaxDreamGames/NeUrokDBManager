namespace NeUrokDBManager.Core.DTOs
{
    public class ClientSearchDTO
    {
        public int UserId { get; set; }
        public string? StudentName { get; set; }
        public int BirthdayDay { get; set; }
        public int BirthdayMonth { get; set; }
        public int BirthdayYear { get; set; }
        public int RegistrationDateDay { get; set; }
        public int RegistrationDateMonth { get; set; }
        public int RegistrationDateYear { get; set; }
        public int? Class { get; set; }
        public string? Courses { get; set; }
        public string? ParentName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? AnotherPhoneNumber { get; set; }
        public string? Comments { get; set; }
    }
}
