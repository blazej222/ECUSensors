import React from 'react';
import LiveChart from '../components/LiveChart';
import sampleData from '../data/sampleData';

const ChartsPage = () => {
    // Pobranie unikalnych instancji czujników z danych
    const sensorInstances = [...new Set(sampleData.map((item) => item.sensorInstance))];

    return (
        <div className="container mx-auto p-4">
            <h1 className="text-3xl font-bold mb-4">Live Charts</h1>
            <p className="text-gray-600 mb-8">
                Poniżej znajdują się wykresy aktualizujące się w czasie rzeczywistym dla każdej instancji czujnika.
            </p>
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                {sensorInstances.map((instance) => (
                    <LiveChart key={instance} sensorInstance={instance} />
                ))}
            </div>
        </div>
    );
};

export default ChartsPage;