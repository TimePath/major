package com.timepath.major.vfs;

import com.timepath.vfs.SimpleVFile;
import com.timepath.vfs.ftp.FTPFS;
import com.timepath.vfs.jdbc.JDBCFS;

import java.io.IOException;
import java.sql.SQLException;

/**
 *
 * @author TimePath
 */
public class JDBCAdapter {

    public static void main(String[] args) throws IOException, SQLException, ClassNotFoundException {
        if(args.length == 0) args = new String[]{"jdbc:mysql://localhost/test"};
        if (args.length >= 2) {
            Class.forName(args[1]);
        }
        SimpleVFile root = new JDBCFS(args[0]) {

        };
        FTPFS web = new FTPFS();
        web.add(root);
        new Thread(web).start();
    }

}
