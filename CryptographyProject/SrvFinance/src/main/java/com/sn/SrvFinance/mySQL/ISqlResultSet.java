package com.sn.SrvFinance.mySQL;

import java.sql.Timestamp;

public interface ISqlResultSet {
    byte getByte(int paramInt) throws Exception;

    byte getByte(String paramString) throws Exception;

    int getInt(int paramInt) throws Exception;

    int getInt(String paramString) throws Exception;

    short getShort(int paramInt) throws Exception;

    short getShort(String paramString) throws Exception;

    float getFloat(int paramInt) throws Exception;

    float getFloat(String paramString) throws Exception;

    double getDouble(int paramInt) throws Exception;

    double getDouble(String paramString) throws Exception;

    long getLong(int paramInt) throws Exception;

    long getLong(String paramString) throws Exception;

    String getString(int paramInt) throws Exception;

    String getString(String paramString) throws Exception;

    boolean getBoolean(int paramInt) throws Exception;

    boolean getBoolean(String paramString) throws Exception;

    Object getObject(int paramInt) throws Exception;

    Object getObject(String paramString) throws Exception;

    Timestamp getTimestamp(int paramInt) throws Exception;

    Timestamp getTimestamp(String paramString) throws Exception;

    void dispose();

    boolean next() throws Exception;

    boolean first() throws Exception;

    boolean gotoResult(int paramInt) throws Exception;

    boolean gotoFirst() throws Exception;

    void gotoBeforeFirst() throws Exception;

    boolean gotoLast() throws Exception;

    int getRows() throws Exception;
}
