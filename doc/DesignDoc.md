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

> Introduction
The domain contains 5 large entities (colored in red) which are player, map, tile, turn, and card. Besides those 5 entities, there are other smaller related entities which are grouped by their color.

> Player
The player defines the character controlled by the person who is playing the game. It can be viewed as an avatar of the actual person. People who are playing the game have to control this character to explore the map to achieve certain goals to win. In the game, the player character will take up exactly one tile in the map and can move and use cards to strive for the win.

> Map
The map defines a small region or scene which player must explore in the game. It is essential to the game because player’s objective is to overcome difficulties in the map in order to achieve some goals. There are 3 types of entities (colored in blue) including enemy, obstacle, and interactable object that may show in the map. Obstacles include walls and boxes which can block the way which the player may want to go. Enemies are hostile creatures acting like guardians that may attack and kill the player. Interactive objects include treasure cases and traps which player can interact with to utilize in order to achieve the goals.

> Tile
The tile defines a squared space in the map that can be taken up by player, obstacles, objects, and enemy. Also, both the movement of the player and enemies and the range of card effect will take measured in tiles.

> Turn
The turn defines the basic time unit of the game. Both the player and enemies have to move or do things turn by turn. Each turn consists of exactly 6 phases (colored in <span style="color:yellow">yellow</span>) which are standby phase (SP), draw phase (DP), move phase 1 (MP1), action phase (AP), move phase 2 (MP2), and end phase (EP). The player can draw 1 card in DP, move in MP1 and MP2 and use cards in AP.

> Card
The card is a item which the player can use to facilitate the exploration. It can be gained both at the beginning of entering any maps and during exploration by beating enemy or getting loots from treasures in the map. There are 3 types of card (colored in green) which are strategy card, enhancement card, and combat card. Strategy card can allow the player to play strategically against the environment. Enhancement card can enhance the player itself or other cards. Combat card will enable the player to attack enemies or obstacles. All those cards play important roles in the game because they help player overcome difficulties which cannot be dealt only by player itself.



## Architecture

This section describes the application architecture.

### Summary
The following model shows a high-level view of the webapp's architecture.

![Architecture Overview](program-architecture.png)
> Replace the placeholder image above with your team's own architecture model. 

> Add a description of the architecture and key technical decisions

### Component 1 ...
> Provide a summary of each component with extra models as needed


### Component 1 ...
> Provide a summary of each component with extra models as needed

## Detailed Design

> You'll add to this section as needed as the project progresses


## Issues and Risks

> Open issues, risks, and your plan to address them (or plan to research options)
