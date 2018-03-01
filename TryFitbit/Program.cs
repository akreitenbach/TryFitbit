using Fitbit.Api.Portable;
using Fitbit.Api.Portable.OAuth2;
using Fitbit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using System.IO;

namespace TryFitbit
{
    class Program
    {
        static void Main(string[] args)
        {
            var clientId = "22CR42";
            var clientSecret = "072b825966d4ce4a05b58c4ab84154d5";
            var userId = "akreitenbach@gmail.com";
            var token = "eyJhbGciOiJIUzI1NiJ9.eyJzdWIiOiIzUTU5TTkiLCJhdWQiOiIyMkNSNDIiLCJpc3MiOiJGaXRiaXQiLCJ0eXAiOiJhY2Nlc3NfdG9rZW4iLCJzY29wZXMiOiJ3aHIgd251dCB3cHJvIHdzbGUgd3dlaSB3c29jIHdzZXQgd2FjdCB3bG9jIiwiZXhwIjoxNTUwNzk3NzU1LCJpYXQiOjE1MTkyNjE3NTV9.WQsgVrO9Gt1KKgFMIyj75CGOuXXnmr_WlTHnDKP0i3M";
            var fitbitAppCredentials = new FitbitAppCredentials
            {
                ClientId = clientId,
                ClientSecret = clientSecret               
            };

            var oAuth2AccessToken = new OAuth2AccessToken
            {
                UserId = userId,
                Token = token,
                ExpiresIn = 31536000,
                TokenType = "Bearer",
                //https://dev.fitbit.com/build/reference/web-api/oauth2/#scope All of these items are displayed to the user when requesting access
                Scope = "profile+activity+settings+social+weight+location+heartrate+sleep+nutrition"
            };

            
            try
            {
                FitbitClient client = new FitbitClient(fitbitAppCredentials, oAuth2AccessToken);
                var date = new DateTime(2018, 2, 10, 5, 30, 0); //changed date
                //what is Activity?  not a variable, but what?

                //Calls method to get the data
                Activity dayActivity = GetDayActivityAsync(client, date).GetAwaiter().GetResult();
                
                Console.WriteLine("steps on " + date.ToShortDateString() + " " + dayActivity.Summary.Steps.ToString());
                Console.WriteLine("Activity Calories" + " " + dayActivity.Summary.ActivityCalories);
                Console.WriteLine("CaloriesBMR" + " " + dayActivity.Summary.CaloriesBMR);
                Console.WriteLine("CaloriesOut" + " " + dayActivity.Summary.CaloriesOut);
                //Distances is a list
                Console.WriteLine("Distances" + " ");
                foreach (var distance in dayActivity.Summary.Distances)
                {
                    Console.Write(distance.Distance + " ");
                    Console.WriteLine(distance.Activity);
                }
                Console.WriteLine("Elevation" + " " + dayActivity.Summary.Elevation);
                Console.WriteLine("Fairly Active Minutes" + " " + dayActivity.Summary.FairlyActiveMinutes);
                Console.WriteLine("Floors" + " " + dayActivity.Summary.Floors);
                Console.WriteLine("HeartRateZones" + " " + dayActivity.Summary.HeartRateZones);
                Console.WriteLine("LightlyActiveMinutes" + " " + dayActivity.Summary.LightlyActiveMinutes);
                Console.WriteLine("MarginalCalories" + " " + dayActivity.Summary.MarginalCalories);
                Console.WriteLine("RestingHeartRate" + " " + dayActivity.Summary.RestingHeartRate);
                Console.WriteLine("SedentaryMinutes" + " " + dayActivity.Summary.SedentaryMinutes);
                Console.WriteLine("Very Active Minutes" + " " + dayActivity.Summary.VeryActiveMinutes);

                //Calls method to get the intradayData
                IntradayData intradayData = GetIntraDayTimeSeriesAsync(client, date).GetAwaiter().GetResult();

                Console.WriteLine("Intraday Data:");
                using (var sw = new StreamWriter(@"IntraDayData.csv"))
                {
                    var writer = new CsvWriter(sw);
                    foreach (var set in intradayData.DataSet.Where(d => d.Value != "0"))
                    {
                        Console.Write(set.Value + " ");
                        Console.WriteLine(set.Time);
                        writer.WriteField(set.Value);
                        writer.WriteField(set.Time);
                        writer.NextRecord();
                        //want to use CsvHelper to write data to a file
                    }
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine("error:");
                Console.WriteLine(ex.Message);
                Console.ReadKey();
            }
            finally
            {
                Console.WriteLine("press any key");
                Console.ReadKey();
            }            
        }

        private static void WriteRecords(IntradayDataValues set)
        {
            throw new NotImplementedException();
        }

        private static async Task<Activity> GetDayActivityAsync(FitbitClient client, DateTime date)
        {
            Activity dayActivity = await client.GetDayActivityAsync(date);
            var x = await client.GetDayActivityAsync(date);
            return dayActivity;
        }

        private static async Task<IntradayData> GetIntraDayTimeSeriesAsync(FitbitClient client, DateTime date)
        {
            TimeSpan timeSpan = date.AddDays(1) - date;
            IntradayData x = await client.GetIntraDayTimeSeriesAsync(IntradayResourceType.Steps, date, timeSpan);
            return x;
        }

        
        //class tempIntraDayData
        //{

        //}
    }
}
