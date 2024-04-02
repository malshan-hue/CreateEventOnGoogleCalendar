using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace GoogleMeetScheduler
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] scopes = { CalendarService.Scope.Calendar };
            string applicationName = "Google Calendar API .NET Quickstart";

            UserCredential credential;

            using (var stream =
                new FileStream("client_secret_1003961439429-f776l0aqrfb2ep6j17qq6bm3dvc8nfji.apps.googleusercontent.com.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }

            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = applicationName,
            });

            Event newEvent = new Event()
            {
                Summary = "Meeting Title",
                Location = "Online",
                Description = "Meeting Description",
                Start = new EventDateTime()
                {
                    DateTime = DateTime.Now.AddHours(1),
                    TimeZone = "Asia/Colombo"
                },
                End = new EventDateTime()
                {
                    DateTime = DateTime.Now.AddHours(2), 
                    TimeZone = "Asia/Colombo"
                },
                Attendees = new List<EventAttendee>()
                {
                    new EventAttendee() { Email = "malshanrathnayake125@gmail.com" },
                    new EventAttendee() { Email = "kisura2003@gmail.com" }
                },
                ConferenceData = new ConferenceData()
                {
                    CreateRequest = new CreateConferenceRequest()
                    {
                        RequestId = Guid.NewGuid().ToString(),
                        ConferenceSolutionKey = new ConferenceSolutionKey()
                        {
                            Type = "hangoutsMeet"
                        }
                    }
                }
            };

            try
            {
                EventsResource.InsertRequest request = service.Events.Insert(newEvent, "primary");
                request.ConferenceDataVersion = 1;
                Event createdEvent = request.Execute();

                string meetUrl = createdEvent.ConferenceData.EntryPoints.FirstOrDefault().Uri;

                Console.WriteLine($"Event created: {createdEvent.HtmlLink}");
                Console.WriteLine($"Meeting url: {meetUrl}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

        }
    }
}
