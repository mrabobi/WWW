using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartHome.UI.Data
{
    public class Thing
    {
        public string IRI { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string id { get; set; }
        List<Property> properties { get; set; }
        public List<Action> actions { get; set; }
    }

    public class Property
    {
        public string title { get; set; }
        public string type { get; set; } //bool int string 
        public string description { get; set; }
        public string IRI { get; set; }
        public int? Minimum { get; set; }
        public int? Maximum { get; set; }
    }

    public class Action
    {
        public string IRI { get; set; }
        public string title { get; set; }
        public string type { get; set; } //bool int string 
        public string description { get; set; }
        List<Parameter> parameters { get; set; }
    }

    public class Parameter
    {
        public string name { get; set; }
        public string type { get; set; }
    }

    public class ThingViewModel
    {
        public string title { get; set; }
        public string description { get; set; }
        public string validation_url { get; set; }
        public string access_code { get; set; }
    }
}
