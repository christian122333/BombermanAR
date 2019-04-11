# BombermanAR
This project is centered around creating a video game, in particular the implementation of a popular video game series called Bomberman. This game features multiplayer, uses augmented reality, and can be played on the phone. The goal of the game is to kill other players by dropping bombs until they are the last remaining player. There are obstacles in the game level and various powerups that the players can pick up. This project was developed using Google’s ARCore software development kit (SDK) and the Unity game engine. To develop multiplayer, an SDK called Photon was used in unison with Unity.

![b1](bomberman1.gif)

Objects rendered in the scene will be attached to the anchor so that they will maintain their relative position and orientation in the world as the camer amoves around and updates each frame. When the user opens their app,they will use the camera to find vertical or horizontal planes in the real world as shown as shown in the gif above. This plane is what is known as a trackable, and the anchor will be attached to the trackable so that the generated game level will maintain a position relative to the plane.

![b2](bomberman2.gif)

The player can move using the directional pad and drop bombs by pressing the fire button. The bombs are dropped in the center of each block, and then explosions spread out in four directionsafter a set number of seconds. If the explosion from another bomb meets a player’s collider, that player will be destroyed regardless of who dropped the bomb. There are several destructible crates that are spawned on a level, and these crates have a chance of spawning a power up when it’s destroyed.
