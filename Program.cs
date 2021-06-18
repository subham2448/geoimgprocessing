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

        HttpClient client = new HttpClient();
        static async Task Main(string[] args)
        {
            Program program = new Program();
            await program.DownloadImageAsync();
        }

        private async Task DownloadImageAsync()
        {
            try
            {
                // using httpClient for download purposes
                using var httpClient = new HttpClient();

                // input datas
                // var bdbx = "47.611833900404896,  -122.36456394195557,  47.626268662877358,  -122.34314918518068";
                var bdbx = "62.443528,  -114.440968, 62.453119, -114.407630";
                // var bdbx = "4761.9048,-122.35384";
                var viewType = "AerialWithLabels";
               


                // API to be used
                // var url = $"https://dev.virtualearth.net/REST/V1/Imagery/Map/{viewType}/{bdbx}/8?dpi=Large&zoomLevel={zoom}&mapSize=2000,1500&format=jpeg&key=AvIGA0FCQHHBF0k_ACLe2v-gxTbnT2vQhqR1mS303f1jJ4JLpFERujV0Awrd9gQb";
                var url = $"https://dev.virtualearth.net/REST/V1/Imagery/Map/{viewType}?ma={bdbx}&dpi=Large&mapSize=2000,1500&format=jpeg&key=AvIGA0FCQHHBF0k_ACLe2v-gxTbnT2vQhqR1mS303f1jJ4JLpFERujV0Awrd9gQb";

                var uri = new Uri(url);

                //directory path
                var directoryPath = "testImages";

                //name of the image
                DateTime now = DateTime.Now;
                var timeTaken = now.ToString();
                var t = timeTaken.Replace("/", string.Empty);
                var fileName = $"test{t}";

                // Get the file extension
                var uriWithoutQuery = uri.GetLeftPart(UriPartial.Path);
                // var fileExtension = Path.GetExtension(uriWithoutQuery);

                // Create file path and ensure directory exists
                var path = Path.Combine(directoryPath, $"{fileName}");
                Directory.CreateDirectory(directoryPath);


                // Download the image and write to the file
                var imageBytes = await httpClient.GetByteArrayAsync(uri);
                await File.WriteAllBytesAsync($"{path}.jpeg", imageBytes);
                Console.WriteLine("successfully saved");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
