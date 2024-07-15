# FROM adoptopenjdk:11-jdk-hotspot
# FROM openjdk:11-jre-slim
FROM openjdk:17-oracle

ADD target/podsofkon-0.0.1-SNAPSHOT.jar /usr/share/podsofkon/podsofkon.jar
ENTRYPOINT ["java", "-jar", "/usr/share/podsofkon/podsofkon.jar"]
