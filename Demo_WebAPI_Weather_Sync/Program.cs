using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;

namespace Demo_WebAPI_Weather
{
    class Program
    {
        static void Main(string[] args)
        {
            DisplayOpeningScreen();
            DisplayMenu();
            DisplayClosingScreen();
        }

        static void DisplayMenu()
        {
            bool quit = false;
            LocationCoordinates coordinates = new LocationCoordinates(0, 0);
            LocationZip zips = new LocationZip(0);

            while (!quit)
            {
                DisplayHeader("Main Menu");

                Console.WriteLine("Enter the number of your menu choice.");
                Console.WriteLine();
                Console.WriteLine("1) Set the Location by Coordinates");
                Console.WriteLine("2) Set the Location by Zip Code");
                Console.WriteLine("3) Display the Current Weather by Coordinates");
                Console.WriteLine("4) Display the Current Weather by Zip");
                Console.WriteLine("5) Exit");
                Console.WriteLine();
                Console.Write("Enter Choice:");
                string userMenuChoice = Console.ReadLine();

                switch (userMenuChoice)
                {
                    case "1":
                        coordinates = DisplayGetLocationByCoordinates();
                        break;

                    case "2":
                        zips = DisplayGetLocationByZip();
                        break;

                    case "3":
                        DisplayCurrentWeather(coordinates);
                        break;

                    case "4":
                        DisplayCurrentWeatherByZip(zips);
                        break;

                    case "5":
                        quit = true;
                        break;

                    default:
                        Console.WriteLine("You must enter a number from the menu.");
                        break;
                }
            }
        }

        static void DisplayOpeningScreen()
        {
            //
            // display an opening screen
            //
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Weather Reporter");
            Console.WriteLine();
            Console.WriteLine();

            DisplayContinuePrompt();
        }

        static void DisplayClosingScreen()
        {
            //
            // display an closing screen
            //
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Thank you for using my application!");
            Console.WriteLine();
            Console.WriteLine();

            //
            // display continue prompt
            //
            Console.WriteLine();
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
            Console.WriteLine();
        }

        static void DisplayContinuePrompt()
        {
            //
            // display continue prompt
            //
            Console.WriteLine();
            Console.WriteLine("Press any key to continue.");
            Console.ReadKey();
            Console.WriteLine();
        }

        static void DisplayHeader(string headerText)
        {
            //
            // display header
            //
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine(headerText);
            Console.WriteLine();
        }

        static LocationCoordinates DisplayGetLocationByCoordinates()
        {
            DisplayHeader("Set Location by Coordinates");

            LocationCoordinates coordinates = new LocationCoordinates();

            Console.Write("Enter Latitude: ");
            coordinates.Latitude = double.Parse(Console.ReadLine());

            Console.Write("Enter longitude: ");
            coordinates.Longitude = double.Parse(Console.ReadLine());

            Console.WriteLine();
            Console.WriteLine($"Location Coordinates: ({coordinates.Latitude}, {coordinates.Longitude})");
            Console.WriteLine();

            DisplayContinuePrompt();

            return coordinates;
        }

        static LocationZip DisplayGetLocationByZip()
        {
            DisplayHeader("Set Location by Zip Code");

            LocationZip zips = new LocationZip();

            Console.Write("Enter Zip Code: ");
            zips.Zip = int.Parse(Console.ReadLine());

            Console.WriteLine();
            Console.WriteLine($"Location Zip Code: ({zips.Zip})");
            Console.WriteLine();

            DisplayContinuePrompt();

            return zips;
        }

        static WeatherData GetCurrentWeatherData(LocationCoordinates coordinates)
        {
            string url;

            StringBuilder sb = new StringBuilder();
            sb.Clear();
            sb.Append("http://api.openweathermap.org/data/2.5/weather?");
            sb.Append("&lat=" + coordinates.Latitude.ToString());
            sb.Append("&lon=" + coordinates.Longitude.ToString());
            sb.Append("&appid=5e8d3f877557f170f55863ff55ad54f5");

            url = sb.ToString();

            WeatherData currentWeather = new WeatherData();
 
            currentWeather = HttpGetCurrentWeatherByLocation(url);

            return currentWeather;
        }

        static WeatherData GetCurrentWeatherDataByZip(LocationZip zips)
        {
            string url;

            StringBuilder sb = new StringBuilder();
            sb.Clear();
            sb.Append("http://api.openweathermap.org/data/2.5/weather?");
            sb.Append("&zip=" + zips.Zip.ToString());
            sb.Append("&appid=5e8d3f877557f170f55863ff55ad54f5");

            url = sb.ToString();

            WeatherData currentWeather = new WeatherData();

            currentWeather = HttpGetCurrentWeatherByLocation(url);

            return currentWeather;
        }

        static WeatherData HttpGetCurrentWeatherByLocation(string url)
        {
            string result = null;

            using (WebClient syncClient = new WebClient())
            {
                result = syncClient.DownloadString(url);
            }

            //Console.WriteLine(result);

            WeatherData currentWeather = JsonConvert.DeserializeObject<WeatherData>(result);

            return currentWeather;
        }

        static void DisplayCurrentWeather(LocationCoordinates coordinates)
        {
            DisplayHeader("Current Weather by Coordinates");

            WeatherData currentWeatherData = GetCurrentWeatherData(coordinates);
            
            Console.WriteLine(String.Format("Temperature (Fahrenheit): {0:0.0}", ConvertToFahrenheit(currentWeatherData.Main.Temp)));
            Console.WriteLine(String.Format("Wind: {0:0.0}", currentWeatherData.Wind.Speed));
            Console.WriteLine(String.Format("Visibility: {0:0.0}", currentWeatherData.Visibility));
            Console.WriteLine(String.Format("Pressure: {0}", currentWeatherData.Main.Pressure));
            Console.WriteLine(String.Format("Humidity: {0:0.0}", currentWeatherData.Main.Humidity));


            DisplayContinuePrompt();
        }

        static void DisplayCurrentWeatherByZip(LocationZip zips)
        {
            DisplayHeader("Current Weather by Zip");

            WeatherData currentWeatherDataByZip = GetCurrentWeatherDataByZip(zips);

            Console.WriteLine(String.Format("Temperature (Fahrenheit): {0:0.0}", ConvertToFahrenheit(currentWeatherDataByZip.Main.Temp)));
            Console.WriteLine(String.Format("Wind: {0:0.0}", currentWeatherDataByZip.Wind.Speed));
            Console.WriteLine(String.Format("Visibility: {0:0.0}", currentWeatherDataByZip.Visibility));
            Console.WriteLine(String.Format("Pressure: {0}", currentWeatherDataByZip.Main.Pressure));
            Console.WriteLine(String.Format("Humidity: {0:0.0}", currentWeatherDataByZip.Main.Humidity));

            DisplayContinuePrompt();
        }

        static double ConvertToFahrenheit(double degreesKalvin)
        {
            return (degreesKalvin - 273.15) * 1.8 + 32;
        }
    }
}
