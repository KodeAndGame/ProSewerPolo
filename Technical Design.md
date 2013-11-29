Pro Sewer Polo Technical Design
===============================

Gameplay Terminology
------------------
- *Application*: Refers to the Pro Sewer Polo application as a whole. Includes menus, in-game play, splash screen and anything that runs as part of the final EXE.
- *Match*: Refers to a match played between competitors in Pro Sewer Polo. "Game" is not used here in case we want to allow room for a game to be a part of a match. For example, in tennis there is a game, a set and a match. 
- *Match Type*: Notes a type of match that can be played. Some collection of rules or modifiers that may distinguish it from another type of match that is played.
- *Modifier*: Notes a gameplay related value that can change in order to alter desired gameplay effects. For example, "Stop to Shoot" can be enabled or disabled. When enabled, the swimmer will stop as soon the Player holds the Shoot button for that Swimmer. 
- *Player*: Refers to the Actor making decisions for a team. 
- *Team*:  A team is composed of two swimmers (normally referred to as Left and Right). A team itself can be referred to by its color (e.g. Red or Blue) The aim of the team is to score as many goals against their opponent as possible. 
- *Swimmer*: A swimmer is primarily what the Player is controlling to win the match. The player will actually normally control two swimmers (Left and Right) as part of a team.

Technical Terminology
-----------------
- *Singleton*
- *Factory*
- *GameObject*
- *MonoBehavior*
- *Interface*

Namespaces
-----------------

Classes
-----------------
- *ApplicationManager* (Singleton)
- *MatchTypes* (Enum)
- *MatchFactory* (Factory)
- *Match* (MonoBehavior)
- *ModifierManager* (Singleton)
- *SwimmerAnimation
- *SwimmerBehavior*
- *ISwimmerController (Interface)
- *PhysicalSwimmerController* (MonoBehavior)
- *ArtificialSwimmerController* (MonoBehavior)
- *SwimmerControllerFactory* (Factory)
- *SwimmerInput* 
