﻿using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using BeerOverflowWindowsApp.DataModels;
using FourSquare.SharpSquare.Core;
using FourSquare.SharpSquare.Entities;

namespace BeerOverflowWindowsApp.BarProviders
{
    class GetBarListFourSquare : IBeerable
    {
        private static readonly string clientId = ConfigurationManager.AppSettings["FourSquareClientId"];
        private static readonly string clientSecret = ConfigurationManager.AppSettings["FourSquareClientSecret"];
        private static readonly string categoryIdBar = ConfigurationManager.AppSettings["FourSquareCategoryIdBar"];

        public List<BarData> GetBarsAround(string latitude, string longitude, string radius)
        {
            List<BarData> barList = null;
            var result = GetBarData(latitude, longitude, radius);
            barList = VenueListToBars(result);
            return barList;
        }

        private List<Venue> GetBarData (string latitude, string longitude, string radius)
        {
            var sharpSquare = new SharpSquare(clientId, clientSecret);

            // let's build the query
            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                { "ll", latitude + "," + longitude }, // Coords
                { "radius", radius },
                { "categoryId", categoryIdBar } 
            };

            return sharpSquare.SearchVenues(parameters);
        }

        private List<BarData> VenueListToBars (IEnumerable<Venue> resultData)
        {
            return resultData.Select(result => new BarData
                {
                    Title = result.name,
                    Latitude = result.location.lat,
                    Longitude = result.location.lng
                })
                .ToList();
        }
    }
}
