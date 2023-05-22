using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using ExpressTracketXamarin.Database;
using System.Collections.Generic;

namespace ExpressTracketXamarin.Expenses
{

    public class ExpensesAdapter : RecyclerView.Adapter
    {
        List<Expense> _expenses;
        public ExpensesAdapter(List<Expense> expenses)
        {
            _expenses = expenses;
        }
        public override int ItemCount => _expenses.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            Expense expense = _expenses[position];
            if (expense == null) return;

            ExpenseViewHolder vh = holder as ExpenseViewHolder;
            vh.textViewType.Text = expense.Type;
            vh.textViewAmount.Text = "Amount: " + Utils.GetFormattedAmount(expense.Amount);
            vh.textViewDate.Text = expense.Date;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {


            // Inflate the CardView for the photo:
            View itemView = LayoutInflater.From(parent.Context).
                        Inflate(Resource.Layout.layout_expense_item, parent, false);

            // Create a ViewHolder to find and hold these view references, and 
            // register OnClick with the view holder:
            ExpenseViewHolder vh = new ExpenseViewHolder(itemView);
            return vh;
        }


    }
    
    public class ExpenseViewHolder : RecyclerView.ViewHolder
    {

        public TextView textViewType;
        public TextView textViewAmount;
        public TextView textViewDate;

        public ExpenseViewHolder(View itemView) : base(itemView)
        {
            textViewType = itemView.FindViewById<TextView>(Resource.Id.textViewType);
            textViewAmount = itemView.FindViewById<TextView>(Resource.Id.textViewAmount);
            textViewDate = itemView.FindViewById<TextView>(Resource.Id.textViewDate);
        }



    }
}