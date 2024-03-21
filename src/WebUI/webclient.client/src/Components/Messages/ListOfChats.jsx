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
import { useAuth } from '../AuthProvider';
import '../Styles/ChatsList.css'

const ListOfChats = () => {

    const { user, userData, loading, isAuthorized, setLoadingState,
        setIsAuthorizedState,
        setUserState,
        setUserDataState } = useAuth();
    const [chats, setChats] = useState(null);
    async function fetchChatData(accessToken) {
        try {
            
            const response_chats = await fetch(`${config.apiBaseUrl}/GetUserChats`, {
                method: 'Get',
                headers: {
                    'Authorization': `Bearer ${accessToken}`,
                    'Content-Type': 'application/json'
                }
            });

            if (response_chats.ok) {
                const chatsData = await response_chats.json();
                setChats(chatsData);
            } else {
                console.error('Error while receiving chats:', response_chats.statusText);
            }
        } catch (error) {
            console.error('Error while sending the request to the UserService ', error);
            return null;
        }
    }



    useEffect(() => {
        const checkAuth = async () => {

            if (isAuthorized) {
                if (user) {
                    fetchChatData(user.access_token);
                    }
                }
            setLoadingState(false);
        };

        if (!loading) {
            checkAuth();
        }
    }, [loading, isAuthorized, user]);

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
                ) : userData === null || chats===null ? (
                    <div>
                        <div>There is any chats(</div>
                    </div>

                    ) : (
                        <div>
                            <h2>Your chats</h2>
                            <br></br>
                            {chats.map((chat, index) => (
                                <div key={index} className="contact">
                                    
                                    <div >
                                        <img className="contactimage"
                                            src={chat.contactPhoto ? `data:image/jpeg;base64,${chat.contactPhoto}` : "../../public/NoPhoto.jpg"}
                                            alt="Contact"
                                        />
                                    </div>
                                    <div className="info">
                                        <div>{chat.contactNickName}</div>
                                        <div>{chat.sender === user.id ? "You: " : ""}{chat.lastMessage || 'No message'}</div>
                                        <div>{chat.lastActivity ? new Date(chat.lastActivity).toLocaleString() : 'N/A'}</div>
                                           
                                    </div>
                                       
                                       
                                </div>
                            ))}

                        </div>
                )}
        </div>
    );
};

export default ListOfChats;
