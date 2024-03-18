import React from 'react';
import { useState, useEffect } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import userManager from '../../AuthFiles/authConfig';
import { isAuthenticated } from '../../Functions/CheckAuthorization';
import '../Styles/NavBarStyles.css'
import { NavLink } from 'react-router-dom';
import { ThreeDots } from 'react-loader-spinner';
import config from '../../config.json'; 

const NavBar = () => {
    const navigate = useNavigate();
    const [isAuthorized, setIsAuthorized] = React.useState(false);
    const [loading, setLoading] = useState(true); 
    const [user, setUser] = useState(null);
    const [userData, setUserData] = useState(null);
    async function fetchUserData(accessToken) {
        try {
            const response = await fetch(`${config.apiBaseUrl}/user`, {
                headers: {
                    'Authorization': `Bearer ${accessToken}`
                }
            });

            if (!response.ok) {
                throw new Error(`HTTP error! Status: ${response.status}`);
            }

            return await response.json();
        } catch (error) {
            console.error('Error while sending the request to the UserService ', error);
            return null;
        }
    }
    useEffect(() => {
        const checkAuth = async () => {
            const authStatus = await isAuthenticated();
            setIsAuthorized(authStatus)
            if (authStatus) {
                userManager.getUser().then(user => {
                    setUser(user);
                    if (user) {
                        fetchUserData(user.access_token).then(data => {
                            setUserData(data);

                        });
                    }
                });

            }
            setLoading(false);
        };

        checkAuth();
    }, []);

    const onLogin = () => {
        userManager.signinRedirect();
    };


    return (
        <div className ="NavBarMain">
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
                                                <NavLink to="/Profile" className="NavBarButton">
                                                    {console.log(userData.nickName)}
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



                            
                        