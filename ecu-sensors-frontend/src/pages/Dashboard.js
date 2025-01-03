import React, { useState, useEffect } from "react";

const Dashboard = () => {
    const [sensors, setSensors] = useState([]);

    useEffect(() => {
        // Pobranie danych początkowych z backendu
        const fetchInitialData = async () => {
            try {
                const response = await fetch("http://localhost:1337/api/sensors/summary?recordCount=100");
                const data = await response.json();
                setSensors(data);
            } catch (error) {
                console.error("Error fetching initial data:", error);
            }
        };

        fetchInitialData();

        // Połączenie WebSocket
        const ws = new WebSocket("ws://localhost:1337/ws");

        ws.onmessage = (event) => {
            const updatedSensor = JSON.parse(event.data);

            setSensors((prevSensors) => {
                const existingIndex = prevSensors.findIndex(
                    (sensor) =>
                        sensor.sensorType === updatedSensor.sensorType &&
                        sensor.instanceId === updatedSensor.instanceId
                );

                if (existingIndex !== -1) {
                    const updatedSensors = [...prevSensors];
                    updatedSensors[existingIndex] = updatedSensor;
                    return updatedSensors;
                } else {
                    return [...prevSensors, updatedSensor];
                }
            });
        };

        ws.onclose = () => console.log("WebSocket closed.");
        ws.onerror = (error) => console.error("WebSocket error:", error);

        return () => ws.close();
    }, []);

    return (
        <div className="dashboard">
            <h1>Sensor Dashboard</h1>
            <div className="sensor-grid">
                {sensors
                    .sort(
                        (a, b) =>
                            a.sensorType.localeCompare(b.sensorType) ||
                            a.instanceId - b.instanceId
                    )
                    .map((sensor, index) => (
                        <div key={index} className="sensor-card">
                            <h2>
                                {sensor.sensorType} (Instance {sensor.instanceId})
                            </h2>
                            <p>Last Value: {sensor.lastValue}</p>
                            <p>
                                Average Value:{" "}
                                {sensor.averageValue
                                    ? sensor.averageValue.toFixed(2)
                                    : "N/A"}
                            </p>
                        </div>
                    ))}
            </div>
        </div>
    );
};

export default Dashboard;