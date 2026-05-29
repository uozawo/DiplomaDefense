using System;

namespace DiplomaDefense.Core.Exceptions
{
    // кидаємо коли такий студент вже є
    public class DuplicateStudentException : Exception
    {
        public DuplicateStudentException(string id)
            : base("студент з id '" + id + "' вже доданий")
        {
        }
    }
}
