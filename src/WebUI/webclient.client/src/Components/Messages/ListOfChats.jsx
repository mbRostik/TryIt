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
import OpenedChat from './OpenedChat'; 

const ListOfChats = () => {

    const { user, userData, loading, isAuthorized, setLoadingState,
        setIsAuthorizedState,
        setUserState,
        setUserDataState, chats, activeChatId,
        setActiveChatId, unknownsmbData, setunknownsmbDataState, hubConnection, setChatsState } = useAuth();   
    const navigate = useNavigate();
    
    const handleImageClick = (contactId) => {
        navigate(`/Someones_Profile/${contactId}`);
    };
    const handleInfoClick = (chatId) => {
        setActiveChatId(chatId);
        setunknownsmbDataState(null);
    };
    return (

        <div className = "MessagePage">
            <ToastContainer position="top-right" autoClose={5000} hideProgressBar newestOnTop closeOnClick rtl={false} pauseOnFocusLoss draggable pauseOnHover />
            {loading ? <div className={`overlay ${loading ? 'visible' : ''}`}>
            </div>
                : isAuthorized === false ? (
                    <div>UnAuthorized</div>
                ) : userData === null || chats === null && activeChatId==null ? (
                    <div>
                            <div>There is nothing( {console.log(userData + " " + activeChatId)} </div>
                    </div>

                    ) : (
                                <div className="Chat_Container">
                                  <div className="Chat_LeftSide">
                                    
                                    {chats && Array.isArray(chats) && chats.map((chat, index) => (
                                            <div key={index}
                                            className={`Chat_contact ${chat.chatId === activeChatId ? "active" : ""}`} 
                                                onClick={() => handleInfoClick(chat.chatId)}>
                                                <div >
                                                <img className="Chat_contactimage"
                                                    src={chat.contactPhoto ? `data:image/jpeg;base64,${chat.contactPhoto}` : "../../public/NoPhoto.jpg"}
                                                        alt="Contact"
                                                        onClick={(e) => { e.stopPropagation(); handleImageClick(chat.contactId); }} 
                                                    />
                                                </div>

                                            <div className="Chat_info">
                                                <div className="Chat_info_up">
                                                        <div>{chat.contactNickName}</div>                                                    </div>
                                                    <div>
                                                        {chat.lastMessageSender !== chat.contactId ? "You: " : ""}
                                                        {chat.lastMessage && chat.lastMessage.length > 15
                                                            ? chat.lastMessage.substring(0, 15) + '...'
                                                            : chat.lastMessage || 'No message'}
                                                </div>
                                                <div className="LastActivityChat">{chat.lastActivity ? new Date(chat.lastActivity).toLocaleString() : 'N/A'}</div>

                                                </div>

                                            </div>
                                        ))}

                                    </div>

                                <div className="Chat_RightSide">
                                        {activeChatId && <OpenedChat chatId={activeChatId} />}
                                    </div>
                                </div>
                                

                )}
        </div>
    );
};

export default ListOfChats;
