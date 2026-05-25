using System;

namespace DiplomaDefense.Core
{
    // головний клас для людей в системі
    // студент, керівник і рецензент беруть звідси спільні поля
    public abstract class Person
    {
        private string id;
        private string firstName;
        private string lastName;

        public string Id
        {
            get { return id; }
        }

        public string FirstName
        {
            get { return firstName; }
            set
            {
                // не дозволяємо пусте ім'я
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("ім'я не може бути порожнім");
                firstName = value;
            }
        }

        public string LastName
        {
            get { return lastName; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("прізвище не може бути порожнім");
                lastName = value;
            }
        }

        // щоб зручно показати прізвище та ім'я разом
        public string FullName
        {
            get { return lastName + " " + firstName; }
        }

        // звичайний конструктор
        public Person(string id, string firstName, string lastName)
        {
            this.id = id;
            FirstName = firstName;
            LastName = lastName;
        }

        // конструктор без даних
        public Person()
        {
            this.id = Guid.NewGuid().ToString().Substring(0, 8);
            this.firstName = "невідомо";
            this.lastName = "невідомо";
        }

        // кожен хто наслідує мусить сказати свою роль
        public abstract string GetRole();

        // і коротко описати себе
        public abstract string GetDisplayInfo();

        public override string ToString()
        {
            return FullName + " (" + GetRole() + ", ID: " + id + ")";
        }
    }
}
