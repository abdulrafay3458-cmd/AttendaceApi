using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

namespace AttendanceAPI.Utilities
{
    public class FirebaseInitializer
    {
        private readonly string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        public static void Initialize(IWebHostEnvironment env)
         {
            if (FirebaseApp.DefaultInstance != null)
                return;
            FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromFile(
                    Path.Combine(env.ContentRootPath, "Utilities", "AttendanceAppSK.json")
                )
            });
        }
    }
}
