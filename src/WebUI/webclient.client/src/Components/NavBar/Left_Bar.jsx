import React from 'react';
import { useState, useEffect } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import userManager from '../../AuthFiles/authConfig';
import '../Styles/Left_BarStyles.css'
import { NavLink } from 'react-router-dom';
import { ThreeDots } from 'react-loader-spinner';
import { useAuth } from '../AuthProvider';

const NavBar = () => {
    const navigate = useNavigate();

    const onLogout = async () => {
        await userManager.signoutRedirect();
        navigate('/');
    };
    return (

        <div className="LeftSide">

            <div className="LeftBar_Up">
                <img className="Left_SideLogo" src="../../public/homePage.png" alt="" />
                <h2 className="inter-font">TryIt</h2>
            </div>

            <div className="LeftBar_Centre">
             
                <NavLink 
                    to="/"
                    className={({ isActive }) => isActive ? "LeftBar_Centre_Component active" : "LeftBar_Centre_Component"}
                >
                    <img className="Left_SideIcon" src="../../public/home.png" alt="Home" />
                    <div className="inter-font">HOME</div>
                </NavLink>

                <NavLink
                    to="/Profile"
                    className={({ isActive }) => isActive ? "LeftBar_Centre_Component active" : "LeftBar_Centre_Component"}
                >
                    <img className="Left_SideIcon" src="../../public/profile.png" alt="Home" />
                    <div className="inter-font">PROFILE</div>
                </NavLink>

                <NavLink
                    to="/Friends"
                    className={({ isActive }) => isActive ? "LeftBar_Centre_Component active" : "LeftBar_Centre_Component"}
                >
                    <img className="Left_SideIcon" src="../../public/people.png" alt="Home" />
                    <div className="inter-font">FRIENDS</div>
                </NavLink>

                <NavLink
                    to="/Chats"
                    className={({ isActive }) => isActive ? "LeftBar_Centre_Component active" : "LeftBar_Centre_Component"}
                >
                    <img className="Left_SideIcon" src="../../public/chat.png" alt="Home" />
                    <div className="inter-font">CHATS</div>
                </NavLink>


                <NavLink
                    to="/Notifications"
                    className={({ isActive }) => isActive ? "LeftBar_Centre_Component active" : "LeftBar_Centre_Component"}
                >
                    <img className="Left_SideIcon" src="../../public/bell.png" alt="Home" />
                    <div className="inter-font">NOTIFICATIONS</div>
                </NavLink>

            </div>

            <div className="LeftBar_Down">
                
                <NavLink
                    to="/Help"
                    className={({ isActive }) => isActive ? "LeftBar_Centre_Component active" : "LeftBar_Centre_Component"}
                >
                    <img className="Left_SideIcon" src="../../public/help.png" alt="Home" />
                    <div className="inter-font">HELP</div>
                </NavLink>
                <button className="signout-button" onClick={onLogout}>
                     SIGN OUT
                </button>

            </div>

        </div>
    );
};

export default NavBar;




