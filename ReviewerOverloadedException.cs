using System;

namespace DiplomaDefense.Core.Exceptions
{
    // кидаємо коли всі рецензенти зайняті
    public class ReviewerOverloadedException : Exception
    {
        public ReviewerOverloadedException()
            : base("немає вільного рецензента, всі зайняті")
        {
        }
    }
}
