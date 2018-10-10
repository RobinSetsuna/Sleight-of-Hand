# PROJECT Design Documentation
> The following template provides the headings for your Design Documentation.  As you edit each section make sure you remove these commentary 'blockquotes'; the lines that start with a > character.

# Team Information
* Team name: TEAM-AWESOME
* Team members
- Yiyan Wang
- Zheyu Zhang
- Haoyu Guo
- Jeff Bauer
- Ollyting Xin

## Executive Summary

This is a summary of the project.

### Purpose
> Provide a very brief statement about the project and the most important user group and user goals.

### Glossary and Acronyms
> Provide a table of terms and acronyms.

| Term | Definition |
|------|------------|
| term/acronym | definition |


## Requirements

### Features

This section describes the main features of the game.

> In this section you do not need to be exhaustive and list every story.  Focus on top-level features and maybe Epics and *critical* Stories.

### Non-functional Requirements
> Key NFRs and technical constraints


## Domain

This section describes the application domain.

![Domain Model](domain-model.png)

The domain contains 5 large entities (colored in red) which are player, map, tile, turn, and card. Besides those 5 entities, there are other smaller related entities which are grouped by their color.

#### Player

The player defines the character controlled by the person who is playing the game. It can be viewed as an avatar of the actual person. People who are playing the game have to control this character to explore the map to achieve certain goals to win. In the game, the player character will take up exactly one tile in the map and can move and use cards to strive for the win.

#### Map

The map defines a small region or scene which player must explore in the game. It is essential to the game because player’s objective is to overcome difficulties in the map in order to achieve some goals. There are 3 types of entities (colored in blue) including enemy, obstacle, and interactable object that may show in the map. Obstacles include walls and boxes which can block the way which the player may want to go. Enemies are hostile creatures acting like guardians that may attack and kill the player. Interactive objects include treasure cases and traps which player can interact with to utilize in order to achieve the goals.

#### Tile

The tile defines a squared space in the map that can be taken up by player, obstacles, objects, and enemy. Also, both the movement of the player and enemies and the range of card effect will take measured in tiles.

#### Turn

The turn defines the basic time unit of the game. Both the player and enemies have to move or do things turn by turn. Each turn consists of exactly 6 phases (colored in yellow) which are standby phase (SP), draw phase (DP), move phase 1 (MP1), action phase (AP), move phase 2 (MP2), and end phase (EP). The player can draw 1 card in DP, move in MP1 and MP2 and use cards in AP.

#### Card

The card is a item which the player can use to facilitate the exploration. It can be gained both at the beginning of entering any maps and during exploration by beating enemy or getting loots from treasures in the map. There are 3 types of card (colored in green) which are strategy card, enhancement card, and combat card. Strategy card can allow the player to play strategically against the environment. Enhancement card can enhance the player itself or other cards. Combat card will enable the player to attack enemies or obstacles. All those cards play important roles in the game because they help player overcome difficulties which cannot be dealt only by player itself.



## Architecture

This section describes the game architecture.

### Summary
The following model shows a high-level view of the game's architecture.

![Architecture Overview](program-architecture.png)

> The overall game architecture consists of three major layers which are I/O layer, Unity API layer and game logic layer. In I/O layer, it contains four components that serve the function of receiving player’s input data and giving feedback to the player. In the Unity API layer, it contains four components which are implemented by the unity engine. We can use the API and other tools offered by unity to create our assets. In the game logic layer, there is usually a main loop in the game program and some other essential logic scripts that make the game run.

> The reason why we choose this architecture over other alternative is that Unity is a supportive game engine. It has a thriving community. There are tons of forums out there where Unity fans unite. The in-built Unity API and tools will do a great help for us to develop our own game. Therefore, we don’t need to pay attention to the low-level actuation of games. Besides, some members of our group have Unity developing experience before. Using unity and the architecture based on that will make us go more smoothly in developing our game.


### Sound
> Sound is the component that handles the control of BGM and soundFX. It will receive the processed sound data from unity sound engine. The programmer can write scripts to control the sound according to the gameplay and story so that player can hear the sound when we want them to.

### Input Device
> Input Device can read data and convert them to a form that a computer can use. 

### Unity Sound
> The unity sound engine is the component that consists of algorithms for dealing with sound. The engine has in-built programs that can handle the sound effects embedded in the game. Besides, it also has the capability to do calculations using the CPU. Abstraction APIs, such as Open-AL, SDL Audio, X-Audio 2, Web Audio, etc. can be available within this engine.

### Display
> Display is the component that read data from unity graphic engine and unity user interface and then converts these data into graphics in the screen.

### Unity Graphics
> The unity graphics engine is in charge of most graphic effects. It produces 3D animated graphics, using different techniques, like rasterization and ray-tracing. Several graphics API are provided for programmers to take control of the graphics on their own. To put simply, it is a set of tools that programmers can use to easily build the scene and effects.

### Unity User interface
> Unity offers a bunch of tools for designing user interface such as canvas, button, image, text, etc. By using those tools, the programmer can build a UI based on the unity user interface.


## Detailed Design

> You'll add to this section as needed as the project progresses


## Issues and Risks

> Open issues, risks, and your plan to address them (or plan to research options)
