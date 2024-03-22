import { useState, useEffect, useRef } from 'react';
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
import { useParams } from 'react-router-dom';
import { useAuth } from '../AuthProvider';
import '../Styles/OpenedChat.css'

function OpenedChat({ chatId }) {
    const { user, userData, loading, isAuthorized, setLoadingState,
        setIsAuthorizedState,
        setUserState,
        setUserDataState, chats, activeChatId,
        setActiveChatId, unknownsmbData, setunknownsmbDataState } = useAuth();

    const foundChat = chats.find(chat => chat.chatId === chatId);
    const [messages, setMessages] = useState(null);
    const navigate = useNavigate();

    async function fetchMessages(accessToken) {
        try {
            const ChatId = foundChat.chatId;
            const response = await fetch(`${config.apiBaseUrl}/GetChatMessages`, {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${accessToken}`,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ ChatId })

            });

            if (!response.ok) {
                throw new Error(`HTTP error! Status: ${response.status}`);
            }
            const responseJs = await response.json();
            setMessages(responseJs);
        } catch (error) {
            console.error('Error while sending the request to the UserService ', error);
            return null;
        }
    }

    const handleImageClick = (contactId) => {
        navigate(`/Someones_Profile/${foundChat.contactId}`);
    };
    const messagesEndRef = useRef(null);

    useEffect(() => {
        messagesEndRef.current?.scrollIntoView({ behavior: "smooth" });
        async function checkAuth() {
            if (foundChat) {
                await fetchMessages(user.access_token);
            }
            setLoadingState(false);
        }

        if (!loading) {
            checkAuth();
        }
    }, [loading, isAuthorized, user, chatId, unknownsmbData]);
    return (
        <div className="openedChat">

            
                {foundChat ? (
                    <div className="Messages">
                        <div>
                            {messages ? (
                                messages.map((message, index) => (
                                    <div key={index} className="Message">
                                        <div className="Messagecontactimage">
                                            {message.senderId === foundChat.contactId ? (
                                                <img
                                                    src={foundChat.contactPhoto ? `data:image/jpeg;base64,${foundChat.contactPhoto}` : "../../public/NoPhoto.jpg"}
                                                    alt="Contact"
                                                    onClick={(e) => { e.stopPropagation(); handleImageClick(foundChat.contactId); }}
                                                />
                                            ) : (
                                                <img
                                                    src={userData.photo ? `data:image/jpeg;base64,${userData.photo}` : "../../public/NoPhoto.jpg"}
                                                    alt="User"
                                                />
                                            )}
                                        </div>
                                        <div className="MessageBody">
                                            <div className="MessageContent">{message.content}</div>
                                            <div className="MessageDate">{new Date(message.date).toLocaleString()}</div>
                                        </div>
                                        <div ref={messagesEndRef} />
                                    </div>



                                ))
                            ) : (
                                <div>There is any messages</div>
                            )}
                            <div className="SendMessageForm">
                                <textarea className="SendMessageInput" placeholder="Type your message here..."></textarea>
                                <button className="SendMessageButton">Send</button>
                            </div>
                        </div>
                    </div>
                ) : unknownsmbData ?
                    (
                        <div>jkdfbdgb

                        </div>

                    ) : (<div>((((</div>)}
            </div>


    );
}

export default OpenedChat;
