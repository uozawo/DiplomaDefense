using System;

namespace DiplomaDefense.Core
{
    // науковий керівник
    public class Supervisor : Person
    {
        public string AcademicTitle { get; set; }  // звання, наприклад доцент
        public string Department { get; set; }      // кафедра

        public Supervisor(string id, string firstName, string lastName,
                          string academicTitle, string department)
            : base(id, firstName, lastName)
        {
            AcademicTitle = academicTitle;
            Department = department;
        }

        public Supervisor() : base()
        {
            AcademicTitle = "—";
            Department = "—";
        }

        public override string GetRole()
        {
            return "керівник";
        }

        public override string GetDisplayInfo()
        {
            return AcademicTitle + " " + FullName + " | кафедра: " + Department;
        }

        public string ToFileString()
        {
            return Id + ";" + FirstName + ";" + LastName + ";" + AcademicTitle + ";" + Department;
        }
    }
}
