# Deprecated! Use https://github.com/Weavy/weavy-webview in your Xamarin app to integrate with Weavy!

# Weavy Messenger Mobile
Weavy Messenger Mobile is a Xamarin Forms hybrid app targeting both the Android and iOS platforms. This repository was created as a boiler plate to allow developers of the Weavy platform to easily get up and running with mobile development.

## What can it be used for?
Build apps for Android and iOS that users of the Weavy platform can use to communicate through the Weavy Messenger and get notified by push notifications.

## How to use it?
Fork the repository and customize it to your needs. Follow the guide below how to customize the various parts. 

## What are the components?

| Project | Description |
|---------|-------------|
|**Messenger**|a .NET Standard shared code library|
|**Messenger.Android**|an Android implementation of the app|
|**Messenger.iOS**|an iOS implementation of the app |


## A short description of the app...
The app basically consists of only two **Views**. The **Select site** page  where the user enters the url to the Weavy site and the **Main** page which contains the hybrid webview displaying the web site.

All communication between the web page and the native app is handled through a javascript bridge executing javascript on the web page and receiving messages from the web page.

Push notifications are sent to the app from the Weavy web application through  Azure Notification Hubs. Please take a look at the **Push Notifications** section below for more info on how to set up this.


## Configuring the app

The first thing you have to do is to configure the app with the correct values required to run and publish the app. The sections below explain in detail what you need to do in each of the projects.

### Messenger project
In the Messenger project, all the values that you need to change are located in the Helpers\Constants.cs file.

| Property   |      Example value      |  Description |
|----------|-------------|------|
| DisplayName |  Weavy Messenger | A title used in various info texts and alerts in the app |
|DisplayShortName |    Messenger   |   A shorter version of the above |
| AzureSuffix | mycloud.com |    This suffix is added on the Select site page when a user enters a site name. Should match what you have set up in the Weavy Console |
| DefaultColor | #156B93 |    The default color to use on startup before the theme colors has been fetched from the selected Weavy site |

*Please note that the example values above are just that, examples.*


### Messenger.Android
In the Android project, you have to set properties such as package name and notification related settings. You also have to change all the application related icons located in the `Resources\*` folders

In the **Properties pane** of the project, set a Package name to use. This is a unique name that identifies the app.

In the `Notifications\Constants.cs` file, please specify all the settings that are related to Notifications. The values for the Azure Notification Hub is explained in detail in the **Push Notifications** section below. Other values  in Constants.cs are:

| Property   |      Example value      |  Description |
|----------|-------------|------|
| NOTIFICATION_CHANNEL_ID |  com.company.droid. GENERAL_NOTIFICATIONS | A unique value identifying an Android notification channel. You can use the package name specified earlier and add for example GENERAL_NOTIFICATIONS |
|NOTIFICATION_GROUP_ID |    com.company.droid. NOTIFICATIONS   |   A unique value identifying an Android notification group where incoming notifications are grouped. You can use the package name specified earlier and add for example NOTIFICATIONS |

*Please note that the example values above are just that, examples.*


### Messenger.iOS
In the iOS project, you have to set properties such as Bundle identifier and notification related settings. You also have to change all the application related icons located in `Assets Catalogs\Assets`

In the **Properties pane** of the project, open up the iOS Manifest editor and set a bundle identifier to use. This is a unique name that identifies the app and should be in [reverse domain name notation](https://en.wikipedia.org/wiki/Reverse_domain_name_notation).

In the `Notifications\Constants.cs` file, please specify all the settings that are related to Notifications. The values for the Azure Notification Hub is explained in detail in the **Push Notifications** section below.

Before you can build/deploy for iOS, you must have an Apple Developer account and the environment setup on an Mac. For more information, please read through this article, https://docs.microsoft.com/en-us/xamarin/ios/get-started/installation/


## Push notifications
Whenever a user gets notified in the Weavy Messenger web app, a notification is also sent to the user's mobile device(s). The device registrations and push notification delivery is all taken care of with the help of [Azure Notification Hubs](https://azure.microsoft.com/en-us/services/notification-hubs/).

In order to make all this to work, both the Weavy web app and the mobile projects must be set up with the correct settings. The process to setup a Notification Hub in Azure is described in the links below:

[Setup for Android](https://docs.microsoft.com/en-us/azure/notification-hubs/xamarin-notification-hubs-push-notifications-android-gcm)

[Setup for iOS](https://docs.microsoft.com/en-us/azure/notification-hubs/xamarin-notification-hubs-ios-push-notification-apns-get-started)

**NOTE! You might already have setup the Notification Hubs in Azure when you configured the Weavy Console. In that case, you only have to get the Access policy connection string already available for the hub**

Please make sure to read through the instructions on the links above to fully understand the process. When you are done, you should have a Notification Hub setup in Azure with both Android (GCM/Firebase) and Apple (iOS) credentials set and an Access Policy for the **DefaultListenSharedAccessSignature**. Take the access policy connection string and update the `Notification\Constants.cs` values in each of the platform projects. 

**Note!** The access policy connection string is handled differently in the Android and the iOS project. In the Android project, the whole connection string is specified as the `ConnectionString` property, but in the iOS project, the connection string is split into a connection string and a shared key. More information and example is in the code comments.

### The google-services.json file
When you created a new project in [Firebase](https://firebase.google.com), you should have a google-services.json file that you downloaded. Add (or replace the existing one) in the Android project. Make sure the Build Action is set to `GoogleServicesJson`. The json file contains all the necessary settings and is automatically merged into the manifest when building the project.

### Update the Android manifest.xml file
Update the `Properties\AndroidManifest.xml` file located in the Android project. The `<category android:name="com.comapnyname.app" />` should match the the Android package name that you have specified.
