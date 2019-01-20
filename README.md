# Genus
2d MMO Engine WIP

 1: build all 4 projects:
 
       Genus2D
       RpgEditor
       RpgGame
       RpgServer
       
 2: use the editor to make some data.
 
       tilesets
       maps
       events
      (more to come)
 
 3: the build folder (GenusBuild) should have a file called /Data/ServerSettings.xml
 
       add xml settings for connection to a Microsoft SQL database for the server.exe to run:
           <SqlConnectionString> </SqlConnectionString>
       the database created can be blank as the server will handle creating the tables required.
       
 4. run the server and client
 
        ip address and port settings can be configured in /Data/ServerSettings.xml for
        remote connections.
    
 5. create an account and login for testing
 
        current map spawn is map 0 at an X and Y of 0, need to add configuration for
        spawn locations and much much more!
