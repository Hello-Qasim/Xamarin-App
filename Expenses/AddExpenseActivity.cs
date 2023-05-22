using Android.App;
using Android.Content;
using Android.Database.Sqlite;
using Android.OS;
using Android.Widget;
using AndroidX.AppCompat.App;
using ExpressTracketXamarin.Database;
using Java.Lang;
using Org.W3c.Dom;
using System;

namespace ExpressTracketXamarin.Expenses
{
    [Activity(Label = "AddExpenseActivity", Theme = "@style/AppTheme")]
    public class AddExpenseActivity : AppCompatActivity
    {
        EditText editTextType;
        EditText editTextAmount;
        EditText editTextDate;
        EditText editTextComments;
        Button btnAddExpense;
        private int tripId = 0;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_add_expense);
            Title = "Add Expense";

            tripId = Intent.Extras.GetInt(Constants.KEY_TRIP_ID, 0);
            if (tripId < 1) return;

            editTextType = FindViewById<EditText>(Resource.Id.editTextType);
            editTextAmount = FindViewById<EditText>(Resource.Id.editTextAmount);
            editTextDate = FindViewById<EditText>(Resource.Id.editTextDate);
            editTextComments = FindViewById<EditText>(Resource.Id.editTextComments);
            btnAddExpense = FindViewById<Button>(Resource.Id.btnAddExpense);

            editTextDate.Text = Utils.GetCurrentDate();
            btnAddExpense.Click += BtnAddExpense_Click;
        }

        private void BtnAddExpense_Click(object sender, EventArgs e)
        {
            Expense expense = getExpenseFromUserInput();
            if (expense != null)
            {
                insertExpense(expense);
            }
        }

        private void insertExpense(Expense expense)
        {
            DatabaseHelper dbHelper = new DatabaseHelper(this, DatabaseHelper.DATABASE_NAME, null, 1);
            SQLiteDatabase db = dbHelper.WritableDatabase;

            ContentValues values = new ContentValues();
            values.Put(DatabaseHelper.EXPENSE_TYPE_COLUMN, expense.Type);
            values.Put(DatabaseHelper.EXPENSE_AMOUNT_COLUMN, expense.Amount);
            values.Put(DatabaseHelper.EXPENSE_DATE_COLUMN, expense.Date);
            values.Put(DatabaseHelper.EXPENSE_COMMENTS_COLUMN, expense.Comments);
            values.Put(DatabaseHelper.TRIP_ID_COLUMN, tripId);

            // Insert a new row for expense in the database, returning the ID of that new row.
            long newRowId = db.Insert(DatabaseHelper.TBL_EXPENSES, null, values);

            // Show a toast message depending on whether or not the insertion was successful
            if (newRowId == -1)
            {
                // If the row ID is -1, then there was an error with insertion.
                Toast.MakeText(this, "Error with saving expense", ToastLength.Short).Show();
            }
            else
            {
                // Otherwise, the insertion was successful and we can display a toast with the row ID.
                Toast.MakeText(this, "Expense added successfully! ID: " + newRowId, ToastLength.Short).Show();
                openAllExpensesActivity();
            }
        }
        private void openAllExpensesActivity()
        {
            Intent intent = new Intent(this, typeof(AllExpensesActivity));
            intent.PutExtra(Constants.KEY_TRIP_ID, tripId);
            StartActivity(intent);
            Finish();
        }

        private Expense getExpenseFromUserInput()
        {
            string type = editTextType.Text;
            string amountInput = editTextAmount.Text;
            string date = editTextDate.Text;
            string comments = editTextComments.Text;

            if (string.IsNullOrEmpty(type) || string.IsNullOrEmpty(amountInput) || string.IsNullOrEmpty(date) || string.IsNullOrEmpty(comments))
            {
                Toast.MakeText(this, "Please fill all data first", ToastLength.Short).Show();
                return null;
            }
            return new Expense(type, getAmount(), date, comments, tripId);
        }

        private int getAmount()
        {
            string amountInput = editTextAmount.Text;
            int amount;
            try
            {
                amount = Integer.ParseInt(amountInput);
            }
            catch (NumberFormatException e)
            {
                amount = 1;
            }
            return amount;
        }
    }
}