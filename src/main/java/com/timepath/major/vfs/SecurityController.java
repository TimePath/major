package com.timepath.major.vfs;

import com.timepath.vfs.SimpleVFile;

import java.io.InputStream;
import java.util.Collection;

/**
 * @author TimePath
 */
public abstract class SecurityController {

    public InputStream openStream(final SimpleVFile file) {
        return file.openStream();
    }

    public void add(final SimpleVFile parent, final SimpleVFile file) {
        parent.add(file);
    }

    public Collection<? extends SimpleVFile> list(final SimpleVFile file) {
        return file.list();
    }

    public SimpleVFile get(final SimpleVFile file) {
        return file;
    }
}
