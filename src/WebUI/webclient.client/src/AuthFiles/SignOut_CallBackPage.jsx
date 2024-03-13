import React, { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import userManager from './authConfig';

const SignoutCallbackPage = () => {
    const navigate = useNavigate();

    useEffect(() => {
        userManager.signoutRedirectCallback().then(() => {
            navigate('/'); 
        }).catch(error => {
            console.error(error);
            navigate('/'); 
        });
    }, [navigate]);

    return <div>Loading...</div>;
};

export default SignoutCallbackPage;
