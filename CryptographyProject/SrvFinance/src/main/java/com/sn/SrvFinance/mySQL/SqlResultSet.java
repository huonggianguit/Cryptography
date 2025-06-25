package com.sn.SrvFinance.mySQL;

import java.sql.ResultSet;
import java.sql.ResultSetMetaData;
import java.sql.Timestamp;
import java.util.HashMap;
import java.util.Map;

public class SqlResultSet implements ISqlResultSet {

    private Map<String, Object>[] data;
    private Object[][] values;
    private int indexData = -1;

    public SqlResultSet(ResultSet rs) throws Exception {
        try {
            rs.last();
            int nRow = rs.getRow();
            rs.beforeFirst();
            ResultSetMetaData rsmd = rs.getMetaData();
            int nColumn = rsmd.getColumnCount();
            this.data = (Map<String, Object>[])new HashMap[nRow];
            for (int i = 0; i < this.data.length; i++) {
                this.data[i] = new HashMap<>();
            }
            this.values = new Object[nRow][nColumn];
            int index = 0;
            while (rs.next()) {
                for (int j = 1; j <= nColumn; j++) {
                    String tableName = rsmd.getTableName(j);
                    String columnName = rsmd.getColumnName(j);
                    Object columnValue = rs.getObject(j);
                    this.data[index].put(columnName.toLowerCase(), columnValue);
                    this.data[index].put(tableName.toLowerCase() + "." + columnName.toLowerCase(), columnValue);
                    this.values[index][j - 1] = columnValue;
                }
                index++;
            }
        } catch (Exception e) {
            throw e;
        } finally {
            if (rs != null) {
                try {
                    rs.getStatement().close();
                    rs.close();
                } catch (Exception exception) {}
            }
        }
    }

    public void dispose() {
        for (Map<String, Object> map : this.data) {
            map.clear();
            map = null;
        }
        this.data = null;
        for (Object[] obj : this.values) {
            for (Object o : obj) {
                o = null;
            }
            obj = null;
        }
        this.values = null;
    }

    public boolean next() throws Exception {
        if (this.data == null) {
            throw new Exception("No data available");
        }
        this.indexData++;
        return (this.indexData < this.data.length);
    }

    public boolean first() throws Exception {
        if (this.data == null) {
            throw new Exception("No data available");
        }
        this.indexData++;
        return (this.indexData == this.data.length - 1);
    }

    public boolean gotoResult(int index) throws Exception {
        if (this.data == null) {
            throw new Exception("No data available");
        }
        if (this.indexData < 0 || this.indexData >= this.data.length) {
            throw new Exception("Index out of bound");
        }
        this.indexData = index;
        return true;
    }

    public boolean gotoFirst() throws Exception {
        if (this.data == null || this.data.length == 0) {
            throw new Exception("No data available");
        }
        this.indexData = 0;
        return true;
    }

    public void gotoBeforeFirst() {
        this.indexData = -1;
    }

    public boolean gotoLast() throws Exception {
        if (this.data == null) {
            throw new Exception("No data available");
        }
        this.indexData = this.data.length - 1;
        return true;
    }

    public int getRows() throws Exception {
        if (this.data == null) {
            throw new Exception("No data available");
        }
        return this.data.length;
    }

    public byte getByte(int column) throws Exception {
        if (this.values == null) {
            throw new Exception("No data available");
        }
        if (this.indexData == -1) {
            throw new Exception("Results need to be prepared in advance");
        }
        return (byte)((Integer)this.values[this.indexData][column - 1]).intValue();
    }

    public byte getByte(String column) throws Exception {
        if (this.data == null) {
            throw new Exception("No data available");
        }
        if (this.indexData == -1) {
            throw new Exception("Results need to be prepared in advance");
        }
        return (byte)((Integer)this.data[this.indexData].get(column.toLowerCase())).intValue();
    }

    public int getInt(int column) throws Exception {
        if (this.values == null) {
            throw new Exception("No data available");
        }
        if (this.indexData == -1) {
            throw new Exception("Results need to be prepared in advance");
        }
        return (int)((Long)this.values[this.indexData][column - 1]).longValue();
    }

    public int getInt(String column) throws Exception {
        if (this.data == null) {
            throw new Exception("No data available");
        }
        if (this.indexData == -1) {
            throw new Exception("Results need to be prepared in advance");
        }
        return ((Integer)this.data[this.indexData].get(column.toLowerCase())).intValue();
    }

    public float getFloat(int column) throws Exception {
        if (this.values == null) {
            throw new Exception("No data available");
        }
        if (this.indexData == -1) {
            throw new Exception("Results need to be prepared in advance");
        }
        return ((Float)this.values[this.indexData][column - 1]).floatValue();
    }

    public float getFloat(String column) throws Exception {
        if (this.data == null) {
            throw new Exception("No data available");
        }
        if (this.indexData == -1) {
            throw new Exception("Results need to be prepared in advance");
        }
        return ((Float)this.data[this.indexData].get(column.toLowerCase())).floatValue();
    }

    public double getDouble(int column) throws Exception {
        if (this.values == null) {
            throw new Exception("No data available");
        }
        if (this.indexData == -1) {
            throw new Exception("Results need to be prepared in advance");
        }
        return ((Double)this.values[this.indexData][column - 1]).doubleValue();
    }

    public double getDouble(String column) throws Exception {
        if (this.data == null) {
            throw new Exception("No data available");
        }
        if (this.indexData == -1) {
            throw new Exception("Results need to be prepared in advance");
        }
        return ((Double)this.data[this.indexData].get(column.toLowerCase())).doubleValue();
    }

    public long getLong(int column) throws Exception {
        if (this.values == null) {
            throw new Exception("No data available");
        }
        if (this.indexData == -1) {
            throw new Exception("Results need to be prepared in advance");
        }
        return ((Long)this.values[this.indexData][column - 1]).longValue();
    }

    public long getLong(String column) throws Exception {
        if (this.data == null) {
            throw new Exception("No data available");
        }
        if (this.indexData == -1) {
            throw new Exception("Results need to be prepared in advance");
        }
        return ((Long)this.data[this.indexData].get(column.toLowerCase())).longValue();
    }

    public String getString(int column) throws Exception {
        if (this.values == null) {
            throw new Exception("No data available");
        }
        if (this.indexData == -1) {
            throw new Exception("Results need to be prepared in advance");
        }
        return String.valueOf(this.values[this.indexData][column - 1]);
    }

    public String getString(String column) throws Exception {
        if (this.data == null) {
            throw new Exception("No data available");
        }
        if (this.indexData == -1) {
            throw new Exception("Results need to be prepared in advance");
        }
        return String.valueOf(this.data[this.indexData].get(column.toLowerCase()));
    }

    public Object getObject(int column) throws Exception {
        if (this.values == null) {
            throw new Exception("No data available");
        }
        if (this.indexData == -1) {
            throw new Exception("Results need to be prepared in advance");
        }
        return this.values[this.indexData][column - 1];
    }

    public Object getObject(String column) throws Exception {
        if (this.data == null) {
            throw new Exception("No data available");
        }
        if (this.indexData == -1) {
            throw new Exception("Results need to be prepared in advance");
        }
        return this.data[this.indexData].get(column.toLowerCase());
    }

    public boolean getBoolean(int column) throws Exception {
        if (this.values == null) {
            throw new Exception("No data available");
        }
        if (this.indexData == -1) {
            throw new Exception("Results need to be prepared in advance");
        }
        try {
            return (((Integer)this.values[this.indexData][column - 1]).intValue() == 1);
        } catch (Exception e) {
            return ((Boolean)this.values[this.indexData][column - 1]).booleanValue();
        }
    }

    public boolean getBoolean(String column) throws Exception {
        if (this.data == null) {
            throw new Exception("No data available");
        }
        if (this.indexData == -1) {
            throw new Exception("Results need to be prepared in advance");
        }
        Object value = this.data[this.indexData].get(column.toLowerCase());
        if (value == null) {
            throw new Exception("Null value for column: " + column);
        }
        if (value instanceof Integer) {
            // Nếu giá trị là Integer, kiểm tra xem có bằng 1 không
            return ((Integer) value).intValue() == 1;
        } else if (value instanceof String) {
            // Nếu giá trị là String, thử chuyển thành số nguyên
            String str = (String) value;
            try {
                int intValue = Integer.parseInt(str);
                return intValue == 1; // "1" -> true, "0" -> false
            } catch (NumberFormatException e) {
                throw new Exception("Cannot parse string to int for boolean: " + str);
            }
        } else if (value instanceof Boolean) {
            // Nếu giá trị là Boolean, trả về trực tiếp
            return ((Boolean) value).booleanValue();
        } else {
            throw new Exception("Unsupported type for boolean: " + value.getClass().getName());
        }
    }

    public Timestamp getTimestamp(int column) throws Exception {
        if (this.values == null) {
            throw new Exception("No data available");
        }
        if (this.indexData == -1) {
            throw new Exception("Results need to be prepared in advance");
        }
        return (Timestamp)this.values[this.indexData][column - 1];
    }

    public Timestamp getTimestamp(String column) throws Exception {
        if (this.data == null) {
            throw new Exception("No data available");
        }
        if (this.indexData == -1) {
            throw new Exception("Results need to be prepared in advance");
        }
        return (Timestamp)this.data[this.indexData].get(column.toLowerCase());
    }

    public short getShort(int column) throws Exception {
        if (this.values == null) {
            throw new Exception("No data available");
        }
        if (this.indexData == -1) {
            throw new Exception("Results need to be prepared in advance");
        }
        return (short)((Integer)this.values[this.indexData][column - 1]).intValue();
    }

    public short getShort(String column) throws Exception {
        if (this.data == null) {
            throw new Exception("No data available");
        }
        if (this.indexData == -1) {
            throw new Exception("Results need to be prepared in advance");
        }
        return (short)((Integer)this.data[this.indexData].get(column.toLowerCase())).intValue();
    }
}
