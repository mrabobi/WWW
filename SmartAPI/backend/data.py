from backend.actions_implementation import turnOnHeating_function, isSomeoneInside, unlockForSeconds, suggestTemperature, fade_action

PORT = 5010
devices = dict()

ledProperties = {
    "brightness": {"description": "The brightness level of the led", "readOnly": "false", "minimum": "0", "maximum": "100", "type": "int",
                   "value": "0"},
    "status": {"description": "Turn On/Off LED", "minimum": "0", "readOnly": "false", "maximum": "1", "type": "boolean", "value": "false"}}

thermostatProperties = {
    "currentTemperature": {"description": "The current temperature in the room.", "readOnly": "true", "minimum": "0",
                           "maximum": "100", "type": "int", "value": "0"},
    "minimumTemperature": {"description": "The minimum temperature required for the heating to start.",
                           "readOnly": "false", "minimum": "0", "maximum": "100", "type": "int", "value": "0"},
    "status": {"description": "Turn the thermostat On/Off.", "readOnly": "false", "minimum": "0", "maximum": "1",
               "type": "boolean", "value": "false"}}

lockProperties = {
    "status": {"description": "Turn the lock On/Off.", "readOnly": "false", "minimum": "0", "maximum": "1",
               "type": "boolean", "value": "false"},
    "roomName": {"description": "The name of the room where the lock is placed.", "readOnly": "false", "minimum": "0",
                 "maximum": "100", "type": "string", "value": ""}
}

ledActions = {
    "fade": {"description": "Dims the light", "type": "void", "parameters": []}
    }

lockActions = {
    "isSomeoneInside": {"description": "Tells if there are people in the room.", "type": "boolean", "parameters": []},
    "unlockForSeconds": {"description": "Unlock the room for x seconds.", "type": "void", "parameters": [
        {"name": "duration", "type": "int", "minimum": "15", "maximum": "180"}]}
}

thermostatActions = {
    "turnOnHeating": {"description": "Turn on heating for a number of minutes", "type": "void", "parameters": [
        {"name": "duration", "type": "int", "minimum": "15", "maximum": "180"}
    ]},
    "suggestTemperature": {
        "description": "Suggests the ideal temperature based on the outside temperature.",
        "type": "int", "parameters": []
    }
}

map_functions = {
    "turnOnHeating": turnOnHeating_function,
    "isSomeoneInside": isSomeoneInside,
    "unlockForSeconds": unlockForSeconds,
    "suggestTemperature": suggestTemperature,
    "fade": fade_action

}

ledDescription = "An intelligent led"
thermostatDescription = "An intelligent thermostat"
lockDescription = "An intelligent lock"
typeDevices = {'LED': ledProperties, 'Thermostat': thermostatProperties, 'Lock': lockProperties}
actionDevices = {'LED': ledActions, 'Thermostat': thermostatActions, 'Lock': lockActions}
descriptionDevices = {'LED': ledDescription, 'Thermostat': thermostatDescription, 'Lock': lockDescription}

templateDevice = {
    "IRI": "",
    "title": "",
    "description": "",
    "id": "",
    "access_code": "",
    "connected": "False",
    "properties": [
    ],
    "actions": [
    ]
}


def getDevices():
    return devices


def getTypeDevices():
    return list(typeDevices.keys())


def getTemplateDevice():
    return templateDevice
