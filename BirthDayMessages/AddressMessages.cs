using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper.Configuration;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using Microsoft.Extensions.Logging;

namespace BirthDayMessages
{
    public class Addresses
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string HomePhone { get; set; }
        public string MobilePhone { get; set; }
        public string streetAddress { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
        public string dateOfBirth { get; set; }
    }
    public class AddressesMap : ClassMap<Addresses>
    {
        public AddressesMap()
        {
            Map(x => x.FirstName).Name("First Name");
            Map(x => x.LastName).Name("Last Name");
            Map(x => x.HomePhone).Name("Home Phone");
            Map(x => x.MobilePhone).Name("Mobile Phone");
            Map(x => x.streetAddress).Name("Street Address");
            Map(x => x.city).Name("City");
            Map(x => x.state).Name("State");
            Map(x => x.zip).Name("Zip");
            Map(x => x.dateOfBirth).Name("Date of Birth");
        }
    }

    public class AddressMessages
    {
        private List<Addresses> EntireList;
        private ILogger log;

        string accountSid = Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID");
        string authToken = Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN");
        private string fromFirstName = Environment.GetEnvironmentVariable("FROM_FIRST_NAME");
        private string fromLastName = Environment.GetEnvironmentVariable("FROM_LAST_NAME");
        private string fromNumber = Environment.GetEnvironmentVariable("FROM_NUMBER");
        private string callBackPhoneNumber = Environment.GetEnvironmentVariable("MY_PHONE_NUMBER");
        

        public AddressMessages(string fileName, ILogger Ilog)
        {
            Twilio.TwilioClient.Init(accountSid, authToken);
            log = Ilog;

            EntireList = Read(fileName);
        }

        public List<Addresses> Read(string File)
        {
            FileInfo inputFile = new FileInfo(File);
            using (var reader = new StreamReader(inputFile.FullName))
            using (var csvReader = new CsvReader(reader, System.Globalization.CultureInfo.CurrentCulture))
            {
                csvReader.Configuration.RegisterClassMap<AddressesMap>();
                var addressList = csvReader.GetRecords<Addresses>().ToList();

                return addressList;
            }
        }

        public void sendBirthDayMessages()
        {
            int currentMonth = DateTime.Now.Month;
            foreach (var contact in EntireList)
            {
                if (contact.dateOfBirth != "")
                {
                    try
                    {
                        var dateTime = DateTime.Parse(contact.dateOfBirth);
                        if (dateTime.Month == currentMonth)
                        {
                            if (contact.MobilePhone != null && contact.MobilePhone != "")
                            {
                                GenerateMessages(contact.FirstName, contact.MobilePhone);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        log.LogInformation($"{DateTime.Now} : {ex.ToString()}");
                    }
                }
            }
        }

        public void GenerateMessages(string firstName, string contactPhoneNum)
        {
            string message = String.Format("Happy Birthday {0} from {1} {2}! Call me at {3} to plan a lunch sometime.", firstName, fromFirstName, fromLastName, callBackPhoneNumber);

            try
            {
                var to = new Twilio.Types.PhoneNumber(contactPhoneNum);
                var from = new Twilio.Types.PhoneNumber(fromNumber);

                var messages = MessageResource.Create(
                    to: to,
                    from: from,
                    body: message);

            }
            catch (Exception ex)
            {
                log.LogInformation($"{DateTime.Now} : {ex.ToString()}");
            }
        }
    }
}
