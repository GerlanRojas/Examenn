using Firebase.Auth.Providers;
using Firebase.Auth;

namespace Examenn.Firebase
{
    public static class FirebaseAuthHelper
    {
        public const string firebaseAppId = "bdexamenn";
        public const string firebaseApiKey = "AIzaSyA3M9YJ_9FSHOLwpoQC_Le6WNHlH-wCdnw";

        public static FirebaseAuthClient setFirebaseAuthClient()
        {
            var auth = new FirebaseAuthClient(new FirebaseAuthConfig()
            {
                ApiKey = firebaseApiKey,
                AuthDomain = $"{firebaseAppId}.firebaseapp.com",
                Providers = new FirebaseAuthProvider[]
                {
                    new EmailProvider()
                }
            });

            return auth;
        }
    }
}