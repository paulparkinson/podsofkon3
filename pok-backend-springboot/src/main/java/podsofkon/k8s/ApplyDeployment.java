package podsofkon.k8s;

import io.kubernetes.client.openapi.ApiClient;
import io.kubernetes.client.openapi.apis.AppsV1Api;
import io.kubernetes.client.openapi.models.V1Deployment;
import io.kubernetes.client.util.Config;
import io.kubernetes.client.util.Yaml;
import okhttp3.OkHttpClient;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.util.concurrent.TimeUnit;

public class ApplyDeployment {
    private static final Logger log = LoggerFactory.getLogger(ApplyDeployment.class);

    public void  createDeployment(String appName, String serviceName)
            throws Exception {

        ApiClient client = Config.defaultClient();
   //     HttpLoggingInterceptor interceptor = new HttpLoggingInterceptor(message -> log.info(message));
   //     interceptor.setLevel(HttpLoggingInterceptor.Level.BODY);
        OkHttpClient newClient = client.getHttpClient()
                .newBuilder()
      //          .addInterceptor(interceptor)
                .readTimeout(0, TimeUnit.SECONDS)
                .build();
        client.setHttpClient(newClient);
        AppsV1Api apiapps = new AppsV1Api(client);
        Yaml yaml = new Yaml();
        String deploymentyaml = "apiVersion: apps/v1\n" +
                "kind: Deployment\n" +
                "metadata:\n" +
                "  name: " + serviceName + "-deployment\n" +
//                "  namespace: player1\n" +
                "spec:\n" +
                "  selector:\n" +
                "    matchLabels:\n" +
                "      app: nginx\n" +
                "  replicas: 1\n" +
                "  template:\n" +
                "    metadata:\n" +
                "      labels:\n" +
                "        app: nginx\n" +
                "    spec:\n" +
                "      containers:\n" +
                "      - name: nginx\n" +
                "        image: nginx:1.14.2\n" +
                "        ports:\n" +
                "        - containerPort: 80\n";
        log.debug("deployment create for yaml..." + deploymentyaml);
        V1Deployment v1Deployment = yaml.loadAs(deploymentyaml, V1Deployment.class);
        String namespace = appName;
        apiapps.createNamespacedDeployment(
                namespace, v1Deployment, null, null, null, null);
    }
//    public void  createDeployment(String isRedeploy, String appName, String serviceName, String imageVersion)
//            throws Exception {
//
//        ApiClient client = Config.defaultClient();
//        HttpLoggingInterceptor interceptor = new HttpLoggingInterceptor(message -> log.info(message));
//        interceptor.setLevel(HttpLoggingInterceptor.Level.BODY);
//        OkHttpClient newClient = client.getHttpClient()
//                .newBuilder()
//                .addInterceptor(interceptor)
//                .readTimeout(0, TimeUnit.SECONDS)
//                .build();
//        client.setHttpClient(newClient);
//
//        if (isRedeploy.equalsIgnoreCase("true")) {
//            System.out.println("Redeploying...");
//            CoreV1Api api = new CoreV1Api(client);
//            V1PodList podsForNamespace = ListInfoForNamespace.getPodsForNamespace(appName);
//            log.debug("deleting any pods for redeployment of " + appName + "/" + serviceName);
//            for (V1Pod v1Pod : podsForNamespace.getItems()) {
//                String podName = v1Pod.getMetadata().getName();
//                if (podName.indexOf(serviceName) > -1) {
//                    log.debug("deleting pod " + podName);
//                    //todo this is a bug if another service contains same name, use getAnnotations or getLabels instead
//                    api.deleteNamespacedPod(podName, appName,
//                            null, null, null, null,
//                            null, null);
//                    log.debug("deleted pod " + podName);
//                }
//            }
//            log.debug("deleted any pods for redeployment of " + appName + "/" + serviceName);
//        } else {
//            AppsV1Api apiapps = new AppsV1Api(client);
//            Yaml yaml = new Yaml();
//            String deploymentName = serviceName;
//            String app = serviceName;
//            String image = appName + "-" + serviceName + ":" + imageVersion;
//            String dataSourceEnv = "";
//            if(XRController.springBindingPrefix != null &&
//                    !XRController.springBindingPrefix.trim().equals("")) {
//                dataSourceEnv =
//                        "          - name: " + XRController.springBindingPrefix + ".username\n" +
//                        "            valueFrom:\n" +
//                        "              secretKeyRef:\n" +
//                        "                name: \"" + serviceName +"-db-secrets\"\n" +
//                        "                key: db.username\n" +
//                        "          - name: " + XRController.springBindingPrefix + ".password\n" +
//                        "            valueFrom:\n" +
//                        "              secretKeyRef:\n" +
//                        "                name: \"" + serviceName +"-db-secrets\"\n" +
//                        "                key: db.password\n" +
//                        "          - name: DB_SERVICE\n" +
//                        "            valueFrom:\n" +
//                        "              secretKeyRef:\n" +
//                        "                name: \"" + serviceName +"-db-secrets\"\n" +
//                        "                key: db.service\n" +
//                        "          - name: " + XRController.springBindingPrefix + ".url\n" +
//                        "            value: jdbc:oracle:thin:@$(DB_SERVICE)?TNS_ADMIN=/oracle/tnsadmin\n" +
//                        "          - name: CONNECT_STRING\n" +
//                        "            value: jdbc:oracle:thin:@$(DB_SERVICE)?TNS_ADMIN=/oracle/tnsadmin\n";
//            }
//            String deploymentyaml =
//                    "apiVersion: apps/v1\n" +
//                            "kind: Deployment\n" +
//                            "metadata:\n" +
//                            "  name: " + deploymentName + "\n" +
//                            "spec:\n" +
//                            "  replicas: 1\n" +
//                            "  selector:\n" +
//                            "    matchLabels:\n" +
//                            "      app: " + app + "\n" +
//                            "  template:\n" +
//                            "    metadata:\n" +
//                            "      labels:\n" +
//                            "        app: " + app + "\n" +
//                            "    spec:\n" +
//                            "      containers:\n" +
//                            "      - name: " + app + "\n" +
//                            "        image: " + XRController.registryUrl + "/" + image + "\n" +
//                            "        imagePullPolicy: Always\n" +
//                            "        env:\n" +
//                            "        env:\n" +
//                            "          - name: eureka.instance.preferIpAddress\n" +
//                            "            value: true\n" +
//                            "          - name: eureka.instance.hostname\n" +
//                            "            value: " + serviceName + "." + appName + "\n" +
//                            "          - name: eureka.client.registerWithEureka\n" +
//                            "            value: true\n" +
//                            "          - name: eureka.client.fetchRegistry\n" +
//                            "            value: true\n" +
//                            "          - name: eureka.client.serviceUrl.defaultZone\n" +
//                            "            value: http://eureka.eureka:8761/eureka\n" +
//                            dataSourceEnv +
//                            "        ports:\n" +
//                            "        - containerPort: 8080\n" +
//                            "      imagePullSecrets:\n" +
//                            "        - name: registry-auth\n";
//            log.debug("deployment create for yaml..." + deploymentyaml);
//            V1Deployment v1Deployment = yaml.loadAs(deploymentyaml, V1Deployment.class);
//            String namespace = appName;
//            apiapps.createNamespacedDeployment(
//                    namespace, v1Deployment, null, null, null, null);
//            log.debug("deployment created");
//        }
//    }
}
