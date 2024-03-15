import React from 'react';
import { Link, useNavigate } from 'react-router-dom';
import userManager from '../../AuthFiles/authConfig';
import { isAuthenticated } from '../../Functions/CheckAuthorization';
import '../Styles/NavBarStyles.css'
import { NavLink } from 'react-router-dom';

const NavBar = () => {
    const navigate = useNavigate();
    const [isAuthorized, setIsAuthorized] = React.useState(null);

    React.useEffect(() => {
        const checkAuth = async () => {
            const authStatus = await isAuthenticated();
            setIsAuthorized(authStatus);
        };

        checkAuth();
    }, []);

    const onLogin = () => {
        userManager.signinRedirect();
    };

    const onLogout = async () => {
        await userManager.signoutRedirect();
        navigate('/');
    };

    return (
        <div>
            <div className="NavBarMain">
                <div className="NavBarMenu">
                    <NavLink to="/" className="NavBarButton">Home</NavLink>
                    {isAuthorized === false ? (
                        <button onClick={onLogin} className="NavBarButton">Login</button>

                    ) : (
                        <div>
                            <NavLink to="/Profile" className="NavBarButton">Profile</NavLink>
                            <button onClick={onLogout} className="NavBarButton">Logout</button>
                        </div>
                    )}
                </div>
            </div>
        </div>
    );
};

export default NavBar;
