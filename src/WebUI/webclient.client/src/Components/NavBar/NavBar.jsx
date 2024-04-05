import React from 'react';
import { useState, useEffect, useRef } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import userManager from '../../AuthFiles/authConfig';
import '../Styles/NavBarStyles.css'
import { NavLink } from 'react-router-dom';
import { ThreeDots } from 'react-loader-spinner';
import { useAuth } from '../AuthProvider';
import config from '../../config.json'; 

const NavBar = () => {
    const navigate = useNavigate();
    const [searchText, setSearchText] = useState('');
    const [searchResults, setSearchResults] = useState([]);
    const { user, userData, loading, isAuthorized } = useAuth();
    const searchRef = useRef(null); 
   

    const handleSearchChange = (event) => {
        setSearchText(event.target.value);
    };
    const onLogin = () => {
        userManager.signinRedirect();
    };

    useEffect(() => {
        let didCancel = false;
        
        async function findUser() {
            const searchingField = encodeURIComponent(searchText);
            try {
                const response = await fetch(`${config.apiBaseUrl}/GetSearchedUsers`, {
                    method: 'POST',
                    headers: {
                        'Authorization': `Bearer ${user.access_token}`,
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify({ searchingField })
                });

                if (!didCancel) { 
                    const data = await response.json();
                    setSearchResults(data);
                }
            } catch (error) {
                console.log('Nothing was found');
                if (!didCancel) {
                    setSearchResults([]);
                }
            }
        }

        if ( searchText.length > 0) {
            
            findUser();
        } else {
            setSearchResults([]);
        }

        return () => {
            didCancel = true;
        };
    }, [searchText]); 
    const handleUserClick = (id) => {
        window.location.href = `/Someones_Profile/${id}`;
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
                                            <div className="SearchContainer">
                                                <input
                                                    type="search"
                                                    ref={searchRef}
                                                    className="search-input-navBar"
                                                    placeholder="Search"
                                                    value={searchText}
                                                    onChange={handleSearchChange}
                                                />
                                                <div className="search-results" >
                                                    {searchResults.map(user => (
                                                        <div key={user.id} className="user-item" onClick={(e) => { handleUserClick(user.id); }}>
                                                            <div>
                                                                <img
                                                                    src={user.photo ? `data:image/jpeg;base64,${user.photo}` : "../../public/NoPhoto.jpg"}
                                                                    alt={user.username}
                                                                    className="user-avatar"
                                                                />
                                                            </div>
                                                            <div className="user-name">{user.nickName}</div>
                                                        </div>
                                                    ))}
                                                </div>
                                            </div>
                                           
                                            <div className="NavProfile">
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



                            
                        