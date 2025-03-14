using Firebase.Database;

namespace AlzhCareHub.Models
{
    public class TeamService
    {
        private static string DatabaseUrl = "https://alzheimercarehubsystem-a42e6-default-rtdb.firebaseio.com/";
        private readonly FirebaseClient _firebaseClient;

        public TeamService()
        {
            _firebaseClient = new FirebaseClient(DatabaseUrl);
        }

        public async Task<List<TeamMember>> GetTeamMembers()
        {
            var teamMembersWithIds = await _firebaseClient.Child("Doctors")
                .OnceAsync<TeamMember>();

            return teamMembersWithIds.Select(t => t.Object).ToList(); // Ensure only TeamMember is returned
        }


        public async Task<bool> AddTeamMember(TeamMember member)
        {
            try
            {
                await _firebaseClient
                    .Child("Doctors") // Changed from "teamMembers" to "Doctors"
                    .PostAsync(member);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding team member: {ex.Message}");
                return false;
            }
        }


    }
}
