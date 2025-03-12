using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlzhCareHub.Models
{
    public class FirebaseService
    {
        private static string DatabaseUrl = "https://alzheimercarehubsystem-a42e6-default-rtdb.firebaseio.com/";
        private static FirebaseClient firebaseClient = new FirebaseClient(DatabaseUrl);

        public static async Task<bool> SaveTask(TaskModel task)
        {
            try
            {
                task.Id = Guid.NewGuid().ToString();
                await firebaseClient
                    .Child("Tasks")
                    .Child(task.Id)
                    .PutAsync(task);

                EmailScheduler.ScheduleEmail(task);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static async Task<List<TaskModel>> GetTasks()
        {
            var tasks = await firebaseClient
                .Child("Tasks")
                .OnceAsync<TaskModel>();

            return tasks.Select(t => new TaskModel
            {
                Id = t.Key,
                Title = t.Object.Title,
                Email = t.Object.Email,
                Date = t.Object.Date,
                Time = t.Object.Time,
                Priority = t.Object.Priority,
                Description = t.Object.Description
            }).ToList();
        }

        public static async Task DeleteTask(string id)
        {
            await firebaseClient
                .Child("Tasks")
                .Child(id)
                .DeleteAsync();
        }
        public static async Task<TaskModel> GetTaskById(string id)
        {
            var task = await firebaseClient
                .Child("Tasks")
                .Child(id)
                .OnceSingleAsync<TaskModel>();

            if (task == null) return null;

            return new TaskModel
            {
                Id = id,
                Title = task.Title,
                Email = task.Email,
                Date = task.Date,
                Time = task.Time,
                Priority = task.Priority,
                Description = task.Description
            };
        }

        public static async Task<bool> UpdateTask(TaskModel task)
        {
            try
            {
                await firebaseClient
                    .Child("Tasks")
                    .Child(task.Id)
                    .PutAsync(task);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}
