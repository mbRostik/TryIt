import { useState, useEffect } from 'react';
import userManager from '../../AuthFiles/authConfig';
import { isAuthenticated } from '../../Functions/CheckAuthorization';
import { NavLink } from 'react-router-dom';
import { ThreeDots } from 'react-loader-spinner';
import { Link, useNavigate } from 'react-router-dom';
import '../Styles/Profile.css'
import axios from '../../../node_modules/axios/index';
import { ToastContainer, toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import config from '../../config.json'; 

import ReactCrop from 'react-image-crop';
import 'react-image-crop/dist/ReactCrop.css';

const Profile = () => {
    const navigate = useNavigate();
    const [isHovered, setIsHovered] = useState(false);

    const [isAuthorized, setIsAuthorized] = useState(null);
    const [user, setUser] = useState(null);
    const [userData, setUserData] = useState(null);
    const [loading, setLoading] = useState(true); 
    const [loadingOverlay, setLoadingOverlay] = useState(false);

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
    const onLogout = async () => {
        await userManager.signoutRedirect();
        navigate('/');
    };
    const handleMouseEnter = () => {
        setIsHovered(true);
    }

    const handleMouseLeave = () => {
        setIsHovered(false);
    }

    const handleImageUpload = (e) => {
        setLoadingOverlay(true);
        const file = e.target.files[0];
        const maxSize = 5 * 1024 * 1024;
        const maxResolution = 1920;

        if (file && !isImageFile(file)) {
            toast.error('Allowed extensions: image/jpeg, image/png, image/svg+xml, image/webp.', {
                position: "top-right",
                autoClose: 5000,
                hideProgressBar: false,
                closeOnClick: true,
                pauseOnHover: true,
                draggable: true,
                progress: undefined,
            });
            e.target.value = null;
            setLoadingOverlay(false);
            return;
        }

        if (file && file.size > maxSize) {
            toast.error('The max size of photo: 2mb.', {
                position: "top-right",
                autoClose: 5000,
                hideProgressBar: false,
                closeOnClick: true,
                pauseOnHover: true,
                draggable: true,
                progress: undefined,
            });
            e.target.value = null;
            setLoadingOverlay(false);
            return;
        }

        if (file) {
            const img = new Image();
            img.onload = () => {
                const width = img.width;
                const height = img.height;
                if (width > maxResolution || height > maxResolution) {
                    toast.error(`The maximum image resolution must be ${maxResolution}x${maxResolution} pixels.`, {
                        position: "top-right",
                        autoClose: 5000,
                        hideProgressBar: false,
                        closeOnClick: true,
                        pauseOnHover: true,
                        draggable: true,
                        progress: undefined,
                    });
                    e.target.value = null;
                    setLoadingOverlay(false);
                    return;
                }
                else {
                    const reader = new FileReader();
                    reader.onloadend = async () => {
                        const imageData = reader.result;
                        const blob = new Blob([new Uint8Array(imageData)], { type: file.type });
                        const base64Avatar = await new Promise((resolve) => {
                            const reader = new FileReader();
                            reader.onloadend = () => resolve(reader.result.split(',')[1]);
                            reader.readAsDataURL(blob);

                        });
                        try {
                            const accessToken = await userManager.getUser().then(user => user.access_token);
                            const response = await fetch(`${config.apiBaseUrl}/userProfilePhotoUpload`, {
                                method: 'POST',
                                headers: {
                                    'Authorization': `Bearer ${accessToken}`,
                                    'Content-Type': 'application/json'
                                },
                                body: JSON.stringify({ avatar: base64Avatar })
                            });

                            if (!response.ok) {
                                toast.error('Smth went wrong.', {
                                    position: "top-right",
                                    autoClose: 5000,
                                    hideProgressBar: false,
                                    closeOnClick: true,
                                    pauseOnHover: true,
                                    draggable: true,
                                    progress: undefined,
                                });
                                throw new Error(`Error from server: ${response.status} ${response.statusText}`);
                            }

                            else {
                                const data = await response.json();
                                setUserData(data);
                                toast.success('Profile photo uploaded successfully.', {
                                    position: "top-right",
                                    autoClose: 5000,
                                    hideProgressBar: false,
                                    closeOnClick: true,
                                    pauseOnHover: true,
                                    draggable: true,
                                    progress: undefined,
                                });
                            }
                           
                        } catch (err) {
                            toast.error(`Error occurred: ${err.message}`, {
                                position: "top-right",
                                autoClose: 5000,
                                hideProgressBar: false,
                                closeOnClick: true,
                                pauseOnHover: true,
                                draggable: true,
                                progress: undefined,
                            });
                            console.error('Error while sending the request', err);
                        } finally {
                            setLoadingOverlay(false);
                        }
                    };
                    reader.readAsArrayBuffer(file);
                }
            };
            img.src = URL.createObjectURL(file);
        }
    };

    const handleImageDelete = async  (e) => {
        setLoadingOverlay(true);
        if (userData.photo == null || (Array.isArray(userData.photo) && userData.photo.length === 0) || userData.photo =='') {
            toast.error('You do not have a photo', {
                position: "top-right",
                autoClose: 5000,
                hideProgressBar: false,
                closeOnClick: true,
                pauseOnHover: true,
                draggable: true,
                progress: undefined,
            });
            e.target.value = null;
            setLoadingOverlay(false);
            return;
        }
        
        try {

            const accessToken = await userManager.getUser().then(user => user.access_token);
            await fetch(`${config.apiBaseUrl}/userProfilePhotoUpload`, {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${accessToken}`,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ avatar: "" })
            });

        }
        catch (err) {
            toast.error(err, {
                position: "top-right",
                autoClose: 5000,
                hideProgressBar: false,
                closeOnClick: true,
                pauseOnHover: true,
                draggable: true,
                progress: undefined,
            });
        }
        setLoadingOverlay(false);
    };

    const isImageFile = (file) => {
        const acceptedImageTypes = ['image/jpeg', 'image/png', 'image/svg+xml', 'image/webp'];

        return acceptedImageTypes.includes(file.type);
    };


    return (

        <div>
            <ToastContainer position="top-right" autoClose={5000} hideProgressBar newestOnTop closeOnClick rtl={false} pauseOnFocusLoss draggable pauseOnHover />
            {loadingOverlay ? <div className={`overlay ${loadingOverlay ? 'visible' : ''}`}>
                <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh' }}>
                    <ThreeDots color="#00BFFF" height={80} width={80} />
                </div>
                

            </div>
                : isAuthorized === false ? (
                <div>UnAuthorized</div>
                ) : userData === null ? (
                        <div>
                            <div>There is no information</div>
                            <button onClick={onLogout} className="ProfileButton">Logout</button>
                        </div>
                
            ) : (
                <div className="profile">
                    {userData && (
                        <>
                        <div className="UpProfile">
                                            <div className="First_UpProfile">

                                                <div className="avatar-container" onMouseEnter={handleMouseEnter} onMouseLeave={handleMouseLeave}>
                                                    <img src={userData.photo ? `data:image/jpeg;base64,${userData.photo}` : "../../public/NoPhoto.jpg"} alt="Avatar" className="avatar" />
                                                    <div className="buttons-container">
                                                        <label className="edit-button">
                                                            New
                                                            <input type="file" name="clientAvatar" accept="image/*" onChange={handleImageUpload} style={{ display: 'none' }} capture="false" />
                                                        </label>
                                                        <button className="delete-button" onClick={handleImageDelete}>Delete</button>
                                                    </div>
                                                </div>




                                                <div className="Profile_Information">
                                                    <div className="profile-info">
                                                        <h2>Name: {userData.name}</h2>
                                                        <h3>NickName: {userData.nickName}</h3>
                                                        <p>Bio: {userData.bio}</p>
                                                        <p>Date of Birth: {new Date(userData.dateOfBirth).toLocaleDateString()}</p>
                                                    </div>
                                                    <NavLink to="/Profile_Settings" className="ProfileButton" >Settings</NavLink>
                                                    <button onClick={onLogout} className="ProfileButton">Logout</button>
                                                </div>
                                                
                                            </div>

                                            <div className="Second_UpProfile">
                                                <h2>Followers: {userData.followersCount}</h2>
                                                <h2>Following: {userData.followsCount}</h2>
                                            </div>
                             </div>
                            
                        </>
                    )}
                </div>
            )}
        </div>
    );
};

export default Profile;
