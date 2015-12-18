# CodeSnips
Different code snips and small projects, for use in other projects.  
Projects marked with [Nuget] is avaliable on this nuget repository: http://nuget.m-mano.dk/  

### CyberMaster
Code to establish connection to a Lego CyberMaster (8482)  
**Not finished**  
If system is unable to find SPIRITLib, you have to register the .ocx file first.  
Get a Command Prompt to the folder and use the following command:  
Regsvr32 [/s] ./Spirit.ocx - /s registers without giving a message back. Optional.
In Visual Studio set Embed interop Types to False, in SPIRITLib reference Properties.

### [mmOAuth](https://github.com/MadSprayerDK/CodeSnips/wiki/mmOAuth) [Nuget]
OAuth 2.0 library for standalone applications (Eg. WPF).  
Contains providers for different services.

### TcpServer [Nuget]
Simple threaded TcpServer

### Utilities
Contains different utility projects
* Extensions: Extensions for existing system classes

### WebSocketServer [Nuget]
Simple threaded WebSocket Server.  
Based on the TcpServer.
