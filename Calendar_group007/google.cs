﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Newtonsoft.Json;
using System.IO;
using System.Collections;
/// <summary>
/// arthor: Peyman Justin
/// </summary>
namespace PJCalender
{
    /// <summary>
    /// Handles all of the google api functionallity 
    /// </summary>
    public class google
    {
        static string[] Scopes = { CalendarService.Scope.Calendar };
        static string ApplicationName = "PJCalender";
        /// <summary>
        /// This object is not meant to be stored.
        /// Will retrieve events from your account and store them locally 
        /// </summary>
        /// <param name="form">The form calling crating this object</param>
        /// <param name="user">username of user</param>
        public google(Menus form, string user)
        {
            using (var stream = new System.IO.FileStream("client_secret.json", System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                UserCredential credential = null;
                //string credPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                string credPath = (".credentials/currentUser");

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(GoogleClientSecrets.Load(stream).Secrets,
                                                                         Scopes,
                                                                         user,
                                                                         System.Threading.CancellationToken.None,
                                                                         new FileDataStore(credPath, true)).Result;
                // Create Google Calendar API service.
                if (credential == null)
                    return;

                var service = new CalendarService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ApplicationName,
                });

                // Define parameters of request.
                EventsResource.ListRequest request = service.Events.List("primary");
                request.TimeMin = DateTime.Now;
                request.ShowDeleted = false;
                request.SingleEvents = true;
                request.TimeMax = DateTime.Now.AddYears(1); //todo change number to 20
                request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

                try
                {
                    form.loginButtonChangeText();
                    Events events = request.Execute();
                    saveEventLocal(events);
                }
                catch (System.Net.Http.HttpRequestException requestEx)
                {
                    System.Windows.Forms.MessageBox.Show(requestEx.ToString(), requestEx.GetType().ToString());
                }
            }
        }
        /// <summary>
        /// Event creation
        /// </summary>
        /// <param name="sum">Summary</param>
        /// <param name="where">Location of Event</param>
        /// <param name="desc">Description of event</param>
        /// <param name="st">Start date in DateTime</param>
        /// <param name="en">End date in DateTime</param>
        /// <param name="re">Recurrence of the event</param>
        static public void createEvent(String sum,
            String where,
            String desc,
            DateTime st,
            DateTime en,
            String[] re)
        {
            using (var stream = new System.IO.FileStream("client_secret.json", System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                UserCredential credential = null;
                //string credPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                string credPath = (".credentials/currentUser");
                string user = "";
                try {
                    user = (Directory.GetFiles(".credentials/currentUser", "*")[0]).Split('-')[1];
                }catch(Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show("Login required before creating an event");
                    return;
                } 
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(GoogleClientSecrets.Load(stream).Secrets,
                                                                         Scopes,
                                                                         user,
                                                                         System.Threading.CancellationToken.None,
                                                                         new FileDataStore(credPath, true)).Result;
                // Create Google Calendar API service.
                if (credential == null)
                    return;

                var service = new CalendarService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ApplicationName,
                });

                Event newEvent = new Event()
                {
                    Summary = sum,
                    Location = where,
                    Description = desc,
                    Start = new EventDateTime()
                    {
                        DateTime = st,
                        TimeZone = "America/Vancouver",
                    },
                    End = new EventDateTime()
                    {
                        DateTime = en,
                        TimeZone = "America/Vancouver",
                    },
                    Recurrence = re,
                    Reminders = new Event.RemindersData()
                    {
                        UseDefault = true,
                    }
                };

                String calendarId = "primary";
                EventsResource.InsertRequest request = service.Events.Insert(newEvent, calendarId);
                try {
                    Event createdEvent = request.Execute();
                } catch(Exception x)
                {
                    System.Windows.Forms.MessageBox.Show("Failed to create event");
                }
            }
        }
        /// <summary>
        /// todo remake fucntion
        /// </summary>
        /// <returns></returns>
        static public void readEventLocal(Menus form)
        {
            form.events = new ArrayList();
            if (!System.IO.Directory.Exists(".save/currentUser"))
                return;
            string[] files = Directory.GetFiles(@".save/currentUser", "*");
            foreach (String fileName in files)
            {
                using (StreamReader file = File.OpenText(fileName))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    Event calEvent = (Event)serializer.Deserialize(file, typeof(Event));
                    form.events.Add(calEvent);
                }
            }
        }
        /// <summary>
        /// todo remake funciton
        /// </summary>
        /// <param name="events"></param>
        public static void saveEventLocal(Events events)
        {
            if (!System.IO.Directory.Exists(".save/currentUser"))
                System.IO.Directory.CreateDirectory(".save/currentUser");

            DatabaseDataSet dbds = new DatabaseDataSet();
            DatabaseDataSet.EventDataDataTable eddt = dbds.EventData;
            var r = eddt.Columns;
            foreach (var eventItem in events.Items)
            {
                try
                {
                    String id = eventItem.Id;
                    DateTime dt = (DateTime)eventItem.Start.DateTime;
                    String time = "";
                    String data = eventItem.ToString();
                    if (((DateTime)eventItem.Start.DateTime).ToLongTimeString() != null)
                        time = ((DateTime)eventItem.Start.DateTime).ToLongTimeString();

                    DatabaseDataSet.EventDataRow edr = eddt.NewEventDataRow();
                    edr[0] = id;
                    edr[1] = dt;
                    edr[2] = time;
                    edr[3] = data;

                    eddt.Rows.Add(edr);

                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.ToString(), ex.GetType().ToString());
                }
            }

            dbds.Tables.Add(eddt);
        }
    }
}
