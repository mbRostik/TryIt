import { UserManager } from 'oidc-client';

const config = {
    authority: 'https://localhost:7174',
    client_id: 'interactive',
    client_secret: 'OnlyUserKnowsThisSecret',
    redirect_uri: 'https://localhost:5173/signin-oidc',
    response_type: 'code',
    scope: 'openid profile Chats.WebApi.read Users.WebApi.read Users.WebApi.write Chats.WebApi.read Chats.WebApi.write Subscriptions.WebApi.read Subscriptions.WebApi.write Notifications.WebApi.read Notifications.WebApi.write Reports.WebApi.read Reports.WebApi.write Posts.WebApi.read Posts.WebApi.write',
    post_logout_redirect_uri: 'https://localhost:5173/signout-callback-oidc',
};

const userManager = new UserManager(config);

export default userManager;