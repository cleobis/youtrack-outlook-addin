using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouTrackSharp;
using Email_to_YouTrack;
using Newtonsoft.Json;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hi");

            var json = new JsonIssue();
            json.project.id = "1-0";
            json.summary = "summary";
            json.description = "";
            json.customFields.Add(new JsonCustomField("Priority", "SingleEnumIssueCustomField", "Critical"));
            Console.WriteLine(JsonConvert.SerializeObject(json));

            Console.WriteLine(json.customFields[0].value.name);

            Console.ReadKey();
        }
    }
}
