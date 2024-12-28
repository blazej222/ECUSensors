import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Header from './components/Header';
import HomePage from './pages/HomePage';
import SensorsPage from './pages/SensorsPage';
import ChartsPage from './pages/ChartsPage';
import Dashboard from './pages/Dashboard'; // Import nowej strony

const App = () => (
    <Router>
        <Header />
        <Routes>
            <Route path="/" element={<HomePage />} />
            <Route path="/sensors" element={<SensorsPage />} />
            <Route path="/charts" element={<ChartsPage />} />
            <Route path="/dashboard" element={<Dashboard />} /> {/* Nowa trasa */}
        </Routes>
    </Router>
);

export default App;