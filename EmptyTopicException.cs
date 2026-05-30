using System;

namespace DiplomaDefense.Core.Exceptions
{
    // кидаємо коли тема порожня
    public class EmptyTopicException : Exception
    {
        public EmptyTopicException()
            : base("тема проекту не може бути порожньою")
        {
        }
    }
}
