## Build

## The game and builds can be done with varying levels of integration/dynamics

1. No build, no backend. Simply download a prebuilt binary and double-click
2. No frontend build, no back backend build. Configure front and/or backend using system properties, etc.
3. Build frontend and/or backend and configure.

### Database setup

There are 4 tables: CURRENTGAME, SCORES, PLAYERINFO, and QANDA

When participant submits the "scan here to play" form it inserts into PLAYERINFO as well as CURRENTGAME.

The QANDA table is where questions for the second/bonus round are kept.  These can be updated at anytime, even while the game is playing. 

### Backend Build

1. Provide necessary database connection values in `pok-backend-springboot`
2. Run `mvn install` 
3. Run `java -jar target/podsofkon.jar` for the Java backend
4. If deploying to k8s, set docker registry and kubectl environment, run ./build.sh and ./deploy.sh
    - The simplest way to setup the k8s cluster and Oracle Database 23ai is to run the "Simply Microservices with Oracle Converged Database" Livelab: https://apexapps.oracle.com/pls/apex/f?p=133:180:132654363894991::::wid:637

### Frontend Build

1. Load `pok-frontend-unity` into Unity editor and simply select the platform to build to. 
2. Note the `Assets\PodsOfKon\Scripts\EnvProperties.cs` file that has two attributes
    - `bool isOffline` which is set to true by default so the game can be played offline
    - `string bankendAddress` which is the address where the `pok-backend-springboot` is running
