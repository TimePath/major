package com.timepath.major.vfs;

import com.timepath.vfs.SimpleVFile;

import java.io.InputStream;
import java.util.Collection;

/**
 * Represents the various actions which can be performed on files, and gives you a chance to override or extend their
 * implementation
 *
 * @author TimePath
 */
public abstract class SecurityController {

    /**
     * Called in response to {@link com.timepath.vfs.SimpleVFile#openStream()}
     *
     * @param file
     *
     * @return
     */
    public InputStream openStream(final SimpleVFile file) {
        return file.openStream();
    }

    /**
     * Called in response to {@link com.timepath.vfs.SimpleVFile#add(com.timepath.vfs.SimpleVFile)}
     *
     * @param parent
     * @param file
     */
    public void add(final SimpleVFile parent, final SimpleVFile file) {
        parent.add(file);
    }

    /**
     * Called in response to {@link com.timepath.vfs.SimpleVFile#list()}
     *
     * @param file
     *
     * @return
     */
    public Collection<? extends SimpleVFile> list(final SimpleVFile file) {
        return file.list();
    }

    /**
     * Called in response to {@link com.timepath.vfs.SimpleVFile#get(String)}
     *
     * @param file
     *
     * @return
     */
    public SimpleVFile get(final SimpleVFile file) {
        return file;
    }
}
