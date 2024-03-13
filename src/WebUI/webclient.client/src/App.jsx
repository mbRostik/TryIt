
import userManager from './AuthFiles/authConfig';
import React, { useState, useEffect } from 'react';
import { isAuthenticated } from './Functions/CheckAuthorization';
import { ThreeDots } from 'react-loader-spinner';

function App() {
    const [authorized, setAuth] = useState(null);
    const onLogin = () => {
        userManager.signinRedirect();
    };

    const onLogout = () => {
        userManager.signoutRedirect();
    };

    
    const [loading, setLoading] = useState(true);
    useEffect(() => {
        const checkAuthentication = async () => {
            try {
                const auth = await isAuthenticated();
                console.log(await isAuthenticated());
                if (!auth) {
                    console.log(auth);
                    setAuth(auth);
                    setLoading(false);
                } else {
                    console.log(auth);
                    setAuth(auth);
                    setLoading(false);
                }
            } catch (err) {
                console.log(err);
            }
        };

        checkAuthentication();
    }, []);
    let contents = loading ? <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh' }} ><ThreeDots color="#00BFFF" height={80} width={80} /></div>
        : authorized ? <button onClick={onLogout}>Logout</button> : <button onClick={onLogin}>Login</button>;
    return (
        <div>
            Hello
        </div>
    );
}


export default App;
