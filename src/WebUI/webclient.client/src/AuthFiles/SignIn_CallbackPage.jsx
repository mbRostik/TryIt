import React, { useEffect } from 'react';
import { useNavigate } from 'react-router-dom'; 
import userManager from './authConfig';

const SignIn_CallbackPage = () => {
    const navigate = useNavigate();
    useEffect(() => {
        userManager.signinRedirectCallback().then(() => {

            window.location.href = '/';
        }).catch(error => {
            console.log();
        });
    }, []);

    return <div>Loading...</div>;
};

export default SignIn_CallbackPage;
