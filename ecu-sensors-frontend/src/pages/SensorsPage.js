import React, { useState } from 'react';
import Filters from '../components/Filters';
import Table from '../components/Table';
import { Line } from 'react-chartjs-2';
import {
    Chart as ChartJS,
    LineElement,
    PointElement,
    LinearScale,
    TimeScale,
    Title,
    Tooltip,
    Legend,
} from 'chart.js';
import 'chartjs-adapter-date-fns';
import { format } from 'date-fns';
import API_URL from '../config';

// Rejestracja komponentów Chart.js
ChartJS.register(LineElement, PointElement, LinearScale, TimeScale, Title, Tooltip, Legend);

const SensorsPage = () => {
    const [sensorData, setSensorData] = useState([]);
    const [chartData, setChartData] = useState(null);
    const [loading, setLoading] = useState(false);
    const [isChartVisible, setIsChartVisible] = useState(true);

    const fetchSensorData = async (filters) => {
        try {
            setLoading(true);
            setChartData(null); // Resetuj dane wykresu

            const queryParams = new URLSearchParams();

            if (filters.sensorType) queryParams.append('sensorType', filters.sensorType);
            if (filters.instanceId) queryParams.append('instanceId', filters.instanceId);

            // Sprawdzenie i dodanie dat do zapytania
            if (filters.startDate) {
                const formattedStartDate = new Date(filters.startDate).toISOString();
                queryParams.append('startDate', formattedStartDate);
            }
            if (filters.endDate) {
                const formattedEndDate = new Date(filters.endDate).toISOString();
                queryParams.append('endDate', formattedEndDate);
            }

            if (filters.sortBy) queryParams.append('sortBy', filters.sortBy);
            if (filters.ascending !== undefined) queryParams.append('ascending', filters.ascending);
            if (filters.limit) queryParams.append('limit', filters.limit);

            const response = await fetch(`${API_URL}/api/sensors/data/filter?${queryParams.toString()}`);
            const data = await response.json();

            setSensorData(Array.isArray(data) ? data : []);

            if (Array.isArray(data)) {
                // Grupowanie danych dla wykresu
                const groupedData = data.reduce((acc, entry) => {
                    const key = `${entry.sensorType} (Instance ${entry.instanceId})`;
                    if (!acc[key]) acc[key] = [];
                    acc[key].push({
                        x: new Date(entry.timestamp),
                        y: entry.value,
                    });
                    return acc;
                }, {});

                // Sortowanie punktów w każdej grupie względem czasu
                const sortedGroupedData = Object.entries(groupedData).reduce((acc, [key, points]) => {
                    acc[key] = points.sort((a, b) => a.x - b.x); // Sortowanie względem czasu
                    return acc;
                }, {});

                // Przygotowanie danych wykresu
                const datasets = Object.entries(sortedGroupedData).map(([key, points]) => ({
                    label: key,
                    data: points,
                    borderColor: `rgba(${Math.floor(Math.random() * 255)}, ${Math.floor(
                        Math.random() * 255
                    )}, ${Math.floor(Math.random() * 255)}, 1)`,
                    backgroundColor: 'rgba(0, 0, 0, 0)',
                    tension: 0, // Linia prosta między punktami
                }));

                setChartData({
                    datasets,
                });
            }
        } catch (error) {
            console.error('Error fetching data:', error);
            setSensorData([]);
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="container mx-auto p-4">
            <h1 className="text-3xl font-bold mb-4">Sensors Data</h1>
            <Filters onApplyFilters={fetchSensorData} />

            {/* Przycisk do rozwijania/zwijania wykresu */}
            <button
                onClick={() => setIsChartVisible(!isChartVisible)}
                className="bg-blue-500 text-white px-4 py-2 rounded mb-4 hover:bg-blue-600"
            >
                {isChartVisible ? 'Hide Chart' : 'Show Chart'}
            </button>

            {/* Wykres - widoczny tylko jeśli isChartVisible jest true */}
            {isChartVisible && chartData && (
                <div className="mb-4">
                    <Line
                        data={chartData}
                        options={{
                            responsive: true,
                            scales: {
                                x: {
                                    type: 'time',
                                    time: {
                                        tooltipFormat: 'yyyy-MM-dd HH:mm:ss',
                                    },
                                    ticks: {
                                        callback: function (value) {
                                            const tickDate = new Date(value);
                                            return format(tickDate, 'HH:mm:ss');
                                        },
                                        maxTicksLimit: 10,
                                    },
                                    title: {
                                        display: true,
                                        text: 'Time',
                                    },
                                },
                                y: {
                                    title: {
                                        display: true,
                                        text: 'Value',
                                    },
                                },
                            },
                        }}
                    />
                </div>
            )}

            {loading ? (
                <div className="text-center mt-4">Loading...</div>
            ) : (
                <Table data={sensorData} />
            )}
        </div>
    );
};

export default SensorsPage;