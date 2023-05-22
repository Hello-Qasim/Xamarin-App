using Android.App;
using Android.Content;
using Android.Database;
using Android.Database.Sqlite;
using Android.OS;
using Android.Text;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.RecyclerView.Widget;
using ExpressTracketXamarin.Database;
using ExpressTracketXamarin.Trips;
using System.Collections.Generic;
using static Xamarin.Essentials.Platform;
using Intent = Android.Content.Intent;

namespace ExpressTracketXamarin.Expenses
{
    [Activity(Label = "AllExpensesActivity", Theme = "@style/AppTheme")]
    public class AllExpensesActivity : AppCompatActivity
    {
        Google.Android.Material.FloatingActionButton.FloatingActionButton btnAddExpense;
        RecyclerView recyclerView;
        TextView textViewNoTrips;
        private int tripId = 0;
        private DatabaseHelper dbHelper;
        private List<Expense> expenses = new List<Expense>();
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_all_expenses);
            Title = "All Expenses";

            tripId = Intent.Extras.GetInt(Constants.KEY_TRIP_ID, 0);
            if (tripId < 1) return;

            btnAddExpense = FindViewById<Google.Android.Material.FloatingActionButton.FloatingActionButton>(Resource.Id.btnAddExpense);
            recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
            textViewNoTrips = FindViewById<TextView>(Resource.Id.textViewNoTrips);
            
            dbHelper = new DatabaseHelper(this, DatabaseHelper.DATABASE_NAME, null, 1);
            btnAddExpense.Click += (s, e) =>
            {
                Intent intent = new Intent(this, typeof(AddExpenseActivity));
                intent.PutExtra(Constants.KEY_TRIP_ID, tripId);
                StartActivity(intent);
                Finish();
            };
        }
       
       
        private void initRecyclerView(List<Expense> expenses)
        {
            var adapter = new ExpensesAdapter(expenses);
            recyclerView.SetAdapter(adapter);
            recyclerView.SetLayoutManager(new LinearLayoutManager(this));

        }

        private void fetchExpenses()
        {
            expenses.Clear();

            // Create and/or open a database to read from it
            SQLiteDatabase db = dbHelper.ReadableDatabase;

            // Define a projection that specifies which columns from the database you will actually use after this query.
            string[] projection = {
                DatabaseHelper.EXPENSE_ID_COLUMN,
                DatabaseHelper.EXPENSE_TYPE_COLUMN,
                DatabaseHelper.EXPENSE_AMOUNT_COLUMN,
                DatabaseHelper.EXPENSE_DATE_COLUMN,
                DatabaseHelper.EXPENSE_COMMENTS_COLUMN,
                DatabaseHelper.TRIP_ID_COLUMN
        };

            string selection = DatabaseHelper.TRIP_ID_COLUMN + " = ?";
            string[] selectionArgs = { tripId.ToString() };

            // Perform a query on the contacts table
            ICursor cursor = db.Query(
                    DatabaseHelper.TBL_EXPENSES,
                    projection,
                    selection,
                    selectionArgs,
                    null,
                    null,
                    null
            );

            try
            {
                // Iterate through all the returned rows in the cursor
                while (cursor.MoveToNext())
                {
                    // Get the values from the cursor
                    int expenseId = cursor.GetInt(cursor.GetColumnIndexOrThrow(DatabaseHelper.EXPENSE_ID_COLUMN));
                    string expenseType = cursor.GetString(cursor.GetColumnIndexOrThrow(DatabaseHelper.EXPENSE_TYPE_COLUMN));
                    int expenseAmount = cursor.GetInt(cursor.GetColumnIndexOrThrow(DatabaseHelper.EXPENSE_AMOUNT_COLUMN));
                    string expenseDate = cursor.GetString(cursor.GetColumnIndexOrThrow(DatabaseHelper.EXPENSE_DATE_COLUMN));
                    string expenseComments = cursor.GetString(cursor.GetColumnIndexOrThrow(DatabaseHelper.EXPENSE_COMMENTS_COLUMN));
                    int tripId = cursor.GetInt(cursor.GetColumnIndexOrThrow(DatabaseHelper.TRIP_ID_COLUMN));

                    // Do something with the values
                    Expense expense = new Expense(expenseType, expenseAmount, expenseDate, expenseComments, tripId);
                    expenses.Add(expense);
                }
            }
            finally
            {
                // Always close the cursor when you're done reading from it. This releases all its resources and makes it invalid.
                cursor.Close();

                if (expenses.Count == 0)
                {
                    textViewNoTrips.Visibility = ViewStates.Visible;
                }
                initRecyclerView(expenses);
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            fetchExpenses();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.all_expenses_menu, menu);
            return true;
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.home)
            {
                openAllTripsActivity();
            }

            return base.OnOptionsItemSelected(item);
        }
        private void openAllTripsActivity()
        {
            Intent intent = new Intent(this, typeof(AllTripsActivity));
            StartActivity(intent);
            Finish();
        }
    }
}