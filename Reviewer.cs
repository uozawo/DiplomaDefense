using System;
using System.Collections.Generic;

namespace DiplomaDefense.Core
{
    // рецензент. дивиться чужі проекти
    // не можна давати йому забагато проектів одразу
    public class Reviewer : Person
    {
        public string Organization { get; set; }    // звідки він
        private List<string> projects;               // список id проектів які він взяв

        // віддаємо копію списку щоб ззовні не зіпсували
        public List<string> AssignedProjects
        {
            get { return new List<string>(projects); }
        }

        public Reviewer(string id, string firstName, string lastName, string organization)
            : base(id, firstName, lastName)
        {
            Organization = organization;
            projects = new List<string>();
        }

        public Reviewer() : base()
        {
            Organization = "—";
            projects = new List<string>();
        }

        public override string GetRole()
        {
            return "рецензент";
        }

        public override string GetDisplayInfo()
        {
            return FullName + " (" + Organization + ") — рецензій: " + projects.Count;
        }

        // дати рецензенту ще один проект
        public void AssignProject(string projectId)
        {
            if (!projects.Contains(projectId))
                projects.Add(projectId);
        }

        // скільки проектів зараз у нього
        public int GetActiveCount()
        {
            return projects.Count;
        }

        // чи може взяти ще один (ліміт лежить в Constants)
        public bool CanTakeMore()
        {
            return projects.Count < Constants.MaxProjectsPerReviewer;
        }

        public string ToFileString()
        {
            return Id + ";" + FirstName + ";" + LastName + ";" + Organization;
        }
    }
}
