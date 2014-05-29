package com.timepath.major.vfs;

import com.timepath.vfs.SimpleVFile;
import com.timepath.vfs.http.HTTPFS;
import com.timepath.vfs.jdbc.JDBCFS;
import java.io.IOException;
import java.sql.SQLException;

/**
 *
 * @author TimePath
 */
public class JDBCAdapter {

    public static void main(String[] args) throws IOException, SQLException, ClassNotFoundException {
        if (args.length >= 2) {
            Class.forName(args[1]);
        }
        SimpleVFile root = new JDBCFS(args[0]) {

        };
        HTTPFS web = new HTTPFS();
        web.add(root);
        web.run();
    }

}
