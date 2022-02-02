from interfaceshp.smartinterface import getWindow
from backend.data import getDevices, PORT, map_functions
from flask import request
from flask import Flask
from flask import Response
import threading

window = getWindow()
app = Flask(__name__)


@app.route('/')
def not_connected():
    devices = getDevices()
    not_connected_devices = []
    for deviceid in devices.keys():
        if devices[deviceid]['connected'] == 'False':
            data_device = dict()
            data_device['title'] = str(devices[deviceid]['title'])
            data_device['description'] = str(devices[deviceid]['description'])
            data_device['validation_url'] = str(devices[deviceid]['IRI'])
            not_connected_devices.append(data_device)
    return Response("{'answer': " + str(not_connected_devices) + "}", status=200, mimetype='application/json')


@app.route('/<product_id>', methods=['GET', 'POST'])
def product_id_operations(product_id):
    devices = getDevices()
    if request.method == 'GET':
        if str(product_id) in devices.keys():
            return devices[str(product_id)]
        return Response("{'answer' : 'Not found'}", status=404, mimetype='application/json')
    if request.method == 'POST':
        answer = request.json
        if devices[product_id]['connected'] == "False":
            if answer['value'] == '0000' or answer['value'] == devices[product_id]['access_code']:
                devices[product_id]['connected'] = "True"
                return Response(str(devices[product_id]), status=200, mimetype='application/json')
            else:
                return Response("{'answer' : 'Wrong password'}", status=400, mimetype='application/json')
        else:
            return Response("{'answer' : 'Device already connected'}", status=400, mimetype='application/json')


@app.route('/<product_id>/<action>', methods=['GET', 'POST'])
def action_operations(product_id, action):
    devices = getDevices()

    if request.method == 'GET':
        if str(product_id) in devices.keys():
            for item in devices[str(product_id)]['properties']:
                if item['title'] == str(action):
                    return Response("{'value' : '" + str(item['value']) + "'}", status=200, mimetype='application/json')
        return Response("{'answer' : 'Not found'}", status=404, mimetype='application/json')
    elif request.method == 'POST':
        answer = request.json
        if answer is None:
            return Response("{'answer' : 'Body is empty'}", status=400, mimetype='application/json')
        if "value" not in answer.keys():
            return Response("{'answer' : 'Body must contain 'value''}", status=400, mimetype='application/json')
        for item in devices[str(product_id)]['properties']:
            if item['title'] == str(action):
                if item['readOnly'] == "true":
                    return Response("{'answer' : 'Property read only'}", status=400, mimetype='application/json')
                item['value'] = answer['value']
                return Response("{'value' : 'Data changed'}", status=200, mimetype='application/json')
        for item in devices[str(product_id)]['actions']:
            if item['title'] == str(action):
                params = []
                for param in answer.keys():
                    params.append(answer[param])
                ans = map_functions[action](params, devices[str(product_id)]['properties'])
                if ans is not None:
                    return Response("{'value' : '" + str(ans) + "' }", status=200, mimetype='application/json')
                else:
                    return Response("{'answer' : 'Missing required parameters!'}", status=404, mimetype='application/json')
        return Response("{'answer' : 'Not found'}", status=404, mimetype='application/json')


if __name__ == '__main__':
    threading.Thread(target=lambda: app.run(port=PORT)).start()
    threading.Thread(target=lambda: window.mainloop()).run()

