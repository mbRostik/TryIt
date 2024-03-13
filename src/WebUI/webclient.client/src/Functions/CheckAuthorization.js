import userManager from '../AuthFiles/authConfig';

export const isAuthenticated = async () => {
    try {
        const user = await userManager.getUser();
        return !!user && !user.expired;
    } catch (error) {
        console.error('There is a problem', error);
        return false;
    }
};
