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
    public  class Expense
    {
        
        private string type;
        private int amount;
        private string date;
        private string comments;
        private int tripId = 0;

        public Expense(string type, int amount, string date, string comments, int tripId)
        {
            this.type = type;
            this.amount = amount;
            this.date = date;
            this.comments = comments;
            this.tripId = tripId;
        }

        
        public string Type { get { return type; } set { this.type = value; } }
        public int Amount { get { return amount; } set { this.amount = value; } }
        public string Date { get { return this.date; } set { this.date= value; } }
        public string Comments { get { return comments; } set { this.comments = value; } }
        public int TripId => tripId;
        
    }
}