
import userManager from './AuthFiles/authConfig';
import React, { useState, useEffect } from 'react';
import { isAuthenticated } from './Functions/CheckAuthorization';
import { ThreeDots } from 'react-loader-spinner';
import config from './config.json'; 

function App() {

    const [authorized, setAuth] = useState(false);
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
                setAuth(auth);
            } catch (err) {
                console.error(err); 
            } finally {
                setLoading(false); 
            }
        };

        checkAuthentication();
    }, []);


    async function Tempfunc() {
        try {

            const accessToken = await userManager.getUser().then(user => user.access_token);
            await fetch(`${config.apiBaseUrl}/Temp`, {
                method: 'Get',
                headers: {
                    'Authorization': `Bearer ${accessToken}`,
                    'Content-Type': 'application/json'
                }
            });

        }
        catch (err) {
            console.log("dfk");
            };
    }


    let contents = loading ? <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh' }} ><ThreeDots color="#00BFFF" height={80} width={80} /></div>
        : authorized ? <button onClick={onLogout}>Logout</button> : <button onClick={onLogin}>Login</button>;
    return (
        <div>
            <h1>MainPage</h1>

            <button onClick={Tempfunc}>Temp</button>
        </div>
    );
}


export default App;
