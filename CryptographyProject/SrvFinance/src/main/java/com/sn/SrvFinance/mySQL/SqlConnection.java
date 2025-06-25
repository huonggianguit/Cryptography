package com.sn.SrvFinance.mySQL;

import com.zaxxer.hikari.HikariConfig;
import com.zaxxer.hikari.HikariDataSource;
import org.slf4j.LoggerFactory;

import java.io.InputStream;
import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.ArrayList;
import java.util.List;
import java.util.Map;
import java.util.Properties;
import org.slf4j.Logger;

public class SqlConnection {
    private static final Logger logger;
    private static String DRIVER;
    private static String URL = "jdbc:mysql://%s:%s/%s?useUnicode=yes&characterEncoding=UTF-8";
    private static String DB_HOST;
    private static String DB_PORT;
    private static String DB_NAME;
    private static String DB_USER;
    private static String DB_PASSWORD;
    private static int MIN_CONN;
    private static int MAX_CONN;
    private static long MAX_LIFE_TIME = 120000L;
    public static boolean LOG_QUERY = false;
    private static HikariConfig config = new HikariConfig();
    private static HikariDataSource ds;

    static {
        logger = LoggerFactory.getLogger(SqlConnection.class);
        loadProperties();

        config.setDriverClassName(DRIVER);
        config.setJdbcUrl(String.format(URL, DB_HOST, DB_PORT, DB_NAME));
        config.setUsername(DB_USER);
        config.setPassword(DB_PASSWORD);
        config.setMinimumIdle(MIN_CONN);
        config.setMaximumPoolSize(MAX_CONN);
        config.setMaxLifetime(MAX_LIFE_TIME);
        config.addDataSourceProperty("cachePrepStmts", "true");
        config.addDataSourceProperty("prepStmtCacheSize", "250");
        config.addDataSourceProperty("prepStmtCacheSqlLimit", "2048");
        config.addDataSourceProperty("useServerPrepStmts", "true");
        config.addDataSourceProperty("useLocalSessionState", "true");
        config.addDataSourceProperty("rewriteBatchedStatements", "true");
        config.addDataSourceProperty("cacheResultSetMetadata", "true");
        config.addDataSourceProperty("cacheServerConfiguration", "true");
        config.addDataSourceProperty("elideSetAutoCommits", "true");
        config.addDataSourceProperty("maintainTimeStats", "true");
        ds = new HikariDataSource(config);
    }

    private static void loadProperties() {
        Properties properties = new Properties();
        try (InputStream inputStream = SqlConnection.class.getClassLoader().getResourceAsStream("database.properties")) {
            if (inputStream != null) {
                properties.load(inputStream);
            } else {
                throw new RuntimeException("File not found: database.properties");
            }
            Object value;
            if ((value = properties.get("driver")) != null) {
                DRIVER = String.valueOf(value);
            }
            if ((value = properties.get("host")) != null) {
                DB_HOST = String.valueOf(value);
            }
            if ((value = properties.get("port")) != null) {
                DB_PORT = String.valueOf(value);
            }
            if ((value = properties.get("name")) != null) {
                DB_NAME = String.valueOf(value);
            }
            if ((value = properties.get("user")) != null) {
                DB_USER = String.valueOf(value);
            }
            if ((value = properties.get("pass")) != null) {
                DB_PASSWORD = String.valueOf(value);
            }
            if ((value = properties.get("min")) != null) {
                MIN_CONN = Integer.parseInt(String.valueOf(value));
            }
            if ((value = properties.get("max")) != null) {
                MAX_CONN = Integer.parseInt(String.valueOf(value));
            }
            if ((value = properties.get("lifetime")) != null) {
                MAX_LIFE_TIME = Integer.parseInt(String.valueOf(value));
            }
            if ((value = properties.get("log")) != null) {
                LOG_QUERY = Boolean.parseBoolean(String.valueOf(value));
            }
           // logger.info("Load file properites thành công!");
        } catch (Exception ex) {
           // logger.error(ex.getMessage());
        } finally {
            properties.clear();
            properties = null;
        }
    }

    public static Connection getConnection() throws SQLException {
        return ds.getConnection();
    }

    public static void close() {
        ds.close();
    }

    private static PreparedStatement prepareScrollableStatement(Connection con, String query) throws SQLException {
        return con.prepareStatement(query, ResultSet.TYPE_SCROLL_INSENSITIVE, ResultSet.CONCUR_READ_ONLY);
    }

    public static SqlResultSet executeQuery(String query) throws Exception {
        try (Connection con = getConnection();
             PreparedStatement ps = con.prepareStatement(query, ResultSet.TYPE_SCROLL_INSENSITIVE, ResultSet.CONCUR_READ_ONLY);
             ResultSet rs = ps.executeQuery()) {
            if (LOG_QUERY) {
                logger.info("Thực thi thành công câu lệnh: " + ps.toString());
                //   Log.gI().log(ps.toString());
            }
            return (SqlResultSet) new SqlResultSet(rs);
        } catch (Exception ex) {
            logger.error("Có lỗi xảy ra khi thực thi câu lệnh: " + query);
            throw ex;
        }
    }

    public static SqlResultSet executeQuery(String query, Object... objs) throws Exception {
        try (Connection con = getConnection();
             PreparedStatement ps = con.prepareStatement(query, ResultSet.TYPE_SCROLL_INSENSITIVE, ResultSet.CONCUR_READ_ONLY)) {
            for (int i = 0; i < objs.length; i++) {
                ps.setObject(i + 1, objs[i]);
            }
            if (LOG_QUERY) {
               // logger.info("Thực thi thành công câu lệnh: " + ps.toString());
                //    Log.gI().log(ps.toString());
            }
            return (SqlResultSet) new SqlResultSet(ps.executeQuery());
        } catch (Exception ex) {
          //  logger.error("Có lỗi xảy ra khi thực thi câu lệnh: " + query);
            throw ex;
        }
    }

    public static int executeUpdate(String query) throws Exception {
        int rowUpdated = -1;
        try (Connection con = getConnection();
             PreparedStatement ps = con.prepareStatement(query, ResultSet.TYPE_SCROLL_INSENSITIVE, ResultSet.CONCUR_READ_ONLY)) {
            if (LOG_QUERY) {
              //  logger.info("Thực thi thành công câu lệnh: " + ps.toString());
                //   Log.gI().log(ps.toString());
            }
            rowUpdated = ps.executeUpdate();
        } catch (Exception e) {
            //logger.error("Có lỗi xảy ra khi thực thi câu lệnh: " + query);
            throw e;
        }
        return rowUpdated;
    }

    public static int executeUpdate(String query, Object... objs) throws Exception {
        int rowUpdated = -1;
        try (Connection con = getConnection();
             PreparedStatement ps = con.prepareStatement(query, ResultSet.TYPE_SCROLL_INSENSITIVE, ResultSet.CONCUR_READ_ONLY)) {

            // Gán các giá trị vào các placeher của PreparedStatement
            for (int i = 0; i < objs.length; i++) {
                ps.setObject(i + 1, objs[i]);
            }

            if (LOG_QUERY) {
              //  logger.info("Thực thi thành công câu lệnh: " + ps.toString());
            }

            rowUpdated = ps.executeUpdate();
        } catch (Exception e) {
          //  logger.error("Có lỗi xảy ra khi thực thi câu lệnh: " + query, e);
            throw e;
        }
        return rowUpdated;
    }


    public static int executeUpdate(String table, Map<String, Object> setColumns, String whereCondition, Object... whereValues) throws Exception {
        // Tạo phần SET từ Map
        StringBuilder setClause = new StringBuilder();
        List<Object> setValues = new ArrayList<>();
        for (Map.Entry<String, Object> entry : setColumns.entrySet()) {
            if (setClause.length() > 0) {
                setClause.append(", ");
            }
            setClause.append(entry.getKey()).append(" = ?");
            setValues.add(entry.getValue());
        }
        // Ghép câu lệnh SQL
        String query = "UPDATE " + table + " SET " + setClause.toString() + " WHERE " + whereCondition;

        // Thực thi câu lệnh với PreparedStatement
        try (Connection con = getConnection();
             PreparedStatement ps = con.prepareStatement(query)) {
            // Gán giá trị cho phần SET
            int paramIndex = 1;
            for (Object setValue : setValues) {
                ps.setObject(paramIndex++, setValue);
            }
            // Gán giá trị cho phần WHERE
            for (Object whereValue : whereValues) {
                ps.setObject(paramIndex++, whereValue);
            }

            return ps.executeUpdate();
        } catch (Exception ex) {
            throw ex;
        }
    }

    public static int executeInsert(String query, Object... objs) throws Exception {
        if (query.indexOf("insert") == 0 && query.lastIndexOf("()") == query.length() - 2) {
            StringBuilder sb = new StringBuilder();
            sb.append("(");
            for (int i = 0; i < objs.length; i++) {
                sb.append("?");
                if (i < objs.length - 1) {
                    sb.append(",");
                } else {
                    sb.append(")");
                }
            }
            query = query.replace("()", sb.toString());
        }
        try (Connection con = getConnection();
             PreparedStatement ps = con.prepareStatement(query)) {
            int i;
            for (i = 0; i < objs.length; i++) {
                ps.setObject(i + 1, objs[i]);
            }
            if (LOG_QUERY) {
                //logger.info("Thực thi thành công câu lệnh: " + ps.toString());
                //Log.gI().log(ps.toString());
            }
            i = ps.executeUpdate();
            return i;
        } catch (Exception ex) {
            //logger.error("Có lỗi xảy ra khi thực thi câu lệnh: " + query);
            throw ex;
        }
    }

}
