using Serilog;
using SmartHome.Stardog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using VDS.RDF.Query;
using VDS.RDF.Storage;
using Action=SmartHome.Stardog.Models.Action;

namespace SmartHome.Stardog.Services
{
    public class ThingsService : StardogService
    {
        public ThingsService(StardogData data, ILogger logger) : base(data, logger)
        {
        }

        public Thing GetById(string thingIRI)
        {
            try
            {
                var connector = GetStardogConnector();
                var query = $"SELECT DISTINCT *  {{?thing a  td:Thing; " +
                    $"fibo:owner ?ownerId; " +
                    $"td:title ?title; " +
                    $"td:description ?description; " +
                    $"td:id ?id; " +
                    $"FILTER(STR(?thing) = '{thingIRI}')}}";
                var result = (SparqlResultSet)connector.Query(query);
                var thing= AssembleQueryResultThings(result).FirstOrDefault();
                if(thing!=null)
                {
                    thing.properties = GetThingProperties(thing.IRI);
                    thing.actions = GetThingActions(thing.IRI);
                    foreach(var action in thing.actions)
                    {
                        action.parameters = GetActionParameters(action.IRI);
                    }
                }
                return thing;
            }
            catch (Exception e)
            {
                _logger.Error($"Could not retrieve user with id {thingIRI}", e);
                return null;
            }
        }

        public List<Thing> GetAll()
        {
            try
            {
                var connector = GetStardogConnector();
                var query = $"SELECT DISTINCT *{{" +
                    $"?thing a  td:Thing; " +
                    $"fibo:owner ?ownerId; " +
                    $"td:title ?title; " +
                    $"td:description ?description; " +
                    $"td:id ?id}}";
                var result = (SparqlResultSet)connector.Query(query);
                var things = AssembleQueryResultThings(result);
                foreach (var thing in things)
                {
                    if (thing != null)
                    {
                        thing.properties = GetThingProperties(thing.IRI);
                        thing.actions = GetThingActions(thing.IRI);
                        foreach (var action in thing.actions)
                        {
                            action.parameters = GetActionParameters(action.IRI);
                        }
                    }
                }
                return things;
            }
            catch (Exception e)
            {
                _logger.Error($"Could not retrieve things", e);
                return null;
            }
        }

        public List<Thing> GetByOwner(string userId)
        {
            try
            {
                var connector = GetStardogConnector();
                var query = $"SELECT DISTINCT *{{" +
                    $"?thing a  td:Thing; " +
                    $"fibo:owner ?ownerId; " +
                    $"td:title ?title; " +
                    $"td:description ?description; " +
                    $"td:id ?id" +
                    $" FILTER(EXISTS{{?thing fibo:owner <{GetUserObjectUrl(_data.BaseObjectUrl, userId)}> }})}}";
                var result = (SparqlResultSet)connector.Query(query);
                var things = AssembleQueryResultThings(result);
                foreach (var thing in things)
                {
                    if (thing != null)
                    {
                        thing.properties = GetThingProperties(thing.IRI);
                        thing.actions = GetThingActions(thing.IRI);
                        foreach (var action in thing.actions)
                        {
                            action.parameters = GetActionParameters(action.IRI);
                        }
                    }
                }
                return things;
            }
            catch (Exception e)
            {
                _logger.Error($"Could not retrieve things owned by {userId}", e);
                return null;
            }
        }

        public List<Thing> GetAccesible(string userId)
        {
            try
            {
                var connector = GetStardogConnector();
                var query = $"SELECT DISTINCT *{{" +
                    $"?thing a  td:Thing; " +
                    $"fibo:owner ?ownerId; " +
                    $"td:title ?title; " +
                    $"td:description ?description; " +
                    $"td:id ?id" +
                    $" FILTER(EXISTS{{?thing fibo:owner <{GetUserObjectUrl(_data.BaseObjectUrl, userId)}> }} || EXISTS{{<{GetUserObjectUrl(_data.BaseObjectUrl, userId)}> foaf:member ?group. ?group foaf:topic_interest ?thing}})}}";
                var result = (SparqlResultSet)connector.Query(query);
                var things = AssembleQueryResultThings(result);
                foreach (var thing in things)
                {
                    if (thing != null)
                    {
                        thing.properties = GetThingProperties(thing.IRI);
                        thing.actions = GetThingActions(thing.IRI);
                        foreach (var action in thing.actions)
                        {
                            action.parameters = GetActionParameters(action.IRI);
                        }
                    }
                }
                return things;
            }
            catch (Exception e)
            {
                _logger.Error($"Could not retrieve things owned by {userId}", e);
                return null;
            }
        }

        public List<ThingViewModel> GetForGroup(string groupId,bool inGroup)
        {
            try
            {
                var connector = GetStardogConnector();
                var query = $"SELECT DISTINCT *{{" +
                    $"?thing a  td:Thing; " +
                    $"td:title ?title; " +
                    $"td:description ?description; " +
                    $"FILTER({(inGroup?"":"NOT ")}EXISTS{{<{GetGroupObjectUrl(_data.BaseObjectUrl, groupId)}> foaf:topic_interest ?thing}})}}";
                var result = (SparqlResultSet)connector.Query(query);
                var things = AssembleQueryResultThingView(result);
                return things;
            }
            catch(Exception e)
            {
                _logger.Error("Could not return things for group",e);
                return new List<ThingViewModel>();
            }
        }

        public List<Property> GetThingProperties(string thingIRI)
        {
            try
            {
                var connector = GetStardogConnector();
                var query = $"SELECT DISTINCT *  {{" +
                    $"<{thingIRI}> td:hasPropertyAffordance ?property. " +
                    $"?property a td:PropertyAffordance;" +
                    $"td:title ?title; " +
                    $"td:description ?description; " +
                    $"hctl:hasOperationType ?type;" +
                    $"jsonschema:readOnly ?readOnly." +
                    $"OPTIONAL{{?property jsonschema:minimum ?minimum;jsonschema:maximum ?maximum.}}}}";
                var result = (SparqlResultSet)connector.Query(query);
                return AssembleQueryResultProperty(result);
            }
            catch (Exception e)
            {
                _logger.Error($"Could not retrieve user with id {thingIRI}", e);
                return null;
            }
        }

        public List<Action> GetThingActions(string thingIRI)
        {
            try
            {
                var connector = GetStardogConnector();
                var query = $"SELECT DISTINCT *  {{" +
                    $"<{thingIRI}> td:hasActionAffordance ?action. " +
                    $"?action a td:ActionAffordance;" +
                    $"td:title ?title; " +
                    $"td:description ?description;" +
                    $"hctl:hasOperationType ?type}}";
                var result = (SparqlResultSet)connector.Query(query);
                return AssembleQueryResultActions(result);
            }
            catch (Exception e)
            {
                _logger.Error($"Could not retrieve user with id {thingIRI}", e);
                return null;
            }
        }

        public List<Parameter> GetActionParameters(string actionIRI)
        {
            try
            {
                var connector = GetStardogConnector();
                var query = $"SELECT DISTINCT *  {{" +
                    $"<{actionIRI}> td:uriVariables ?parameter. " +
                    $"?parameter td:name ?name;" +
                    $"hctl:hasOperationType ?type." +
                    $"OPTIONAL{{?parameter jsonschema:minimum ?minimum;jsonschema:maximum ?maximum.}}}}";
                var result = (SparqlResultSet)connector.Query(query);
                return AssembleQueryResultParameter(result);
            }
            catch (Exception e)
            {
                _logger.Error($"Could not retrieve user with id {actionIRI}", e);
                return null;
            }
        }

        public Thing AddThing(Thing thing)
        {
            StardogConnector connector = null;
            try
            {
                connector = GetStardogConnector();
                connector.Begin();
                var query = $"INSERT DATA{{<{thing.IRI}> a td:Thing; fibo:owner <{GetUserObjectUrl(_data.BaseObjectUrl,thing.ownerId)}>; td:title '{thing.title}';td:description '{thing.description}';td:id '{thing.id}'}}";
                connector.Update(query);
                foreach(var property in thing.properties)
                {
                    if(AddThingProperty(property,thing.IRI,connector)==null)
                    {
                        connector.Rollback();
                        return null;
                    }
                }
                foreach (var action in thing.actions)
                {
                    if (AddThingAction(action, thing.IRI, connector) == null)
                    {
                        connector.Rollback();
                        return null;
                    }
               }
                connector.Commit();
                return thing;
            }
            catch (Exception e)
            {
                _logger.Error("Could not add User", e);
                connector?.Rollback();
                return null;
            }
        }

        private Property AddThingProperty(Property property, string thingId, StardogConnector connector)
        {
            try
            {
                var query = $"INSERT DATA{{<{thingId}> td:hasPropertyAffordance <{property.IRI}>. " +
                    $"<{property.IRI}> a td:PropertyAffordance; " +
                    $"td:title '{property.title}'; " +
                    $"td:description '{property.description}'; " +
                    $"td:readProperty '{property.readOnly}'; " +
                    $"hctl:hasOperationType '{property.type}';" +
                    $"jsonschema:readOnly '{property.readOnly}'";
                if(property.maximum!=property.minimum && (!string.IsNullOrEmpty(property.maximum) || !string.IsNullOrEmpty(property.minimum)))
                {
                    query += ";" +
                        $"jsonschema:minimum '{property.minimum}';" +
                        $"jsonschema:maximum '{property.maximum}'}}";
                }
                else
                {
                    query += ".}";
                }
                connector.Update(query);
                return property;
            }
            catch (Exception e)
            {
                _logger.Error("Could not add User", e);
                return null;
            }
        }

        public Action AddThingAction(Action action, string thingId, StardogConnector connector)
        {
            try
            {
                var query = $"INSERT DATA{{<{thingId}> td:hasActionAffordance <{action.IRI}>. " +
                                    $"<{action.IRI}> a td:ActionAffordance; " +
                                    $"td:title '{action.title}'; " +
                                    $"td:description '{action.description}';" +
                                    $"hctl:hasOperationType '{action.type}'}}";
                connector.Update(query);
                foreach(var parameter in action.parameters)
                {
                    if(AddParameter(parameter, action.IRI, connector)==null)
                    {
                        return null;
                    }
                }
                return action;
            }
            catch (Exception e)
            {
                _logger.Error("Could not add User", e);
                return null;
            }
        }

        public Parameter AddParameter(Parameter parameter, string actionId, StardogConnector connector)
        {
            try
            {
                var query = $"INSERT DATA{{<{actionId}> td:uriVariables <{actionId}/{parameter.name}>.<{actionId}/{parameter.name}> td:name '{parameter.name}';hctl:hasOperationType '{parameter.type}'";
                if (parameter.maximum != parameter.minimum && (!string.IsNullOrEmpty(parameter.maximum) || !string.IsNullOrEmpty(parameter.minimum)))
                {
                    query += ";" +
                        $"jsonschema:minimum '{parameter.minimum}';" +
                        $"jsonschema:maximum '{parameter.maximum}'}}";
                }
                else
                {
                    query += "}";
                }
                connector.Update(query);
                return parameter;
            }
            catch (Exception e)
            {
                _logger.Error("Could not add User", e);
                return null;
            }
        }

        private List<Thing> AssembleQueryResultThings(SparqlResultSet resultSet)
        {
            var resultConcepts = new List<Thing>();
            foreach (var result in resultSet)
            {

                resultConcepts.Add(AssembleResultThings(result));
            }
            return resultConcepts;
        }

        private Thing AssembleResultThings(SparqlResult result)
        {
            var concept = new Thing
            {
                title = result["title"]?.ToString(),
                description = result["description"]?.ToString(),
                id = result["id"]?.ToString(),
                IRI = result["thing"]?.ToString()
            };
            return concept;
        }

        private List<Property> AssembleQueryResultProperty(SparqlResultSet resultSet)
        {
            var resultConcepts = new List<Property>();
            foreach (var result in resultSet)
            {

                resultConcepts.Add(AssembleResultProperty(result));
            }
            return resultConcepts;
        }

        private Property AssembleResultProperty(SparqlResult result)
        {
            var concept = new Property
            {
                title = result["title"]?.ToString(),
                description = result["description"]?.ToString(),
                type = result["type"]?.ToString(),
                IRI = result["property"]?.ToString(),
                maximum=result["maximum"]?.ToString(),
                minimum=result["minimum"]?.ToString(),
                readOnly=result["readOnly"]?.ToString()
            };
            return concept;
        }

        private List<Action> AssembleQueryResultActions(SparqlResultSet resultSet)
        {
            var resultConcepts = new List<Action>();
            foreach (var result in resultSet)
            {

                resultConcepts.Add(AssembleResultActions(result));
            }
            return resultConcepts;
        }

        private Action AssembleResultActions(SparqlResult result)
        {
            var concept = new Action
            {
                title = result["title"]?.ToString(),
                description = result["description"]?.ToString(),
                IRI = result["action"]?.ToString(),
                type=result["type"]?.ToString()
            };
            return concept;
        }

        private List<Parameter> AssembleQueryResultParameter(SparqlResultSet resultSet)
        {
            var resultConcepts = new List<Parameter>();
            foreach (var result in resultSet)
            {

                resultConcepts.Add(AssembleResultParameter(result));
            }
            return resultConcepts;
        }

        private Parameter AssembleResultParameter(SparqlResult result)
        {
            var concept = new Parameter
            {
                name = result["name"]?.ToString(),
                type = result["type"]?.ToString(),
                maximum = result["maximum"]?.ToString(),
                minimum = result["minimum"]?.ToString()
            };
            return concept;
        }

        private List<ThingViewModel> AssembleQueryResultThingView(SparqlResultSet resultSet)
        {
            var resultConcepts = new List<ThingViewModel>();
            foreach (var result in resultSet)
            {

                resultConcepts.Add(AssembleResultThingView(result));
            }
            return resultConcepts;
        }

        private ThingViewModel AssembleResultThingView(SparqlResult result)
        {
            var concept = new ThingViewModel
            {
                title = result["title"]?.ToString(),
                description = result["description"]?.ToString(),
                validation_url=result["thing"]?.ToString()
            };
            return concept;
        }
    }
}
