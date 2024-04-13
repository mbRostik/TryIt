
import userManager from './AuthFiles/authConfig';
import React, { useState, useEffect } from 'react';
import { isAuthenticated } from './Functions/CheckAuthorization';
import { ThreeDots } from 'react-loader-spinner';
import config from './config.json'; 
import { useAuth } from './Components/AuthProvider';
import { Link, useNavigate } from 'react-router-dom';
import './Components/Styles/MainPage.css'

function App() {
    const [posts, setPosts] = useState(null);
    const navigate = useNavigate();

    const { user, userData, loading, isAuthorized, setLoadingState,
        setIsAuthorizedState,
        setUserState,
        setUserDataState } = useAuth();
    async function fetchPostsData(accessToken) {
        try {
            const response_posts = await fetch(`${config.apiBaseUrl}/GetFollowedPosts`, {
                method: 'GET',
                headers: {
                    'Authorization': `Bearer ${accessToken}`,
                    'Content-Type': 'application/json'
                }
            });
            if (response_posts.ok) {
                let response = await response_posts.json();

                if (response && Array.isArray(response)) {
                    setPosts(response);
                    console.log("Fetching posts");
                } else {
                    console.log("Response is not an array or is null");
                }
            }
        } catch (error) {
            console.log('There is no posts');
        }
    }
    function formatDate(isoString) {
        const date = new Date(isoString);
        return date.toLocaleString('en-US', {
            year: 'numeric',
            month: 'long',
            day: 'numeric',
            hour: '2-digit',
            minute: '2-digit'
        });
    }
    useEffect(() => {
        setLoadingState(true);
        if (user) { 
            const asyncFetchingPosts = async () => {
                await fetchPostsData(user.access_token);
            }
            asyncFetchingPosts(); }
       
        setLoadingState(false);

    }, [user, userData]);
    function isImage(fileName) {
        return /\.(jpg|jpeg|png|gif)$/i.test(fileName);
    }

    function FilePreview({ file }) {
        const isFileImage = isImage(file.name);

        if (isFileImage) {
            const fileSrc = `data:image/jpeg;base64,${file.file}`;
            return (
                <div className="Followedimage-container">
                    <img src={fileSrc} alt={file.name} className="FollowedPostPhoto" />
                </div>
            );
        } else {
            const fileHref = `data:application/octet-stream;base64,${file.file}`;
            return (
                <div className="FollowedPost_Files">
                    📄 {file.name}
                    <a href={fileHref} download={file.name} className="download-button">Download</a>
                </div>
            );
        }
    }

    const handleImageClick = (contactId) => {
        navigate(`/Someones_Profile/${contactId}`);
    };
    return (
        <div>
            {loading ? (
                <div className={`overlay ${loading ? 'visible' : ''}`}>
                    <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh' }} />
                </div>
            ) : !isAuthorized ? (
                <div>UnAuthorized</div>
            ) : (
                <div>
                    <div>
                        <h1>What is new</h1>
                    </div>
                    <div className="Followed_Posts">
                        {posts && posts.length > 0 ? (
                                    posts.map((post, index) => (
                                console.log(posts),
                                <div key={index} className="Followed_Post_Container">
                                    <div className="Followed_PostUserInformation">
                                        <img className="Followed_User_Photo"
                                            src={post.photo ? `data:image/jpeg;base64,${post.photo}` : "NoPhoto.jpg"}
                                            alt="Contact"
                                            onClick={(e) => { e.stopPropagation(); handleImageClick(post.userId); }}
                                        />
                                        <div className="Followed_User_Info">
                                            <div>{post.nickName}</div>
                                        </div>
                                    </div>
                                    <div className="FollowedPost">
                                        <div className="Followed_Post_Title_Info">
                                            <div className="Followed_Post_Title">{post.title}</div>
                                            <div className="Followed_Post_Title_Date">{formatDate(post.date)}</div>
                                        </div>
                                        <div className="Followed_Post_Description">{post.content}</div>
                                        {post.postFiles && post.postFiles.map((file, fileIndex) => (
                                            <FilePreview key={fileIndex} file={file} />
                                        ))}
                                    </div>
                                </div>
                            ))
                        ) : (
                            <div>No posts to display.</div>
                        )}
                    </div>
                </div>
            )}
        </div>
    );


}



export default App;
