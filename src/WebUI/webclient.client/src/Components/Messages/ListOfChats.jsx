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

const ListOfChats = () => {

    const [isAuthorized, setIsAuthorized] = useState(null);
    const [user, setUser] = useState(null);
    const [userData, setUserData] = useState(null);
    const [loading, setLoading] = useState(true);
    const [loadingOverlay, setLoadingOverlay] = useState(false);
    const [chats, setChats] = useState(null);
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
                ) : userData === null || chats===null ? (
                    <div>
                        <div>There is any chats(</div>
                    </div>

                ) : (
                            <div>
                                {chats.map((chat, index) => (
                                    <div key={index} style={{ marginBottom: '20px', paddingBottom: '10px', borderBottom: '1px solid #ccc' }}>
                                        <div>Chat ID: {chat.chatId}</div>
                                        <div>Contact ID: {chat.contactId}</div>
                                        <div>Last Activity: {chat.lastActivity ? new Date(chat.lastActivity).toLocaleString() : 'N/A'}</div>
                                        <div>Last Message: {chat.lastMessage || 'No message'}</div>
                                        <div>Last Message Sender: {chat.lastMessageSender}</div>
                                        <div>Contact NickName: {chat.contactNickName}</div>
                                        {chat.contactPhoto && (
                                            <div>
                                                <img src={`data:image/jpeg;base64,${chat.contactPhoto}`} alt="Contact" style={{ maxWidth: '100px', maxHeight: '100px' }} />
                                            </div>
                                        )}
                                    </div>
                                ))}
                            </div>
                )}
        </div>
    );
};

export default ListOfChats;
