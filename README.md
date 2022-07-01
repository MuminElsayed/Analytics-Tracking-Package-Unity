# Analysis-Tracking-Package-Unity
A unity package to track game prototype stats for Android/IOS.

## Implementation Steps:

**1. GAME ANALYTICS:**
1. Create a Game Analytics account on: https://go.gameanalytics.com
2. Add your organization, studio, and game on their website
1. Download and import Game Analytics SDK as a custom package using this link: https://download.gameanalytics.com/unity/GA_SDK_UNITY.unitypackage
1. Wait and go through the external dependency manager resolver.
1. Go to "Window/Game Analytics/Select Settings"
1. Login with your Game Analytics account.

**2. TRACKING MANAGER:**
1. Import the "TrackingManager" custom package.
2. Add the "Tracking Manager" prefab to your hierarchy
3. Add your Game keys to the tracking manager and press on "Sync Settings", this will save and sync your settings across all SDK's (Game key and secret key can be found in Game settings on the Game Analytics website)

~~**3. FACEBOOK:**~~ (IGNORE, crashes on initialization)
1. Add the game to your developers account.
2. Download and import the "Facebook Unity SDK" to your Unity project
3. Add the "App Events" product to your game and follow it's integration steps.
4. Note: if "OpenSSL not found" error appears in Facebook SDK, follow these steps: https://answers.unity.com/questions/616484/open-ssl-not-found.html

## Next Steps:
* Setup remote configs & A/B testing from Game Analytics website (more info here: https://gameanalytics.com/docs/s/article/A-B-Testing-Setup)
* Add remote config keys to your game through the Tracking Manager

## Future Milestones:
Integrating Facebook Unity SDK & Firebase into the tracking manager tool.

## Notes:
If you are using Proguard for Android follow these steps (at the end of the page: https://gameanalytics.com/docs/s/article/Integration-Unity-SDK#install)
Facebook Unity SDK currently crashing on initilization in an empty project.
