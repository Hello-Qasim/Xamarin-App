using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExpressTracketXamarin.Database
{
    public class Trip
    {
        private int id;
        private string name;
        private string destination;
        private string date;
        private int requiresAssessment;
        private string description = " ";
        private int daysSpent = 1;

        public Trip()
        {
        }

        public Trip(string name, string destination, string date, int requiresAssessment, string description, int daysSpent)
        {
            this.name = name;
            this.destination = destination;
            this.date = date;
            this.requiresAssessment = requiresAssessment;
            this.description = description;
            this.daysSpent = daysSpent;
        }

        public int Id { get { return id; } set { this.id = value; } }
        public string Name { get { return this.name; } set { this.name = value; } }
        public string Destination { get { return this.destination; } set { this.destination = value; } }
        public string Date { get { return this.date; } set { this.date = value; } }
        public int RequiresAssessment { get { return this.requiresAssessment; } set { this.requiresAssessment = value; } }
        public string Description { get { return this.description; } set { this.description = value; } }
        public int DaysSpent { get { return this.daysSpent; } set { this.daysSpent = value; } }
    }
}