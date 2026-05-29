using System;

namespace DiplomaDefense.Core.Exceptions
{
    // кидаємо коли проекту з таким id нема
    public class ProjectNotFoundException : Exception
    {
        public ProjectNotFoundException(string id)
            : base("проект з id '" + id + "' не знайдено")
        {
        }
    }
}
