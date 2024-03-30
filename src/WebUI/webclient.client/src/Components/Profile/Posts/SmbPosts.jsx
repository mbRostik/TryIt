import { useState, useEffect } from 'react';
import userManager from '../../../AuthFiles/authConfig';
import { isAuthenticated } from '../../../Functions/CheckAuthorization';
import { NavLink } from 'react-router-dom';
import { ThreeDots } from 'react-loader-spinner';
import { Link, useNavigate } from 'react-router-dom';
import '../../Styles/MyPosts.css'
import axios from '../../../../node_modules/axios/index';
import { ToastContainer, toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import config from '../../../config.json';
import { useAuth } from './../../AuthProvider';
import ReactCrop from 'react-image-crop';
import 'react-image-crop/dist/ReactCrop.css';

const SmbPosts = ({ ProfileId }) => {
    const navigate = useNavigate();
    const [posts, setPosts] = useState(null);

    const [title, setTitle] = useState('');
    const [content, setContent] = useState('');
    const [files, setFiles] = useState([]);
    const [fileNames, setFileNames] = useState([]);

    const [isModalOpen, setIsModalOpen] = useState(false);


    const [isHovered, setIsHovered] = useState(false);
    const { user, userData, loading, isAuthorized, setLoadingState,
        setIsAuthorizedState,
        setUserState,
        setUserDataState } = useAuth();

 

    async function fetchsmbPostsData(accessToken) {
        try {
            const response_posts = await fetch(`${config.apiBaseUrl}/GetsmbPosts`, {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${accessToken}`,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ ProfileId })

            });
            let response = await response_posts.json();
            if (response_posts.ok) {
                setPosts(response);
                console.log("Fetching posts");
            }
        } catch (error) {
            console.log('There is no posts', error);
        }
    }

    useEffect(() => {
        setLoadingState(true);
        const asyncFetchingPosts = async () => {
            await fetchsmbPostsData(user.access_token);
        }
        asyncFetchingPosts();
        setLoadingState(false);

    }, [user]);



    function isImage(fileName) {
        return /\.(jpg|jpeg|png|gif)$/i.test(fileName);
    }

    function FilePreview({ file }) {
        const isFileImage = isImage(file.name);

        if (isFileImage) {
            const fileSrc = `data:image/jpeg;base64,${file.file}`;
            return (
                <div className="image-container">
                    <img src={fileSrc} alt={file.name} className="PostPhoto" />
                </div>
            );
        } else {
            const fileHref = `data:application/octet-stream;base64,${file.file}`;
            return (
                <div className="Post_Files">
                    📄 {file.name}
                    <a href={fileHref} download={file.name} className="download-button">Download</a>
                </div>
            );
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

    return (
        <div>
            {loading ? (
                <div className={`overlay ${loading ? 'visible' : ''}`}>
                    <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh' }}>
                    </div>
                </div>
            ) : isAuthorized === false ? (
                <div>UnAuthorized</div>
            ) : (
                <div>
                    <div className="smbposts">
                        {posts && posts.length > 0 ? (
                            posts.map((post, index) => (
                                <div key={index} className="post">
                                    <div className="Post_Title_Info">
                                        <div className="Post_Title">{post.title}</div>
                                        <div className="Post_Title_Date">{formatDate(post.date)}</div>
                                    </div>
                                    <div className="Post_Description">{post.content}</div>
                                    {post.files && post.files.map((file, fileIndex) => (
                                        <FilePreview key={fileIndex} file={file} />
                                    ))}
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
};
export default SmbPosts;
