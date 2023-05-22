using Android.App;
using Android.Content;
using Android.Database.Sqlite;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using ExpressTracketXamarin.Database;
using Java.Lang;
using Newtonsoft.Json;

namespace ExpressTracketXamarin.Trips
{
    [Activity(Label = "AddTripActivity" ,Theme = "@style/AppTheme")]
    public class AddTripActivity :  AppCompatActivity
    {
        private bool isAddTripFlow = true;
        private Trip trip;
        Button btnAddTrip, btnUpdateTripDetails;
        EditText editTextName, editTextDestination, editTextDate, editTextDaysSpent, editTextDescription;
        TextView title;
        RadioButton rdbYes, rdbNo;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_add_trip);
            btnAddTrip = FindViewById<Button>(Resource.Id.btnAddTrip);
            btnUpdateTripDetails = FindViewById<Button>(Resource.Id.btnUpdateTripDetails);
            editTextName = FindViewById<EditText>(Resource.Id.editTextName);
            editTextDestination = FindViewById<EditText>(Resource.Id.editTextDestination);
            editTextDate = FindViewById<EditText>(Resource.Id.editTextDate);
            editTextDaysSpent = FindViewById<EditText>(Resource.Id.editTextDaysSpent);
            editTextDescription = FindViewById<EditText>(Resource.Id.editTextDescription);
            title= FindViewById<TextView>(Resource.Id.title);
            rdbYes = FindViewById<RadioButton>(Resource.Id.rdbYes);
            rdbNo = FindViewById<RadioButton>(Resource.Id.rdbNo);

            isAddTripFlow = Intent.GetBooleanExtra(Constants.IS_ADD_TRIP_FLOW, true);
            trip = Intent.GetStringExtra(Constants.KEY_TRIP)!=null? JsonConvert.DeserializeObject<Trip>(Intent.GetStringExtra(Constants.KEY_TRIP)):null;
            if (isAddTripFlow)
            {
                btnAddTrip.Visibility=ViewStates.Visible;
            }
            else
            {
                btnUpdateTripDetails.Visibility=ViewStates.Visible;
            }
            updateTitle();
            if (trip != null)
            {
                updateUI(trip);
            }
            btnAddTrip.Click+= (s, e) =>
            {
                Trip trip = getTripFromUserInput();
                if (trip != null)
                {
                    showConfirmationDialog(trip);
                }
            };

            btnUpdateTripDetails.Click+= (s, e) =>
            {
                Trip trip = getTripFromUserInput();
                if (trip != null)
                {
                    showConfirmationDialog(trip);
                }
            };
            // Create your application here
        }
        private void showConfirmationDialog(Trip trip)
        {
            Dialog dialog = new Dialog(this);
            dialog.SetContentView(Resource.Layout.layout_confirm_trip_dialog);

            int requiresAssessmentFlag = trip.RequiresAssessment;
            string requiresAssessmentStatus = Utils.GetRequiresAssessmentStatus(requiresAssessmentFlag);
            string description = trip.Description;

            TextView textViewName = (TextView)dialog.FindViewById(Resource.Id.textViewName);
            TextView textViewDestination = (TextView)dialog.FindViewById(Resource.Id.textViewDestination);
            TextView textViewDate = (TextView)dialog.FindViewById(Resource.Id.textViewDate);
            TextView textViewDaysSpent = (TextView)dialog.FindViewById(Resource.Id.textViewDaysSpent);
            TextView titleRequiresAssessment = (TextView)dialog.FindViewById(Resource.Id.titleRequiresAssessment);
            TextView textViewDescription = (TextView)dialog.FindViewById(Resource.Id.textViewDescription);
            Button btnConfirm = (Button)dialog.FindViewById(Resource.Id.btnConfirm);
            Button btnEditDetails = (Button)dialog.FindViewById(Resource.Id.btnEditDetails);

            textViewName.Text="Name: " + trip.Name;
            textViewDestination.Text="Destination: " + trip.Destination;
            textViewDate.Text="Date: " + trip.Date;
            titleRequiresAssessment.Text="Requires Assessment: " + requiresAssessmentStatus;
            textViewDaysSpent.Text="Days Spent: " + trip.DaysSpent;

            if (!string.IsNullOrEmpty(description))
            {
                textViewDescription.Visibility=ViewStates.Visible;
                textViewDescription.Text="Description: " + trip.Destination;
            }

            btnConfirm.Click+=  (s, e) =>
            {
                dialog.Dismiss();
                if (isAddTripFlow)
                {
                    insertTrip(trip);
                }
                else
                {
                    updateTrip(trip);
                }
            };
            
            btnEditDetails.Click+=(s,e)=>dialog.Dismiss();
            dialog.Show();
        }

        private Trip getTripFromUserInput()
        {
            string name = editTextName.Text;
            string destination = editTextDestination.Text;
            string date = editTextDate.Text;
            string description = editTextDescription.Text;

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(destination) || string.IsNullOrEmpty(date))
            {
                Toast.MakeText(this, "Please fill all data first", ToastLength.Short).Show();
                return null;
            }
            return new Trip(name, destination, date, getRiskAssessmentStatus(), description, getDaysSpent());
        }

        private void updateUI(Trip trip)
        {
            int requiresAssessmentFlag = trip.RequiresAssessment;

            title.Text="Update Trip Details";
            editTextName.Text="" + trip.Name;
            editTextDestination.Text="" + trip.Destination;
            editTextDate.Text="" + trip.Date;
            editTextDescription.Text="" + trip.Description;
            editTextDaysSpent.Text="" + trip.DaysSpent;

            if (requiresAssessmentFlag == 1)
            {
                rdbYes.Checked=true;
            }
            else
            {
                rdbNo.Checked=true;
            }
        }

        private void insertTrip(Trip trip)
        {
            DatabaseHelper dbHelper = new DatabaseHelper(this, DatabaseHelper.DATABASE_NAME, null, 1);
            SQLiteDatabase db = dbHelper.WritableDatabase;

            ContentValues values = new ContentValues();
            values.Put(DatabaseHelper.TRIP_NAME_COLUMN, trip.Name);
            values.Put(DatabaseHelper.TRIP_DESTINATION_COLUMN, trip.Destination);
            values.Put(DatabaseHelper.TRIP_DATE_COLUMN, trip.Date);
            values.Put(DatabaseHelper.TRIP_REQUIRES_ASSESSMENT_COLUMN, trip.RequiresAssessment);
            values.Put(DatabaseHelper.TRIP_DESCRIPTION_COLUMN, trip.Description);
            values.Put(DatabaseHelper.TRIP_DAYS_SPENT_COLUMN, trip.DaysSpent);

            // Insert a new row for trip in the database, returning the ID of that new row.
            long newRowId = db.Insert(DatabaseHelper.TBL_TRIPS, null, values);

            // Show a toast message depending on whether or not the insertion was successful
            if (newRowId == -1)
            {
                // If the row ID is -1, then there was an error with insertion.
                Toast.MakeText(this, "Error with saving trip", ToastLength.Short).Show();
            }
            else
            {
                // Otherwise, the insertion was successful and we can display a toast with the row ID.
                Toast.MakeText(this, "Trip saved successfully! ID: " + newRowId, ToastLength.Short).Show();
                openAllTripsActivity();
            }
        }

        private void updateTrip(Trip trip)
        {
            DatabaseHelper dbHelper = new DatabaseHelper(this, DatabaseHelper.DATABASE_NAME, null, 1);
            SQLiteDatabase db = dbHelper.WritableDatabase;

            ContentValues values = new ContentValues();
            values.Put(DatabaseHelper.TRIP_NAME_COLUMN, trip.Name);
            values.Put(DatabaseHelper.TRIP_DESTINATION_COLUMN, trip.Destination);
            values.Put(DatabaseHelper.TRIP_DATE_COLUMN, trip.Date);
            values.Put(DatabaseHelper.TRIP_REQUIRES_ASSESSMENT_COLUMN, trip.RequiresAssessment);
            values.Put(DatabaseHelper.TRIP_DESCRIPTION_COLUMN, trip.Description);
            values.Put(DatabaseHelper.TRIP_DAYS_SPENT_COLUMN, trip.DaysSpent);

            // Updating row
            int count = db.Update(
                    DatabaseHelper.TBL_TRIPS,
                    values,
                    DatabaseHelper.TRIP_ID_COLUMN + " = ?",
                    new string[] { this.trip.Id.ToString() }
            );
            db.Close();

            // Check the result
            if (count > 0)
            {
                // Update was successful
                Toast.MakeText(this, "Trip updated successfully!", ToastLength.Short).Show();
                openAllTripsActivity();
            }
            else
            {
                // No rows were updated
                Toast.MakeText(this, "Error with updating trip", ToastLength.Short).Show();
            }
        }

        private void updateTitle()
        {
            string title;
            if (isAddTripFlow)
            {
                title = "Add Trip";
            }
            else
            {
                title = "Update Trip";
            }
            Title=title;
        }
        private void openAllTripsActivity()
        {
            Android.Content.Intent intent = new Android.Content.Intent(this, typeof(AllTripsActivity));
            StartActivity(intent);
            Finish();
        }
        private int getDaysSpent()
        {
            string daysSpentInput = editTextDaysSpent.Text;
            int daysSpent;
            try
            {
                daysSpent = Integer.ParseInt(daysSpentInput);
            }
            catch (NumberFormatException e)
            {
                daysSpent = 1;
            }
            if (daysSpent < 1)
            { // Set the number of days spent to 1 even if user updates it to 0
                return 1;
            }
            else
            {
                return daysSpent;
            }
        }

        private int getRiskAssessmentStatus()
        {
            if (rdbYes.Checked)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }
}