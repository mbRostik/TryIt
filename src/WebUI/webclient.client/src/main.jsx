import React from 'react';
import ReactDOM from 'react-dom/client';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import App from './App';
import SignIn_CallbackPage from './AuthFiles/SignIn_CallbackPage';
import SignOut_CallBackPage from './AuthFiles/SignOut_CallBackPage';
import NavBar from './Components/NavBar/NavBar'; 
import Profile from './Components/Profile/Profile';
import Profile_Settings from './Components/Profile/Profile_Settings';
import Someones_Profile from './Components/Profile/Someones_Profile';
import './index.css';

const root = ReactDOM.createRoot(document.getElementById('root'));
root.render(
    
    <React.StrictMode>
        <Router>
            <NavBar />
            <div className="container">
                <Routes>
                    <Route path="/" element={<App />} />
                    <Route path="/Profile" element={<Profile />} />
                    <Route path="/signin-oidc" element={<SignIn_CallbackPage />} />
                    <Route path="/Profile_Settings" element={<Profile_Settings />} />
                    <Route path="/signout-callback-oidc" element={<SignOut_CallBackPage />} />
                    <Route path="/Someones_Profile/:id" element={<Someones_Profile />} />
                </Routes>
            </div >
        </Router>
        </React.StrictMode>
    
);