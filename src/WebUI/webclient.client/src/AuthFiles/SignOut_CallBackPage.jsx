import React, { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import userManager from './authConfig';

const SignoutCallbackPage = () => {
    const navigate = useNavigate();

    useEffect(() => {
        userManager.signoutRedirectCallback().then(() => {
            
            window.location.href = '/';
        }).catch(error => {
            console.error(error);

            window.location.href = '/';
        });
    }, []);

    return <div>Loading...</div>;
};

export default SignoutCallbackPage;
