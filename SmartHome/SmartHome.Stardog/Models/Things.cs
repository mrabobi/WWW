using System.Collections.Generic;

namespace SmartHome.Stardog.Models
{
    public class Thing
    {
        public string IRI { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string id { get; set; }
        public string ownerId { get; set; }
        public List<Property> properties { get; set; }
        public List<Action> actions { get; set; }
    }

    public class Property
    {
        public string title { get; set; }
        public string type { get; set; } //boolean int string 
        public string description { get; set; }
        public string IRI { get; set; }
        public string readOnly { get; set; }
        public string minimum { get; set; }
        public string maximum { get; set; }
    }

    public class Action
    {
        public string IRI { get; set; }
        public string title { get; set; }
        public string type { get; set; } //boolean int string 
        public string description { get; set; }
        public List<Parameter> parameters { get; set; }
    }

    public class Parameter
    {
        public string name { get; set; }
        public string type { get; set; }
        public string minimum { get; set; }
        public string maximum { get; set; }
    }

    public class ThingViewModel
    {
        public string title { get; set; }
        public string description { get; set; }
        public string thingId { get; set; }
        public string validation_url { get; set; }
        public string access_code { get; set; }
    }
}
