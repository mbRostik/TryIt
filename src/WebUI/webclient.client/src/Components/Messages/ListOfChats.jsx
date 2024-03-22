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
        setActiveChatId, unknownsmbData, setunknownsmbDataState } = useAuth();   
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
                                <div className="chatContainer">
                                    <div className="LeftSide">
                                        {chats.map((chat, index) => (
                                            <div key={index}
                                                className={`contact ${chat.chatId === activeChatId ? "active" : ""}`} 
                                                onClick={() => handleInfoClick(chat.chatId)}>

                                                <div >
                                                    <img className="contactimage"
                                                        src={chat.contactPhoto ? `data:image/jpeg;base64,${chat.contactPhoto}` : "../../public/NoPhoto.jpg"}
                                                        alt="Contact"
                                                        onClick={(e) => { e.stopPropagation(); handleImageClick(chat.contactId); }} 
                                                    />
                                                </div>

                                                <div className="info">
                                                    <div className="info_up">
                                                        <div>{chat.contactNickName}</div>
                                                        <div>{chat.lastActivity ? new Date(chat.lastActivity).toLocaleString() : 'N/A'}</div>
                                                    </div>
                                                    <div>
                                                        {chat.lastMessageSender !== chat.contactId ? "You: " : ""}
                                                        {chat.lastMessage && chat.lastMessage.length > 18
                                                            ? chat.lastMessage.substring(0, 18) + '...'
                                                            : chat.lastMessage || 'No message'}
                                                    </div>
                                                </div>

                                            </div>
                                        ))}

                                    </div>

                                    <div className="RightSide">
                                        {activeChatId && <OpenedChat chatId={activeChatId} />}
                                    </div>
                                </div>
                                

                )}
        </div>
    );
};

export default ListOfChats;
