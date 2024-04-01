import React from 'react';
import ReactDOM from 'react-dom/client';
import { BrowserRouter as Router, Route, Routes, useLocation } from 'react-router-dom';
import { AuthProvider } from './Components/AuthProvider';
import App from './App';
import SignIn_CallbackPage from './AuthFiles/SignIn_CallbackPage';
import SignOut_CallBackPage from './AuthFiles/SignOut_CallBackPage';
import NavBar from './Components/NavBar/NavBar';
import Left_Bar from './Components/NavBar/Left_Bar';
import { useAuth } from './Components/AuthProvider';
import { useNavigate } from 'react-router-dom';

import Profile from './Components/Profile/Profile';
import Profile_Settings from './Components/Profile/Profile_Settings';
import Someones_Profile from './Components/Profile/Someones_Profile';
import ListOfChats from './Components/Messages/ListOfChats';
import './index.css';
import { useState, useEffect } from 'react';

const root = ReactDOM.createRoot(document.getElementById('root'));

function AppContainer() {
    const { user, userData, loading, isAuthorized, setLoadingState,
        setIsAuthorizedState,
        setUserState,
        setUserDataState, chats, activeChatId,
        setActiveChatId, unknownsmbData, setunknownsmbDataState, hubConnection, setChatsState } = useAuth();   

    const location = useLocation();
    const [isChatPage, setIsChatPage] = useState(false);
    const navigate = useNavigate();

    const handleImageClick = (contactId) => {
        navigate(`/Someones_Profile/${contactId}`);
    };
    const handleInfoClick = (chatId) => {
        console.log("Activating " + chatId);
        setActiveChatId(chatId);
        setunknownsmbDataState(null);
        navigate(`/Chats`);

    };

    React.useEffect(() => {
        setIsChatPage(location.pathname === "/Chats");
    }, [location]);

    return (
        <div className="All_container">
            <div className="LeftSide">
                <Left_Bar />
            </div>
            <div className="Main_container">
                <div className="Up_Bar">
                    <NavBar />
                </div>
                <div className="Centre_Div">
                <div className="Centre">
                    <Routes>
                        <Route path="/" element={<App />} />
                        <Route path="/Profile" element={<Profile />} />
                        <Route path="/signin-oidc" element={<SignIn_CallbackPage />} />
                        <Route path="/Profile_Settings" element={<Profile_Settings />} />
                        <Route path="/signout-callback-oidc" element={<SignOut_CallBackPage />} />
                        <Route path="/Someones_Profile/:ProfileId" element={<Someones_Profile />} />
                        <Route path="/Chats" element={<ListOfChats />} />
                    </Routes>
                </div>
                {!isChatPage && isAuthorized && (
                        <div className="RightSide">
                            <div className="RightSideComponent">
                                <div className="RightSideTitle">
                                    Messages

                                </div>
                                <div className="RightChats">
                                    {chats && Array.isArray(chats) ? <div className="SeparateRightChat">
                                        {chats && Array.isArray(chats) && chats.map((chat, index) => (
                                            <div key={index}
                                                className={`contact ${chat.chatId === activeChatId ? "active" : ""}`}
                                                onClick={() => handleInfoClick(chat.chatId)}>

                                                <div >
                                                    <img className="Right_contactimage"
                                                        src={chat.contactPhoto ? `data:image/jpeg;base64,${chat.contactPhoto}` : "../../public/NoPhoto.jpg"}
                                                        alt="Contact"
                                                        onClick={(e) => { e.stopPropagation(); handleImageClick(chat.contactId); }}
                                                    />
                                                </div>
                                                
                                                <div className="Right_info">
                                                    <div className="Right_info_up">
                                                        <div>{chat.contactNickName}</div>
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

                                        : <div className="RightSideTitle">There is nothing</div>}
                                </div>
                            </div>
                                
                        </div>
                    )}
                </div>
            </div>
        </div>
    );
}

root.render(
    <React.StrictMode>
        <AuthProvider>
            <Router>
                <AppContainer />
            </Router>
        </AuthProvider>
    </React.StrictMode>
);