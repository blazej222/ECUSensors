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
    const [filters, setFilters] = useState(null); // Przechowuje ostatnio użyte filtry
    const [loading, setLoading] = useState(false);
    const [isChartVisible, setIsChartVisible] = useState(true);

    const fetchSensorData = async (appliedFilters) => {
        try {
            setLoading(true);
            setChartData(null); // Resetuj dane wykresu

            const queryParams = new URLSearchParams();
            Object.keys(appliedFilters).forEach((key) => {
                if (appliedFilters[key]) {
                    queryParams.append(key, appliedFilters[key]);
                }
            });

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

                // Przygotowanie danych wykresu
                const datasets = Object.entries(groupedData).map(([key, points]) => ({
                    label: key,
                    data: points.sort((a, b) => a.x - b.x), // Sortowanie punktów względem czasu
                    borderColor: `rgba(${Math.floor(Math.random() * 255)}, ${Math.floor(
                        Math.random() * 255
                    )}, ${Math.floor(Math.random() * 255)}, 1)`,
                    backgroundColor: 'rgba(0, 0, 0, 0)',
                    tension: 0, // Proste linie na wykresie
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

    const handleFiltersApply = (appliedFilters) => {
        setFilters(appliedFilters); // Zapisz ostatnio użyte filtry
        fetchSensorData(appliedFilters);
    };

    const downloadCsv = () => {
        if (!filters) return; // Jeśli brak filtrów, nic nie rób

        const queryParams = new URLSearchParams();
        Object.keys(filters).forEach((key) => {
            if (filters[key]) {
                queryParams.append(key, filters[key]);
            }
        });
        queryParams.append('format', 'csv');
        const csvUrl = `${API_URL}/api/sensors/data/filter?${queryParams.toString()}`;
        window.open(csvUrl, '_blank');
    };

    return (
        <div className="container mx-auto p-4">
            <h1 className="text-3xl font-bold mb-4">Sensors Data</h1>
            <Filters onApplyFilters={handleFiltersApply} />

            <div className="flex justify-between items-center my-4">
                <button
                    onClick={() => setIsChartVisible(!isChartVisible)}
                    className="bg-blue-500 text-white px-4 py-2 rounded hover:bg-blue-600"
                >
                    {isChartVisible ? 'Hide Chart' : 'Show Chart'}
                </button>

                <button
                    onClick={downloadCsv}
                    disabled={!filters} // Przycisk zablokowany, jeśli brak filtrów
                    className={`px-4 py-2 rounded ${
                        !filters
                            ? 'bg-gray-400 cursor-not-allowed'
                            : 'bg-green-500 text-white hover:bg-green-600'
                    }`}
                >
                    Download CSV
                </button>
            </div>

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