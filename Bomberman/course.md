# Bomberman

This challenge will see you playing a game of Bomberman against the other players.

## Server API

This describes the API of the server. All messages will be sent as text terminated by a line ending across a TCP connection. The hostname and port number of the server to connect to will be made known during the competition.

### Connecting

To connect to the server, open a TCP connection. If the server still has room for another player, you will receive an acknowledgement (`ACK`). If the server is full, you will receive `NACK`. If your connection has been acknowledged, you can now wait for the game ready command, which is `READY`. The ready command will be followed by the game configuration.

The game configuration consists of a number of keys followed by their value (here indicated by `N`). The keys are:

* `TURNS N`: Maximum number of turns the game will last
* `PLAYERS N`: Total number of players
* `NUMBER N`: Your player number
* `WIDTH N`: Width of playing field
* `HEIGHT N`: Height of playing field

The game configuration will be followed by the playing field: `HEIGHT` strings of length `WIDTH` that describe the playing field. For example:

```
##########
#1   #  2#
#   ##   #
### XX # #
# # XX ###
#   ##   #
#3  #   4#
##########
```

Where `#` is a wall, ` ` (a space) is empty, `X` is a box and `1`-`4` are the player positions.

Once the field has been received, you will receive the start command `START`, and you can send your first move to the server.

### Playing

During the game, you can send one of a number of messages to the server for actions. These are:

* `WAIT`: skip this turn and do nothing
* `UP`: move up (if possible)
* `DOWN`: move down (if possible)
* `LEFT`: move left (if possible)
* `RIGHT`: move right (if possible)
* `BOMB`: place a bomb at your current location (if possible)

You can only move in a direction if that space is empty (i.e. does not contain a box, wall, bomb or player). If two players attempt to move to the same space during a turn, both players will remain in place. You can only place a bomb if you do not already have a bomb in play.

After sending your action, you will receive a status update for each player. This will be preceded by a message of the form `UPDATE N`, where `N` will indicate the number of updates you will receive. These come in the form `N ACTION`, where `N` is the player number and `ACTION` is their action. These can be:

* `WAIT`: they skipped a turn or their action was unsuccesful
* `UP`: they moved up
* `DOWN`: they moved down
* `LEFT`: they moved left
* `RIGHT`: they moved right
* `BOMB`: they placed a bomb at their current location
* `DEAD`: they died

It is important that you check the action for your own player number too, as this will tell you whether your action has been succesful. Once a player has died, you will no longer receive updates for them. If you have died, you are free to remain connected to the game and await the outcome, but the server will no longer accept messages from your side of the connection.

### Game end

The game ends once only one player is left, all players have died or the number of turns have run out. At this point, instead of an `UPDATE N` message, you will receive a `END` message. This message will be followed by the result of the game: either `TIE` if everyone died or the turns ran out, or `WIN N` if a single player is left, where `N` is the number of the winning player.

After the game end message, the server will terminate the connection.

## The game

The goal of the game is to be the last man (bot) standing. You achieve this by blowing up the other players.

The game is turn based: every turn, you can either move one square or place a bomb. Five turns after a bomb has been placed, it will explode. It does so in a cross pattern that extends two squares in every direction. The explosion cannot pass through walls, but will destroy any boxes it touches. For example, if a bomb (the `O`) was placed here:

```
##########
#        #
#  ###   #
#   OX   #
#        #
#        #
#        #
##########
```

the resulting explosion (indicated with `*`) would be:

```
##########
#        #
#  ###   #
# ****   #
#   *    #
#   *    #
#        #
##########
```

Any player caught in the explosion will die and lose the game. You win the game by being the last player left alive. If no player is left alive or the maximum number of turns has passed, the game ends in a tie.

You can only place a bomb if you do not currently have another bomb in play. You have a bomb in play if you have placed a bomb and it has not yet exploded. You can only move into an empty space: you cannot move through walls, boxes, bombs or other players. The only "exception" to this is if you have just placed a bomb: in this case you are standing on the bomb, and you are allowed to step off it.

Good luck!
