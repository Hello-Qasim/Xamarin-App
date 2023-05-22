using Android.App;
using Android.Database;
using Android.Database.Sqlite;
using Android.OS;
using Android.Text;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.RecyclerView.Widget;
using ExpressTracketXamarin.Database;
using Newtonsoft.Json;
using System.Collections.Generic;
using AlertDialog = Android.App.AlertDialog;
using Intent = Android.Content.Intent;

namespace ExpressTracketXamarin.Trips
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class AllTripsActivity : AppCompatActivity
    {
        private DatabaseHelper dbHelper;
        private TripsAdapter adapter;
        private List<Trip> trips = new List<Trip>();
        EditText editTextSearch;
        Google.Android.Material.FloatingActionButton.FloatingActionButton btnAddTrip;
        RecyclerView recyclerView;
        TextView textViewNoTrips;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_all_trips);
            Title = "All Trips";
            dbHelper = new DatabaseHelper(this, DatabaseHelper.DATABASE_NAME, null, 1);
            editTextSearch = FindViewById<EditText>(Resource.Id.editTextSearch);
            btnAddTrip = FindViewById<Google.Android.Material.FloatingActionButton.FloatingActionButton>(Resource.Id.btnAddTrip);
            recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
            textViewNoTrips = FindViewById<TextView>(Resource.Id.textViewNoTrips);

            editTextSearch.AfterTextChanged += EditTextSearch_AfterTextChanged;
            btnAddTrip.Click += (s, e) =>
            {
                Intent intent = new Intent(this, typeof(AddTripActivity));
                StartActivity(intent);
                Finish();
            };
        }

        private void EditTextSearch_AfterTextChanged(object sender, AfterTextChangedEventArgs e)
        {
            filter(editTextSearch.Text);
        }

        private void initRecyclerView(List<Trip> trip)
        {
            adapter = new TripsAdapter(trip, OnTripItemClick);
            recyclerView.SetAdapter(adapter);
            recyclerView.SetLayoutManager(new LinearLayoutManager(this));

        }
        void OnTripItemClick(Trip trip)
        {
            Intent intent = new Intent(this, typeof(TripDetailsActivity));
            intent.PutExtra(Constants.KEY_TRIP, JsonConvert.SerializeObject(trip));
            StartActivity(intent);
            Finish();
        }

        private void filter(string text)
        {
            List<Trip> filteredList = new List<Trip>();
            foreach (Trip trip in trips)
            {
                if (trip.Name.ToLower().Contains(text.ToLower()) ||
                        trip.Destination.ToLower().Contains(text.ToLower()) ||
                        trip.Date.ToLower().Contains(text.ToLower()))
                {
                    filteredList.Add(trip);
                }
            }
            initRecyclerView(filteredList);

            if (filteredList.Count == 0)
            {
                textViewNoTrips.Visibility = ViewStates.Visible;
            }
            else
            {
                textViewNoTrips.Visibility = ViewStates.Invisible;
            }
        }

        private void fetchTrips()
        {
            trips.Clear();

            // Create and/or open a database to read from it
            SQLiteDatabase db = dbHelper.ReadableDatabase;

            // Define a projection that specifies which columns from the database you will actually use after this query.
            string[] projection = {
                DatabaseHelper.TRIP_ID_COLUMN,
                DatabaseHelper.TRIP_NAME_COLUMN,
                DatabaseHelper.TRIP_DESTINATION_COLUMN,
                DatabaseHelper.TRIP_REQUIRES_ASSESSMENT_COLUMN,
                DatabaseHelper.TRIP_DATE_COLUMN,
                DatabaseHelper.TRIP_DESCRIPTION_COLUMN,
                DatabaseHelper.TRIP_DAYS_SPENT_COLUMN
        };

            // Perform a query on the contacts table
            ICursor cursor = db.Query(
                    DatabaseHelper.TBL_TRIPS,             // The table to query
                    projection,            // The columns to return
                    null,                  // The columns for the WHERE clause
                    null,                  // The values for the WHERE clause
                    null,                  // Don't group the rows
                    null,                  // Don't filter by row groups
                    null                   // The sort order
            );

            try
            {
                // Iterate through all the returned rows in the cursor
                while (cursor.MoveToNext())
                {
                    // Get the values from the cursor
                    int id = cursor.GetInt(cursor.GetColumnIndexOrThrow(DatabaseHelper.TRIP_ID_COLUMN));
                    string name = cursor.GetString(cursor.GetColumnIndexOrThrow(DatabaseHelper.TRIP_NAME_COLUMN));
                    string destination = cursor.GetString(cursor.GetColumnIndexOrThrow(DatabaseHelper.TRIP_DESTINATION_COLUMN));
                    int requiresAssessment = cursor.GetInt(cursor.GetColumnIndexOrThrow(DatabaseHelper.TRIP_REQUIRES_ASSESSMENT_COLUMN));
                    string date = cursor.GetString(cursor.GetColumnIndexOrThrow(DatabaseHelper.TRIP_DATE_COLUMN));
                    string description = cursor.GetString(cursor.GetColumnIndexOrThrow(DatabaseHelper.TRIP_DESCRIPTION_COLUMN));
                    int days = cursor.GetInt(cursor.GetColumnIndexOrThrow(DatabaseHelper.TRIP_DAYS_SPENT_COLUMN));

                    // Do something with the values
                    Trip trip = new Trip();
                    trip.Id = id;
                    trip.Name = name;
                    trip.Destination = destination;
                    trip.RequiresAssessment = requiresAssessment;
                    trip.Date = date;
                    trip.Description = description;
                    trip.DaysSpent = days;
                    trips.Add(trip);
                }
            }
            finally
            {
                // Always close the cursor when you're done reading from it. This releases all its resources and makes it invalid.
                cursor.Close();

                if (trips.Count == 0)
                {
                    textViewNoTrips.Visibility = ViewStates.Visible;
                }
                initRecyclerView(trips);

            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            fetchTrips();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.all_trips_menu, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.clearDatabase)
            {
                showConfirmationDialog();
            }
            return base.OnOptionsItemSelected(item);
        }

        private void showConfirmationDialog()
        {
             new AlertDialog.Builder(this)
                     .SetTitle("Clear Database?")
                     .SetMessage("Do you really want to clear entire database?")
                     .SetPositiveButton("Yes", (dialog, whichButton) =>
                     {
                         SQLiteDatabase db = dbHelper.WritableDatabase;
                         dbHelper.clearDatabase(db);
                         initRecyclerView(new List<Trip>());

                         textViewNoTrips.Visibility = ViewStates.Visible;
                     })
                 .SetNegativeButton("No", (dialog, whichButton) =>
                 {

                     (dialog as AlertDialog).Dismiss();
                 })
                 .Show();
        }


    }
}