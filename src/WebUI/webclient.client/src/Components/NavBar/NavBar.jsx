import React from 'react';
import { useState, useEffect } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import userManager from '../../AuthFiles/authConfig';
import '../Styles/NavBarStyles.css'
import { NavLink } from 'react-router-dom';
import { ThreeDots } from 'react-loader-spinner';
import { useAuth } from '../AuthProvider';

const NavBar = () => {
    const navigate = useNavigate();

    const { user, userData, loading, isAuthorized  } = useAuth();
    
    const onLogin = () => {
        userManager.signinRedirect();
    };
    return (
        
        <div className="NavBarMain">

            {loading ? <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh' }} ><ThreeDots color="#00BFFF" height={80} width={80} /></div>
                : isAuthorized === false ? (
                    <div className="NavBarMenu">
                        <div><NavLink to="/" className="NavBarButton">Home</NavLink></div>
                        <div><button onClick={onLogin} className="NavBarButton">Login</button></div>
                    </div>
                ) : userData === null ? (
                        <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh' }}>
                            <ThreeDots color="#00BFFF" height={80} width={80} />
                        </div>
                ) : (
                    <div className="profile">
                        {userData && (
                            <>
                                <div className="NavBarMenu">
                                    <div>
                                        <div>
                                            <NavLink to="/">
                                                <img className="NavBarHome" src="../../public/homePage.png" alt="" />
                                            </NavLink>
                                        </div>
                                    </div>
                                            <div>
                                                <NavLink to="/Chats" className="NavBarButton">
                                                   Messages
                                                </NavLink>
                                            </div>
                                            <div>
                                            <NavLink to="/Profile" className="NavBarButton">
                                                {userData.nickName}
                                                <img
                                                    className="NavBarAvatar"
                                                    src={userData.photo ? `data:image/jpeg;base64,${userData.photo}` : "../../public/NoPhoto.jpg"}
                                                    alt=""
                                                />
                                                </NavLink>
                                            </div>
                                        </div>
                            </>
                        )}
                    </div>
                )}
        </div>
    );
};

export default NavBar;



                            
                        