using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using System.Text;


namespace firstcapp
{
    class Program
    {
        private const double EarthRadius = 6378137;  
        private const double MinLatitude = -85.05112878;  
        private const double MaxLatitude = 85.05112878;  
        private const double MinLongitude = -180;  
        private const double MaxLongitude = 180; 

        HttpClient client = new HttpClient();
        static async Task Main(string[] args)
        {   
            
            //parameters to be passed
            var bdbx = "-33.278333, 20.579722, -33.231389, 20.633333";
            var viewType = "Aerial";
            var zm = 14;


            Program program = new Program();
            await program.DownloadImageAsync(bdbx, viewType, zm);
           
        }
//br lat tl long
        private async Task DownloadImageAsync(string boundingBox, string viewT, int zoom)
        {
            try
            {
                // using httpClient for download purposes
                using var httpClient = new HttpClient();

                // input datas
                // var bdbx = "47.611833900404896,  -122.36456394195557,  47.626268662877358,  -122.34314918518068";
                var bdbx =boundingBox;
                // var bdbx = "4761.9048,-122.35384";
                var viewType = viewT;
                var zm = zoom;
                var pushpin1 = "-33.231389, 20.579722";
                var pushpin2 = "-33.278333, 20.633333";
                
               


                // API to be used
                // var url = $"https://dev.virtualearth.net/REST/V1/Imagery/Map/{viewType}/{bdbx}/8?dpi=Large&zoomLevel={zoom}&mapSize=2000,1500&format=jpeg&key=AvIGA0FCQHHBF0k_ACLe2v-gxTbnT2vQhqR1mS303f1jJ4JLpFERujV0Awrd9gQb";
                var url = $"https://dev.virtualearth.net/REST/V1/Imagery/Map/{viewType}?pp={pushpin1}&pp={pushpin2}&ma={bdbx}&dpi=Large&mapSize=2000,1500&format=jpeg&key=AvIGA0FCQHHBF0k_ACLe2v-gxTbnT2vQhqR1mS303f1jJ4JLpFERujV0Awrd9gQb";
                var metaurl = $"https://dev.virtualearth.net/REST/V1/Imagery/Map/{viewType}?mapMetadata=1&pp={pushpin1}&ma={bdbx}&dpi=Large&mapSize=2000,1500&format=jpeg&key=AvIGA0FCQHHBF0k_ACLe2v-gxTbnT2vQhqR1mS303f1jJ4JLpFERujV0Awrd9gQb";

                var uri = new Uri(url);

                //directory path
                var directoryPath = "testImages";

                //name of the image
                DateTime now = DateTime.Now;
                var timeTaken = now.ToString();
                var t = timeTaken.Replace("/", string.Empty);
                var fileName = $"{zm}_test{t}";

                // Get the file extension
                var uriWithoutQuery = uri.GetLeftPart(UriPartial.Path);
                // var fileExtension = Path.GetExtension(uriWithoutQuery);

                // Create file path and ensure directory exists
                var path = Path.Combine(directoryPath, $"{fileName}");
                Directory.CreateDirectory(directoryPath);


                // Download the image and write to the file
                var metadata = await httpClient.GetStringAsync(metaurl);
                // Console.WriteLine($"metadata : {metadata}");
                  
                


                Console.WriteLine(metadata.GetType());
                var imageBytes = await httpClient.GetByteArrayAsync(uri);
                await File.WriteAllBytesAsync($"{path}.jpeg", imageBytes);
                Console.WriteLine("successfully saved");
                // var latCenter = ((-54.835630) - (-54.841647))/2 + (-54.841647);
                // var longCenter = ((-68.287705 ) - (-68.299114))/2 + (-68.299114);
                var latCenter = 62.455000;
                var longCenter = -114.476666;
                Console.WriteLine($"center calculated lat&long(in deg) is: {latCenter} and {longCenter}");                
                Console.WriteLine(LatLongToPixelXY( latCenter , longCenter ,14));



            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public static uint MapSize(int levelOfDetail)  
        {  
            return (uint) 256 << levelOfDetail;  
        }  

        private static double Clip(double n, double minValue, double maxValue)  
        {  
            return Math.Min(Math.Max(n, minValue), maxValue);  
        } 
        public static string PixelXYToLatLong(int pixelX, int pixelY, int levelOfDetail)  
        {  
            Console.WriteLine($"top-left calculated lat&long(in pixels) is: {pixelX} and {pixelY}");
            double mapSize = MapSize(levelOfDetail);  
            double x = (Clip(pixelX, 0, mapSize - 1) / mapSize) - 0.5;  
            double y = 0.5 - (Clip(pixelY, 0, mapSize - 1) / mapSize);  
  
            var latitude = 90 - 360 * Math.Atan(Math.Exp(-y * 2 * Math.PI)) / Math.PI;  
            var longitude = 360 * x;  

            string outputPL = $" top-left calculated lat&long(in pixels) is: {latitude} and {longitude}";

            return outputPL;

        } 
        public static string LatLongToPixelXY(double latitude, double longitude, int levelOfDetail)
        {
            latitude = Clip(latitude, MinLatitude, MaxLatitude);  
            longitude = Clip(longitude, MinLongitude, MaxLongitude);  
  
            double x = (longitude + 180) / 360;   
            double sinLatitude = Math.Sin(latitude * Math.PI / 180);  
            double y = 0.5 - Math.Log((1 + sinLatitude) / (1 - sinLatitude)) / (4 * Math.PI);  
  
            uint mapSize = MapSize(levelOfDetail);  
            var pixelX = (int) Clip(x * mapSize + 0.5, 0, mapSize - 1);  
            var pixelY = (int) Clip(y * mapSize + 0.5, 0, mapSize - 1);

            Console.WriteLine($"canter calculated lat&long(in pixels) is: {pixelX} and {pixelY}");


                //finding the top-left
            // string output = PixelXYToLatLong(pixelX-(2000/2), pixelY-(1500/2), levelOfDetail);

                //checking the values of lat long
            string output = PixelXYToLatLong(pixelX, pixelY, levelOfDetail);
             
            return output;
        }
    }
}
