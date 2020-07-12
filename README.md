# Genus
2d MMO Engine WIP

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
 
 3: the build folder (GenusBuild) should have a file called /Data/ServerSettings.xml
 
       You can use SQLLite for getting started, SQLLite stores the database in a local file,
       in reality you would want to create an sql database required for the server to run, you 
       can do this using cloud based services such as amazon aws, or on the host computer.
       configure the SqlConnectionString for connection to your SQL database:
           <SqlConnectionString> </SqlConnectionString>
       
 4. run the server and client
 
        ip address and port settings can be configured in /Data/ServerSettings.xml for
        remote connections.
    
 5. create an account and login for testing
 
        current map spawn is map 0 at an X and Y of 0, need to add configuration for
        spawn locations and much much more!
