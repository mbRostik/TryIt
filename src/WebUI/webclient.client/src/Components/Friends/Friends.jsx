import { useState, useEffect } from 'react';
import userManager from '../../AuthFiles/authConfig';
import { isAuthenticated } from '../../Functions/CheckAuthorization';
import { NavLink } from 'react-router-dom';
import { ThreeDots } from 'react-loader-spinner';
import { Link, useNavigate } from 'react-router-dom';
import '../Styles/Friends.css'
import axios from '../../../node_modules/axios/index';
import { ToastContainer, toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import config from '../../config.json'; 
import { useAuth } from '../AuthProvider';

import ReactCrop from 'react-image-crop';
import 'react-image-crop/dist/ReactCrop.css';

const Friends = () => {
    const navigate = useNavigate();
    const [isHovered, setIsHovered] = useState(false);
    const { user, userData, loading, isAuthorized, setLoadingState,
        setIsAuthorizedState,
        setUserState,
        setUserDataState, chats, activeChatId,
        setActiveChatId, openedChat, unknownsmbData, setunknownsmbDataState } = useAuth();

    const [friends, setFrieds] = useState(null);
    async function fetchFriendsData(accessToken) {
        try {
            const response_posts = await fetch(`${config.apiBaseUrl}/GetUserFriends`, {
                method: 'GET',
                headers: {
                    'Authorization': `Bearer ${accessToken}`,
                    'Content-Type': 'application/json'
                }
            });
            let response = await response_posts.json();
            if (response_posts.ok) {
                setFrieds(response);
                console.log("Fetching friends");
            }
        } catch (error) {
            console.log('There is no friend');
        }
    }
    const OpenChat = async (ProfileId) => {
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
                if (response.status === 400) {
                    const errorData = await response.json();
                    console.error('Validation errors:', errorData);

                    if (Array.isArray(errorData)) {
                        errorData.forEach(err => {
                            console.error(err.error);
                        });
                    }
                    throw new Error('Validation failed');
                } else {
                    throw new Error(`HTTP error! Status: ${response.status}`);
                }
            }
            const data = await response.json();
            console.log(data);
            if (data) {
                setActiveChatId(data);
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

    const handleImageClick = (contactId) => {
        navigate(`/Someones_Profile/${contactId}`);
    };
    const Follow = async (ProfileId, isCurrentlyFollowed) => {
        const accessToken = user.access_token;
        setLoadingState(true);

        try {
            const response = await fetch(`${config.apiBaseUrl}/Follow`, {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${accessToken}`,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ ProfileId })
            });
            if (!response.ok) {
                if (response.status === 400) {
                    const errorData = await response.json();
                    console.error('Validation errors:', errorData);

                    if (Array.isArray(errorData)) {
                        errorData.forEach(err => {
                            console.error(err.error);
                        });
                    }
                    throw new Error('Validation failed');
                } else {
                    throw new Error(`HTTP error! Status: ${response.status}`);
                }
            }

            const data = await response.json();
            if (data) {
                setFrieds(friends.map(friend => {
                    if (friend.id === ProfileId) {
                        return { ...friend, isFollowedByUser: !isCurrentlyFollowed };
                    }
                    return friend;
                }));
            } else {
                console.error('Error returned from the server');
            }
        } catch (error) {
            console.error('Error while following / unfollowing ', error);
        } finally {
            setLoadingState(false);
        }
    };
    useEffect(() => {
        const asyncFetchingFriends = async () => {
            try {
                setLoadingState(true); 
                if (isAuthorized) {
                    await fetchFriendsData(user.access_token);
                }
            } catch (error) {
                console.error("Error while getting friends", error);
            } finally {
                setLoadingState(false); 
            }
        };

        asyncFetchingFriends();

        console.log(friends); 

    }, [isAuthorized]);

    return (
        <div>
            <ToastContainer position="top-right" autoClose={5000} hideProgressBar newestOnTop closeOnClick rtl={false} pauseOnFocusLoss draggable pauseOnHover />
            {loading ? <div className={`overlay ${loading ? 'visible' : ''}`}>
                <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh' }}>
                </div>
                

            </div>
                : isAuthorized === false ? (
                <div>UnAuthorized</div>
                ) : userData === null ? (
                        <div>
                            <div>There is no information</div>
                        </div>
                
            ) : (
                            <div className="FriendsContainer">
                                {friends ? (
                                    friends.map((friend, index) => (

                                        <div key={index} className="FriendItem">
                                            <div className="LeftAndCentreFriend">
                                                <div className="LeftFriendSide">
                                                    <img className="Friend_Image"
                                                        src={friend.photo ? `data:image/jpeg;base64,${friend.photo}` : "../../public/NoPhoto.jpg"}
                                                        alt="Friend"
                                                        onClick={() => handleImageClick(friend.id)}
                                                    />
                                                </div>
                                                <div className="CentreFriendSide">
                                                    <div className="FriendInfo">
                                                        <div className="FriendNickName">{friend.nickName}</div>
                                                        <div className="FriendBio">
                                                            {friend.bio.length > 25 ? `${friend.bio.substring(0, 25)}...` : friend.bio}
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                           
                                            <div className="RightFriendSide">
                                                <button className="MessageButton" onClick={() => OpenChat(friend.id)}>Message</button>
                                                <button className="UnFollowButton" onClick={() => Follow(friend.id, friend.isFollowedByUser)} > {friend.isFollowedByUser ? "UnFollow" : "Follow"}</button>
                                            </div>
                                           
                                        </div>
                                    ))
                                ) : (
                                        <div><h1>No friends to display.</h1></div> 
                                )}
                            </div>
            )}
        </div>
    );
};

export default Friends;
