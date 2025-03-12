using AlzhCareHub.Models;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Timers;  // Keep this for using System.Timers.Timer

public class EmailScheduler
{
    private static List<System.Timers.Timer> timers = new List<System.Timers.Timer>(); // Explicit namespace

    public static void ScheduleEmail(TaskModel task)
    {
        DateTime taskDateTime = DateTime.Parse(task.Date + " " + task.Time);
        TimeSpan timeUntilAlert = taskDateTime - DateTime.Now; // No -15 minutes adjustment

        if (timeUntilAlert.TotalMilliseconds > 0)
        {
            System.Timers.Timer timer = new System.Timers.Timer(timeUntilAlert.TotalMilliseconds);
            timer.Elapsed += async (sender, e) =>
            {
                await SendEmail(task);
                timer.Dispose(); // Free resources after execution
            };
            timer.AutoReset = false;
            timer.Start();
            timers.Add(timer);
        }
    }

    private static async Task SendEmail(TaskModel task)
    {
        try
        {
            Console.WriteLine($"Sending email to {task.Email} for task: {task.Title}");

            MailMessage mail = new MailMessage();
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");

            mail.From = new MailAddress("shazaibabubashar@gmail.com");
            mail.To.Add(task.Email);
            mail.Subject = "Task Reminder: " + task.Title;
            mail.Body = $"Reminder: {task.Description}\nPriority: {task.Priority}\nTime: {task.Time}";

            smtpClient.Port = 587;  // Changed from 465
            smtpClient.Credentials = new System.Net.NetworkCredential("shazaibabubashar@gmail.com", "zkmo mgbv wnvt iemo");
            smtpClient.EnableSsl = true;

            await smtpClient.SendMailAsync(mail);
            Console.WriteLine("Email sent successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Email Error: " + ex.Message);
        }
    }

}
