using System.Text.Json.Serialization;

namespace AttendanceAPI.Contracts.Response
{
    public class FaceVerificationResponse
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("match")]
        public bool Match { get; set; }

        [JsonPropertyName("best_confidence")]
        public float BestConfidence { get; set; }

        [JsonPropertyName("total_faces_detected")]
        public int TotalFacesDetected { get; set; }

        [JsonPropertyName("results")]
        public List<FaceMatchResult> Results { get; set; }
    }

    public class FaceMatchResult
    {
        [JsonPropertyName("box")]
        public float[] Box { get; set; }

        [JsonPropertyName("confidence")]
        public float Confidence { get; set; }

        [JsonPropertyName("match")]
        public bool Match { get; set; }
    }
}
