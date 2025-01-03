import React, { useEffect, useState } from 'react';
import { Line } from 'react-chartjs-2';
import {
    Chart as ChartJS,
    LineElement,
    CategoryScale,
    LinearScale,
    PointElement,
    Title,
    Tooltip,
    Legend,
} from 'chart.js';

ChartJS.register(LineElement, CategoryScale, LinearScale, PointElement, Title, Tooltip, Legend);

const LiveChart = ({ sensorInstance }) => {
    const [data, setData] = useState({
        labels: [],
        datasets: [
            {
                label: `Sensor ${sensorInstance} Data`,
                data: [],
                borderColor: 'rgba(75, 192, 192, 1)',
                borderWidth: 2,
                tension: 0.4,
            },
        ],
    });

    useEffect(() => {
        // Aktualizacja danych co 2 sekundy
        const interval = setInterval(() => {
            setData((prevData) => {
                const now = new Date();
                const newValue = Math.random() * 100; // Symulowane dane
                const updatedLabels = [...prevData.labels, now.toLocaleTimeString()].slice(-10); // Ograniczenie do 10 punktów
                const updatedData = [...prevData.datasets[0].data, newValue].slice(-10);

                return {
                    ...prevData,
                    labels: updatedLabels,
                    datasets: [
                        {
                            ...prevData.datasets[0],
                            data: updatedData,
                        },
                    ],
                };
            });
        }, 2000);

        return () => clearInterval(interval); // Czyścimy interval przy odmontowywaniu
    }, [sensorInstance]);

    return (
        <div className="bg-white p-4 shadow-md rounded-md">
            <Line data={data} />
        </div>
    );
};

export default LiveChart;