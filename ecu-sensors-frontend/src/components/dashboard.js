import React, { useEffect, useState } from 'react';
import API_URL from '../config';

const Dashboard = () => {
    const [sensorData, setSensorData] = useState({});
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchSensorData = async () => {
            try {
                setLoading(true);
                const response = await fetch(`${API_URL}/api/sensors/data`);
                const data = await response.json();

                const groupedData = {};
                data.forEach((item) => {
                    if (!groupedData[item.instanceId]) {
                        groupedData[item.instanceId] = [];
                    }
                    groupedData[item.instanceId].push(item.value);
                });

                setSensorData(groupedData);
            } catch (error) {
                console.error('Błąd podczas pobierania danych:', error);
            } finally {
                setLoading(false);
            }
        };

        fetchSensorData();
    }, []);

    const calculateAverage = (values) => {
        if (values.length === 0) return 0;
        return (values.reduce((sum, value) => sum + value, 0) / values.length).toFixed(2);
    };

    if (loading) {
        return <div className="text-center py-4">Ładowanie danych...</div>;
    }

    return (
        <div className="container mx-auto p-4">
            <h1 className="text-3xl font-bold mb-4">Sensor Dashboard</h1>
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                {Object.keys(sensorData).map((instanceId) => (
                    <div
                        key={instanceId}
                        className="p-4 bg-white shadow-md rounded-md border border-gray-200"
                    >
                        <h2 className="text-lg font-bold">Instancja {instanceId}</h2>
                        <p>Ostatnia wartość: {sensorData[instanceId].slice(-1)[0] || 'Brak danych'}</p>
                        <p>
                            Średnia wartość: {calculateAverage(sensorData[instanceId]) || 'Brak danych'}
                        </p>
                    </div>
                ))}
            </div>
        </div>
    );
};

export default Dashboard;