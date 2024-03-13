import { useState, useEffect } from 'react';
import userManager from '../../AuthFiles/authConfig';
import { isAuthenticated } from '../../Functions/CheckAuthorization';
import { NavLink } from 'react-router-dom';
import { ThreeDots } from 'react-loader-spinner';

const Profile = () => {
    const [isAuthorized, setIsAuthorized] = useState(null);
    const [user, setUser] = useState(null);
    const [userData, setUserData] = useState(null);
    const [loading, setLoading] = useState(true); 

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
            if (authStatus)
            {
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
                        <>
                            <div className="profile-header">
                                <img src={`data:image/jpeg;base64,${userData.photo}`} alt="Profile" className="profile-photo" />
                            </div>
                            <div className="profile-info">
                                <p>Name: {userData.name}</p>
                                <p>NickName: {userData.nickName}</p>
                                <p>Email: {userData.email}</p>
                                <p>Phone: {userData.phone}</p>
                                <p>Bio: {userData.bio}</p>
                                <p>Date of Birth: {new Date(userData.dateOfBirth).toLocaleDateString()}</p>
                                <p>Private Account: {userData.isPrivate ? 'Yes' : 'No'}</p>
                                <p>Followers: {userData.followersCount}</p>
                                <p>Following: {userData.followsCount}</p>
                            </div>
                            <NavLink to="/Profile_Settings" className="">Settings</NavLink>
                        </>
                    )}
                </div>
            )}
        </div>
    );
};

export default Profile;
