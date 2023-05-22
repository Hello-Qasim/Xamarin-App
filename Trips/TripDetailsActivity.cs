using Android.App;
using Android.Database;
using Android.Database.Sqlite;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using ExpressTracketXamarin.Database;
using ExpressTracketXamarin.Expenses;
using Newtonsoft.Json;
using System;
using Intent = Android.Content.Intent;

namespace ExpressTracketXamarin.Trips
{
    [Activity(Label = "TripDetailsActivity", Theme = "@style/AppTheme")]
    public class TripDetailsActivity : AppCompatActivity
    {
        private DatabaseHelper dbHelper;
        
        private Trip trip;
        Button btnEditTrip, btnDeleteTrip, btnAllExpenses;
        TextView textViewName, textViewDestination, textViewDate, titleRequiresAssessment, textViewDaysSpent, textViewDescription;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_trip_details);
            Title = "Trip Details";
            dbHelper = new DatabaseHelper(this, DatabaseHelper.DATABASE_NAME, null, 1);
            btnEditTrip = FindViewById<Button>(Resource.Id.btnEditTrip);
            btnDeleteTrip = FindViewById<Button>(Resource.Id.btnDeleteTrip);
            btnAllExpenses = FindViewById<Button>(Resource.Id.btnAllExpenses);

            textViewName = FindViewById<TextView>(Resource.Id.textViewName);
            textViewDestination = FindViewById<TextView>(Resource.Id.textViewDestination);
            textViewDate = FindViewById<TextView>(Resource.Id.textViewDate);
            titleRequiresAssessment = FindViewById<TextView>(Resource.Id.titleRequiresAssessment);
            textViewDaysSpent = FindViewById<TextView>(Resource.Id.textViewDaysSpent);
            textViewDescription = FindViewById<TextView>(Resource.Id.textViewDescription);
            

            trip = JsonConvert.DeserializeObject<Trip>(Intent.GetStringExtra(Constants.KEY_TRIP));
            if (trip != null)
            {
                updateUI(trip);
            }
            btnEditTrip.Click += (s, e) =>
            {
                openEditTripDetailsActivity();
            };

            btnDeleteTrip.Click += (s, e) =>
            {
                deleteTrip();
            };

            btnAllExpenses.Click += (s, e) =>
            {
                openAllExpensesActivity();
            };
        }

        private void openEditTripDetailsActivity()
        {
            if (trip == null) return;
            Intent intent = new Intent(this, typeof(AddTripActivity));
            intent.PutExtra(Constants.IS_ADD_TRIP_FLOW, false);
            intent.PutExtra(Constants.KEY_TRIP, JsonConvert.SerializeObject(trip));
            StartActivity(intent);
            Finish();
        }

        private void openAllExpensesActivity()
        {
            if (trip == null) return;
            Intent intent = new Intent(this, typeof(AllExpensesActivity));
            intent.PutExtra(Constants.KEY_TRIP_ID, trip.Id);
            StartActivity(intent);
            Finish();
        }

        //private Trip fetchTripDetails()
        //{
        //    Trip trip = new Trip();

        //    // Open the database
        //    SQLiteDatabase db = dbHelper.ReadableDatabase;

        //    // Define the columns you want to retrieve
        //    String[] projection = {
        //        DatabaseHelper.TRIP_ID_COLUMN,
        //        DatabaseHelper.TRIP_NAME_COLUMN,
        //        DatabaseHelper.TRIP_DESTINATION_COLUMN,
        //        DatabaseHelper.TRIP_REQUIRES_ASSESSMENT_COLUMN,
        //        DatabaseHelper.TRIP_DATE_COLUMN,
        //        DatabaseHelper.TRIP_DESCRIPTION_COLUMN,
        //        DatabaseHelper.TRIP_DAYS_SPENT_COLUMN
        //};

        //    // Define the selection criteria
        //    string selection = "tripId = ?";
        //    string[] selectionArgs = { tripId.ToString() };

        //    // Execute the query and get the results
        //    ICursor cursor = db.Query(
        //            DatabaseHelper.TBL_TRIPS,   // The table to query
        //            projection,    // The columns to retrieve
        //            selection,     // The columns for the WHERE clause
        //            selectionArgs, // The values for the WHERE clause
        //            null,          // Don't group the results
        //            null,          // Don't filter by row groups
        //            null           // The sort order
        //    );

        //    // Loop through the results and process each row
        //    if (cursor.MoveToFirst())
        //    {

        //        // Get the values from the cursor
        //        int id = cursor.GetInt(cursor.GetColumnIndexOrThrow(DatabaseHelper.TRIP_ID_COLUMN));
        //        String name = cursor.GetString(cursor.GetColumnIndexOrThrow(DatabaseHelper.TRIP_NAME_COLUMN));
        //        String destination = cursor.GetString(cursor.GetColumnIndexOrThrow(DatabaseHelper.TRIP_DESTINATION_COLUMN));
        //        int requiresAssessment = cursor.GetInt(cursor.GetColumnIndexOrThrow(DatabaseHelper.TRIP_REQUIRES_ASSESSMENT_COLUMN));
        //        String date = cursor.GetString(cursor.GetColumnIndexOrThrow(DatabaseHelper.TRIP_DATE_COLUMN));
        //        String description = cursor.GetString(cursor.GetColumnIndexOrThrow(DatabaseHelper.TRIP_DESCRIPTION_COLUMN));
        //        int days = cursor.GetInt(cursor.GetColumnIndexOrThrow(DatabaseHelper.TRIP_DAYS_SPENT_COLUMN));

        //        // Do something with the values
        //        trip.Id = id;
        //        trip.Name = name;
        //        trip.Destination = destination;
        //        trip.RequiresAssessment = requiresAssessment;
        //        trip.Date = date;
        //        trip.Description = description;
        //        trip.DaysSpent = days;
        //    }

        //    // Close the cursor and the database
        //    cursor.Close();
        //    db.Close();

        //    if (!string.IsNullOrEmpty(trip.Name))
        //    {
        //        return trip;
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}

        public void deleteTrip()
        {
            if (trip == null) return;

            SQLiteDatabase db = dbHelper.WritableDatabase;
            string selection = DatabaseHelper.TRIP_ID_COLUMN + " = ?";
            string[] selectionArgs = { trip.Id.ToString() };

            int deletedExpenseRowsCount = db.Delete(DatabaseHelper.TBL_EXPENSES, selection, selectionArgs);
            int deletedTripRowsCount = db.Delete(DatabaseHelper.TBL_TRIPS, selection, selectionArgs);
            db.Close();

            if (deletedTripRowsCount > 0 || deletedExpenseRowsCount > 0)
            {
                Toast.MakeText(this, "Record deleted successfully!", ToastLength.Short).Show();
                openAllTripsActivity();
            }
        }

        private void openAllTripsActivity()
        {
            Intent intent = new Intent(this, typeof(AllTripsActivity));
            StartActivity(intent);
            Finish();
        }

        private void updateUI(Trip trip)
        {
            int requiresAssessmentFlag = trip.RequiresAssessment;
            String requiresAssessmentStatus = Utils.GetRequiresAssessmentStatus(requiresAssessmentFlag);
            String description = trip.Description;

           textViewName.Text="Name: " + trip.Name;
           textViewDestination.Text="Destination: " + trip.Destination;
           textViewDate.Text="Date: " + trip.Date;
           titleRequiresAssessment.Text="Requires Assessment: " + requiresAssessmentStatus;
           textViewDaysSpent.Text="Days Spent: " + trip.DaysSpent;

            if (!string.IsNullOrEmpty(description))
            {
                textViewDescription.Visibility = ViewStates.Visible;
                textViewDescription.Text="Description: " + trip.Description;
            }
        }
    }
}