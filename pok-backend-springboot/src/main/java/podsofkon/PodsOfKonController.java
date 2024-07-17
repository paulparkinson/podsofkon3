package podsofkon;


import com.fasterxml.jackson.databind.ObjectMapper;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import podsofkon.k8s.*;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.web.bind.annotation.*;

//import jakarta.servlet.http.HttpServletRequest;
//import jakarta.servlet.http.HttpServletResponse;

import javax.sql.DataSource;
import java.sql.*;
import java.util.*;

@Controller
@RequestMapping("/podsofkon")
public class PodsOfKonController {

    private static Logger log = LoggerFactory.getLogger(podsofkon.PodsOfKonController.class);

    @Autowired
    DataSource datasource;

    String player1Name = "steelix";
    String player2Name = "umbreon";
    String questionsquery = "SELECT * FROM ORDERUSER.QANDA";
    String setPlayerNamesSQL =
            "INSERT INTO playerinfo (firstname, lastname, email, company, jobrole, tshirtsize, comments, playername) " +
                    "VALUES (?, ?, ?, ?, ?, ?, ?, ?)";

    @GetMapping("/test")
    public String test() throws Exception {
        return "test successful";
    }


    @GetMapping({"/form"})
    public String form(Model model) {
        return "player1or2";
    }

    @GetMapping({"/formplayerinfo"})
    public String formplayerinfo(Model model, @RequestParam("player") String player) {
        model.addAttribute("player", player);
        return "playerinfoform";
    }

    @PostMapping({"/setPlayerNamesAndIds"})
    public String setPlayerNamesAndIds(
            @RequestParam(value = "player1Name", required = false) String player1Name,
            @RequestParam(value = "player2Name", required = false) String player2Name,
            @RequestParam(value = "firstName") String firstName,
            @RequestParam(value = "lastName") String lastName,
            @RequestParam(value = "email") String email,
            @RequestParam(value = "company") String company,
            @RequestParam(value = "jobrole") String jobrole,
            @RequestParam(value = "tshirtsize") String tshirtsize,
            @RequestParam(value = "comments") String comments,
            Model model) {
        String playerName = "unknown";
        if (player1Name!=null && !player1Name.trim().equals("")) {
            this.player1Name = player1Name;
            playerName = player1Name;
        }
        if (player2Name!=null && !player2Name.trim().equals("")) {
            this.player2Name = player2Name;
            playerName = player2Name;
        }
        System.out.println("PodsOfKonController.setPlayerNamesAndIds, saving playerName:" + playerName);
        try (Connection conn = datasource.getConnection();
             PreparedStatement pstmt = conn.prepareStatement(setPlayerNamesSQL)) {
            pstmt.setString(1, firstName);
            pstmt.setString(2, lastName);
            pstmt.setString(3, email);
            pstmt.setString(4, company);
            pstmt.setString(5, jobrole);
            pstmt.setString(6, tshirtsize);
            pstmt.setString(7, comments);
            pstmt.setString(8, playerName);
            pstmt.executeUpdate();
        } catch (SQLException e) {
            System.out.println("PodsOfKonController.setPlayerNamesAndIds SQLException:" + e);
        }
        return "<html>Successfully recorded player info.  Thank You!<br><br></html>";
    }


    @GetMapping("/questions")
    public String questions() throws Exception {
        System.out.println("PodsOfKonController.questions...");
  //      log.debug("questions datasource:" + datasource + "...");
        BonusRound quiz = new BonusRound();
        List<Question> questionsList = new ArrayList<>();
        try (Connection connection = datasource.getConnection();
             Statement statement = connection.createStatement();
             ResultSet resultSet = statement.executeQuery(questionsquery)) {

            while (resultSet.next()) {
                Question question = new Question();
                String questionText = resultSet.getString("question");
                question.setText(questionText);

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
            System.out.println("PodsOfKonController.questions SQLException:" + e);
        }
        ObjectMapper objectMapper = new ObjectMapper();
        try {
            String jsonString = objectMapper.writeValueAsString(quiz);
            System.out.println("success length of questions:" + jsonString.length());
            return jsonString;
        } catch (Exception e) {
            e.printStackTrace();
            return "question query failed e:" + e;
        }
    }

    @GetMapping("/createTables")
    public String createTables() throws Exception {
        log.debug("createTables for datasource:" + datasource + "...");

        try (Connection conn = datasource.getConnection()) {
            conn.createStatement().execute("create table currentgame( playername varchar(256), score number(10)  )");
            conn.createStatement().execute("insert into currentgame values ( 'player1', 0  )");
            conn.createStatement().execute("insert into currentgame values ( 'player2', 0  )");
            conn.createStatement().execute("create table scores( playername varchar(256), score number(10) )");
            String returnString = "createTables success";
            return returnString;
        } catch (Exception e) {
            return "Exception occurred during create tables:" + e;
        }
    }

    @GetMapping("/createDeployment")
    public String createDeployment(@RequestParam("appName") String appName, @RequestParam("serviceName") String serviceName) {
        log.debug("create deployment and appName  = " + appName);
        log.debug("create deployment and service  = " + serviceName);
        try {
            new ApplyDeployment().createDeployment(appName, serviceName);
        } catch (Exception e) {
            return "Exception occurred during create deployment operation (perhaps dupe):" + e;
        }
        return "create deployment and service  = " + serviceName + " successful";
    }


    @GetMapping("/movescores")
    public String movescores(@RequestParam("player1Name") String player1Name, @RequestParam("player1Score") int player1Score,
                             @RequestParam("player2Name") String player2Name, @RequestParam("player2Score") int player2Score) throws Exception {
        try (Connection conn = datasource.getConnection();
             PreparedStatement insertFinalScore = conn.prepareStatement(
                     "INSERT INTO scores (playername, score) VALUES (?, ?)")) {
            //insert scores
            insertFinalScore.setString(1, player1Name);
            insertFinalScore.setInt(2, player1Score);
            insertFinalScore.executeUpdate();
            insertFinalScore.setString(1, player2Name);
            insertFinalScore.setInt(2, player2Score);
            insertFinalScore.executeUpdate();
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
        } catch (SQLException ex) {
            System.out.println("movescores SQLException:" + ex);
            return "movescores failed";
        }
    }

    @GetMapping("/getPlayerName")
    public String getPlayerNamesAndIds(@RequestParam("playerName") String playerName) throws Exception {
        return playerName.equals("player1") ? player1Name : player2Name;

    }

    @GetMapping("/updateScores")
    public String updateScores(@RequestParam("player1Score") int player1Score,
                               @RequestParam("player2Score") int player2Score) throws Exception {
        log.debug("updateScores player1Score:" + player1Score + "...");
        updateScore("player1", player1Score);
        log.debug("updateScores player2Score:" + player2Score + "...");
        updateScore("player2", player2Score);
        return "updateScores success";
    }

    private void updateScore(String playerName, int amount) {
        try (Connection conn = datasource.getConnection();
             PreparedStatement preparedStatementUpdateScore = conn.prepareStatement(
                     "UPDATE currentgame SET score=? WHERE playername=?")) {
            preparedStatementUpdateScore.setInt(1, amount);
            preparedStatementUpdateScore.setString(2, playerName);
            preparedStatementUpdateScore.execute();
        } catch (Exception ex) {
            System.out.println("PodsOfKonController.updateScore ex:" + ex);
        }
    }

    private void clearScore(String playerName) throws SQLException {

        try (Connection conn = datasource.getConnection();
             PreparedStatement preparedStatementClearScore = conn.prepareStatement(
                     "UPDATE currentgame SET score=0 WHERE playername=?")) {
            preparedStatementClearScore.setString(1, playerName);
            preparedStatementClearScore.execute();
        } catch (Exception ex) {
            System.out.println("PodsOfKonController.clearScore ex:" + ex);
        }
    }

    @GetMapping("/deleteDeployment")
    public String deleteDeployment(@RequestParam("appName") String appName, @RequestParam("serviceName") String serviceName) {
        System.out.println("deleteDeployment appName = " + appName + ", serviceName = " + serviceName);
        try {
            return new DeleteDeployment().deleteDeployment(appName, serviceName);
        } catch (Exception e) {
            return "Exception occurred during delete operation:" + e.getMessage();
        }
    }
}

