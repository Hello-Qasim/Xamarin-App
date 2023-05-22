using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using ExpressTracketXamarin.Database;
using ExpressTracketXamarin.Expenses;
using System;
using System.Collections.Generic;

namespace ExpressTracketXamarin.Trips
{

    public class TripsAdapter : RecyclerView.Adapter
    {
        List<Trip> _trips;
        private readonly Action<Trip> onTripItemClick;

        public TripsAdapter(List<Trip> trips,Action<Trip> onTripItemClick)
        {
            _trips = trips;
            this.onTripItemClick = onTripItemClick;
        }
        public override int ItemCount => _trips.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            Trip trip = _trips[position];
            if (trip == null) return;
            TripViewHolder vh = holder as TripViewHolder;
            vh.textViewName.Text = trip.Name;
            vh.textViewDestination.Text=trip.Destination;
            vh.textViewDate.Text=trip.Date;
            vh.rootLayout.Click += (s, e) =>
            {
                onTripItemClick(trip);
            };
                
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            // Inflate the CardView for the photo:
            View itemView = LayoutInflater.From(parent.Context).
                        Inflate(Resource.Layout.layout_trip_item, parent, false);

            // Create a ViewHolder to find and hold these view references, and 
            // register OnClick with the view holder:
            TripViewHolder vh = new TripViewHolder(itemView);
            return vh;
        }
    }

    class TripViewHolder : RecyclerView.ViewHolder
    {

        public RelativeLayout rootLayout;
        public TextView textViewName;
        public TextView textViewDestination;
        public TextView textViewDate;


        public TripViewHolder(View itemView) : base(itemView)
        {


            rootLayout = itemView.FindViewById<RelativeLayout>(Resource.Id.rootLayout);
            textViewName = itemView.FindViewById<TextView>(Resource.Id.textViewName);
            textViewDestination = itemView.FindViewById<TextView>(Resource.Id.textViewDestination);
            textViewDate = itemView.FindViewById<TextView>(Resource.Id.textViewDate);
        }
    }
}