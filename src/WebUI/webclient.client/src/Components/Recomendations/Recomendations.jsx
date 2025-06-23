import React, { useState, useEffect, useRef, useCallback } from 'react';
import { useAuth } from '../AuthProvider';
import config from '../../config.json';
import { useNavigate } from 'react-router-dom';
import '../Styles/MainPage.css';

function RecommendedPosts() {
    const [posts, setPosts] = useState([]);
    const [skip, setSkip] = useState(0);
    const [loading, setLoading] = useState(false);
    const [hasMore, setHasMore] = useState(true);
    const observer = useRef();
    const navigate = useNavigate();

    const { user, isAuthorized } = useAuth();

    const fetchRecommendedPosts = useCallback(async () => {
        if (!user || posts.length >= 50) return;

        const payload = {
            skip: skip,
            limit: 10,
            excludeIds: posts.map(p => p.id),
        };

        setLoading(true);
        try {
            const response = await fetch(`${config.apiBaseUrl}/getrecommendedposts`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    Authorization: `Bearer ${user.access_token}`,
                },
                body: JSON.stringify(payload),
            });

            if (response.ok) {
                const data = await response.json();
                if (data.length < 10 || posts.length + data.length >= 50) {
                    setHasMore(false);
                }
                setPosts(prev => [...prev, ...data]);
                setSkip(prev => prev + 10);
            } else {
                console.error("Failed to fetch recommended posts.");
            }
        } catch (error) {
            console.error("Error:", error);
        } finally {
            setLoading(false);
        }
    }, [user, skip, posts]);

    const lastPostRef = useCallback(node => {
        if (loading) return;
        if (observer.current) observer.current.disconnect();
        observer.current = new IntersectionObserver(entries => {
            if (entries[0].isIntersecting && hasMore) {
                fetchRecommendedPosts();
            }
        });
        if (node) observer.current.observe(node);
    }, [loading, hasMore, fetchRecommendedPosts]);

    useEffect(() => {
        if (user && posts.length === 0) {
            fetchRecommendedPosts();
        }
    }, [user, fetchRecommendedPosts]);

    const formatDate = (iso) => new Date(iso).toLocaleString('en-US', {
        year: 'numeric', month: 'long', day: 'numeric', hour: '2-digit', minute: '2-digit'
    });

    const isImage = (name) => /\.(jpg|jpeg|png|gif)$/i.test(name);

    const FilePreview = ({ file }) => {
        return isImage(file.name) ? (
            <div className="Followedimage-container">
                <img src={`data:image/jpeg;base64,${file.file}`} alt={file.name} className="FollowedPostPhoto" />
            </div>
        ) : (
            <div className="FollowedPost_Files">
                📄 {file.name}
                <a href={`data:application/octet-stream;base64,${file.file}`} download={file.name} className="download-button">Download</a>
            </div>
        );
    };

    const handleImageClick = (userId) => navigate(`/Someones_Profile/${userId}`);

    const handleReaction = async (postId, isLike) => {
        if (!user) return;

        try {
            await fetch(`${config.apiBaseUrl}/react`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    Authorization: `Bearer ${user.access_token}`,
                },
                body: JSON.stringify({ postId, isLike }),
            });

            setPosts(prevPosts =>
                prevPosts.map(p => {
                    if (p.id !== postId) return p;

                    if (isLike === null) {
                        return { ...p, postReaction: null };
                    }

                    const newReaction = isLike ? 'like' : 'dislike';
                    return {
                        ...p,
                        postReaction: p.postReaction === newReaction ? null : newReaction
                    };
                })
            );
        } catch (error) {
            console.error('Error sending reaction:', error);
        }
    };

    return (
        <div>
            <h1>Recommended Posts</h1>
            <div className="Followed_Posts">
                {posts.map((post, i) => (
                    <div ref={i === posts.length - 1 ? lastPostRef : null} key={i} className="Followed_Post_Container">
                        <div className="Followed_PostUserInformation">
                            <img
                                className="Followed_User_Photo"
                                src={post.photo ? `data:image/jpeg;base64,${post.photo}` : "NoPhoto.jpg"}
                                alt="User"
                                onClick={() => handleImageClick(post.userId)}
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

                            {post.files?.map((file, index) => <FilePreview key={index} file={file} />)}

                            {post.tags?.length > 0 && (
                                <div className="Followed_Post_Tags">
                                    {post.tags.map((tag, idx) => <span key={idx} className="tag">#{tag}</span>)}
                                </div>
                            )}

                            <div className="Followed_Post_Actions">
                                <button
                                    onClick={() => handleReaction(post.id, true)}
                                    className={`like-button ${post.postReaction === true ? 'active' : ''}`}
                                >
                                    👍
                                </button>
                                <button
                                    onClick={() => handleReaction(post.id, false)}
                                    className={`dislike-button ${post.postReaction === false ? 'active' : ''}`}
                                >
                                    👎
                                </button>
                            </div>
                        </div>
                    </div>
                ))}
                {loading && <p>Loading more...</p>}
                {!hasMore && <p>No more posts.</p>}
            </div>
        </div>
    );
}

export default RecommendedPosts;
