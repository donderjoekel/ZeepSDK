# ZeepSDK

The ZeepSDK is a community built SDK that aims to make it easier to develop custom plugins for Zeepkist. It does this by exposing APIs that hide the actual implementation that is required to achieve the desired functionality, making it easy to use for the developer.

Api documentation is available here: https://donderjoekel.github.io/ZeepSDK/api/index.html

## Current features

### Chat Api
The ChatApi allows you to interact with the chat. It gives you tools to easily send a chat message, or add a message to your local chat box.

### Chat Commands Api
The ChatCommandsApi can be used to create local and remote chat commands. Local chat commands are for the local user, the person that is playing the game. Remote chat commands can be used by others in the same lobby, but only if you are currently the host. This could be useful for things like a vote skip.
It comes with a couple of built in commands
Local:
- Help
    - Shows you the available local commands with a description
- Clear
    - Clears the chat

Remote:
- Help
    - Shows you the available remote commands with a description

### Leaderboard Api
The LeaderboardApi allows you to implement your custom leaderboard page. This allows multiple developers to all add their own pages to the leaderboard. Good examples of this are the GTR Leaderboard and the COTD Leaderboard.

### Level Editor Api
The LevelEditorApi contains a collection of functions and events that are related to the level editor and it's usage.

Currently there are two events:
- EnteredTestMode
    - As the name suggests, this event fires off whenever the user enters the track testing mode
- EnteredLevelEditor
    - As the name suggests, this event fires off when the user enters the level editor. This is either from the main menu or when exiting the test mode

There's functionality to block mouse input and/or keyboard input. This is useful for mods who display an overlay in the level editor.

Another cool feature is that there is functionality that allows you to create custom folders and items that will show up in the block gui!

### Racing Api
The RacingApi currently contains a collection of events that are related to the player racing. Think of events like crossing the finish line, entering first/third person, or crashing.
