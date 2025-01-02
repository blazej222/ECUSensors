import React, { useEffect, useState } from 'react';

const generateRandomValue = () => Math.floor(Math.random() * 100) + 1; // Losowa wartość z zakresu 1-100

const Dashboard = () => {
    const [sensorData, setSensorData] = useState({});

    useEffect(() => {
        const sensorInstances = [101, 102, 103, 104, 201, 202, 203, 204, 301, 302, 303, 304, 401, 402, 403, 404]; // 16 czujników

        const interval = setInterval(() => {
            setSensorData((prevData) => {
                const updatedSensorData = { ...prevData };

                sensorInstances.forEach((instance) => {
                    // Inicjalizujemy dane, jeśli ich jeszcze nie ma
                    if (!updatedSensorData[instance]) {
                        updatedSensorData[instance] = [];
                    }

                    // Dodajemy nową wartość do tablicy i ograniczamy do ostatnich 100 wartości
                    const newValue = generateRandomValue();
                    updatedSensorData[instance] = [...updatedSensorData[instance], newValue].slice(-100);
                });

                return updatedSensorData;
            });
        }, 2000); // Co 2 sekundy generujemy nowe dane

        return () => clearInterval(interval); // Czyścimy interwał przy odmontowaniu
    }, []);

    const calculateAverage = (values) => {
        if (values.length === 0) return 0;
        return (values.reduce((sum, value) => sum + value, 0) / values.length).toFixed(2);
    };

    return (
        <div className="container mx-auto p-4">
            <h1 className="text-3xl font-bold mb-4">Sensor Dashboard</h1>
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                {Object.keys(sensorData).map((sensorInstance) => (
                    <div
                        key={sensorInstance}
                        className="p-4 bg-white shadow-md rounded-md border border-gray-200"
                    >
                        <h2 className="text-lg font-bold">Sensor {sensorInstance}</h2>
                        <p>Ostatnia wartość: {sensorData[sensorInstance].slice(-1)[0] || 'Brak danych'}</p>
                        <p>
                            Średnia wartość: {calculateAverage(sensorData[sensorInstance]) || 'Brak danych'}
                        </p>
                    </div>
                ))}
            </div>
        </div>
    );
};

export default Dashboard;