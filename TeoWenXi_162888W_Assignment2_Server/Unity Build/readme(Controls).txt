Controls:
Basic movement is the same(WASD/Mouse/Arrowkeys)
F - attack enemies when close to them
Y - Friend List
I - Inventory
G - Gamespark Menu
M - Toggle Full map

When Inventory is open:
1 or 2 to select the item
Q to drop selected item

FriendList uses mouse to click on the buttons

Chat Commands:
/f <text> - sends a message to all your friends
/addFriend <playerName> - sends a friend request to the targetted player

General Info about the scene:

//Logging in
-New player will be automatically registed on enter
-Returning player will be kicked if password is wrong and menu will say "Wrong Password"

//Player Info
-Player information is stored during runtime and retreived when the player logs back in (MySQL)
-Player info includes: Position, rotation, pet position, pet rotation, camera position, camera rotation, inventory and friendslist

//UI
-Player names are shown on top of players
-Same for enemies hp, it is shown as a hp bar on top of the enemies

//Pet
-Pet follows you wherever u go

//Inventory, Enemies and Items
-5 enemies will be spawned by the host on join, it will spawn more if any dies
-Enemies drops 2 items on death, items can be picked up by walking over them
-Items are stored in inventory and can be dropped
-Dropped items have the same stat as the one dropped from the inventory (it syncs)
-Other players can pick up the drop items for trading if wanted
