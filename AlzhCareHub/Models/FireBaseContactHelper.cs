using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlzhCareHub.Models
{
    public class FireBaseContactHelper
    {
        private static string DatabaseUrl = "https://alzheimercarehubsystem-a42e6-default-rtdb.firebaseio.com/";
        private static FirebaseClient firebaseClient = new FirebaseClient(DatabaseUrl);

        // Saving Contact Details to DataBase

        public static async Task<bool> SaveContact(ContactModel contact)
        {
            try
            {
                Console.WriteLine($"Saving contact: {contact.FirstName} {contact.LastName}");
                await firebaseClient
                    .Child("Contacts")
                    .PutAsync(contact);
                Console.WriteLine("Contact saved successfully");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving contact: {ex.Message}");
                return false;
            }
        }

    }
}
