package podsofkon;


import com.fasterxml.jackson.databind.ObjectMapper;
import podsofkon.k8s.*;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.web.bind.annotation.*;

import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;
import javax.sql.DataSource;
import java.sql.*;
import java.util.*;

    @RestController
    @RequestMapping("/podsofkon")
    public class PodsOfKonController {

        private static Logger log = LoggerFactory.getLogger(podsofkon.PodsOfKonController.class);


        private static Connection conn;
        private static PreparedStatement preparedStatementIncrementScore;
        private static PreparedStatement preparedStatementUpdateScore;
        private static PreparedStatement preparedStatementClearScore;
        private static PreparedStatement insertFinalScore;

        @Autowired
        DataSource datasource;


        String query = "SELECT * FROM ORDERUSER.QANDA";

        @GetMapping("/test")
        public String test() throws Exception {
            return "test successful";
        }

        @GetMapping("/questions")
        public String questions() throws Exception {
            log.debug("questions datasource:" + datasource + "...");
                BonusRound quiz = new BonusRound();
                    List<Question> questionsList = new ArrayList<>();
                    try (Connection connection = datasource.getConnection();
                         Statement statement = connection.createStatement()) {
                        ResultSet resultSet = statement.executeQuery(query);
                        while (resultSet.next()) {
                            Question question = new Question();
                            String question1 = resultSet.getString("question");
                            question.setText(question1);
//                            System.out.println("PodsOfKonController.questions question" + question1);
                            List<Answer> answers = new ArrayList<>();
                            for (int i = 1; i <= 5; i++) {
                                String answerText = resultSet.getString("answer" + i);
                                String isCorrect = resultSet.getString("answer" + i + "IsCorrect");
                                if (answerText != null) {  // Assuming the answer fields can be null
                                    answers.add(new Answer(answerText, isCorrect));
                                }
                            }
                            question.setAnswers(answers);
                            questionsList.add(question);
                        }
                        quiz.setQuestions(questionsList);
                    } catch (SQLException e) {
                        e.printStackTrace();
                    }
            ObjectMapper objectMapper = new ObjectMapper();
            try {
                String jsonString = objectMapper.writeValueAsString(quiz);
                System.out.println(jsonString);
                return  jsonString;
            } catch (Exception e) {
                e.printStackTrace();
                return "question query failed e:" + e;
            }
        }

        @GetMapping("/createTables")
        public String createTables() throws Exception {
            log.debug("createTables for datasource:" + datasource + "...");
            initConn();
            conn.createStatement().execute("create table currentgame( playername varchar(256), score number(10)  )");
            conn.createStatement().execute("insert into currentgame values ( 'player1', 0  )");
            conn.createStatement().execute("insert into currentgame values ( 'player2', 0  )");
            conn.createStatement().execute("create table scores( playername varchar(256), score number(10) )");
            String returnString = "createTables success";
            return returnString;
        }

        @GetMapping("/createDeployment")
        public String createDeployment(@RequestParam("appName") String appName, @RequestParam("serviceName") String serviceName) {
            log.debug("create deployment and appName  = " + appName );
            log.debug("create deployment and service  = " + serviceName );
            try {
                new ApplyDeployment().createDeployment(appName, serviceName);
            //    updateScore(appName, 100);
            } catch (Exception e) {
//                return "Exception occurred during create deployment operation (perhaps dupe)";
                return "Exception occurred during create deployment operation (perhaps dupe):" + e;
            }
//            try {
//      //          incrementScore(appName, 200);
//            } catch (Exception e) {
//                return "Exception occurred during incrementScoreoperation:" + e;
//            }
            return "create deployment and service  = " + serviceName + " successful";
        }



        @GetMapping("/movescores")
        public String movescores(@RequestParam("player1Name") String player1Name, @RequestParam("player1Score") int player1Score,
                                 @RequestParam("player2Name") String player2Name, @RequestParam("player2Score") int player2Score) throws Exception {
            System.out.println("movescores for datasource:" + datasource + "...");
            initConn();
            insertFinalScore.setString(1, player1Name);
            insertFinalScore.setInt(2, player1Score);
            insertFinalScore.execute();
            insertFinalScore.setString(1, player2Name);
            insertFinalScore.setInt(2, player2Score);
            insertFinalScore.execute();
            //clear the current game...
            clearScore("player1");
            clearScore("player2");
            System.out.println("deleteDeployments for player1...");
            createDeployment("player1", "database");
            deleteDeployment("player1", "javascript-deployment");
            createDeployment("player1", "graalvm");
            deleteDeployment("player1", "rust-deployment");
            deleteDeployment("player1", "go-deployment");
            deleteDeployment("player1", "python-deployment");
            deleteDeployment("player1", "dotnet-deployment");
            createDeployment("player1", "springboot");
            System.out.println("deleteDeployments for player2...");
            createDeployment("player2", "database");
            deleteDeployment("player2", "javascript-deployment");
            createDeployment("player2", "graalvm");
            deleteDeployment("player2", "rust-deployment");
            deleteDeployment("player2", "go-deployment");
            deleteDeployment("player2", "python-deployment");
            deleteDeployment("player2", "dotnet-deployment");
            createDeployment("player2", "springboot");
            return "movescores success";
        }


        String player1Name = "steelix";
        String player2Name = "umbreon";
        @GetMapping("/setPlayerNamesAndIds")
        public void setPlayerNamesAndIds(HttpServletRequest request, HttpServletResponse response,
                @RequestParam(name = "isRegistered", required = false) boolean isRegistered,
                @RequestParam("player1Name") String player1Name,
                @RequestParam("player2Name") String player2Name) throws Exception {
            if (!player1Name.trim().equals("") ) this.player1Name = player1Name;
            if (!player2Name.trim().equals("") ) this.player2Name = player2Name;
            System.out.println("PodsOfKonController.setPlayerNamesAndIds isRegistered:" + isRegistered);
//            return "success";
            response.setStatus(HttpServletResponse.SC_MOVED_PERMANENTLY);
            if (isRegistered) {
                response.setHeader("Location", "/podsofkon/nameupdatedsuccess");
            } else {
                response.setHeader("Location",
                        "https://wkrfs4xeqva1jcu-indadw.adb.us-phoenix-1.oraclecloudapps.com/" +
                                "ords/r/demouserws/contactinformation/crm2det");
            }
        }

        @GetMapping("/getPlayerName")
        public String getPlayerNamesAndIds(@RequestParam("playerName") String playerName) throws Exception {
            return playerName.equals("player1")?player1Name:player2Name;

        }
        @GetMapping("/nameupdatedsuccess")
        public String nameupdatedsuccess() throws Exception {
            return "Successfully updated player name.  Thanks!";

        }

        @GetMapping("/incrementScore")
        public String incrementScore(@RequestParam("playerName") String playerName,
                                     @RequestParam("amount") int amount) throws Exception {
            log.debug("incrementScore for playerName:" + playerName + "...");
            log.debug("incrementScore for datasource:" + datasource + "...");
            updateScore(playerName, amount);
            return "incrementScore success";
        }

        @GetMapping("/updateScores")
        public String updateScores(@RequestParam("player1Score") int player1Score,
                                     @RequestParam("player2Score") int player2Score) throws Exception {
            log.debug("updateScores player1Score:" + player1Score + "...");
            log.debug("updateScores player2Score:" + player2Score + "...");
            updateScore("player1", player1Score);
            updateScore("player2", player2Score);
            return "updateScores success";
        }

        private void updateScore(String playerName, int amount) throws SQLException {
            initConn();
            try {
                preparedStatementUpdateScore.setInt(1, amount);
                preparedStatementUpdateScore.setString(2, playerName);
                preparedStatementUpdateScore.execute();
            } catch (Exception ex) {
                ex.printStackTrace();
            }
        }

        private void increment(String playerName, int amount) throws SQLException {
            initConn();
            try {
                preparedStatementIncrementScore.setInt(1, amount);
                preparedStatementIncrementScore.setString(2, playerName);
                preparedStatementIncrementScore.execute();
            } catch (Exception ex) {
                ex.printStackTrace();
            }
        }
        private void clearScore(String playerName) throws SQLException {
            initConn();
            try {
                preparedStatementClearScore.setString(1, playerName);
                preparedStatementClearScore.execute();
            } catch (Exception ex) {
                ex.printStackTrace();
            }
        }

        private void initConn() throws SQLException {
            if (conn == null || conn.isClosed() || conn.isValid(1) ||
                    preparedStatementIncrementScore ==null) {
                conn = datasource.getConnection();
                preparedStatementIncrementScore = conn.prepareStatement(
                        "UPDATE currentgame SET score=score+? WHERE playername=?");
                preparedStatementUpdateScore = conn.prepareStatement(
                        "UPDATE currentgame SET score=? WHERE playername=?");
                preparedStatementClearScore = conn.prepareStatement(
                        "UPDATE currentgame SET score=0 WHERE playername=?");
                insertFinalScore = conn.prepareStatement(
                        "insert into scores values ( ?, ? )");
            }
        }

//        @GetMapping("/deleteDeployment")
//        public String delete(@RequestParam("appName") String appName, String deploymentName) throws Exception {
            @GetMapping("/deleteDeployment")
            public String deleteDeployment(@RequestParam("appName") String appName, @RequestParam("serviceName") String serviceName) {
                System.out.println("deleteDeployment appName = " + appName + ", serviceName = " + serviceName);
            try {
                return new DeleteDeployment().deleteDeployment(appName, serviceName);
            } catch (Exception e) {
                return "Exception occurred during delete operation:" + e.getMessage();
            }
        }
        private static final String[] HEADERS_TO_TRY = {
                "X-Forwarded-For",
                "Proxy-Client-IP",
                "WL-Proxy-Client-IP",
                "HTTP_X_FORWARDED_FOR",
                "HTTP_X_FORWARDED",
                "HTTP_X_CLUSTER_CLIENT_IP",
                "HTTP_CLIENT_IP",
                "HTTP_FORWARDED_FOR",
                "HTTP_FORWARDED",
                "HTTP_VIA",
                "REMOTE_ADDR" };

        private String getClientIpAddress(HttpServletRequest request) {
            for (String header : HEADERS_TO_TRY) {
                String ip = request.getHeader(header);
                if (ip != null && ip.length() != 0 && !"unknown".equalsIgnoreCase(ip)) {
                    return ip;
                }
            }

            return request.getRemoteAddr();
        }



    }

