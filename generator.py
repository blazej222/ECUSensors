import paho.mqtt.client as mqtt
import random
import time
import json

BROKER = "localhost"
PORT = 1883
TOPIC = "sensors"

SENSOR_TYPES = {
    "coolant_temperature": {"min": 70, "max": 105},
    "map_pressure": {"min": 20, "max": 100},
    "oxygen_level": {"min": 0.1, "max": 21.0}, 
    "rpm": {"min": 600, "max": 7000}
}

SENSORS =[
    {"type": "coolant_temperature", "id":1, "rate":3},
    {"type": "coolant_temperature", "id":2, "rate":3},
    {"type": "coolant_temperature", "id":3, "rate":3},
    {"type": "coolant_temperature", "id":4, "rate":3},
    
    {"type": "map_pressure", "id":1, "rate":10},
    {"type": "map_pressure", "id":2, "rate":10},
    {"type": "map_pressure", "id":3, "rate":10},
    {"type": "map_pressure", "id":4, "rate":10},
    
    {"type": "oxygen_level", "id":1, "rate":6},
    {"type": "oxygen_level", "id":2, "rate":6},
    {"type": "oxygen_level", "id":3, "rate":6},
    {"type": "oxygen_level", "id":4, "rate":6},
    
    {"type": "rpm", "id":1, "rate":60},
    {"type": "rpm", "id":2, "rate":4},
    {"type": "rpm", "id":3, "rate":4},
    {"type": "rpm", "id":4, "rate":4},
    
    
]

def generate_data(sensor_type, instance_id):
    sensor_range = SENSOR_TYPES[sensor_type]
    value = round(random.uniform(sensor_range["min"], sensor_range["max"]), 2)
    timestamp = time.time()
    return {
        "sensorType": sensor_type,
        "instanceId": instance_id,
        "value": value,
        "timestamp": timestamp
    }

def publish_data(client):
    last_published = {sensor["type"]+str(sensor["id"]): -3000 for sensor in SENSORS}

    while True:
        for sensor in SENSORS:
            sensor_type = sensor['type']
            instance_id = sensor['id'] 
            sensor_id = sensor_type+str(instance_id)
            rate = sensor['rate']
            
            current_time = time.time()
            time_since_last_publish = current_time - last_published[sensor_id]
            required_time_interval = 60 / rate 

            if time_since_last_publish >= required_time_interval:
                data = generate_data(sensor_type, instance_id)
                client.publish(TOPIC, json.dumps(data))
                print(f"Published: {data}")
                last_published[sensor_id] = current_time

        # time.sleep(1)  # Ogólne opóźnienie na czas synchronizacji pętli

client = mqtt.Client()
client.connect(BROKER, PORT)

try:
    while True:
        publish_data(client)
except KeyboardInterrupt:
    client.disconnect()
