using AttendanceAPI.Contracts.Response;
using DlibDotNet;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AttendanceAPI.Utilities
{
    public class ApiClass
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration configuration;

        public ApiClass(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            this.configuration = configuration;
        }

        public async Task<string> GetMeanFacialAsync(List<byte[]> payload)
        {
            try
            {
                string apiUrl = $"{configuration["FaceVerificationURL"]}/enroll";

                var a = new payloadClass1()
                {
                    images = payload
                };

                var content = new StringContent(JsonSerializer.Serialize(a), Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(apiUrl, content);

                try
                {
                    response.EnsureSuccessStatusCode();
                } catch (Exception e)
                {
                    throw new Exception("Error recognizing Facials in an image.");
                }

                var result = await response.Content.ReadFromJsonAsync<apiResponse>();
                return result.embedding;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<FaceVerificationResponse> CompareFacesAsync(byte[] current, byte[] stored)
        {
            try
            {
                string apiUrl = $"{configuration["FaceVerificationURL"]}/verify";

                //if (stored == null || stored.Length != 512)
                //    throw new ArgumentException($"Embedding must be 512 floats, got {stored?.Length ?? 0}.");

                //if (current == null || current.Length == 0)
                //    throw new ArgumentException("Image bytes cannot be null or empty.");

                //for (int i = 0; i < stored.Length; i++)
                //{
                //    float v = stored[i];
                //    if (float.IsNaN(v) || float.IsInfinity(v))
                //        stored[i] = 0f; // or some sentinel value
                //}

                //// --- CONVERT EMBEDDING float[] TO byte[] ---
                //byte[] embeddingBytes;
                //try
                //{
                //    embeddingBytes = MemoryMarshal.Cast<float, byte>(stored.AsSpan()).ToArray();
                //}
                //catch (Exception ex)
                //{
                //    throw new InvalidOperationException("Failed to convert float[] to byte[].", ex);
                //}

                // --- BASE64 ENCODE ---
                string embeddingBase64 = Convert.ToBase64String(stored);
                string imageBase64 = Convert.ToBase64String(current);

                var payload = new payloadClass2
                {
                    embedding = embeddingBase64,
                    image = imageBase64
                };

                // --- SERIALIZE TO JSON WITH SAFE SETTINGS ---
                string json;
                try
                {
                    var jsonOptions = new JsonSerializerOptions
                    {
                        WriteIndented = false,
                        NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowNamedFloatingPointLiterals
                    };
                    json = JsonSerializer.Serialize(payload, jsonOptions);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Failed to serialize payload to JSON.", ex);
                }

                using var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(apiUrl, content);

                // Ensure success or throw exception
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals
                };

                FaceVerificationResponse result =
                    JsonSerializer.Deserialize<FaceVerificationResponse>(jsonResponse, options)
                    ?? throw new InvalidOperationException("Empty response from API");

                return result;
            } catch (Exception e)
            {
                throw;
            }
        }
    }

    public class payloadClass1 {
        public List<byte[]> images { get; set; }
    }
    public class payloadClass2 {
        public string embedding { get; set; }
        public string image { get; set; }
    }

    public class apiResponse
    {
        public string status { get; set; }
        public string embedding { get; set; }
        public int num_faces_processed { get; set; }
    }
}
