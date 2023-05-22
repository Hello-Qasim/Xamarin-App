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
using Android.Database.Sqlite;

namespace ExpressTracketXamarin.Database
{
    public class DatabaseHelper : SQLiteOpenHelper
    {
        public  const string DATABASE_NAME = "db_expense_tracker";

    public const string TBL_TRIPS = "tbl_trips";
    public const string TRIP_ID_COLUMN = "tripId";
    public const string TRIP_NAME_COLUMN = "name";
    public const string TRIP_DESTINATION_COLUMN = "destination";
    public const string TRIP_DATE_COLUMN = "date";
    public const string TRIP_REQUIRES_ASSESSMENT_COLUMN = "requiresAssessment";
    public const string TRIP_DESCRIPTION_COLUMN = "description";
    public const string TRIP_DAYS_SPENT_COLUMN = "days";

        public static string CREATE_TABLE_TRIPS =
                $"CREATE TABLE {TBL_TRIPS} (" +
                        $" {TRIP_ID_COLUMN} INTEGER PRIMARY KEY AUTOINCREMENT, " +
                        $" {TRIP_NAME_COLUMN} TEXT, " +
                        $" {TRIP_DESTINATION_COLUMN} TEXT, " +
                        $" {TRIP_REQUIRES_ASSESSMENT_COLUMN} INTEGER DEFAULT 0, " +
                        $" {TRIP_DATE_COLUMN} TEXT, " +
                        $" {TRIP_DESCRIPTION_COLUMN} TEXT, " +
                        $" {TRIP_DAYS_SPENT_COLUMN} INTEGER DEFAULT 1)";
            
    

    public const string TBL_EXPENSES = "tbl_expenses";
    public const  string EXPENSE_ID_COLUMN = "expenseId";
    public const  string EXPENSE_TYPE_COLUMN = "type";
    public const  string EXPENSE_AMOUNT_COLUMN = "amount";
    public const  string EXPENSE_DATE_COLUMN = "date";
    public const string EXPENSE_COMMENTS_COLUMN = "comments";

        public static string CREATE_TABLE_EXPENSES =
                $"CREATE TABLE {TBL_EXPENSES} (" +
                        $" {EXPENSE_ID_COLUMN} INTEGER PRIMARY KEY AUTOINCREMENT, " +
                        $" {EXPENSE_TYPE_COLUMN} TEXT, " +
                        $" {EXPENSE_AMOUNT_COLUMN} INTEGER DEFAULT 0, " +
                        $" {EXPENSE_DATE_COLUMN} TEXT, " +
                        $" {EXPENSE_COMMENTS_COLUMN} TEXT, " +
                        $" {TRIP_ID_COLUMN} INTEGER, " +
                        "FOREIGN KEY (tripId) REFERENCES tbl_trips(tripId))";
            
   
        public DatabaseHelper(Context context, string name, SQLiteDatabase.ICursorFactory factory, int version) : base(context, name, factory, version)
        {
        }

        public override void OnCreate(SQLiteDatabase db)
        {
            db.ExecSQL(CREATE_TABLE_TRIPS);
            db.ExecSQL(CREATE_TABLE_EXPENSES);
        }

        public override void OnUpgrade(SQLiteDatabase db, int oldVersion, int newVersion)
        {
           
        }
        public override void OnConfigure(SQLiteDatabase db)
        {
            base.OnConfigure(db);
            db.SetForeignKeyConstraintsEnabled(true);
        }

        public void clearDatabase(SQLiteDatabase db)
        {
            db.ExecSQL("DROP TABLE IF EXISTS " + TBL_EXPENSES + ";");
            db.ExecSQL("DROP TABLE IF EXISTS " + TBL_TRIPS + ";");
            OnCreate(db);
        }
    }
}