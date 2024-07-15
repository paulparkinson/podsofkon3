FROM --platform=linux/amd64 alpine:latest AS build

# Install necessary packages
RUN apk update && apk add --no-cache \
    openjdk17 \
    maven \
    git

# Set the working directory
WORKDIR /app

# Copy the application source code to the container
COPY . /app

# Build the application using Maven
RUN mvn clean install




FROM --platform=linux/amd64 alpine:latest
# Install necessary packages for runtime
RUN apk update && apk add --no-cache openjdk17

# Set the working directory
WORKDIR /app

# Copy the built application from the build stage
COPY --from=build /app/target/podsofkon-0.0.1-SNAPSHOT.jar .

# Run the application
CMD ["java", "-jar", "podsofkon-0.0.1-SNAPSHOT.jar"]
