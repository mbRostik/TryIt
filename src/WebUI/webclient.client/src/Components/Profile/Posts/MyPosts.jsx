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

const MyPosts = () => {
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

    const handleFileChange = (event) => {
        const newFiles = Array.from(event.target.files);
        const newFileNames = newFiles.map(file => file.name);

        setFiles(prevFiles => [...prevFiles, ...newFiles]);
        setFileNames(prevNames => [...prevNames, ...newFileNames]);
    };

    async function fetchPostsData(accessToken) {
        try {
            const response_posts = await fetch(`${config.apiBaseUrl}/GetUserPosts`, {
                method: 'GET',
                headers: {
                    'Authorization': `Bearer ${accessToken}`,
                    'Content-Type': 'application/json'
                }
            });
            let response = await response_posts.json();

            if (response_posts.ok && response.$values) {
                console.log("Response from server:", response);
                response = processFiles(response.$values);
                setPosts(response);
                console.log("Fetching posts");
            }
        } catch (error) {
            console.log('There is no posts', error);
        }
    }
    function processFiles(posts) {
        const objectStore = {};

        posts.forEach(post => {
            if (post.files && post.files.$values) {
                post.files.$values.forEach(file => {
                    if (file.$id) {
                        objectStore[file.$id] = file;
                    }
                });
            }
        });

        posts.forEach(post => {
            if (post.files && post.files.$values) {
                post.files.$values = post.files.$values.map(file => {
                    if (file.$ref) {
                        return objectStore[file.$ref];
                    }
                    return file; 
                });
            }
        });

        return posts; 
    }

    useEffect(() => {
        setLoadingState(true);
        const asyncFetchingChats = async () => {
            await fetchPostsData(user.access_token);
        }
        asyncFetchingChats();
        setLoadingState(false);

    }, [user]);

    const toggleModal = () => {
        setTitle('');
        setContent('');
        setFiles([]);
        setFileNames([]);
        setIsModalOpen(!isModalOpen);
    };


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
                toggleModal();

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

    const onLogout = async () => {
        await userManager.signoutRedirect();
        navigate('/');
    };

    function FilePreview({ file }) {
        console.log("KJGJGJWJFDGJWGFJGWKJF");
        console.log(file);
        try {
            const fileSrc = file.file.startsWith('data:image') ? file.file : `data:image/jpeg;base64,${file.file}`;

            if (/^data:image\/[a-zA-Z]+;base64,/.test(fileSrc)) {
                return (
                    <div>
                        <img src={fileSrc} alt={file.name} className="PostPhoto" />
                    </div>
                );
            } else {
                return (
                    <div className="Post_Files">
                        <p>📄 {file.name}</p>
                    </div>
                );
            }
        }

        catch (ex) {
            return(<div></div>)
        }
       
    }

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
                            <div className="Buttons">
                                <NavLink to="/Profile_Settings" className="button outline" >Settings</NavLink>
                                <button className="button solid" onClick={toggleModal}>Create Post</button>
                                <button onClick={onLogout} className="button outline">LogOut</button>
                            </div>
                           

                            <div className={isModalOpen ? "modal display-block" : "modal display-none"}>
                                <div className="modal-main">
                                    <div className="form-container">
                                        <form onSubmit={handleSubmit}>
                                            <button onClick={toggleModal} className="close-modal-button">✖</button>

                                            <h2>Post Creation</h2>

                                            <div>
                                                <label htmlFor="title">Title</label>
                                                <input
                                                    id="title"
                                                    type="text"
                                                    value={title}
                                                    onChange={e => setTitle(e.target.value)}
                                                    required
                                                />
                                            </div>
                                            <div>
                                                <label htmlFor="content">Description</label>
                                                <textarea
                                                    id="content"
                                                    value={content}
                                                    onChange={e => setContent(e.target.value)}
                                                    required
                                                    rows="10"  
                                                    cols="50"
                                                />
                                            </div>
                                            <div>
                                                <label htmlFor="files">Files</label>
                                                <input
                                                    id="files"
                                                    type="file"
                                                    onChange={handleFileChange}
                                                    className="custom-file-input"
                                                    multiple
                                                />
                                                <div className="selected-files">
                                                    {files.map((file, index) => (
                                                        <div key={index} className="file-preview">
                                                            {file.name}
                                                        </div>
                                                    ))}
                                                </div>
                                            </div>
                                            <button type="submit">Publish</button>
                                        </form>
                                    </div>

                                </div>
                            </div>
                            <div className="posts">
                                {posts && posts.length > 0 ? (
                                    console.log(posts),
                                    posts.map((post, index) => (
                                        <div key={index} className="post">
                                            <div className="Post_Title">{post.title}</div>
                                            <div className="Post_Description">{post.content}</div>
                                            {post.files && post.files.$values && post.files.$values.map((file, fileIndex) => (
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
export default MyPosts;
