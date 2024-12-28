import React from 'react';

const Filters = ({ filters, setFilters }) => {
    const handleFilterChange = (e) => {
        setFilters({ ...filters, [e.target.name]: e.target.value });
    };

    return (
        <div className="bg-gray-100 p-4 rounded-md shadow-md mb-4">
            <h2 className="text-lg font-bold mb-2">Filter Sensors</h2>
            <div className="flex flex-wrap gap-4">
                {/* Dropdown dla Typu czujnika */}
                <select
                    name="sensorType"
                    value={filters.sensorType}
                    onChange={handleFilterChange}
                    className="border border-gray-300 p-2 rounded-md w-full md:w-1/3"
                >
                    <option value="">All Sensor Types</option>
                    <option value="Temperature">Temperature</option>
                    <option value="Humidity">Humidity</option>
                    <option value="Pressure">Pressure</option>
                    <option value="Light">Light</option>
                </select>

                {/* Pole dla Instancji czujnika */}
                <input
                    type="text"
                    name="sensorInstance"
                    placeholder="Sensor Instance"
                    value={filters.sensorInstance}
                    onChange={handleFilterChange}
                    className="border border-gray-300 p-2 rounded-md w-full md:w-1/3"
                />

                {/* Pole dla daty początkowej */}
                <input
                    type="date"
                    name="fromDate"
                    value={filters.fromDate}
                    onChange={handleFilterChange}
                    className="border border-gray-300 p-2 rounded-md w-full md:w-1/3"
                />

                {/* Pole dla daty końcowej */}
                <input
                    type="date"
                    name="toDate"
                    value={filters.toDate}
                    onChange={handleFilterChange}
                    className="border border-gray-300 p-2 rounded-md w-full md:w-1/3"
                />
            </div>
        </div>
    );
};

export default Filters;