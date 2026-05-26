namespace DiplomaDefense.Core
{
    // всі числа і назви файлів в одному місці
    public static class Constants
    {
        // максимальні довжини
        public const int MaxTopicLength = 200;
        public const int MaxNameLength = 50;

        // обмеження
        public const int MaxStagesPerProject = 20;   // скільки етапів на проект
        public const int MaxProjectsPerReviewer = 3;  // скільки рецензій на одного рецензента
        public const int PassingGrade = 60;           // прохідний бал (шкала 0-100)

        // назви файлів
        public const string StudentsFile = "students.txt";
        public const string SupervisorsFile = "supervisors.txt";
        public const string ReviewersFile = "reviewers.txt";
        public const string ProjectsFile = "projects.txt";

        // коли рецензента ще нема
        public const string NotAssigned = "не призначено";
    }
}
