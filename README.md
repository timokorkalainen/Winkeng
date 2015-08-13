# Winkeng
Stream capture software for the Lenkeng HDMI extenders, for Windows. Forked from bp2008's work at https://hdmiextender.codeplex.com which in turn is heavily based on danman's reverse engineering efforts at https://danman.eu/blog/reverse-engineering-lenkeng-hdmi-over-ip-extender/. 99% of the current work is done by them.

## Requirements
* Recent WinPcap http://www.winpcap.org/
* Visual Studio
* Brave heart, much to do still...

## Command line arguments
* c:cmd		Run as a console application rather than a Windows Service.
* i:ip		Input device IP.
* n:interface		Network interface id where the input device is connected to.
* p:port		Port to use for the output streams.
* v:verbose		Print 'netdrop1' or 'netdrop2'  in the console when a frame is dropped.

