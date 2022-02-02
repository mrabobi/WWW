using Microsoft.AspNetCore.Components;
using MudBlazor;
using SmartHome.UI.ApiClients;
using SmartHome.UI.Data;
using SmartHome.UI_Auth.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Action = SmartHome.UI.Data.Action;

namespace SmartHome.UI.Pages
{
    public partial class Devices
    {
        [Inject]
        public ThingsApiClient ApiClient { get; set; }
        [Inject]
        public ISnackbar SnackBar { get; set; }
        public List<Thing> Things { get; set; }
        public bool DataIsLoading { get; set; }
        public Dictionary<string, Dictionary<string, string>> ThingsPropertyValues { get; set; }
        public Dictionary<string, Dictionary<string, string>> ThingsPropertySetValues { get; set; }
        public Dictionary<string, string> ParameterValues { get; set; }
        public object Value { get; set; }
        protected override async Task OnInitializedAsync()
        {
            DataIsLoading = true;
            Things = await ApiClient.GetAccesibleThings(AppState.CurrentUser.UserId);
            ThingsPropertyValues = new Dictionary<string, Dictionary<string, string>>();
            ThingsPropertySetValues = new Dictionary<string, Dictionary<string, string>>();
            ParameterValues = new Dictionary<string, string>();

            foreach (var thing in Things)
            {
                ThingsPropertyValues[thing.id] = new Dictionary<string, string>();
                ThingsPropertySetValues[thing.id] = new Dictionary<string, string>();
                foreach (var property in thing.properties)
                {
                    if (property.IRI != null)
                    {
                        ThingsPropertyValues[thing.id][property.title] = await ApiClient.ReadProperty(property.IRI);
                        ThingsPropertySetValues[thing.id][property.title] = "";
                    }
                }
                foreach(var action in thing.actions)
                {
                    foreach(var parameter in action.parameters)
                    {
                        ParameterValues[thing.id + parameter.name] = "";
                    }
                }
            }

            DataIsLoading = false;
        }
        public async Task SetValue(string thingId, Property property)
        {
            DataIsLoading = true;
            var value = ThingsPropertySetValues[thingId][property.title]?.ToString() ?? "";
            if (property.type == "int" && !int.TryParse(value, out _))
            {
                SnackBar.Add($"Invalid value for property {property.title}", Severity.Error);
                DataIsLoading = false;
                return;
            }
            var success = await ApiClient.WriteProperty(property.IRI, value);
            if (success)
            {
                SnackBar.Add("Property Set", Severity.Success);
                ThingsPropertyValues[thingId][property.title] = await ApiClient.ReadProperty(property.IRI);
            }
            else
            {
                SnackBar.Add("Could not set property", Severity.Error);
            }
            DataIsLoading = false;
        }

        public async Task DoAction(string thingId, Action action)
        {
            DataIsLoading = true;
            var parameters = new Dictionary<string, string>();
            foreach (var parameter in action.parameters)
            {
                var intValue = 0;
                if(parameter.type=="int" && !int.TryParse(ParameterValues[thingId + parameter.name],out intValue))
                {
                    SnackBar.Add($"Invalid value for parameter {parameter.name}", Severity.Error);
                    DataIsLoading = false;
                    return;
                }
                else if(int.TryParse(parameter.minimum,out var minimumInt) && int.TryParse(parameter.maximum, out var maximumInt))
                {
                     if(intValue < minimumInt || intValue>maximumInt)
                    {
                        SnackBar.Add($"Invalid value for parameter {parameter.name}", Severity.Error);
                        DataIsLoading = false;
                        return;
                    }
                }
                parameters[parameter.name] = ParameterValues[thingId + parameter.name];
            }
            if (action.type.ToLower() == "void")
            {
                var success = await ApiClient.DoActionVoid(action.IRI, parameters);
                if(success)
                {
                    SnackBar.Add("Action success", Severity.Success);
                }
                else
                {
                    SnackBar.Add("Action failed", Severity.Error);
                }

            }
            else
            {
                var result = await ApiClient.DoAction(action.IRI, parameters);
                if(result==null)
                {
                    SnackBar.Add("Action failed", Severity.Error);

                }
                else
                {
                    SnackBar.Add($"Action success, result:{result}", Severity.Success);
                }
            }
            DataIsLoading = false;
        }
    }
}
