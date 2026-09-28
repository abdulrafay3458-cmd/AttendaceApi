using AttendanceAPI.Domain.Responses;
using System.Globalization;
using System.Text.Json;

namespace AttendanceAPI.Services
{
    public class AddressGeoService
    {
        private readonly HttpClient _http;

        public AddressGeoService(HttpClient http)
        {
            _http = http;

            // Required by OpenStreetMap policy
            _http.DefaultRequestHeaders.UserAgent.ParseAdd(
                "AttendanceApi/1.0 (kamran.jameel@vconn.biz)"
            );
        }

        public async Task<(double? lat, double? lng)> GetLatLngFromAddress(string address)
        {

            var url =
                $"https://nominatim.openstreetmap.org/search" +
                $"?q={Uri.EscapeDataString(address)}&format=json&limit=1";

            var response = await _http.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return (null, null);

            var json = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<List<NominatimResponse>>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            if (result == null || result.Count == 0)
                return (null, null);

            return (
                double.Parse(result[0].lat, CultureInfo.InvariantCulture),
                double.Parse(result[0].lon, CultureInfo.InvariantCulture)
            );
        }

        public class GeoLocation
        {
            public double? latitude { get; set; }
            public double? longitude { get; set; }
        }
    }

}
