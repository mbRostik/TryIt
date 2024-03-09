import { useEffect, useState } from 'react';
import { UserManager, WebStorageStateStore } from 'oidc-client';

function App() {
    useEffect(() => {
        const userManager = new UserManager({
            authority: 'https://localhost:7174',
            client_id: 'interactive',
            redirect_uri: 'https://localhost:5173/signin-oidc', 
            response_type: 'code',
            scope: 'openid profile', 
            post_logout_redirect_uri: 'https://localhost:5173/signout-callback-oidc',
            userStore: new WebStorageStateStore({ store: window.localStorage }),
        });

        userManager.getUser().then(user => {
            if (!user || user.expired) {
                userManager.signinRedirect();
            } else {}
        });
    }, []);

    return (
        <div>
            <h1 id="tabelLabel">Weather forecast</h1>
            <p>This component demonstrates fetching data from the server.</p>
        </div>
    );
}

export default App;
