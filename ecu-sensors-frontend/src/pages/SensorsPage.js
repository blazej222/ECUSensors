import React, { useState } from 'react';
import Filters from '../components/Filters';
import Table from '../components/Table';
import sampleData from '../data/sampleData';

const SensorsPage = () => {
    const [filters, setFilters] = useState({
        sensorType: '',
        sensorInstance: '',
        fromDate: '',
        toDate: '',
    });

    const filteredData = sampleData.filter((item) => {
        const matchesSensorType = filters.sensorType
            ? item.sensorType.toLowerCase() === filters.sensorType.toLowerCase()
            : true;

        const matchesSensorInstance = filters.sensorInstance
            ? item.sensorInstance.toString().includes(filters.sensorInstance)
            : true;

        const matchesFromDate = filters.fromDate
            ? new Date(item.timestamp) >= new Date(filters.fromDate)
            : true;

        const matchesToDate = filters.toDate
            ? new Date(item.timestamp) <= new Date(filters.toDate)
            : true;

        return matchesSensorType && matchesSensorInstance && matchesFromDate && matchesToDate;
    });

    return (
        <div className="container mx-auto p-4">
            <h1 className="text-3xl font-bold mb-4">Sensors Data</h1>
            <Filters filters={filters} setFilters={setFilters} />
            <Table data={filteredData} />
        </div>
    );
};

export default SensorsPage;