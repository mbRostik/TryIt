
import userManager from './AuthFiles/authConfig';
import React, { useState, useEffect } from 'react';
import { isAuthenticated } from './Functions/CheckAuthorization';
import { ThreeDots } from 'react-loader-spinner';
import config from './config.json'; 

function App() {

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
            }
    }

    return (
        <div>
            <h1>MainPage</h1>
            <button onClick={Tempfunc}>Temp</button>
        </div>
    );
}


export default App;
