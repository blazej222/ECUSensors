import React, { useState } from 'react';

const Table = ({ data }) => {
    const [sortConfig, setSortConfig] = useState({ key: '', direction: 'default' });

    const sortedData = [...data].sort((a, b) => {
        if (sortConfig.direction === 'default') {
            return 0; // Brak sortowania
        }
        if (sortConfig.key === 'sensorInstance') {
            return sortConfig.direction === 'ascending'
                ? a[sortConfig.key] - b[sortConfig.key]
                : b[sortConfig.key] - a[sortConfig.key];
        }
        if (a[sortConfig.key] < b[sortConfig.key]) {
            return sortConfig.direction === 'ascending' ? -1 : 1;
        }
        if (a[sortConfig.key] > b[sortConfig.key]) {
            return sortConfig.direction === 'ascending' ? 1 : -1;
        }
        return 0;
    });

    const handleSort = (key) => {
        let direction = 'ascending';
        if (sortConfig.key === key) {
            if (sortConfig.direction === 'ascending') {
                direction = 'descending';
            } else if (sortConfig.direction === 'descending') {
                direction = 'default';
            }
        }
        setSortConfig({ key, direction });
    };

    return (
        <div className="overflow-x-auto shadow-md">
            <table className="min-w-full bg-white border border-gray-200">
                <thead className="bg-gray-100">
                <tr>
                    <th
                        className="px-4 py-2 text-left cursor-pointer"
                        onClick={() => handleSort('sensorInstance')}
                    >
                        Instancja
                        {sortConfig.key === 'sensorInstance' &&
                            (sortConfig.direction === 'ascending' ? ' ↑' : sortConfig.direction === 'descending' ? ' ↓' : '')}
                    </th>
                    <th
                        className="px-4 py-2 text-left cursor-pointer"
                        onClick={() => handleSort('sensorType')}
                    >
                        Typ czujnika
                        {sortConfig.key === 'sensorType' &&
                            (sortConfig.direction === 'ascending' ? ' ↑' : sortConfig.direction === 'descending' ? ' ↓' : '')}
                    </th>
                    <th
                        className="px-4 py-2 text-left cursor-pointer"
                        onClick={() => handleSort('timestamp')}
                    >
                        Data
                        {sortConfig.key === 'timestamp' &&
                            (sortConfig.direction === 'ascending' ? ' ↑' : sortConfig.direction === 'descending' ? ' ↓' : '')}
                    </th>
                    <th className="px-4 py-2 text-left">Wartość</th>
                </tr>
                </thead>
                <tbody>
                {sortedData.map((item) => (
                    <tr key={item.sensorInstance} className="border-t">
                        <td className="px-4 py-2">{item.sensorInstance}</td>
                        <td className="px-4 py-2">{item.sensorType}</td>
                        <td className="px-4 py-2">{new Date(item.timestamp).toLocaleString()}</td>
                        <td className="px-4 py-2">{item.value}</td>
                    </tr>
                ))}
                </tbody>
            </table>
        </div>
    );
};

export default Table;