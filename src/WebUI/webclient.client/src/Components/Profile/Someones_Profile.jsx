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
import config from '../../config.json'; 
import { useAuth } from '../AuthProvider';

const Someones_Profile = () => {
    const navigate = useNavigate();

    const [smbData, setsmbData] = useState(null);
    const { ProfileId } = useParams();
    const { user, userData, loading, isAuthorized, setLoadingState,
        setIsAuthorizedState,
        setUserState,
        setUserDataState, chats, activeChatId,
        setActiveChatId, openedChat, unknownsmbData, setunknownsmbDataState } = useAuth();   
    async function fetchsmbData(accessToken) {
        try {
            const response = await fetch(`${config.apiBaseUrl}/SomeonesProfile`, {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${accessToken}`,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ ProfileId })
            });

            if (ProfileId == null || ProfileId == '') {
                console.log("Smth went wrong");
            }



            if (!response.ok) {
                throw new Error(`HTTP error! Status: ${response.status}`);
            }

            return await response.json();
        } catch (error) {
            setLoadingState(false);
            console.error('Error while sending the request to the UserService ', error);
            return null;
        }
    }


    useEffect(() => {
        async function checkAuth() {

            if (isAuthorized) {
                if (user) {
                    const data = await fetchsmbData(user.access_token);
                    setsmbData(data);
                }
            }
            setLoadingState(false);
        }

        if (!loading) {
            checkAuth();
        }
    }, [loading, isAuthorized, user]);
   
    
    const OpenChat = async () => {
        const accessToken = user.access_token;
        let existingChat = null;
        setLoadingState(true);
        if (chats !== null) {
            existingChat = chats.find(chat => chat.contactId === ProfileId);
        }
        if (existingChat) {
            setActiveChatId(existingChat.chatId); 
            navigate(`/Chats`); 
            setLoadingState(false);
            return;
        }

        try {
            const response = await fetch(`${config.apiBaseUrl}/GetChatByUserId`, {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${accessToken}`,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ ProfileId })
            });
            if (!response.ok) {
                throw new Error(`HTTP error! Status: ${response.status}`);
            }
            const data = await response.json();
            console.log(data);
            if (data) {
                setActiveChatId(data); 
                setunknownsmbDataState(smbData);
                navigate(`/Chats`); 
            } else {
                console.error('No chatId returned from the server');
            }
        } catch (error) {
            console.error('Error while creating or fetching the chat', error);
        } finally {
            setLoadingState(false);
        }
    };


    return (

        <div>
            <ToastContainer position="top-right" autoClose={5000} hideProgressBar newestOnTop closeOnClick rtl={false} pauseOnFocusLoss draggable pauseOnHover />
            {loading ? <div className={`overlay ${loading ? 'visible' : ''}`}>
                <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh' }}>
                    <ThreeDots color="#00BFFF" height={80} width={80} />
                </div>


            </div>
                : isAuthorized === false ? (
                    <div>UnAuthorized</div>
                ) : smbData === null ? (
                    <div>
                        <div>There is no information</div>
                    </div>

                ) : (
                    <div className="profile">
                                {smbData && (
                            <>
                                <div className="UpProfile">
                                    <div className="First_UpProfile">
                                        <div className="avatar-container">
                                                    <img src={smbData.photo ? `data:image/jpeg;base64,${smbData.photo}` : "../../public/NoPhoto.jpg"} alt="Avatar" className="avatar" />
                                        </div>
                                        <div className="Profile_Information">
                                            <div className="profile-info">
                                                        <h2>Name: {smbData.name}</h2>
                                                        <h3>NickName: {smbData.nickName}</h3>
                                                        <p>Bio: {smbData.bio}</p>
                                                        <p>Date of Birth: {new Date(smbData.dateOfBirth).toLocaleDateString()}</p>
                                            </div>
                                        </div>
                                    </div>
                                    <div className="Second_UpProfile">
                                                <h2>Followers: {smbData.followersCount}</h2>
                                                <h2>Following: {smbData.followsCount}</h2>
                                            </div>
                                            <div>
                                                <button className="delete-button" onClick={OpenChat}>Write</button>

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
