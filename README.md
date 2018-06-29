# profile-manager

## A tool for managing profiles.

### Things you'll need
- A Face API - you can use the Azure one [here](https://azure.microsoft.com/en-us/services/cognitive-services/face/)
- A Storage account for storing user photos
- A Cosmos DB or other document database

### Extensibility
- You can write your own [Blob](https://github.com/jpda/profile-manager/blob/master/ProfileManager.AppService/IBlobProvider.cs), [Face](https://github.com/jpda/profile-manager/blob/master/ProfileManager.AppService/IFaceInfoProvider.cs) and [Data](https://github.com/jpda/profile-manager/blob/master/ProfileManager.AppService/IDocumentProvider.cs) providers.

### Authentication and Authorization
Disabled by default, but can be enabled via `Authorization:Enforced = true` - this uses Azure AD for authentication and authorization, along with an AppRole called 'Admin.'

#### Anonymous users can
- View the employee list

#### Authenticated users can
- View employee details

#### Authorized admins (assigned the Admin AAD app role) can
- Create new users
- Edit users
- Delete users

## WIP
- See the [issues](https://github.com/jpda/profile-manager/issues) list for stuff that's pending