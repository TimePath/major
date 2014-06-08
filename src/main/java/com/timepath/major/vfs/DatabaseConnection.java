package com.timepath.major.vfs;

import com.timepath.vfs.SimpleVFile;
import com.timepath.vfs.jdbc.JDBCFS;

import java.io.ByteArrayInputStream;
import java.io.InputStream;
import java.nio.charset.StandardCharsets;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.Collection;
import java.util.LinkedList;
import java.util.logging.Level;
import java.util.logging.Logger;

/**
 * Controls how files are to be accessed from the database
 *
 * @author TimePath
 */
public class DatabaseConnection extends JDBCFS {

    private static final Logger LOG = Logger.getLogger(DatabaseConnection.class.getName());

    public DatabaseConnection(String url) throws SQLException {
        super(url);
    }

    private Collection<? extends SimpleVFile> list(String path) {
        LinkedList<DatabaseFile> files = new LinkedList<>();
        try {
            PreparedStatement stmt = conn.prepareStatement("SELECT name FROM children(?)");
            stmt.setString(1, path);
            ResultSet rs = stmt.executeQuery();
            while(rs.next()) {
                System.out.println(rs.getString("name"));
                files.add(new DatabaseFile(path, rs.getString("name")));
            }
            rs.close();
        } catch(SQLException e) {
            LOG.log(Level.SEVERE, null, e);
        }
        return files;
    }

    class DatabaseFile extends SimpleVFile {

        private final String name;
        private final String path;

        DatabaseFile(String path, String name) {
            this.path = path;
            this.name = name;
        }

        @Override
        public String getPath() {
            return path;
        }

        @Override
        public String getName() {
            return name;
        }

        @Override
        public boolean isDirectory() {
            return false;
        }

        @Override
        public Collection<? extends SimpleVFile> list() {
            return DatabaseConnection.this.list(path);
        }

        @Override
        public InputStream openStream() {
            return new ByteArrayInputStream("Test".getBytes(StandardCharsets.UTF_8));
        }
    }

    @Override
    public Collection<? extends SimpleVFile> list() {
        return list("");
    }
}
