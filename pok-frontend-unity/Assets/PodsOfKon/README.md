# Pods of Kon

## Flow
- Participants badge is scanned - this is for sales
- Participant enters basic info into system  - this is for dev database
- Either participant or Oracle EE enters name (last initial in most cases is ok) in form: http://143.47.96.92/podsofkon/form - this is for game (correlates with dev db)
- Names are loaded by the game and stored in SCORES table
- Optionally, pulling either joystick down in the first page of the game (architecture page) triggers reload of game names to check ahead of time
- 

## Database tables for game
- Dev info table (this resides in separate database from the game at this point)
- CURRENTGAME table with two columns (playername and score) and two rows for playerone and playertwo
- SCORES table with two columns (playername and score).
- CURRENTGAME is the live game. When the game ends (when the "Thanks Scene" is entered) the scores are transferred to SCORES and CURRENTGAME table scores are set to 0

## Architecture

### Database Interactions (all from Spring Boot microservice)

- Opening Screen: set player1 and player2 currentgamescore to 0
- Directions Screen 1/objective: none
- Directions Screen 2/scoring: none
- Game Scene: enqueue points from targets hit. batch update scores
- Bonus Scene: enqueue points from correct answers. batch update scores
- Thank You Scene: none

### Kubernetes Interactions (all from Spring Boot microservice)

- Opening Screen: delete deployments from player1 and player2 namespaces
- Directions Screen 1/objective: none
- Directions Screen 2/scoring: none
- Game Scene: create and delete deployments from targets hit.
- Bonus Scene: none
- Thank You Scene: none

### Unity Details

- Opening Screen: delete deployments from player1 and player2 namespaces
- Directions Screen 1/objective: none
- Directions Screen 2/scoring: none
- Game Scene: create and delete deployments from targets hit.
- Bonus Scene: none
- Thank You Scene: none

### Oracle Tech Used
- Kubernetes for all microservices and database access: Oracle Kubernetes Environment
- Spring Boot, etc. microservices running as native images: Java GraalVM
### Oracle Database Tech Used
- JSON, Relational, and Graph access to same data: Oracle Database JSON Duality
- Multi-language support for data-driven and message-driven microservices: Oracle Database drivers
- Sagas and auto-compensating data types for microservices recovery: MicroTx and Oracle Database Saga Engine
- Observability via Grafana: Oracle exporters and unified observability strategy.
- App tier caching: Oracle TrueCache (Redis alternative)
- 3D model storage and processing: Oracle Database Spatial
- Audio storage: Oracle 