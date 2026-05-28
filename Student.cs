using System;

namespace DiplomaDefense.Core
{
    // студент який пише диплом
    public class Student : Person
    {
        public string Group { get; set; }       // група
        public string RecordBook { get; set; }   // номер залікової книжки
        public string ProjectId { get; set; }    // id його проекту

        public Student(string id, string firstName, string lastName,
                       string group, string recordBook)
            : base(id, firstName, lastName)
        {
            Group = group;
            RecordBook = recordBook;
        }

        public Student() : base()
        {
            Group = "—";
            RecordBook = "—";
        }

        public override string GetRole()
        {
            return "студент";
        }

        public override string GetDisplayInfo()
        {
            return FullName + " | група: " + Group + " | залікова: " + RecordBook;
        }

        // перевіряємо чи є в студента проект
        public bool HasProject()
        {
            return !string.IsNullOrWhiteSpace(ProjectId);
        }

        // робимо один рядок щоб записати у файл
        public string ToFileString()
        {
            return Id + ";" + FirstName + ";" + LastName + ";" + Group + ";" + RecordBook;
        }

        // порівнюємо двох студентів по id
        public static bool operator ==(Student a, Student b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (a is null || b is null) return false;
            return a.Id == b.Id;
        }

        public static bool operator !=(Student a, Student b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            return obj is Student other && this == other;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
