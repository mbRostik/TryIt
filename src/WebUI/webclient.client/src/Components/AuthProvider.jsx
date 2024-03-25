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
    const [chatReady, setChatReady] = useState(false);


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
            console.log('Error while sending the request to the UserService');
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
                setChats(await response_chats.json());
                console.log("Fetching chats");
            } 
        } catch (error) {
            console.log('There is no chats');
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
                    setChatReady(true);
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
        if (user && !hubConnection && chatReady) {
            const connection = new signalR.HubConnectionBuilder()
                .withUrl(`https://localhost:7234/SendMessage`, {
                    accessTokenFactory: () => user.access_token
                })
                .withAutomaticReconnect()
                .configureLogging(signalR.LogLevel.None)
                .build();


            const subscribeToEvents = (conn) => {
                console.log("ReceiveMessage connected");
                const receiveMessage = (message) => {
                    if (chats) {
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
                        }
                        ));
                    }
                };

                conn.on("ReceiveMessage", receiveMessage);
            };

            const subscribeToNewChat = (conn) => {
                console.log("Subscribing to NotifyNewChat");

                conn.on("NotifyNewChat", async () => {
                    console.log("New chat notification received, fetching chat data...");
                    try {
                        await fetchChatData(user.access_token);
                        console.log("Chat data fetched successfully.");
                    } catch (error) {
                        console.error("Failed to fetch chat data:", error);
                    }
                });
            };

            const startConnection = (conn) => {
                try {

                     conn.start().then(() => {
                         connectSubscriptions(conn);
                        setHubConnection(connection);
                    })
                        .catch(err => console.log(err));

                    console.log("SignalR connection established.");
                } catch (err) {
                    console.error("SignalR Connection failed: ", err);
                    setTimeout(() => startConnection(conn), 5000); 
                }
            };

            const connectSubscriptions = (conn) => {
                 subscribeToEvents(conn);
                
                subscribeToNewChat(conn);
            };
            startConnection(connection);

            connection.onclose(async () => {
                console.log('Connection closed');

            });
          
            connection.onreconnecting(error => {
                setHubConnection(null);
                console.assert(connection.state === signalR.HubConnectionState.Reconnecting);
                console.log(`Connection lost due to error "${error}". Reconnecting.`);
            });

            connection.onreconnected(connectionId => {
                console.assert(connection.state === signalR.HubConnectionState.Connected);
                console.log(`Connection reestablished. Connected with connectionId "${connectionId}".`);
                console.log(`Connection state: ${connection.state === signalR.HubConnectionState.Connected ? 'Connected' : connection.state}`);
                connectSubscriptions(connection);
                setHubConnection(connection);
            });
        }
    }, [user, hubConnection, setHubConnection, setChatsState, chats, chatReady]);

    useEffect(() => {
        if (hubConnection && chats) {
            chats.forEach(chat => {
                hubConnection.invoke("JoinChat", chat.chatId)
                    .catch(err => console.log(`Could not join chat ${chat.chatId}:`, err));
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
