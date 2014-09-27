package com.timepath.launcher;

import com.timepath.major.DefaultServer;
import com.timepath.major.vfs.DatabaseConnection;
import com.timepath.major.vfs.SecurityAdapter;
import com.timepath.major.vfs.SecurityController;
import com.timepath.vfs.SimpleVFile;
import com.timepath.vfs.ftp.FTPFS;
import com.timepath.vfs.jdbc.JDBCFS;

import java.io.ByteArrayInputStream;
import java.io.IOException;
import java.io.InputStream;
import java.nio.charset.StandardCharsets;
import java.sql.SQLException;
import java.util.Collection;
import java.util.logging.Level;
import java.util.logging.Logger;

public class Main {

    private static final Logger LOG = Logger.getLogger(Main.class.getName());

    public static void main(String[] args) throws IOException {
        if (args.length == 0) args = new String[]{"jdbc:postgresql://localhost/" + System.getProperty("user.name")};
        if (args.length >= 2) {
            try {
                Class.forName(args[1]);
            } catch (ClassNotFoundException e) {
                LOG.log(Level.SEVERE, null, e);
            }
        }
        try {
            final JDBCFS jdbcfs = new DatabaseConnection(args[0]);
            FTPFS ftp = new FTPFS();
            ftp.add(new SecurityAdapter(jdbcfs, new SecurityController() {
                String user = System.getProperty("user.name");

                @Override
                public InputStream openStream(final SimpleVFile file) {
                    if (Math.round(Math.random()) == 1) {
                        return new ByteArrayInputStream("Go away".getBytes(StandardCharsets.UTF_8));
                    }
                    return super.openStream(file);
                }

                @Override
                public Collection<? extends SimpleVFile> list(final SimpleVFile file) {
                    return file.list();
                }

                @Override
                public SimpleVFile get(final SimpleVFile file) {
                    LOG.log(Level.INFO, "{0} is accessing {1}", new Object[]{user, file});
                    return file;
                }
            }));
            new Thread(ftp).start();
            new DefaultServer(jdbcfs).run();
        } catch (SQLException | IOException e) {
            LOG.log(Level.SEVERE, null, e);
        }
    }

}
