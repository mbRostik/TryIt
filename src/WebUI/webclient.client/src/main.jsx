import React from 'react';
import ReactDOM from 'react-dom/client';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import App from './App';
import SignIn_CallbackPage from './AuthFiles/SignIn_CallbackPage';
import SignOut_CallBackPage from './AuthFiles/SignOut_CallBackPage';
import NavBar from './Components/NavBar/NavBar'; 
import './index.css';

const root = ReactDOM.createRoot(document.getElementById('root'));
root.render(
    <React.StrictMode>
        <Router>
            <NavBar />
            <Routes>
                <Route path="/" element={<App />} />
                <Route path="/signin-oidc" element={<SignIn_CallbackPage />} />
                <Route path="/signout-callback-oidc" element={<SignOut_CallBackPage />} />
            </Routes>
        </Router>
    </React.StrictMode>
);