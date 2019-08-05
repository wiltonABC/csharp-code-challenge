using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace StockPrices
{
    class Solution
    {

        static void openAndClosePrices(string firstDate, string lastDate, string weekDay) {

            string requestUrl;
            string firstMonthYear = firstDate.Substring(firstDate.IndexOf("-") + 1);
            string lastMonthYear = lastDate.Substring(lastDate.IndexOf("-") + 1);
            string firstYear = firstDate.Substring(firstDate.Length - 4);
            string lastYear = lastDate.Substring(lastDate.Length - 4);
            string firstYearFirstThree = firstYear.Substring(0,3);
            string lastYearFirstThree = lastYear.Substring(0,3);
            DateTime startDate = DateTime.Parse(firstDate);
            DateTime endDate = DateTime.Parse(lastDate);
            DayOfWeek dayOfWeek = DayOfWeek.Monday;

            switch (weekDay) {
                case "Monday":
                    dayOfWeek = DayOfWeek.Monday;
                    break;
                case "Tuesday":
                    dayOfWeek = DayOfWeek.Tuesday;
                    break;
                case "Wednesday":
                    dayOfWeek = DayOfWeek.Wednesday;
                    break;
                case "Thursday":
                    dayOfWeek = DayOfWeek.Thursday;
                    break;
                case "Friday":
                    dayOfWeek = DayOfWeek.Friday;
                    break;
                case "Saturday":
                    dayOfWeek = DayOfWeek.Saturday;
                    break;
                case "Sunday":
                    dayOfWeek = DayOfWeek.Sunday;
                    break;
            }

            //Code to prepare the request based on the parameters
            //in a way to retrieve less data and do less requests
            if (firstDate.Equals(lastDate)) { //Query by exact date
                requestUrl = $"https://jsonmock.hackerrank.com/api/stocks/?date={firstDate}";
            } else if (firstMonthYear.Equals(lastMonthYear)) { //Query by month
                requestUrl = $"https://jsonmock.hackerrank.com/api/stocks/search/?date={firstMonthYear}";
            } else if (firstYear.Equals(lastYear)) { //Query by year
                requestUrl = $"https://jsonmock.hackerrank.com/api/stocks/search/?date={firstYear}";
            } else if (firstYearFirstThree.Equals(lastYearFirstThree)) { //Query by year's first 3 positions
                requestUrl = $"https://jsonmock.hackerrank.com/api/stocks/search/?date={firstYearFirstThree}";
            } else { //Retrieve all
                requestUrl = "https://jsonmock.hackerrank.com/api/stocks/";
            }

            //Http Request
            using (HttpClient httpClient = new HttpClient()) {
                HttpResponseMessage responseMessage = httpClient.GetAsync(requestUrl).Result;
                string responseBody = responseMessage.Content.ReadAsStringAsync().Result;

                //Handling Json Data
                JObject responseObject = JObject.Parse(responseBody);
                int currentPage = 1;
                int totalPages = (int)responseObject["total_pages"];
                List<JToken> stocksList = responseObject["data"].Where(jToken => {
                    DateTime currentDate = DateTime.Parse((string)jToken["date"]);
                    return currentDate >= startDate && currentDate <= endDate && currentDate.DayOfWeek == dayOfWeek;
                }).ToList();

                //Get next pages if present
                while (currentPage < totalPages) {
                    currentPage++;

                    string pageUrl;
                    if (requestUrl.Contains("?")) {
                        pageUrl = requestUrl + ($"&page={currentPage}");
                    } else {
                        pageUrl = requestUrl + ($"?page={currentPage}");
                    }

                    responseMessage = httpClient.GetAsync(pageUrl).Result;
                    responseBody = responseMessage.Content.ReadAsStringAsync().Result;
                    responseObject = JObject.Parse(responseBody);

                    stocksList.AddRange(responseObject["data"].Where(jToken => {
                        DateTime currentDate = DateTime.Parse((string)jToken["date"]);
                        return currentDate >= startDate && currentDate <= endDate && currentDate.DayOfWeek == dayOfWeek;
                    }));
                }

                foreach (JToken jToken in stocksList) {
                    Console.WriteLine((string)jToken["date"] + " " + (string)jToken["open"] + " " + (string)jToken["close"]);
                }

                
            }

        }
        
        static void Main(string[] args) {
            string _firstDate;
            _firstDate = Console.ReadLine();
        
            string _lastDate;
            _lastDate = Console.ReadLine();
        
            string _weekDay;
            _weekDay = Console.ReadLine();
        
            openAndClosePrices(_firstDate, _lastDate, _weekDay);
        }
    }
}
