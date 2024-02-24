# TryIt

TryIt is an innovative social network that combines the best features of popular platforms. Our app is designed to provide users with a unique and convenient space for communication. In TryIt, you can share the latest news, photos, and personal thoughts, as well as engage in real-time conversations while maintaining privacy and security. This network is created to ensure freedom of expression for users and the ability to stay connected with friends, family, and colleagues, regardless of distance. TryIt is more than just a platform; it's a community where everyone can find something for themselves. Thanks to its intuitive interface and modern features, your experience in the world of social networking will be unforgettable.



# Tech

In the TryIt project, we employ a modern and efficient technology stack to provide the best experience for our users. Our web application is built on ASP.NET using the C# programming language and Entity Framework for database operations. For secure authorization and authentication, we use IdentityServer. Inter-service communication is facilitated through RabbitMQ, MassTransit, and gRPC, while Redis is used for data caching. System resilience to errors is enhanced by Polly retry. Ocelot API Gateway is utilized for API management, and SignalR enables real-time "live" communication. Additionally, for logging everything, we are using Elasticsearch in conjunction with Kibana, providing powerful search capabilities and insightful data visualization. Finally, for the development of the client-side, we use JavaScript, React, CSS/HTML, and AJAX to create an intuitive and visually appealing interface.



# Arch (temporary)

  6 services + IdentityServer

  1. Creation, modification, deletion of a user - messages are sent from IdentityServer to RabbitMQ where all services store the necessary information in their user tables (mostly this is just the Id).

  2. Post Service - a service for managing user publications (After creation, deletion, modification - through MessageBus some services will collect information), contains such tables as Post (the publication itself), User (Id), Comments (user comments), PostReaction (user reactions), Bans (banned posts), Files (table of files that were added to posts).

  3. User Service - a service for managing user information, contains such tables as BannedUser (banned users), Follow (who the users are following), Post (Id of all publications), SavedPost (saved posts of other users), Sex (gender), SexTypes, User (Description of the user, avatar, name, nickname, email, phone, date of birth).

  4. Chat Service - a service for communication between users, contains such tables as User (Id), Chat (Stores 2 users, Id of the last message, and last activity), Message (user messages), File (files sent in messages), MessageWithFile (for linking files and messages).

  5. Notification Service - a service for managing notifications, contains such tables as User (Id), Type (Types of notifications), Notification.

  6. Subscription Service - a service for managing subscriptions to the user service, contains such tables as User (Id), Subscriptions, Transactions.

  7. Report Service - a service for managing complaints, contains such tables as User (Id), Post (Id), ReportedPost, ReportedUser.

# use-cases

1. User Wants to Sign Up in the System

  The user accesses the "TryIt" registration page.
  They fill in required details such as email, username, and password.
  Optional information may include interests, location, and a profile photo.
  After agreeing to the terms and conditions, the user clicks on the "Sign Up" button.
  The system sends a verification email; the user must click on a link to activate the account.


2. User Wants to Sign In to the System

  The user opens the "TryIt" login page.
  They enter their username and password.
  If the credentials are correct, the user is granted access to their account.
  If the user forgets their password, there's an option to reset it via email.


3. User Wants to Publish a Post

  The user navigates to the "Create Post" section.
  They can type text, add images or videos, and tag friends or locations.
  Before posting, the user can select privacy settings (public, friends, private).
  The user clicks the "Post" button, and the content is shared on their timeline and visible to others based on the selected privacy settings.


4. User Wants to Alter the Profile

  The user accesses their profile page.
  They click on the "Edit Profile" button.
  Here, they can change their profile picture, cover photo, bio, personal information, and other customizable settings.
  After making changes, the user saves them, updating the profile immediately.


5. User Wants to Subscribe to Somebody

  The user searches for a person or selects them from their friend suggestions.
  They visit the person's profile.
  The user clicks on the "Subscribe" or "Follow" button.
  They will now receive updates from this person in their news feed.


6. User Wants to Set a Like, Dislike, or Comment on a Post

  The user finds a post in their feed or on someone's profile.
  To like or dislike, they click the respective button under the post.
  To comment, they click the comment field, type their message, and press enter or the "Comment" button.
  These interactions are recorded and displayed on the post.


7. User Wants to Alter a Post

  The user goes to the post they previously published.
  They select the "Edit" option (usually a small pencil icon) near the post.
  The user makes the desired changes to the text, images, or videos.
  After editing, they click "Save" to update the post.


8. Moderator Wants to Delete Someone's Post

  The moderator reviews a post reported by users or flagged by the system.
  They assess if the post violates the platform's community guidelines.
  If it does, they select the option to delete the post.
  The post is removed, and a notification can be sent to the original poster regarding the violation.


9. Moderator Wants to Ban Someone

  The moderator reviews the reported account's activity.
  If the account consistently violates community guidelines, the moderator accesses the account settings.
  They select the "Ban User" option, specifying the duration and reason.
  The user is notified of the ban and cannot access their account for the duration of the ban.


10. User Wants to Send a Message to Someone

  The user navigates to the messaging feature of "TryIt."
  They select the "New Message" button and search for the recipient by name.
  The user types their message in the chat window and sends it.
  The message is delivered to the recipient's "TryIt" inbox, where they can read and reply.


# Features:

  1. For all:
    1. Like and dislike posts.
    2. Leave comments under posts.
    3. Send reports on posts or users.
    4. Follow or unfollow users.
    5. Change and delete profiles (including avatar, name, email, password, description).
    6. Edit published posts (change or delete embedded media files, edit text).
    7. Delete published posts.
    8. Chat with other users.
    9. Change profile privacy settings.
    10. Save others' publications.
    11. View notifications.
    12. Purchase premium for uploading photos and videos in higher quality.

  2. For Moderators:
    1. Ban users.
    2. Delete others' posts.
    3. View complaints.

    
