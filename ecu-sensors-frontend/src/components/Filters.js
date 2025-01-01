import React, { useState } from 'react';

const Filters = ({ onApplyFilters }) => {
    const [filters, setFilters] = useState({
        sensorType: 'all',
        instanceId: '',
        startDate: '',
        startTime: '',
        endDate: '',
        endTime: '',
        sortBy: '',
        ascending: true,
        limit: '',
    });

    const handleChange = (e) => {
        const { name, value } = e.target;
        setFilters({
            ...filters,
            [name]: value,
        });
    };

    const handleApply = () => {
        // Łączenie daty i czasu w jeden format ISO
        const startDateTime =
            filters.startDate && filters.startTime
                ? new Date(`${filters.startDate}T${filters.startTime}`).toISOString()
                : '';
        const endDateTime =
            filters.endDate && filters.endTime
                ? new Date(`${filters.endDate}T${filters.endTime}`).toISOString()
                : '';

        // Tworzenie aktywnych filtrów
        const activeFilters = Object.keys(filters)
            .filter(
                (key) =>
                    filters[key] !== '' &&
                    !(key === 'sensorType' && filters[key] === 'all') &&
                    !['startDate', 'startTime', 'endDate', 'endTime'].includes(key)
            )
            .reduce((obj, key) => {
                obj[key] = filters[key];
                return obj;
            }, {});

        if (startDateTime) activeFilters.startDate = startDateTime;
        if (endDateTime) activeFilters.endDate = endDateTime;

        onApplyFilters(activeFilters);
    };

    return (
        <div className="filters-container p-4 bg-gray-100 rounded shadow-md flex flex-wrap gap-4">
            {/* Sensor Type Filter */}
            <div className="flex flex-col">
                <label className="font-bold mb-1">Sensor Type</label>
                <select
                    name="sensorType"
                    value={filters.sensorType}
                    onChange={handleChange}
                    className="border rounded p-2"
                >
                    <option value="all">All Sensor Types</option>
                    <option value="rpm">RPM</option>
                    <option value="oxygen_level">Oxygen Level</option>
                    <option value="map_pressure">MAP Pressure</option>
                    <option value="coolant_temperature">Coolant Temperature</option>
                </select>
            </div>

            {/* Instance ID Filter */}
            <div className="flex flex-col">
                <label className="font-bold mb-1">Instance ID</label>
                <input
                    type="number"
                    name="instanceId"
                    placeholder="Enter Instance ID"
                    value={filters.instanceId}
                    onChange={handleChange}
                    className="border rounded p-2"
                />
            </div>

            {/* Start Date Filter */}
            <div className="flex flex-col">
                <label className="font-bold mb-1">Start Date</label>
                <input
                    type="date"
                    name="startDate"
                    value={filters.startDate}
                    onChange={handleChange}
                    className="border rounded p-2"
                />
            </div>

            {/* Start Time Filter */}
            <div className="flex flex-col">
                <label className="font-bold mb-1">Start Time</label>
                <input
                    type="time"
                    name="startTime"
                    value={filters.startTime}
                    onChange={handleChange}
                    className="border rounded p-2"
                />
            </div>

            {/* End Date Filter */}
            <div className="flex flex-col">
                <label className="font-bold mb-1">End Date</label>
                <input
                    type="date"
                    name="endDate"
                    value={filters.endDate}
                    onChange={handleChange}
                    className="border rounded p-2"
                />
            </div>

            {/* End Time Filter */}
            <div className="flex flex-col">
                <label className="font-bold mb-1">End Time</label>
                <input
                    type="time"
                    name="endTime"
                    value={filters.endTime}
                    onChange={handleChange}
                    className="border rounded p-2"
                />
            </div>

            {/* Sort By Filter */}
            <div className="flex flex-col">
                <label className="font-bold mb-1">Sort By</label>
                <select
                    name="sortBy"
                    value={filters.sortBy}
                    onChange={handleChange}
                    className="border rounded p-2"
                >
                    <option value="">No Sorting</option>
                    <option value="sensorType">Sensor Type</option>
                    <option value="instanceId">Instance ID</option>
                    <option value="value">Value</option>
                    <option value="timestamp">Timestamp</option>
                </select>
            </div>

            {/* Order Filter */}
            <div className="flex flex-col">
                <label className="font-bold mb-1">Order</label>
                <select
                    name="ascending"
                    value={filters.ascending}
                    onChange={(e) =>
                        setFilters({
                            ...filters,
                            ascending: e.target.value === 'true',
                        })
                    }
                    className="border rounded p-2"
                >
                    <option value="true">Ascending</option>
                    <option value="false">Descending</option>
                </select>
            </div>

            {/* Limit Filter */}
            <div className="flex flex-col">
                <label className="font-bold mb-1">Limit</label>
                <input
                    type="number"
                    name="limit"
                    placeholder="Limit results"
                    value={filters.limit}
                    onChange={handleChange}
                    className="border rounded p-2"
                />
            </div>

            {/* Apply Filters Button */}
            <button
                onClick={handleApply}
                className="bg-blue-500 text-white px-4 py-2 rounded hover:bg-blue-600"
            >
                Apply Filters
            </button>
        </div>
    );
};

export default Filters;