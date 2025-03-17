using Firebase;
using Google.Cloud.Firestore;
using System.Xml.Linq;
using Examenn.Firebase;

namespace Examenn.Models
{
    public class UserModel
    {
        internal string name;

        public string uuid { get; set; }
        public String Email { get; set; }
        public string email { get; internal set; }
        public String Name { get; set; }

       public int Saldo { get; set; }
    }

    public class UserHelper
    {
        public async Task<UserModel> getUserInfo(string email)
        {
            Query query = FirestoreDb.Create(FirebaseAuthHelper.firebaseAppId).Collection("User").WhereEqualTo("email", email);
            QuerySnapshot querySnapshot = await query.GetSnapshotAsync();

            Dictionary<string, object> data = querySnapshot.Documents[0].ToDictionary();

            UserModel user = new UserModel
            {
                Email = data["email"].ToString(),
                Name = data["name"].ToString(),
                Saldo = Convert.ToInt32(data["saldo"]),
            };

            return user;
        }
    }
}