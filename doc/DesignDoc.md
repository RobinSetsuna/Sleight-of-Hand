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
> Replace the placeholder image above with your team's own domain model. 

> Provide a high-level overview of the domain. You can discuss the more important domain entities and their relationship to each other.



## Architecture

This section describes the game architecture.

### Summary
The following model shows a high-level view of the game's architecture.

![Architecture Overview](program-architecture.png)

> The overall game architecture consists of three major layers which are I/O layer, Unity API layer and game logic layer. In I/O layer, it contains four components that serve the function of receiving player’s input data and giving feedback to the player. In the Unity API layer, it contains four components which are implemented by the unity engine. We can use the API and other tools offered by unity to create our assets. In the game logic layer, there is usually a main loop in the game program and some other essential logic scripts that make the game run.

The reason why we choose this architecture over other alternative is that Unity is a supportive game engine. It has a thriving community. There are tons of forums out there where Unity fans unite. The in-built Unity API and tools will do a great help for us to develop our own game. Therefore, we don’t need to pay attention to the low-level actuation of games. Besides, some members of our group have Unity developing experience before. Using unity and the architecture based on that will make us go more smoothly in developing our game.


### Sound
> Sound is the component that handles the control of BGM and soundFX. It will receive the processed sound data from unity sound engine. The programmer can write scripts to control the sound according to the gameplay and story so that player can hear the sound when we want them to.


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
