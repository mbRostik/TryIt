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
        const selectedFiles = Array.from(event.target.files); 
        if (files.length + selectedFiles.length > 8) { 
            
            toast.error("You can only upload up to 8 files.", {
                position: "top-right",
                autoClose: 5000,
                hideProgressBar: false,
                closeOnClick: true,
                pauseOnHover: true,
                draggable: true,
                progress: undefined,
            });
            return; 
        }
        
        setFiles(prevFiles => [...prevFiles, ...selectedFiles]);
        const newFileNames = selectedFiles.map(file => file.name);
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

                    errorData.forEach(errorItem => {
                        toast.error(errorItem.error, {
                            position: "top-right",
                            autoClose: 5000,
                            hideProgressBar: false,
                            closeOnClick: true,
                            pauseOnHover: true,
                            draggable: true,
                            progress: undefined,
                        });
                    });
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
            console.log(err.message);

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

    const handleRemoveFile = (indexToRemove) => {
        setFiles(prevFiles => prevFiles.filter((_, index) => index !== indexToRemove));
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
                                                <input
                                                    id="files"
                                                    type="file"
                                                    onChange={handleFileChange}
                                                    className="custom-file-input"
                                                    multiple
                                                    style={{ display: 'none' }}
                                                />
                                                <label htmlFor="files" className="upload-button">
                                                    Upload Files
                                                </label>
                                                <div className="selected-files">
                                                    {files.map((file, index) => (
                                                        <div key={index} className="file-preview">
                                                            {file.name.length > 10 ? `${file.name.slice(0, 10)}...` : file.name}
                                                            <button onClick={() => handleRemoveFile(index)} className="remove-file-button">✖</button>
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
export default MyPosts;
