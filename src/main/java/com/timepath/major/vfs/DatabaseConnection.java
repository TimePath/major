package com.timepath.major.vfs;

import com.timepath.vfs.SimpleVFile;
import com.timepath.vfs.jdbc.JDBCFS;

import java.io.BufferedInputStream;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.io.InputStream;
import java.net.URI;
import java.net.URISyntaxException;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.Timestamp;
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

    @Override
    public SimpleVFile get(final String name) {
        for(SimpleVFile f : list()) {
            if(name.equals(f.getName())) {
                return f;
            }
        }
        return null;
    }

    @Override
    public Collection<? extends SimpleVFile> list() {
        return list("");
    }

    private Collection<? extends SimpleVFile> list(String path) {
        LOG.log(Level.FINE, "Children(''{0}'')", path);
        LinkedList<DatabaseFile> files = new LinkedList<>();
        try {
            PreparedStatement stmt = conn.prepareStatement("SELECT name, uri, mtime FROM children(?);");
            stmt.setString(1, path);
            ResultSet rs = stmt.executeQuery();
            while(rs.next()) {
                String name = rs.getString("name");
                String uri = rs.getString("uri");
                Timestamp mtimeRaw = rs.getTimestamp("mtime");
                long mtime = mtimeRaw == null ? System.currentTimeMillis() : mtimeRaw.getTime();
                LOG.log(Level.FINE, "{0} -> {1}", new Object[] { name, uri });
                files.add(new DatabaseFile(path + "/" + name, name, uri, mtime));
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
        private final String uri;
        private       long   mtime;

        DatabaseFile(String path, String name, String uri, long mtime) {
            this.path = path;
            this.name = name;
            this.uri = uri;
            this.mtime = mtime;
        }

        @Override
        public String getName() {
            return name;
        }

        @Override
        public InputStream openStream() {
            if(uri == null) return null;
            try {
                return new BufferedInputStream(new URI(uri).toURL().openStream());
            } catch(FileNotFoundException e) {
                LOG.log(Level.SEVERE, "File not found", e);
            } catch(IOException e) {
                LOG.log(Level.SEVERE, "Other IO error", e);
            } catch(URISyntaxException e) {
                LOG.log(Level.SEVERE, "Bad URI", e);
            }
            return null;
        }

        @Override
        public long length() {
            try {
                URI u = new URI(uri);
                try {
                    int length = u.toURL().openConnection().getContentLength();
                    if(length >= 0) return length;
                } catch(IOException e) {
                    LOG.log(Level.SEVERE, "Bad URL", e);
                }
            } catch(URISyntaxException e) {
                LOG.log(Level.SEVERE, "Bad URI", e);
            }
            return super.length();
        }

        @Override
        public Collection<? extends SimpleVFile> list() {
            return DatabaseConnection.this.list(path);
        }

        @Override
        public SimpleVFile get(final String name) {
            for(SimpleVFile f : list()) {
                if(name.equals(f.getName())) {
                    return f;
                }
            }
            return null;
        }

        @Override
        public String getPath() {
            return path;
        }

        @Override
        public long lastModified() {
            return mtime;
        }

        @Override
        public boolean isDirectory() {
            return uri == null;
        }
    }
}
