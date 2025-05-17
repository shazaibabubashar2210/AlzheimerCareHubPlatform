using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Threading.Tasks;

namespace AlzhCareHub.Models
{
    public class FireBaseContactHelper
    {
        private static string DatabaseUrl = "https://alzheimercarehubsystem-a42e6-default-rtdb.firebaseio.com/";
        private static FirebaseClient firebaseClient = new FirebaseClient(DatabaseUrl);

        // Saving Contact Details to Database
        public static async Task<bool> SaveContact(ContactModel contact)
        {
            try
            {
                Console.WriteLine($"Saving contact: {contact.FirstName} {contact.LastName}");

                // Using PostAsync to add a new entry
                await firebaseClient
                    .Child("Contacts")
                    .PostAsync(contact);

                Console.WriteLine("Contact saved successfully");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving contact: {ex.Message}");
                return false;
            }
        }

        // Fetch All Contact from the DataBase
        public static async Task<List<ContactModel>> GetAllContacts()
        {
            try
            {
                var contacts = await firebaseClient
                    .Child("Contacts")
                    .OnceAsync<ContactModel>();

                return contacts.Select(item => item.Object).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching contacts: {ex.Message}");
                return new List<ContactModel>();
            }
        }

    }
}
