import React, { useState, useEffect } from 'react';
import userManager from '../../AuthFiles/authConfig';
import { isAuthenticated } from '../../Functions/CheckAuthorization';
import { NavLink, useNavigate } from 'react-router-dom';
import { ThreeDots } from 'react-loader-spinner';

const Profile_Settings = () => {
    const [isAuthorized, setIsAuthorized] = useState(null);
    const [user, setUser] = useState(null);
    const [userData, setUserData] = useState({});
    const [loading, setLoading] = useState(true);
    const navigate = useNavigate();

    async function fetchUserData(accessToken) {
        try {
            const response = await fetch('https://localhost:7062/ocelot/user', {
                headers: {
                    'Authorization': `Bearer ${accessToken}`
                }
            });

            if (!response.ok) {
                throw new Error(`HTTP error! Status: ${response.status}`);
            }

            return await response.json();
        } catch (error) {
            setLoading(false);
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


    const handleSaveChanges = async (e) => {
        e.preventDefault();
        setLoading(true);
        try {
            const accessToken = await userManager.getUser().then(user => user.access_token);
            const response = await fetch('https://localhost:7062/ocelot/userUpdate', {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${accessToken}`,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(userData)
            });

            if (!response.ok) {
                throw new Error(`HTTP error! Status: ${response.status}`);
            }

            console.log("Data saved successfully!");
            navigate('/profile'); 
        } catch (error) {
            console.error('Error while saving the user data', error);
        } finally {
            setLoading(false);
        }
    };

    const handleChange = (e) => {
        const value = e.target.type === 'checkbox' ? e.target.checked : e.target.value;
        setUserData({ ...userData, [e.target.name]: value });
    };

    return (
        <div>
            {loading ? <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh' }} ><ThreeDots color="#00BFFF" height={80} width={80} /></div>
                : isAuthorized === false ? (
                    <div>UnAuthorized</div>
                ) : userData === null ? (
                    <div>There is no information</div>
                ) : (
                    <div className="profile">
                        {userData && (
                                    <form onSubmit={handleSaveChanges}>
                                        <label>
                                            Name:
                                            <input type="text" name="name" value={userData.name || ''} onChange={handleChange} />
                                        </label>
                                        <label>
                                            NickName:
                                            <input type="text" name="nickName" value={userData.nickName || ''} onChange={handleChange} />
                                        </label>
                                        <label>
                                            Email:
                                            <input type="email" name="email" value={userData.email || ''} onChange={handleChange} />
                                        </label>
                                        <label>
                                            Phone:
                                            <input type="tel" name="phone" value={userData.phone || ''} onChange={handleChange} />
                                        </label>
                                        <label>
                                            Bio:
                                            <textarea name="bio" value={userData.bio || ''} onChange={handleChange} />
                                        </label>
                                        <label>
                                            Private Account:
                                            <input type="checkbox" name="isPrivate" checked={userData.isPrivate || false} onChange={handleChange} />
                                        </label>
                                        <label>
                                            Date of Birth:
                                            <input
                                                type="date"
                                                name="dateOfBirth"
                                                value={userData.dateOfBirth ? new Date(userData.dateOfBirth).toISOString().split('T')[0] : ''}
                                                onChange={handleChange}
                                            />
                                        </label>
                                        <button type="submit">Save Changes</button>
                                    </form>
                        )}
                    </div>
                )}
        </div>
    );
};

export default Profile_Settings;
