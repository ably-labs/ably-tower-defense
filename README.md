[![Discord](https://img.shields.io/discord/823552399909584908?label=Discord&style=for-the-badge)](https://discord.gg/7qWKqdQWYB) 

# Multiplayer Tower Defense with Ably

This repo demonstrates a method of creating a  multiplayer tower defense game. The core game based on [raywenderlich's amazing tutorial series](https://www.raywenderlich.com/269-how-to-create-a-tower-defense-game-in-unity-part-1) on creating a Tower Defense game in Unity. For more details on the core game functionality, check out that tutorial. The multiplayer functionality makes use of [Ably](https://ably.com/), a realtime edge messaging broker.

## Functionality

The core functionality available in this version of tower defense are:
* Turrets (or 'monsters' in this case) can be placed on certain tiles denoted by an X
    * Turrets cost money to place, which is finite. Destroying enemies will earn the player(s) more money
* Enemies will at the start of each wave start emerging from a the start of a path, and make their way to your 'cookie' to destroy it
    * Turrets will fire at these enemies when they come into proximity of them
* If enough enemies reach your cookie, they will destroy it, resulting in you losing the game
* If the player(s) manage to survive all the waves, they win the game

The multiplayer functionality implemented with Ably allows for:
* Multiple players can join a game from a Lobby with the use of a code
* Players can start the game, resulting in the game starting at the same time for everyone
* All players can place and upgrade monsters, and these updates will by synced between all players

## Ensuring synchronisation between clients

One of the main issues with a multiplayer game is ensuring that all the clients are seeing the same version of events as one another, and are eventually consistent in terms of the outcome of a game. It'd be bad if 2 players could play the same game but one see that they won, and the other that they lost.

In this implementation, we make use of two simple methods of ensuring synchronisation is maintained which will work for most scenarios. Firstly, if a client falls behind the current true state of the game, they will fast-forward through the game to catch up, playing player actions at the appropriate times.

Secondly, to offset the potential for some clients to have a poor internet connection, resulting in delayed message delivery between clients, and thus some actions being actioned later for some clients than others, we have an inherent delay between a monster being placed and a monster being able to fire. This means that so long as a client is able to get a message in this window (which is customisable), all the game-impacting actions (destroying enemies) will remain synced.

In order to make these things possible however, we require certain things:
* A source to act as the truth as to when actions occur between all clients
* A way to ensure that all actions are communicated in the same order between clients

Thankfully Ably can provide each of these things inherently. All messages distributed by Ably will have a timestamp attached which represents when the messages was received by Ably. Clients can use this timestamp on each message to establish when it should be enactioned. If the time is in the future in comparison to the amount of game time that's passed, it's an indicator that the game needs to fast-forward to match the indicated time.

In addition, Ably provides a message ordering guarantee as we as an ['exactly-once' delivery guarantee](https://ably.com/blog/achieving-exactly-once-message-processing-with-ably), which ensures that all clients will maintain the same sequence of events between one another.

## Feedback and Questions

If there's any questions you'd like to ask, please reach out to us on [Twitter](https://twitter.com/ablyrealtime) or [Discord](https://discord.gg/7qWKqdQWYB).
