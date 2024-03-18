import { useState, useEffect } from 'react';
import userManager from '../../AuthFiles/authConfig';
import { isAuthenticated } from '../../Functions/CheckAuthorization';
import { NavLink } from 'react-router-dom';
import { ThreeDots } from 'react-loader-spinner';
import { Link, useNavigate } from 'react-router-dom';
import '../Styles/Profile.css'
import { useParams } from 'react-router-dom';
import axios from '../../../node_modules/axios/index';
import { ToastContainer, toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
const Someones_Profile = () => {

    const [isAuthorized, setIsAuthorized] = useState(null);
    const [user, setUser] = useState(null);
    const [userData, setUserData] = useState(null);
    const [loading, setLoading] = useState(true);
    const [loadingOverlay, setLoadingOverlay] = useState(false);
    const { id } = useParams();

    async function fetchUserData(accessToken) {
        try {
            const response = await fetch('https://localhost:7062/ocelot/SomeonesProfile', {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${accessToken}`,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ id })
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
                    </div>

                ) : (
                    <div className="profile">
                        {userData && (
                            <>
                                <div className="UpProfile">
                                    <div className="First_UpProfile">
                                        <div className="avatar-container">
                                            <img src={userData.photo ? `data:image/jpeg;base64,${userData.photo}` : "../../public/NoPhoto.jpg"} alt="Avatar" className="avatar" />
                                        </div>
                                        <div className="Profile_Information">
                                            <div className="profile-info">
                                                <h2>Name: {userData.name}</h2>
                                                <h3>NickName: {userData.nickName}</h3>
                                                <p>Bio: {userData.bio}</p>
                                                <p>Date of Birth: {new Date(userData.dateOfBirth).toLocaleDateString()}</p>
                                            </div>
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

export default Someones_Profile;
