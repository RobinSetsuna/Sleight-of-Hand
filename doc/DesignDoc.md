# PROJECT Design Documentation
> The following template provides the headings for your Design Documentation.  As you edit each section make sure you remove these commentary 'blockquotes'; the lines that start with a > character.


## Team Information
* Team name: TEAM-AWESOME
* Team members:
  * Haoyu Guo
  * Jeff Bauer
  * Ollyting Xin
  * Yiyan Wang
  * Zheyu Zhang


## Executive Summary
### Purpose
- Statement about the project: To create a casual strategy, stealth card game which can be taken beyond class as well as to improve our teammates' code, art and teamwork skills during the process. 
- User group: The most important user group as our target user are players who love strategy stealth games and cards game no matter they have much time to play or not.
- User goals: can enjoy the fun of the casual strategy stealth card game even when don't have too much time for entertainment. Easy to play and get access to the game on iOS/Android.  

### Glossary and Acronyms
| Term | Definition |
|------|------------|
| Map | A region or scene that players have to explore |
| Object | Anything in the map |
| Attribute | A nuimerical value representing objects' characteristics |
| Tile | A unit squared space in maps |
| Turn | A time period when either player character or computer-controled units can take actions |
| Phase | A specfic time in each turn when cards can take effect and player character can perform different actions |
| Standby Phase (SP) | The beginning of each turn when some cards can take effect |
| Draw Phase (DP) | The time in each turn when player character draws a card |
| Move Phase (MP) | The time in each turn when player character is able to move |
| Action Phase (AP) | The time in each turn when player character can use cards |
| End Phase (EP) | The end of each turn when some cards can take effect |
| Hand | All cards that player have |
| Strategy Card | A card that enables the player to perform special actions that are not available in regular game play |
| Enhancement Card | A card that can modify the attributes or status of units and other cards |
| Combat Card | A card that directly deals damages to units and objects or gives player character abilities to do so |


## Requirements
### Features
* **Main Menu**: The first interactable screen that users will encounter
* **Player Character**: The player's avatar
  * **Attribute**: A numerical value representing character's ability
    * **Speed**: A value determining how many grids can the character move in a map
* **Level**: A map
  * **Obstacle**: An object that is able to block the way of other objects
    * **Wall**
    * **Cage**
    * ...
  * **Enemy**: A Hostile creature that is generally controlled by artifical intelligence to stop players
    * **Guard**: A humanlike creature that detects invaders and fight against them
  * **The Interactible**: An object that players can interact with
    * **Save Point**: A special tile in the level that allows players to save their progression
* **User Interface**: A space on screen to demonstrate the status of the game and for players to interact
  * **Character Cultivation**: An user interface that allows players to nurture their character
  * **Level Selection**: An user interface that allows players to choose the level they want to play

### Non-functional Requirements
| NFRs |  Technical Constraints |
|------|------------------------|
| Maintainability | the ability to detect issues in codes and be able to solve those bugs after being released|
| Localization | the skill of understanding and using languages |
| Readability | good coding habit of each team member which adding proper comments to certain important parts of the codes |


## Domain
### Summary
This section describes the application domain.

![Domain Model](domain-model.png)

The domain contains 5 large entities (colored in red) which are player, map, tile, turn, and card. Besides those 5 entities, there are other smaller related entities which are grouped by their color.

### Player
The player defines the character controlled by the person who is playing the game. It can be viewed as an avatar of the actual person. People who are playing the game have to control this character to explore the map to achieve certain goals to win. In the game, the player character will take up exactly one tile in the map and can move and use cards to strive for the win.

### Map
The map defines a small region or scene which player must explore in the game. It is essential to the game because player’s objective is to overcome difficulties in the map in order to achieve some goals. There are 3 types of entities (colored in blue) including enemy, obstacle, and interactable object that may show in the map. Obstacles include walls and boxes which can block the way which the player may want to go. Enemies are hostile creatures acting like guardians that may attack and kill the player. Interactive objects include treasure cases and traps which player can interact with to utilize in order to achieve the goals.

### Tile
The tile defines a squared space in the map that can be taken up by player, obstacles, objects, and enemy. Also, both the movement of the player and enemies and the range of card effect will take measured in tiles.

### Turn
The turn defines the basic time unit of the game. Both the player and enemies have to move or do things turn by turn. Each turn consists of exactly 6 phases (colored in yellow) which are standby phase, draw phase, move phase 1, action phase, move phase 2, and end phase. The player can draw 1 card in DP, move in MP1 and MP2 and use cards in AP.

### Card
The card is a item which the player can use to facilitate the exploration. It can be gained both at the beginning of entering any maps and during exploration by beating enemy or getting loots from treasures in the map. There are 3 types of card (colored in green) which are strategy card, enhancement card, and combat card. Strategy card can allow the player to play strategically against the environment. Enhancement card can enhance the player itself or other cards. Combat card will enable the player to attack enemies or obstacles. All those cards play important roles in the game because they help player overcome difficulties which cannot be dealt only by player itself.


## Architecture
### Summary
The following model shows a high-level view of the game's architecture.

![Architecture Overview](program-architecture.png)

The overall game architecture consists of three major layers which are I/O layer, Unity API layer and game logic layer. In I/O layer, it contains four components that serve the function of receiving player’s input data and giving feedback to the player. In the Unity API layer, it contains four components which are implemented by the unity engine. We can use the API and other tools offered by unity to create our assets. In the game logic layer, there is usually a main loop in the game program and some other essential logic scripts that make the game run. The important technical choice is that since our game is a mobile platform game, we deciede use display as both our input and ouput component in the I/O layer. 

The reason why we choose this architecture over other alternative is that Unity is a supportive game engine. It has a thriving community. There are tons of forums out there where Unity fans unite. The in-built Unity API and tools will do a great help for us to develop our own game. Therefore, we don’t need to pay attention to the low-level actuation of games. Besides, some members of our group have Unity developing experience before. Using unity and the architecture based on that will make us go more smoothly in developing our game.

### Display
Display is the component that read data from unity graphic engine and unity user interface and then converts these data into graphics in the screen. Player will also put data to Unity graphic through the display.

### Sound
Sound is the component that handles the control of BGM and soundFX. It will receive the processed sound data from unity sound engine. The programmer can write scripts to control the sound according to the gameplay and story so that player can hear the sound when we want them to.

### Input Device
Input Device can read data and convert them to a form that a computer can use. 

### Unity Sound
The unity sound engine is the component that consists of algorithms for dealing with sound. The engine has in-built programs that can handle the sound effects embedded in the game. Besides, it also has the capability to do calculations using the CPU. Abstraction APIs, such as Open-AL, SDL Audio, X-Audio 2, Web Audio, etc. can be available within this engine.

### Unity Graphics\Physics
The unity graphics engine is in charge of most graphic effects. It produces 3D animated graphics, using different techniques, like rasterization and ray-tracing. Several graphics API are provided for programmers to take control of the graphics on their own. To put simply, it is a set of tools that programmers can use to easily build the scene and effects.

### Unity User interface
Unity offers a bunch of tools for designing user interface such as canvas, button, image, text, etc. By using those tools, the programmer can build a UI based on the unity user interface.


## Detailed Design
> You'll add to this section as needed as the project progresses


## Issues and Risks
* The reliability of Enemy AI could be one of the main factors will impact the user experience. To make the game not too easy or too       hard, we need to adjust enemy intelligence very carefully.
* The diversity of the cards is one of another issue. If we don't have enough compelling card, there is no power to attract customers.
* The length of each play session may limited the scale of map size and also effect the actual oppotunity of using cards in game.
