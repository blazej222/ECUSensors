import React from 'react';
import { Link } from 'react-router-dom';

const Header = () => (
    <header className="bg-blue-500 text-white py-4 shadow-md">
        <div className="container mx-auto flex justify-between items-center">
            <h1 className="text-2xl font-bold">MQTT Monitor</h1>
            <nav>
                <Link to="/" className="px-4 hover:underline">Home</Link>
                <Link to="/sensors" className="px-4 hover:underline">Sensors</Link>
                <Link to="/charts" className="px-4 hover:underline">Charts</Link>
                <Link to="/dashboard" className="px-4 hover:underline">Dashboard</Link> {/* Nowa zakładka */}
            </nav>
        </div>
    </header>
);

export default Header;