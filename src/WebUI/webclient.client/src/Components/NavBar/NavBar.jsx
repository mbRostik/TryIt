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

            {loading ? <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh' }} ><ThreeDots color="orange" height={80} width={80} /></div>
                : isAuthorized === false ? (
                    <div className="NavBarMenuUnAuth">
                        <div><button onClick={onLogin} className="NavBarButton_Login">Login</button></div>
                        <div><button onClick={onLogin} className="NavBarButton_Registration">Sign Up</button></div>
                    </div>
                ) : userData === null ? (
                        <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh' }}>
                            <ThreeDots color="#00BFFF" height={80} width={80} />
                        </div>
                ) : (
                    <div className="NavBarMenuAuth">
                        {userData && (
                            <>
                                <div className="NavBarMenu">
                                    
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



                            
                        