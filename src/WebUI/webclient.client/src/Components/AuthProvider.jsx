import React, { createContext, useContext, useState, useEffect } from 'react';
import PropTypes from 'prop-types'; 
import userManager from '../AuthFiles/authConfig';
import config from '../config.json';

const AuthContext = createContext();

export function useAuth() {
    return useContext(AuthContext);
}

export const AuthProvider = ({ children }) => {
    const [user, setUser] = useState(null);
    const [userData, setUserData] = useState(null);
    const [loading, setLoading] = useState(true); 
    const [isAuthorized, setIsAuthorized] = React.useState(false);

    const setLoadingState = (isLoading) => {
        setLoading(isLoading);
    };

    const setIsAuthorizedState = (isAuth) => {
        setIsAuthorized(isAuth);
    };

    const setUserState = (newUser) => {
        setUser(newUser);
    };

    const setUserDataState = (newUserData) => {
        setUserData(newUserData);
    };

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

            return await response.json();
        } catch (error) {
            console.error('Error while sending the request to the UserService', error);
            return null;
        }
    }

    useEffect(() => {
        async function checkAuth() {
            setLoading(true);
            const user = await userManager.getUser();

            if (user) {
                setUser(user);
                fetchUserData(user.access_token).then(userData => {
                    if (userData) {
                        setIsAuthorized(true);
                        setUserData(userData);
                    } else {
                        setIsAuthorized(false);
                    }
                    setLoading(false);
                });
            } else {
                setIsAuthorized(false);
                setLoading(false);
            }
        }

        checkAuth();
    }, []);


    const value = {
        user,
        userData,
        loading,
        isAuthorized,
        setLoadingState,
        setIsAuthorizedState,
        setUserState,
        setUserDataState
    };

    return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};

AuthProvider.propTypes = {
    children: PropTypes.node.isRequired 
};
