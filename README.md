# SA-MPtrainer
A simple trainer for GTA San Andreas multiplayer mod (SA-MP).

# What is SA-MP
<a href="https://www.sa-mp.com/">SA-MP</a> is a multiplayer mod for the game GTA San Andreas. It works by starting the GTA San Andreas game executable and then idjecting a samp.dll into its memory. The mod enables a player to connect to a player created multiplayer server and enjoy the game with other players.

This is a program that exploits a certain SA-MP server. The server has a system of stealing cars in which in order to start the engine of a car, the user is given a list of scrambled words, if player manages to unscrabmle them, then the hotwiring is successful.

By examining the executable's memory with <a href="https://www.cheatengine.org/">Cheat Engine</a> I was able to find the pointer to the address of the ChatBox input (samp.dll + 17FBB8 where saml.dll represents the base address of the samp.dll module and 17FBB8 is the offset), thus being able to read the current input that is in the chatbox. I also found *(samp.dll + 0x2ACA14) + 14E0 which is a boolean that indicates whether the chatbox is opened or not. 

The program checks for the input /word00 in the chatbox, attempts to unscramble the word using an english dictionary and then atomatically returns the unscrabmled word to the user's chatbox.

Because the samp.dll is injected into GTA executable and works off it, there are a number of exploitable vulnerabilities that can be used in order to gain an unfair advantage over other players (calling gta sa game functions to create weapons or vehicles, or enabling cheats).

The GTA games are famous for their hidden cheat codes, cheats that enable players to spawn vehicles and weapons, or even become invisible.
The samp.dll disables those cheats by NOP-ing the function that checks the input of those cheats. But we can get by that by enabling each cheat individually. Example: this trainer can enable the Invisible Cars cheat by triggering the cheat's isOn boolean value to true, or turn it off in the same manner. The address of that boolean value can easily be found with Cheat Engine, but many are already available on the <a href="https://gtamods.com/wiki/Memory_Addresses_(SA)">internet</a>. This trainer checks an array of characters inputs already available in the game (the same array that the gta checks for cheat inputs 0x969110) to see if the key 0 was pressed and enables of disables the cheat accordingly. 


