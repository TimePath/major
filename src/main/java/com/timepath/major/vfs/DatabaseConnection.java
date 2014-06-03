package com.timepath.major.vfs;

import com.timepath.vfs.jdbc.JDBCFS;

import java.sql.SQLException;

/**
 * Controls how files are to be accessed from the database
 *
 * @author TimePath
 */
public class DatabaseConnection extends JDBCFS {

    public DatabaseConnection(String url) throws SQLException {
        super(url);
    }
}
