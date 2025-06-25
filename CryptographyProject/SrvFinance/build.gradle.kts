plugins {
    id("java")
    id("application")
}
application {
    // Đặt tên đầy đủ của class có hàm main, ví dụ:
    mainClass.set("com.sn.SrvFinance.server.ServerManager")
}
group = "com.sn"
version = "1.0-SNAPSHOT"

repositories {
    mavenCentral()
}

dependencies {
    testImplementation(platform("org.junit:junit-bom:5.10.0"))
    testImplementation("org.junit.jupiter:junit-jupiter")
    compileOnly("org.projectlombok:lombok:1.18.38")
    annotationProcessor("org.projectlombok:lombok:1.18.38")
    implementation("com.fasterxml.jackson.core:jackson-databind:2.18.2")
    implementation("com.zaxxer:HikariCP:6.2.1")
    implementation("org.testcontainers:jdbc:1.20.5")
    implementation("mysql:mysql-connector-java:8.0.33")
    implementation ("org.apache.commons:commons-lang3:3.12.0")
    implementation("com.googlecode.json-simple:json-simple:1.1.1")
    implementation("ch.qos.logback:logback-classic:1.4.14")
    implementation("org.mindrot:jbcrypt:0.4")
}

sourceSets {
    main {
        java.setSrcDirs(listOf("src/main/java"))
    }
 
}
tasks.test {
    useJUnitPlatform()
 testLogging {
        events("passed", "skipped", "failed")
    }
}