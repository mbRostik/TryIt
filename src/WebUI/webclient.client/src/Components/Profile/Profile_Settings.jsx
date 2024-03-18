import React, { useState, useEffect } from 'react';
import userManager from '../../AuthFiles/authConfig';
import { isAuthenticated } from '../../Functions/CheckAuthorization';
import { NavLink, useNavigate } from 'react-router-dom';
import { ThreeDots } from 'react-loader-spinner';
import '../Styles/Profile_Settings.css'
import config from '../../config.json'; 


import { ToastContainer, toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css'; 
const Profile_Settings = () => {
    const [isAuthorized, setIsAuthorized] = useState(null);
    const [user, setUser] = useState(null);
    const [userData, setUserData] = useState({});
    const [loading, setLoading] = useState(true);
    const navigate = useNavigate();
    const validateData = () => {
        let isValid = true;

        if (userData.nickName.length < 4 || userData.nickName.length > 10) {
            toast.error("NickName between 4 and 10");
            isValid = false;
        }

        if (userData.name.length > 30) {
            toast.error("Name 30");
            isValid = false;
        }

        if (!userData.email.endsWith('@gmail.com')) {
            toast.error("Email should ends @gmail.com");
            isValid = false;
        }

        if (userData.phone.length > 10) {
            toast.error("Phone 10 ");
            isValid = false;
        }

        if (userData.bio.length > 400) {
            toast.error("Bio 400 ");
            isValid = false;
        }

        return isValid;
    };
    
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
        if (!validateData()) return; 

        setLoading(true);
        try {
            const accessToken = await userManager.getUser().then(user => user.access_token);
            const response = await fetch(await fetch(`${config.apiBaseUrl}/userUpdate`, {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${accessToken}`,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(userData)
            }));

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
            <ToastContainer position="top-right" autoClose={5000} hideProgressBar={false} newestOnTop={false} closeOnClick rtl={false} pauseOnFocusLoss draggable pauseOnHover />
            {loading ? <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh' }} ><ThreeDots color="#00BFFF" height={80} width={80} /></div>
                : isAuthorized === false ? (
                    <div>UnAuthorized</div>
                ) : userData === null ? (
                    <div>There is no information</div>
                ) : (
                     <div className="Settings_Form">
                        {userData && (
                                    <form onSubmit={handleSaveChanges}>
                                        <div className="Name">
                                            <label>
                                                Name:
                                                <input type="text" name="name" value={userData.name || ''} onChange={handleChange} />
                                            </label>
                                        </div>

                                        <div className="NickName">
                                            <label>
                                                NickName:
                                                <input type="text" name="nickName" value={userData.nickName || ''} onChange={handleChange} />
                                            </label>
                                        </div>

                                        <div className="Email">
                                            <label>
                                                Email:
                                                <input type="email" name="email" value={userData.email || ''} onChange={handleChange} />
                                            </label>
                                        </div>

                                        <div className="Phone">
                                            <label>
                                                Phone:
                                                <input type="tel" name="phone" value={userData.phone || ''} onChange={handleChange} />
                                            </label>
                                        </div>

                                        <div className="Bio">
                                            <label>
                                                Bio:
                                                <textarea name="bio" value={userData.bio || ''} onChange={handleChange} />
                                            </label>
                                        </div>

                                        <div className="PrivateAccount">
                                            <label>
                                                Private Account:
                                                <input type="checkbox" name="isPrivate" checked={userData.isPrivate || false} onChange={handleChange} />
                                            </label>
                                        </div>

                                        <div className="DateOfBirth">
                                            <label>
                                                Date of Birth:
                                                <input
                                                    type="date"
                                                    name="dateOfBirth"
                                                    value={userData.dateOfBirth ? new Date(userData.dateOfBirth).toISOString().split('T')[0] : ''}
                                                    onChange={handleChange}
                                                />
                                            </label>
                                        </div>

                                        <div className="Sex">
                                            <label>
                                                Sex:
                                                <select name="sexId" value={userData.sexId || 'UnIdentify'} onChange={handleChange}>
                                                    <option value="UnIdentify">UnIdentify</option>
                                                    <option value="Man">Man</option>
                                                    <option value="Woman">Woman</option>
                                                </select>
                                            </label>
                                        </div>
                                        <div className="Settings_Component">
                                            <button type="submit" className="ProfileSettingsButton">Save Changes</button>
                                        </div>


                                        
                                    </form>
                        )}
                    </div>
                )}
        </div>
    );
};

export default Profile_Settings;
