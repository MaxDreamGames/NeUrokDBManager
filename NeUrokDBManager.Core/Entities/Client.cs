namespace NeUrokDBManager.Core.Entities
{
    public class Client
    {
        public Guid Id { get; set; }
        public int UserId { get; set; }
        public string StudentName { get; set; }
        public DateTime? Birthday { get; set; }
        public DateTime RegistrationDate { get; set; }
        public int Class { get; set; }
        public string? Courses { get; set; }
        public string ParentName { get; set; }
        public string PhoneNumber { get; set; }
        public string? AnotherPhoneNumber { get; set; }
        public string? Comments { get; set; }

        public ClientColor? ClientColor { get; set; }

#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Рассмотрите возможность добавления модификатора "required" или объявления значения, допускающего значение NULL.
        protected Client() { }
#pragma warning restore CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Рассмотрите возможность добавления модификатора "required" или объявления значения, допускающего значение NULL.

        public Client(int userId,
                      string? studentName,
                      DateTime? birthday,
                      DateTime? registrationDate,
                      int class_num,
                      string? courses,
                      string? parentName,
                      string? phoneNumber,
                      string? anotherPhoneNumber,
                      string? comments)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            StudentName = studentName ?? throw new ArgumentNullException();
            Birthday = birthday;
            RegistrationDate = registrationDate ?? DateTime.UtcNow;
            Class = class_num;
            Courses = courses;
            ParentName = parentName ?? throw new ArgumentNullException();
            PhoneNumber = phoneNumber ?? throw new ArgumentNullException();
            AnotherPhoneNumber = anotherPhoneNumber;
            Comments = comments;
        }

    }
}