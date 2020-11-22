# Genus
2d MMO Engine WIP

 Programed using C# and Visual Studio, OpenGL rendering using OpenTK and the game editor uses WinForms.
 Project configured against .net 4.7, when built all build files go to a root folder called 'GenusBuild'.

 1: build all 4 projects:
 
       Genus2D - Engine code
       RpgEditor - Game editor
       RpgGame - Game client
       RpgServer - Server executable
       
 2: use the editor to make some data.
       
       * Maps
       * Tilesets
       * Events
       * Sprites
       * Items & Projectiles
       * Classes / Enemies / Drop Tables
       * Quests / Shops
       * System Data
      (more to come)
 
 3: the build folder (GenusBuild) you have a file called /Data/ServerSettings.xml for server config
 
       For the database you can use SQLLite for getting started, SQLLite stores the database in 
       a local file but in reality you would want to create an sql database for the server to use. 
       You could host an SQL database using cloud based services such as amazon aws, or on the host 
       computer, configure the SqlConnectionString for connection to your SQL database and set SQLite 
       to false:
           
           <SQLite>false</SQLite>
           <SqlConnectionString> CONNECTION_STRING </SqlConnectionString>
       
 4. run the server and client
 
        IP address and port settings can also be configured in /Data/ServerSettings.xml for remote 
        connections, make sure ports are opened on the firewall / network for that chosen (TCP):
            
            <ExternalIpAddress>127.0.0.1</ExternalIpAddress>
            <Port>55998</Port>
            
            (External IP is for the client to find the server, the server launches with 'LocalHost')
    
 5. create an account and login for testing. You can run all the .exe's from the same project folder
    however later it would be desirable to remove the editor and server .exe's when sending out the
    client, also some data files are only used by the server but the client does also use some so that
    all data doesn't have to be sent over the network. To confirm the full list later, but for example
    the server sends map data over the network (as events / map enemies change) and the client has no
    interaction with raw event data/commands.
 
 6. Some misc controller information (will be refined as the project develops):
 
        * WASD is movement
        * Space is action trigger (triggers events first, if no event can be triggered then it initiates the combat trigger)
        * Enter key to close a message box shown from an event (will change this at somepoint when the UI is better)
        * Escape key to return to logon screen
        * Left mouse interaction with some UI's, such as inventory, trade panel ect
        * Right mouse interaction with some UI's, such as inventory/trade panel, map entities (players/enemies/items)
        
        * In the game editor with the Event map tool selected: left mouse creates / edits an event, right click removes it 
        
        
