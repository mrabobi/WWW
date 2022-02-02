import threading
import time
import random


def timer(seconds, var, oldValue):
    time.sleep(int(seconds))
    var['value'] = oldValue


def fade_out(device):
    seconds = 100
    for prop in device:
        if prop['title'] == "status":
            prop['value'] = "true"

        if prop['title'] == "brightness":
            while seconds > 10:
                seconds = seconds - 10
                prop['value'] = str(seconds)
                time.sleep(1)
                if seconds == 10:
                    prop['value'] = str(100)


def turnOnHeating_function(params, device):
    if len(params) < 2:
        return None

    for prop in device:
        if prop['title'] == "status":
            prop['value'] = "true"
            threading.Thread(target=lambda: timer(params[1], prop, "false")).start()
    return True


def fade_action(params, device):
    threading.Thread(target=lambda: fade_out(device)).start()
    return True


def isSomeoneInside(params, device):
    return random.choice(["true", "false"])


def unlockForSeconds(params, device):
    if len(params) < 2:
        return None

    for prop in device:
        if prop['title'] == "status":
            prop['value'] = "true"
            threading.Thread(target=lambda: timer(params[1], prop, "false")).start()
    return True


def suggestTemperature(params, device):
    return random.randint(20, 30)
