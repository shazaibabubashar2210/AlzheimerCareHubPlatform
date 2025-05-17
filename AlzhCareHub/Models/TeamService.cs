using Firebase.Database;
using Firebase.Database.Query;

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

            // Assign Firebase key to TeamMember.Id property
            return teamMembersWithIds.Select(t =>
            {
                var member = t.Object;
                member.Id = t.Key;
                return member;
            }).ToList();
        }



        public async Task<bool> AddTeamMember(TeamMember member)
        {
            try
            {
                var result = await _firebaseClient
                    .Child("Doctors")
                    .PostAsync(member);

                // Save the Firebase key inside the object
                member.Id = result.Key;

                // Update the record with Id now included in the object
                await _firebaseClient
                    .Child("Doctors")
                    .Child(result.Key)
                    .PutAsync(member);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding team member: {ex.Message}");
                return false;
            }
        }

        public async Task<TeamMember> GetTeamMemberById(string id)
        {
            var result = await _firebaseClient.Child("Doctors").Child(id).OnceSingleAsync<TeamMember>();
            return result;
        }

        public async Task<bool> UpdateTeamMember(string id, TeamMember member)
        {
            try
            {
                await _firebaseClient
                    .Child("Doctors")
                    .Child(id)
                    .PutAsync(member);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating team member: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteTeamMember(string id)
        {
            try
            {
                await _firebaseClient.Child("Doctors").Child(id).DeleteAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting team member: {ex.Message}");
                return false;
            }
        }



    }
}
