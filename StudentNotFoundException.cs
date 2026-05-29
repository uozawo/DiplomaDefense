using System;

namespace DiplomaDefense.Core.Exceptions
{
    // кидаємо коли студента з таким id нема
    public class StudentNotFoundException : Exception
    {
        public StudentNotFoundException(string id)
            : base("студента з id '" + id + "' не знайдено")
        {
        }
    }
}
