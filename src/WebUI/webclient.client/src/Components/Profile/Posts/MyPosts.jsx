import { useState, useEffect } from 'react';
import userManager from '../../../AuthFiles/authConfig';
import { isAuthenticated } from '../../../Functions/CheckAuthorization';
import { NavLink } from 'react-router-dom';
import { ThreeDots } from 'react-loader-spinner';
import { Link, useNavigate } from 'react-router-dom';
import '../../Styles/Profile.css'
import axios from '../../../../node_modules/axios/index';
import { ToastContainer, toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import config from '../../../config.json'; 
import { useAuth } from './../../AuthProvider';
import ReactCrop from 'react-image-crop';
import 'react-image-crop/dist/ReactCrop.css';

const MyPosts = () => {
    const navigate = useNavigate();
    const [posts, setPosts] = useState(null);

    const [title, setTitle] = useState('');
    const [content, setContent] = useState('');
    const [files, setFiles] = useState([]);
    const [fileNames, setFileNames] = useState([]);


    const [isHovered, setIsHovered] = useState(false);
    const { user, userData, loading, isAuthorized, setLoadingState,
        setIsAuthorizedState,
        setUserState,
        setUserDataState } = useAuth();

    const handleFileChange = (event) => {
        const newFiles = Array.from(event.target.files);
        const newFileNames = newFiles.map(file => file.name);

        setFiles(prevFiles => [...prevFiles, ...newFiles]);
        setFileNames(prevNames => [...prevNames, ...newFileNames]);
    };

    function FilePreview({ file }) {
        const isImage = file.type.startsWith('image/');

        if (isImage) {
            const src = URL.createObjectURL(file);
            return <img src={src} alt={file.name} style={{ width: 100, height: 100 }} onLoad={() => URL.revokeObjectURL(src)} />;
        } else {
            return (
                <div>
                    <span>📄</span> {file.name}
                </div>
            );
        }
    }

    async function fetchPostsData(accessToken) {
        try {
            const response_posts = await fetch(`${config.apiBaseUrl}/GetUserPosts`, {
                method: 'GET',
                headers: {
                    'Authorization': `Bearer ${accessToken}`,
                    'Content-Type': 'application/json'
                }
            });
            const response = await response_posts.json();

            if (response_posts.ok && response.$values) {
                setPosts(response.$values);
                console.log("Fetching posts");
            }
        } catch (error) {
            console.log('There is no posts');
        }
    }


    useEffect(() => {
        setLoadingState(true);
        const asyncFetchingChats = async () => {
            await fetchPostsData(user.access_token);
        }
        asyncFetchingChats();
        setLoadingState(false);
        console.log(posts);

    }, [user]);


    const handleSubmit = async (event) => {
        event.preventDefault();

        const filesBase64 = await Promise.all(
            files.map(file => new Promise((resolve, reject) => {
                const reader = new FileReader();
                reader.readAsDataURL(file);
                reader.onload = () => resolve({
                    name: file.name,
                    content: reader.result.split(',')[1]
                });
                reader.onerror = error => reject(error);
            }))
        );

        const model = {
            Title: title,
            Content: content,
            files: filesBase64,

        };
        console.log(model);
        try {
            const accessToken = await userManager.getUser().then(user => user.access_token);
            const response = await fetch(`${config.apiBaseUrl}/CreatePost`, {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${accessToken}`,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(model)
            });

            if (!response.ok) {
                if (response.status === 400) {
                    const errorData = await response.json();
                    const errors = errorData.errors;

                    for (const key in errors) {
                        if (errors.hasOwnProperty(key)) {
                            const errorMessages = errors[key];
                            errorMessages.forEach(message => {
                                toast.error(`${key}: ${message}`, {
                                    position: "top-right",
                                    autoClose: 5000,
                                    hideProgressBar: false,
                                    closeOnClick: true,
                                    pauseOnHover: true,
                                    draggable: true,
                                    progress: undefined,
                                });
                            });
                        }
                    }
                } else {
                    toast.error(`HTTP error! Status: ${response.status}`, {
                        position: "top-right",
                        autoClose: 5000,
                        hideProgressBar: false,
                        closeOnClick: true,
                        pauseOnHover: true,
                        draggable: true,
                        progress: undefined,
                    });
                }
                throw new Error(`HTTP error! Status: ${response.status}`);
            }

            else {
                await fetchPostsData(user.access_token);

                toast.success('Post created.', {
                    position: "top-right",
                    autoClose: 5000,
                    hideProgressBar: false,
                    closeOnClick: true,
                    pauseOnHover: true,
                    draggable: true,
                    progress: undefined,
                });

                setTitle('');
                setContent('');
                setFiles([]);
                setFileNames([]);


            }

        } catch (err) {
            toast.error(`Error occurred: ${err.message}`, {
                position: "top-right",
                autoClose: 5000,
                hideProgressBar: false,
                closeOnClick: true,
                pauseOnHover: true,
                draggable: true,
                progress: undefined,
            });
            console.error('Error while sending the request', err);
        }


    };

    return (
        <div>
            {loading ? (
                <div className={`overlay ${loading ? 'visible' : ''}`}>
                    <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh' }}>
                        <ThreeDots color="#00BFFF" height={80} width={80} />
                    </div>
                </div>
            ) : isAuthorized === false ? (
                <div>UnAuthorized</div>
            ) : (
                <div>
                    <form onSubmit={handleSubmit}>
                        <div>
                            <label htmlFor="title">Title:</label>
                            <input
                                id="title"
                                type="text"
                                value={title}
                                onChange={e => setTitle(e.target.value)}
                                required
                            />
                        </div>
                        <div>
                            <label htmlFor="content">Content:</label>
                            <textarea
                                id="content"
                                value={content}
                                onChange={e => setContent(e.target.value)}
                                required
                            />
                        </div>
                        <div>
                            <label htmlFor="files">Files:</label>
                            <input
                                id="files"
                                type="file"
                                onChange={handleFileChange}
                                multiple
                            />
                            <div className="selected-files">
                                {files.map((file, index) => (
                                    <div key={index} className="file-preview">
                                        <FilePreview file={file} />
                                    </div>
                                ))}
                            </div>
                        </div>
                        <button type="submit">Create Post</button>
                    </form>
                            <div className="posts">
                                {posts && posts.length > 0 ? (
                                    posts.map((post, index) => (
                                        <div key={index}>
                                            <h2>{post.title}</h2>
                                            <p>{post.content}</p>
                                            {post.files && post.files.$values && post.files.$values.map((file, fileIndex) => (
                                                <div key={fileIndex} className="file-preview">
                                                    {typeof file.file === 'string' && file.file.startsWith("/9j/") ? (
                                                        <div>
                                                            <img src={`data:image/jpeg;base64,${file.file}`} alt={file.name} style={{ width: 100, height: 100 }} />
                                                            <p>{file.name}</p>
                                                        </div>
                                                    ) : (
                                                        <div>
                                                            <p>📄 {file.name}</p>
                                                        </div>
                                                    )}
                                                </div>
                                            ))}
                                        </div>
                                    ))
                                ) : posts !== null ? (
                                    <div>
                                        <div>There are no posts yet.</div>
                                    </div>
                                ) : null}
                            </div>
                </div>
            )}
        </div>
    );
};
export default MyPosts;
