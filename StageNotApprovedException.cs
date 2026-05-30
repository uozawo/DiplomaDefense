using System;

namespace DiplomaDefense.Core.Exceptions
{
    // кидаємо коли хочуть на захист а етапи ще не готові
    public class StageNotApprovedException : Exception
    {
        public StageNotApprovedException(string projectId)
            : base("проект '" + projectId + "' ще не готовий: не всі етапи прийняті або нема рецензента")
        {
        }
    }
}
