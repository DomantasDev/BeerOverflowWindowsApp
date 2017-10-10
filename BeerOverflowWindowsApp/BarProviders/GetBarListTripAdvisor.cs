using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using BeerOverflowWindowsApp.DataModels;
using Newtonsoft.Json;
using static BeerOverflowWindowsApp.DataModels.TripAdvisorDataModel;

namespace BeerOverflowWindowsApp.BarProviders
{
    class GetBarListTripAdvisor : IBeerable
    {
        private static readonly string _tripAdvisorAccessKey = ConfigurationManager.AppSettings["TripAdvisorAccessKey"];
        private static readonly string _tripAdvisorMapperLink = ConfigurationManager.AppSettings["TripAdvisorMapperLink"];
        private readonly string _tripAdvisorLocationApiLink = ConfigurationManager.AppSettings["TripAdvisorLocationAPILink"];

        public List<BarData> GetBarsAround(string latitude, string longitude, string radius)
        {
            List<BarData> barList = null;
            try
            {
                var result = GetBarData(latitude, longitude, radius);
                barList = PlacesApiQueryResponseToBars(result, radius);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return barList;
        }

        private PlacesResponse GetBarData(string latitude, string longitude, string radius)
        {
            using (var client = new HttpClient())
            {
                PlacesResponse result = null;
                try
                {
                    var webClient = new WebClient();
                    var response = webClient.DownloadString(string.Format(_tripAdvisorMapperLink, latitude, longitude, _tripAdvisorAccessKey));
                    result = JsonConvert.DeserializeObject<PlacesResponse>(response);

                    FetchLocations(result);
                }
                catch (Exception exception)
                {
                    throw exception;
                }
                return result;
            }
        }

        private List<BarData> PlacesApiQueryResponseToBars(PlacesResponse resultData, string radius)
        {
            return (from result in resultData.data
                let distanceMeters = ConvertMilesToMeters(double.Parse(result.distance, CultureInfo.InvariantCulture))
                where (int) distanceMeters <= int.Parse(radius)
                select new BarData
                {
                    Title = result.name,
                    Latitude = double.Parse(result.location.latitude, CultureInfo.InvariantCulture),
                    Longitude = double.Parse(result.location.longitude, CultureInfo.InvariantCulture)
                }).ToList();
        }

        private double ConvertMilesToMeters(double miles)
        {
            return miles * 1.609344 * 1000;
        }

        private void GetLocationForPlace(Datum place)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var webClient = new WebClient();
                    var response =
                        webClient.DownloadString(string.Format(_tripAdvisorLocationApiLink, place.location_id, _tripAdvisorAccessKey));
                    var result = JsonConvert.DeserializeObject<Location>(response);
                    place.location = result;
                }
                catch (Exception exception)
                {
                    throw exception;
                }
            }
        }

        private void FetchLocations(PlacesResponse result)
        {
            foreach (var place in result.data)
            {
                GetLocationForPlace(place);
            }
        }
    }
}
