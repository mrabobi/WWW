from tkinter import *
from tkinter.ttk import Treeview
import pyperclip

from backend.data import getDevices, getTypeDevices, getTemplateDevice, PORT, typeDevices, descriptionDevices, \
    actionDevices
import random
import uuid
from tkinter import font as tkFont

# Settings

window = Tk()
window.title("SmartHome Simulator")
window.geometry('915x585')
window.resizable(False, False)
TreeviewDevices_frame = Frame(window)
TreeviewDevices = Treeview(TreeviewDevices_frame)


######### FUNCTIONS ##########

def createDevice(deviceName):
    var = getDevices()
    new_id = deviceName + "-" + str(uuid.uuid4())
    access_key = "".join([str(random.randint(0, 9)) for i in range(0, 6)])

    while new_id in var.keys():
        new_id = deviceName + "-" + str(uuid.uuid4())

    # Generate device
    var[new_id] = dict(getTemplateDevice())

    # Properties
    propertiesList = []
    for prop in typeDevices[deviceName].keys():
        propDict = dict()
        propDict['title'] = str(prop)

        for item in typeDevices[deviceName][prop].keys():
            propDict[item] = typeDevices[deviceName][prop][item]

        propDict['IRI'] = "http://127.0.0.1:" + str(PORT) + "/" + new_id + "/" + str(prop)
        propertiesList.append(propDict)
    var[new_id]['properties'] = propertiesList

    # Actions
    actionList = []
    for prop in actionDevices[deviceName].keys():
        propDict = dict()
        propDict['title'] = str(prop)
        propDict['IRI'] = "http://127.0.0.1:" + str(PORT) + "/" + new_id + "/" + str(prop)

        for item in actionDevices[deviceName][prop].keys():
            propDict[item] = actionDevices[deviceName][prop][item]

        actionList.append(propDict)
    var[new_id]['actions'] = actionList

    var[new_id]['title'] = str(deviceName)
    var[new_id]['IRI'] = "http://127.0.0.1:" + str(PORT) + "/" + new_id
    var[new_id]['id'] = new_id
    var[new_id]['access_code'] = access_key
    var[new_id]['description'] = str(descriptionDevices[deviceName])
    update_TreeviewDevices()
    print(var)


def update_TreeviewDevices():
    var = getDevices()
    for item in TreeviewDevices.get_children():
        TreeviewDevices.delete(item)

    for key in var.keys():
        TreeviewDevices.insert(parent='', index='end',
                               values=(var[key]['id'], var[key]['title'], var[key]['connected'],
                                       var[key]['access_code']))
    TreeviewDevices.update()


def copy_iri():
    currentItem = TreeviewDevices.focus()
    if len(currentItem) != 0:
        value = TreeviewDevices.item(currentItem)['values'][0]
        if value != '':
            devices = getDevices()
            pyperclip.copy(devices[value]['IRI'])
            #print(devices[value]['IRI'])


def getWindow():
    return window


######### INTERFACE ##########

top = Frame(window)
top.pack(side=TOP)

spacer = Label(window, text="")

header = Frame(window)
header.pack(side=TOP)
spacer.pack(in_=header, side=BOTTOM)

default_font = tkFont.Font(family='Helvetica', size=30, weight='bold')

LEDButton = Button(window, text="💡 LED", command=lambda: createDevice("LED"), height=5, width=10, padx=10, pady=10)
LEDButton['font'] = default_font
LEDButton.pack(in_=top, side=LEFT)

spacer.pack(in_=top, side=LEFT)

ThermostatButton = Button(window, text="🌡 TEMP", command=lambda: createDevice("Thermostat"), height=5, width=10,
                          padx=10, pady=10)
ThermostatButton['font'] = default_font
ThermostatButton.pack(in_=top, side=LEFT)

spacer.pack(in_=top, side=LEFT)

LockButton = Button(window, text="🔑 LOCK", command=lambda: createDevice("Lock"), height=5, width=10, padx=10, pady=10)
LockButton['font'] = default_font
LockButton.pack(in_=top, side=LEFT)

spacer.pack(in_=top, side=BOTTOM)


TreeviewDevices_frame.pack()
TreeviewDevices['columns'] = ('id', 'title', 'connected', 'code')

TreeviewDevices.column("#0", width=0, stretch=NO)
TreeviewDevices.column("id", anchor=CENTER, width=180)
TreeviewDevices.column("title", anchor=CENTER, width=180)
TreeviewDevices.column("connected", anchor=CENTER, width=180)
TreeviewDevices.column("code", anchor=CENTER, width=180)

TreeviewDevices.heading("id", text="ID", anchor=CENTER)
TreeviewDevices.heading("title", text="Title", anchor=CENTER)
TreeviewDevices.heading("connected", text="Connected", anchor=CENTER)
TreeviewDevices.heading("code", text="Code", anchor=CENTER)


TreeviewDevices.pack()

Refresh_frame = Frame(window)
Refresh_frame.pack()
spacer.pack(in_=Refresh_frame, side=BOTTOM)

refreshButton = Button(window, text="Refresh", command=update_TreeviewDevices, height=1, width=5, padx=10, pady=10)
refreshButton.pack(in_=Refresh_frame, side=LEFT)

refreshButton = Button(window, text="Copy", command=copy_iri, height=1, width=5, padx=10, pady=10)
refreshButton.pack(in_=Refresh_frame, side=LEFT)

