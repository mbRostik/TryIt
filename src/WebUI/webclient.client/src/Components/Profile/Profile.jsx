import { useState, useEffect } from 'react';
import userManager from '../../AuthFiles/authConfig';
import { isAuthenticated } from '../../Functions/CheckAuthorization';
const Profile = () => {
    const [isAuthorized, setIsAuthorized] = useState(null);
    const [user, setUser] = useState(null);
    const [userData, setUserData] = useState(null);

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
        };

        checkAuth();
    }, []);

    return (
        <div>
            {isAuthorized === false ? (
                <div>UnAuthorized</div>
            ) : (
                <div>
                    <div className="profile">
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
                        </div>
                    </div>
            )}
        </div>
    );
};

export default Profile;
