import paho.mqtt.client as mqtt
import random
import time
import json

BROKER = "localhost"
PORT = 1883
TOPIC = "sensors"

SENSORS = {
    "coolant_temperature": {"min": 70, "max": 105},
    "map_pressure": {"min": 20, "max": 100},
    "oxygen_level": {"min": 0.1, "max": 21.0}, 
    "rpm": {"min": 600, "max": 7000}
}

def generate_data(sensor_type, instance_id):
    sensor_range = SENSORS[sensor_type]
    value = round(random.uniform(sensor_range["min"], sensor_range["max"]), 2)
    timestamp = time.time()
    return {
        "sensorType": sensor_type,
        "instanceId": instance_id,
        "value": value,
        "timestamp": timestamp
    }

def publish_data(client):
    for sensor_type in SENSORS:
        for instance_id in range(1, 5):
            data = generate_data(sensor_type, instance_id)
            client.publish(TOPIC, json.dumps(data))
            print(f"Published: {data}")

client = mqtt.Client()
client.connect(BROKER, PORT)

try:
    while True:
        publish_data(client)
        # exit()
        time.sleep(2)
except KeyboardInterrupt:
    client.disconnect()
