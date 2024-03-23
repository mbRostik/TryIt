import { createContext, useContext, useState, useEffect } from 'react';
import PropTypes from 'prop-types';
import userManager from '../AuthFiles/authConfig';
import config from '../config.json';
import * as signalR from "@microsoft/signalr";

const AuthContext = createContext();

export function useAuth() {
    return useContext(AuthContext);
}

export const AuthProvider = ({ children }) => {
    const [hubConnection, setHubConnection] = useState(null);
    const [shubConnection, setsHubConnection] = useState(null);

    const [user, setUser] = useState(null);
    const [userData, setUserData] = useState(null);
    const [loading, setLoading] = useState(true);
    const [isAuthorized, setIsAuthorized] = useState(false);
    const [unknownsmbData, setunknownsmbData] = useState(null);
    const [chats, setChats] = useState(null);
    const [activeChatId, setActiveChatId] = useState(null);

    const setLoadingState = (isLoading) => setLoading(isLoading);
    const setIsAuthorizedState = (isAuth) => setIsAuthorized(isAuth);
    const setUserState = (newUser) => setUser(newUser);
    const setUserDataState = (newUserData) => setUserData(newUserData);
    const setunknownsmbDataState = (smbData) => setunknownsmbData(smbData);
    const setChatsState = (smbData) => setChats(smbData);
    async function fetchUserData(accessToken) {
        try {
            const response = await fetch(`${config.apiBaseUrl}/user`, {
                headers: { 'Authorization': `Bearer ${accessToken}` }
            });

            if (!response.ok) throw new Error(`HTTP error! Status: ${response.status}`);

            return await response.json();
        } catch (error) {
            console.error('Error while sending the request to the UserService', error);
            return null;
        }
    }

    async function fetchChatData(accessToken) {
        try {
            const response_chats = await fetch(`${config.apiBaseUrl}/GetUserChats`, {
                method: 'GET',
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
            console.error('Error while fetching chats', error);
        }
    }

    useEffect(() => {
        const checkAuth = async () => {
            setLoading(true);
            const user = await userManager.getUser();
            if (user) {
                setUser(user);
                const userData = await fetchUserData(user.access_token);
                if (userData) {
                    setIsAuthorized(true);
                    setUserData(userData);
                    await fetchChatData(user.access_token);
                } else {
                    setIsAuthorized(false);
                }
            } else {
                setIsAuthorized(false);
            }
            setLoading(false);
        };

        checkAuth();
    }, []);

    useEffect(() => {
        if (user && !hubConnection) {
            const connection = new signalR.HubConnectionBuilder()
                .withUrl(`https://localhost:7234/SendMessage`, {
                    accessTokenFactory: () => user.access_token
                })
                .configureLogging(signalR.LogLevel.None)
                .build();

            const subscribeToEvents = (conn) => {
                const receiveMessage = (message) => {
                    console.log(message);
                    setChatsState(prevChats => prevChats.map(chat => {
                        if (chat.chatId === message.chatId) {
                            return {
                                ...chat,
                                lastActivity: message.date,
                                lastMessage: message.content,
                                lastMessageSender: message.senderId
                            };
                        }
                        return chat;
                    }));
                };

                conn.on("ReceiveMessage", receiveMessage);
            };

            const subscribeToNewChat = async (conn) => {
                await fetchChatData(user.access_token);
                conn.on("NotifyNewChat");

            };

            connection.onclose(async () => {
                setHubConnection(null);

                const connection2 = new signalR.HubConnectionBuilder()
                    .withUrl(`https://localhost:7234/SendMessage`, {
                        accessTokenFactory: () => user.access_token
                    })
                    .configureLogging(signalR.LogLevel.None)
                    .build();

                subscribeToEvents(connection2);
                subscribeToNewChat(connection2);
                try {
                    await connection2.start();
                    setHubConnection(connection2);
                } catch (err) {
                    console.log('');
                }
            });
            connection.start()
                .then(() => {
                    subscribeToNewChat(connection);
                    subscribeToEvents(connection);
                    setHubConnection(connection);
                })
                .catch(err => console.error('SignalR Connection Error:', err));

            return () => {
                connection.off("ReceiveMessage");
            };
        }
    }, [user, hubConnection, setHubConnection, setChatsState, chats]);

    useEffect(() => {
        if (hubConnection && chats) {
            chats.forEach(chat => {
                hubConnection.invoke("JoinChat", chat.chatId)
                    .catch(err => console.error(`Could not join chat ${chat.chatId}:`, err));
            });
        }
    }, [hubConnection, chats]);

    

    const value = {
        user,
        userData,
        loading,
        isAuthorized,
        setLoadingState,
        setIsAuthorizedState,
        setUserState,
        setUserDataState,
        chats,
        setChatsState,
        activeChatId,
        setActiveChatId,
        unknownsmbData,
        setunknownsmbDataState,
        hubConnection
    };

    return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};

AuthProvider.propTypes = {
    children: PropTypes.node.isRequired
};
