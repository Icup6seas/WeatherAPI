using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demo_WebAPI_Weather.ConsoleUtilities;
using Demo_WebAPI_Weather.DataAccessLayer;
using Demo_WebAPI_Weather.BusinessLayer;
using Demo_WebAPI_Weather.Models;
using RestSharp;

namespace Demo_WebAPI_Weather.PresentationLayer
{
    class Presenter
    {
        enum LocationDesignationMethod { None, LongitudeLatitude, ZipCode }

        BusinessLogic _businessLogic;
        IRestApiClient _restApiClient;

        public Presenter(BusinessLogic businessLogic)
        {
            _businessLogic = businessLogic;
            RunApplicationLoop();
        }

        private void RunApplicationLoop()
        {
            DisplayWelcomeScreen();
            DisplayMainMenu();
            DisplayClosingScreen();
        }

        private void DisplayMainMenu()
        {
            bool runApp = true;
            WeatherData _weatherData = null;
            LocationInformation _locationInformation = null;
            LocationDesignationMethod locationDesignationMethod = LocationDesignationMethod.None;

            InitializeApplicationWindow();

            do
            {
                DisplayHeader("\t\tMain Menu");
                Console.WriteLine("");
                Console.WriteLine("\tA. Get Weather Data by Longitude and Latitude");
                Console.WriteLine("\tB. Get Weather Data by Zip Code");
                Console.WriteLine("\tC. Display Weather Data Short Format");
                Console.WriteLine("\tQ. Quit");
                Console.WriteLine();
                Console.Write("\tEnter Menu Choice:");
                switch (Console.ReadLine().ToLower())
                {
                    case "a":
                        _weatherData = DisplayGetWeatherByLonLat(out _locationInformation);
                        locationDesignationMethod = LocationDesignationMethod.LongitudeLatitude;
                        break;
                    case "b":
                        _weatherData = DisplayGetWeatherByZipCode(out _locationInformation);
                        locationDesignationMethod = LocationDesignationMethod.ZipCode;
                        break;
                    case "c":
                        DisplayWeatherDataShortFormat(_weatherData, _locationInformation, locationDesignationMethod);
                        break;
                    case "q":
                        runApp = false;
                        break;
                    default:
                        Console.WriteLine("Please make a selection A-C or Q.");
                        DisplayContinuePrompt();
                        break;
                }

            } while (runApp);

        }

        /// <summary>
        /// display get the weather data by longitude and latitude
        /// </summary>
        /// <returns>weather data</returns>
        private WeatherData DisplayGetWeatherByLonLat(out LocationInformation locationInformation)
        {
            WeatherData weatherData;
            double lon, lat;

            locationInformation = new LocationInformation();

            DisplayHeader("Weather by Longitude and latitude");

            //
            // get longitude and latitude from user
            //
            do
            {
                Console.Write("\tEnter Longitude:");
            } while (!double.TryParse(Console.ReadLine(), out lon));
            do
            {
                Console.Write("\tEnter latitude:");
            } while (!double.TryParse(Console.ReadLine(), out lat));
            
            //
            // acquire weather data from Open Weather Map
            //
            weatherData = _businessLogic.GetWeatherByLonLat(new LocationCoordinates() { Longitude = lon, Latitude = lat });

            //
            // update LocationInformation object
            //
            locationInformation.LocationCoordinates = new LocationCoordinates(){ Longitude = lon, Latitude = lat };
            locationInformation.Name = weatherData.Name;
            locationInformation.ZipCode = 0;

            Console.WriteLine($"\tWeather data for Longitude:{lon:0.##} and Latitude:{lat:0.##} acquired.");

            DisplayContinuePrompt();

            return weatherData;
        }

        /// <summary>
        /// display get the weather data by zip code
        /// </summary>
        /// <returns>weather data</returns>
        private WeatherData DisplayGetWeatherByZipCode(out LocationInformation locationInformation)
        {
            WeatherData weatherData;
            int zipCode;

            locationInformation = new LocationInformation();

            DisplayHeader("Weather by Zip Code");

            //
            // get zip code from user
            //
            do
            {
                Console.Write("\tEnter Zip Code:");
            } while (!int.TryParse(Console.ReadLine(), out zipCode));

            //
            // acquire weather data from Open Weather Map
            //
            weatherData = _businessLogic.GetWeatherByZipCode(zipCode);

            //
            // update LocationInformation object
            //
            locationInformation.ZipCode = zipCode;
            locationInformation.Name = weatherData.Name;
            locationInformation.LocationCoordinates = new LocationCoordinates() { Longitude = weatherData.Coord.Lon, Latitude = weatherData.Coord.Lat };

            Console.WriteLine($"\tWeather data for Zip Code:{zipCode} acquired.");

            DisplayContinuePrompt();

            return weatherData;
        }

        private void DisplayWeatherDataShortFormat(WeatherData weatherData, LocationInformation locationInformation, LocationDesignationMethod locationDesignationMethod)
        {
            DisplayHeader("Current Weather Data");

            Console.WriteLine($"\tWeather Data for {locationInformation.Name}");
            if (locationInformation.ZipCode != 0 ) Console.WriteLine("\tZip Code:" + locationInformation.ZipCode);
            Console.WriteLine($"\tLongitude: {locationInformation.LocationCoordinates.Longitude:0.##}");
            Console.WriteLine($"\tLatitude: {locationInformation.LocationCoordinates.Latitude:0.##}");
            Console.WriteLine();

            Console.WriteLine($"\tTemperature: {DisplayFahrenheit(weatherData.Main.Temp)}");
            Console.WriteLine($"\tHumidity: {weatherData.Main.Humidity:0.}%");
            Console.WriteLine($"\tWind: {DisplayMilesPerHour(weatherData.Wind.Speed)} {DisplayCardinalDirection(weatherData.Wind.Deg)}");

            DisplayContinuePrompt();
        }


        /// <summary>
        /// Display the Closing Screen
        /// </summary>
        static void DisplayClosingScreen()
        {
            InitializeWelcomeClosingWindow();

            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("\tDemo Provided by NMC CIT Department");

            DisplayContinuePrompt();
        }

        /// <summary>
        /// Display the Welcome Screen
        /// </summary>
        static void DisplayWelcomeScreen()
        {
            InitializeWelcomeClosingWindow();

            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("\tWeather Web API Demo");
            Console.WriteLine();

            DisplayContinuePrompt();
        }

        #region HEDPER METHODS

        /// <summary>
        /// initialize application screen configuration
        /// </summary>
        static void InitializeApplicationWindow()
        {
            Console.ForegroundColor = ConsoleTheme.ApplicationForegroundColor;
            Console.BackgroundColor = ConsoleTheme.ApplicationBackgroundColor;
        }

        /// <summary>
        /// initialize welcome and closing screen configuration
        /// </summary>
        static void InitializeWelcomeClosingWindow()
        {
            Console.ForegroundColor = ConsoleTheme.WelcomeClosingScreenForegroundColor;
            Console.BackgroundColor = ConsoleTheme.WelcomeClosingScreenBackgroundColor;
        }

        /// <summary>
        /// display a screen header
        /// </summary>
        /// <param name="headerText">header content</param>
        static void DisplayHeader(string headerText)
        {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("\t\t" + headerText);
            Console.WriteLine();
        }

        /// <summary>
        /// display a continue prompt w/ ReadKey()
        /// </summary>
        static void DisplayContinuePrompt()
        {
            Console.CursorVisible = false;
            Console.WriteLine();
            Console.WriteLine("\tPress any key to continue.");
            Console.ReadKey();
            Console.CursorVisible = true;
        }

        /// <summary>
        /// convert Kalvin to Fahrenheit
        /// </summary>
        /// <param name="degreesKalvin"></param>
        /// <returns>degrees Fahrenheit</returns>
        static string DisplayFahrenheit(double degreesKalvin)
        {
            return ((degreesKalvin - 273.15) * 1.8 + 32) + "\u00B0F";
        }

        /// <summary>
        /// convert meter/second to miles/hour
        /// </summary>
        /// <param name="speedMetersPerSecond"></param>
        /// <returns>miles per hour</returns>
        static string DisplayMilesPerHour(double speedMetersPerSecond)
        {
            return speedMetersPerSecond * (3600 / 1609) + "mph";   
        }

        /// <summary>
        /// convert directions in degrees to cardinal directions
        /// </summary>
        /// <param name="degrees">directions in degrees</param>
        /// <returns>cardinal directions</returns>
        static string DisplayCardinalDirection(double degrees)
        {
            string[] caridnalDirections = { "N", "NE", "E", "SE", "S", "SW", "W", "NW", "N" };
            return $"{degrees:0}\u00B0" + caridnalDirections[(int)Math.Round(((double)degrees % 360) / 45)];
        }

        #endregion
    }
}
