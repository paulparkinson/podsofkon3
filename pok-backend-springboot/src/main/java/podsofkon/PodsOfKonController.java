Decompiler.com
JAR String Editor
Release Notes
support@decompiler.com
Secure critical workloads. Build reliable, private networking across any cloud. Try Tailscale free.
Ads by EthicalAds
PodsOfKonController.java
PodsOfKonController.java
Download file
    package xrservice;

import com.fasterxml.jackson.databind.ObjectMapper;
import java.io.File;
import java.sql.Connection;
import java.sql.DatabaseMetaData;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.Statement;
import java.util.ArrayList;
import java.util.Date;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.Timer;
import java.util.TimerTask;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;
import javax.sql.DataSource;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;
import xrservice.PodsOfKonController.1;
        import xrservice.cli.Microservice;
import xrservice.cli.MicroserviceDetails;
import xrservice.container.ContainerOperations;
import xrservice.k8s.ApplyDeployment;
import xrservice.k8s.ApplySecret;
import xrservice.k8s.DeleteDeployment;
import xrservice.k8s.DeleteNamespace;
import xrservice.messaging.OracleAQConfiguration;
import xrservice.oci.CreateContainerRepos;
import xrservice.upload.storage.StorageService;

@RestController
@RequestMapping({"/podsofkon"})
public class PodsOfKonController {
    private static Logger log = LoggerFactory.getLogger(PodsOfKonController.class);
    public static String registryUrl = System.getenv("registry.url");
    private final StorageService storageService;
    public static Map<Microservice, MicroserviceDetails> microserviceDetailsMap = new HashMap();
    public static String springBindingPrefix;
    public static String schemaName;
    private static Connection conn;
    private static PreparedStatement preparedStatementIncrementScore;
    private static PreparedStatement preparedStatementUpdateScore;
    private static PreparedStatement preparedStatementClearScore;
    private static PreparedStatement insertFinalScore;
    @Autowired
    DataSource datasource;
    String query = "SELECT * FROM ORDERUSER.QANDA";
    String player1Name = "steelix";
    String player2Name = "umbreon";
    private static final String[] HEADERS_TO_TRY = new String[]{"X-Forwarded-For", "Proxy-Client-IP", "WL-Proxy-Client-IP", "HTTP_X_FORWARDED_FOR", "HTTP_X_FORWARDED", "HTTP_X_CLUSTER_CLIENT_IP", "HTTP_CLIENT_IP", "HTTP_FORWARDED_FOR", "HTTP_FORWARDED", "HTTP_VIA", "REMOTE_ADDR"};

    @Autowired
    public PodsOfKonController(StorageService storageService) {
        this.storageService = storageService;
    }

    @GetMapping({"/questions"})
    public String questions() throws Exception {
        log.debug("questions datasource:" + this.datasource + "...");
        BonusRound quiz = new BonusRound();
        ArrayList questionsList = new ArrayList();

        try {
            Connection connection = this.datasource.getConnection();

            try {
                Statement statement = connection.createStatement();

                try {
                    ResultSet resultSet = statement.executeQuery(this.query);

                    while(true) {
                        if (!resultSet.next()) {
                            quiz.setQuestions(questionsList);
                            break;
                        }

                        Question question = new Question();
                        String question1 = resultSet.getString("question");
                        question.setText(question1);
                        List<Answer> answers = new ArrayList();

                        for(int i = 1; i <= 5; ++i) {
                            String answerText = resultSet.getString("answer" + i);
                            String isCorrect = resultSet.getString("answer" + i + "IsCorrect");
                            if (answerText != null) {
                                answers.add(new Answer(answerText, isCorrect));
                            }
                        }

                        question.setAnswers(answers);
                        questionsList.add(question);
                    }
                } catch (Throwable var15) {
                    if (statement != null) {
                        try {
                            statement.close();
                        } catch (Throwable var14) {
                            var15.addSuppressed(var14);
                        }
                    }

                    throw var15;
                }

                if (statement != null) {
                    statement.close();
                }
            } catch (Throwable var16) {
                if (connection != null) {
                    try {
                        connection.close();
                    } catch (Throwable var13) {
                        var16.addSuppressed(var13);
                    }
                }

                throw var16;
            }

            if (connection != null) {
                connection.close();
            }
        } catch (SQLException var17) {
            var17.printStackTrace();
        }

        ObjectMapper objectMapper = new ObjectMapper();

        try {
            String jsonString = objectMapper.writeValueAsString(quiz);
            System.out.println(jsonString);
            return jsonString;
        } catch (Exception var12) {
            var12.printStackTrace();
            return "question query failed e:" + var12;
        }
    }

    @GetMapping({"/createTables"})
    public String createTables() throws Exception {
        log.debug("createTables for datasource:" + this.datasource + "...");
        this.initConn();
        conn.createStatement().execute("create table currentgame( playername varchar(256), score number(10)  )");
        conn.createStatement().execute("insert into currentgame values ( 'player1', 0  )");
        conn.createStatement().execute("insert into currentgame values ( 'player2', 0  )");
        conn.createStatement().execute("create table scores( playername varchar(256), score number(10) )");
        String returnString = "createTables success";
        return returnString;
    }

    @GetMapping({"/createDeployment"})
    public String createDeployment(@RequestParam("appName") String appName, @RequestParam("serviceName") String serviceName) {
        log.debug("create deployment and appName  = " + appName);
        log.debug("create deployment and service  = " + serviceName);

        try {
            (new ApplyDeployment()).createDeployment(appName, serviceName);
        } catch (Exception var4) {
            return "Exception occurred during create deployment operation (perhaps dupe):" + var4;
        }

        return "create deployment and service  = " + serviceName + " successful";
    }

    @GetMapping({"/movescores0"})
    public String movescores0(@RequestParam("player1Name") String player1Name, @RequestParam("player2Name") String player2Name) throws Exception {
        System.out.println("movescores for datasource:" + this.datasource + "...");
        this.initConn();
        conn.createStatement().execute("insert into scores( playername , score ) select '" + this.player1Name + "', score from currentgame where playername='player1'");
        conn.createStatement().execute("insert into scores( playername , score ) select '" + this.player2Name + "', score from currentgame where playername='player2'");
        System.out.println("movescores for datasource:" + this.datasource + "...");
        this.clearScore("player1");
        this.clearScore("player2");
        return "movescores success";
    }

    @GetMapping({"/movescores"})
    public String movescores(@RequestParam("player1Name") String player1Name, @RequestParam("player1Score") int player1Score, @RequestParam("player2Name") String player2Name, @RequestParam("player2Score") int player2Score) throws Exception {
        System.out.println("movescores for datasource:" + this.datasource + "...");
        this.initConn();
        insertFinalScore.setString(1, player1Name);
        insertFinalScore.setInt(2, player1Score);
        insertFinalScore.execute();
        insertFinalScore.setString(1, player2Name);
        insertFinalScore.setInt(2, player2Score);
        insertFinalScore.execute();
        this.clearScore("player1");
        this.clearScore("player2");
        System.out.println("deleteDeployments for player1...");
        this.createDeployment("player1", "database");
        this.deleteDeployment("player1", "javascript-deployment");
        this.createDeployment("player1", "graalvm");
        this.deleteDeployment("player1", "rust-deployment");
        this.deleteDeployment("player1", "go-deployment");
        this.deleteDeployment("player1", "python-deployment");
        this.deleteDeployment("player1", "dotnet-deployment");
        this.createDeployment("player1", "springboot");
        System.out.println("deleteDeployments for player2...");
        this.createDeployment("player2", "database");
        this.deleteDeployment("player2", "javascript-deployment");
        this.createDeployment("player2", "graalvm");
        this.deleteDeployment("player2", "rust-deployment");
        this.deleteDeployment("player2", "go-deployment");
        this.deleteDeployment("player2", "python-deployment");
        this.deleteDeployment("player2", "dotnet-deployment");
        this.createDeployment("player2", "springboot");
        return "movescores success";
    }

    @GetMapping({"/form"})
    public String form() {
        return "                <html><head><meta charset=\"UTF-8\">\n    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\"></head><form a method=\"post\" action=\"/podsofkon/setPlayerNamesAndIds\" >  Enter Either Player 1 or Player 2 Name...<br>        <div>            <label for=\"player1Name\">Player 1 Name:</label>            <input type=\"text\" id=\"player1Name\" name=\"player1Name\" autocomplete=\"player1Name\">        </div>        <div>            <label for=\"player2Name\">Player 2 Name:</label>            <input type=\"text\" id=\"player2Name\" name=\"player2Name\" autocomplete=\"player2Name\">        </div>        <div>            <label for=\"firstName\">First Name:</label>            <input type=\"text\" id=\"firstName\" name=\"firstName\" autocomplete=\"given-name\">        </div>        <div>            <label for=\"lastName\">Last Name:</label>            <input type=\"text\" id=\"lastName\" name=\"lastName\" autocomplete=\"family-name\">        </div>        <div>            <label for=\"email\">Email:</label>            <input type=\"email\" id=\"email\" name=\"email\" autocomplete=\"email\">        </div>        <div>            <label for=\"company\">Company:</label>            <input type=\"company\" id=\"company\" name=\"company\" autocomplete=\"company\">        </div>        <div>            <label for=\"jobrole\">Job Role:</label>            <input type=\"jobrole\" id=\"jobrole\" name=\"jobrole\" autocomplete=\"jobrole\">        </div>        <div>            <label for=\"tshirtsize\">T-Shirt Size:</label>            <input type=\"tshirtsize\" id=\"tshirtsize\" name=\"tshirtsize\" autocomplete=\"tshirtsize\">        </div>        <div>            <label for=\"comments\">Comments:</label>            <input type=\"comments\" id=\"comments\" name=\"comments\" autocomplete=\"comments\">        </div>        <div>            <input type=\"submit\" value=\"Submit\">        </div>                </form></html>";
    }

    @PostMapping({"/setPlayerNamesAndIds"})
    public String setPlayerNamesAndIds(HttpServletRequest request, HttpServletResponse response, @RequestParam(name = "isRegistered",required = false) boolean isRegistered, @RequestParam("player1Name") String player1Name, @RequestParam("player2Name") String player2Name, @RequestParam("firstName") String firstName, @RequestParam("lastName") String lastName, @RequestParam("email") String email, @RequestParam("company") String company, @RequestParam("jobrole") String jobrole, @RequestParam("tshirtsize") String tshirtsize, @RequestParam("comments") String comments) throws Exception {
        if (!player1Name.trim().equals("")) {
            this.player1Name = player1Name;
        }

        if (!player2Name.trim().equals("")) {
            this.player2Name = player2Name;
        }

        this.initConn();
        conn.createStatement().execute("insert into playerinfo values ('" + firstName + "', '" + lastName + "', '" + email + "', '" + company + "', '" + jobrole + "', '" + tshirtsize + "', '" + comments + "' )");
        return "<html>Successfully updated player name.  Thanks!<br><br></html>";
    }

    @GetMapping({"/getPlayerName"})
    public String getPlayerNamesAndIds(@RequestParam("playerName") String playerName) throws Exception {
        return playerName.equals("player1") ? this.player1Name : this.player2Name;
    }

    @GetMapping({"/nameupdatedsuccess"})
    public String nameupdatedsuccess() throws Exception {
        return "<html>Successfully updated player name.  Thanks!<br><br>If this is your first time playing, please tell us just a little about yourself, <a href=\"https://wkrfs4xeqva1jcu-indadw.adb.us-phoenix-1.oraclecloudapps.com/ords/r/demouserws/contactinformation/crm2det\">click here</a></html>";
    }

    @GetMapping({"/incrementScore"})
    public String incrementScore(@RequestParam("playerName") String playerName, @RequestParam("amount") int amount) throws Exception {
        log.debug("incrementScore for playerName:" + playerName + "...");
        log.debug("incrementScore for datasource:" + this.datasource + "...");
        this.updateScore(playerName, amount);
        return "incrementScore success";
    }

    @GetMapping({"/updateScores"})
    public String updateScores(@RequestParam("player1Score") int player1Score, @RequestParam("player2Score") int player2Score) throws Exception {
        log.debug("updateScores player1Score:" + player1Score + "...");
        log.debug("updateScores player2Score:" + player2Score + "...");
        this.updateScore("player1", player1Score);
        this.updateScore("player2", player2Score);
        return "updateScores success";
    }

    private void updateScore(String playerName, int amount) throws SQLException {
        this.initConn();

        try {
            preparedStatementUpdateScore.setInt(1, amount);
            preparedStatementUpdateScore.setString(2, playerName);
            preparedStatementUpdateScore.execute();
        } catch (Exception var4) {
            var4.printStackTrace();
        }

    }

    private void increment(String playerName, int amount) throws SQLException {
        this.initConn();

        try {
            preparedStatementIncrementScore.setInt(1, amount);
            preparedStatementIncrementScore.setString(2, playerName);
            preparedStatementIncrementScore.execute();
        } catch (Exception var4) {
            var4.printStackTrace();
        }

    }

    private void clearScore(String playerName) throws SQLException {
        this.initConn();

        try {
            preparedStatementClearScore.setString(1, playerName);
            preparedStatementClearScore.execute();
        } catch (Exception var3) {
            var3.printStackTrace();
        }

    }

    private void initConn() throws SQLException {
        if (conn == null || preparedStatementIncrementScore == null) {
            conn = this.datasource.getConnection();
            preparedStatementIncrementScore = conn.prepareStatement("UPDATE currentgame SET score=score+? WHERE playername=?");
            preparedStatementUpdateScore = conn.prepareStatement("UPDATE currentgame SET score=? WHERE playername=?");
            preparedStatementClearScore = conn.prepareStatement("UPDATE currentgame SET score=0 WHERE playername=?");
            insertFinalScore = conn.prepareStatement("insert into scores values ( ?, ? )");
        }

    }

    @GetMapping({"/deleteApp"})
    public String delete(@RequestParam("appName") String appName) throws Exception {
        log.debug("delete application/namespace...");
        String returnString = "";

        try {
            returnString = returnString + (new DeleteNamespace()).deleteNamespace(appName);
            return returnString;
        } catch (Throwable var4) {
            return "Exception occurred during delete operation:" + var4.getMessage();
        }
    }

    @GetMapping({"/deleteDeployment"})
    public String deleteDeployment(@RequestParam("appName") String appName, @RequestParam("serviceName") String serviceName) {
        System.out.println("deleteDeployment appName = " + appName + ", serviceName = " + serviceName);

        try {
            return (new DeleteDeployment()).deleteDeployment(appName, serviceName);
        } catch (Exception var4) {
            return "Exception occurred during delete operation:" + var4.getMessage();
        }
    }

    private String getClientIpAddress(HttpServletRequest request) {
        String[] var2 = HEADERS_TO_TRY;
        int var3 = var2.length;

        for(int var4 = 0; var4 < var3; ++var4) {
            String header = var2[var4];
            String ip = request.getHeader(header);
            if (ip != null && ip.length() != 0 && !"unknown".equalsIgnoreCase(ip)) {
                return ip;
            }
        }

        return request.getRemoteAddr();
    }

    @PostMapping({"/db"})
    public String db(@RequestParam("key") String key, @RequestParam("value") String value) throws Exception {
        log.info("db key = " + key + ", value = " + value);
        if (!key.equalsIgnoreCase("deRez") && !key.equalsIgnoreCase("deRezConfig")) {
            throw new Exception("invalid key");
        } else {
            Connection connection = (new OracleAQConfiguration()).dataSource().getConnection();
            DatabaseMetaData meta = connection.getMetaData();
            ResultSet res = meta.getTables((String)null, (String)null, (String)null, new String[]{"TABLE"});

            String returnString;
            for(returnString = "tables... "; res.next(); System.out.println(returnString)) {
                String table_name = res.getString("TABLE_NAME");
                if (table_name.equalsIgnoreCase("properties")) {
                    returnString = returnString + table_name + " ";
                }
            }

            return returnString;
        }
    }

    @GetMapping({"/imageBuildAndPush"})
    public String dockerBuildAndPush(@RequestParam("appName") String appName, @RequestParam("serviceName") String serviceName, @RequestParam("imageVersion") String imageVersion, @RequestParam("javaVersion") String javaVersion) throws Exception {
        log.debug("docker build and push appName:" + appName + " serviceName:" + serviceName + " imageVersion:" + imageVersion + " javaVersion:" + javaVersion);
        File baseDir = this.storageService.getRootLocation().toFile();
        (new ContainerOperations()).buildAndPush(baseDir, appName, serviceName, imageVersion, javaVersion);
        this.deleteDirectory(baseDir);
        return "docker build and push successful";
    }

    void deleteDirectory(File directoryToBeDeleted) {
        File[] allContents = directoryToBeDeleted.listFiles();
        if (allContents != null) {
            File[] var3 = allContents;
            int var4 = allContents.length;

            for(int var5 = 0; var5 < var4; ++var5) {
                File file = var3[var5];
                file.delete();
            }
        }

    }

    @GetMapping({"/updateOracleSpringAdminSecret"})
    public String updateOracleSpringAdminSecret(@RequestParam("password") String password) throws Exception {
        String returnString = "";
        log.debug("updateOracleSpringAdminSecret...");

        try {
            returnString = returnString + (new ApplySecret()).updateOracleSpringAdminSecret(password);
        } catch (Exception var4) {
            var4.printStackTrace();
            return "Exception occurred during updateOracleSpringAdminSecret operation:" + var4;
        }

        log.debug("restartAdminServer...");
        TimerTask exitApp = new 1(this);
        (new Timer()).schedule(exitApp, new Date(System.currentTimeMillis() + 5000L));
        return returnString;
    }

    @GetMapping({"/createRepos"})
    public String createRepos(@RequestParam("compartmentid") String compartmentid, @RequestParam("reposName") String reposName) throws Exception {
        log.debug("updateOracleSpringAdminSecret...");

        try {
            return (new CreateContainerRepos()).createReposForImage(compartmentid, reposName);
        } catch (Exception var4) {
            var4.printStackTrace();
            return "Exception occurred during updateOracleSpringAdminSecret operation:" + var4;
        }
    }
}