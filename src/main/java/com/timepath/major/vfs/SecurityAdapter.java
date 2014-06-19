package com.timepath.major.vfs;

import com.timepath.vfs.FileChangeListener;
import com.timepath.vfs.SimpleVFile;

import javax.swing.*;
import java.io.File;
import java.io.IOException;
import java.io.InputStream;
import java.util.Collection;
import java.util.LinkedList;
import java.util.List;

/**
 * Decorates files to force all access through a {@link com.timepath.major.vfs.SecurityController}
 *
 * @author TimePath
 */
public class SecurityAdapter extends SimpleVFile {

    private final SimpleVFile        data;
    private final SecurityController security;

    public SecurityAdapter(SimpleVFile data, SecurityController policy) {
        this.data = data;
        this.security = policy;
    }

    @Override
    public void addFileChangeListener(FileChangeListener listener) {data.addFileChangeListener(listener);}

    @Override
    public boolean canExecute() {return data.canExecute();}

    @Override
    public boolean canRead() {return data.canRead();}

    @Override
    public boolean canWrite() {return data.canWrite();}

    @Override
    public boolean createNewFile() {return data.createNewFile();}

    @Override
    public boolean delete() {return data.delete();}

    @Override
    public boolean exists() {return data.exists();}

    @Override
    public SimpleVFile getParent() {
        return wrap(data.getParent());
    }

    @Override
    public Collection<? extends SimpleVFile> list() {
        return wrap(security.list(data));
    }

    /**
     * TODO: be smart about the original collection type rather than assume list
     *
     * @param unwrapped
     *         A collection of files to be decorated with the current security settings
     *
     * @return The original list, decorated
     */
    private List<SimpleVFile> wrap(final Collection<? extends SimpleVFile> unwrapped) {
        if(unwrapped == null) return null;
        List<SimpleVFile> wrapped = new LinkedList<>();
        for(SimpleVFile v : unwrapped) {
            wrapped.add(wrap(v));
        }
        return wrapped;
    }

    @Override
    public SimpleVFile get(String name) {
        return wrap(security.get(data.get(name)));
    }

    @Override
    public String getPath() {return data.getPath();}

    @Override
    public long getTotalSpace() {return data.getTotalSpace();}

    @Override
    public long getUsableSpace() {return data.getUsableSpace();}

    @Override
    public boolean isDirectory() {return data.isDirectory();}

    @Override
    public boolean isFile() {return data.isFile();}

    @Override
    public long lastModified() {return data.lastModified();}

    @Override
    public long length() {return data.length();}

    @Override
    public boolean renameTo(SimpleVFile dest) {return data.renameTo(dest);}

    @Override
    public boolean setExecutable(boolean executable) {return data.setExecutable(executable);}

    @Override
    public boolean setExecutable(boolean executable, boolean ownerOnly) {
        return data.setExecutable(executable, ownerOnly);
    }

    @Override
    public boolean setLastModified(long time) {return data.setLastModified(time);}

    @Override
    public boolean setReadable(boolean readable) {return data.setReadable(readable);}

    @Override
    public boolean setReadable(boolean readable, boolean ownerOnly) {return data.setReadable(readable, ownerOnly);}

    @Override
    public boolean setWritable(boolean writable) {return data.setWritable(writable);}

    @Override
    public boolean setWritable(boolean writable, boolean ownerOnly) {return data.setWritable(writable, ownerOnly);}

    /**
     * This method intentionally does not delegate
     * {@inheritDoc}
     */
    @SuppressWarnings("EmptyMethod")
    public boolean setParent(SimpleVFile newParent) {
        return super.setParent(newParent);
    }

    @Override
    public SimpleVFile query(String path) {
        return wrap(data.query(path));
    }

    @Override
    public SimpleVFile add(SimpleVFile file) {
        security.add(data, file);
        return this;
    }

    @Override
    public SimpleVFile addAll(Iterable<? extends SimpleVFile> c) {
        for(SimpleVFile file : c) {
            security.add(data, file);
        }
        return this;
    }

    @Override
    public void remove(SimpleVFile file) {data.remove(file);}

    @Override
    public void removeAll(Iterable<? extends SimpleVFile> files) {data.removeAll(files);}

    @Override
    public void extract(File dir) throws IOException {data.extract(dir);}

    @Override
    public void fileAdded(SimpleVFile f) {data.fileAdded(f);}

    @Override
    public void fileModified(SimpleVFile f) {data.fileModified(f);}

    @Override
    public void fileRemoved(SimpleVFile f) {data.fileRemoved(f);}

    @Override
    public List<SimpleVFile> find(String search) {
        return wrap(data.find(search));
    }

    @Override
    public Icon getIcon() {return data.getIcon();}

    @Override
    public String group() {return data.group();}

    @Override
    public String owner() {return data.owner();}

    @Override
    public String toString() {return data.toString();}

    private SecurityAdapter wrap(final SimpleVFile simpleVFile) {
        if(simpleVFile == null) return null;
        return new SecurityAdapter(simpleVFile, security);
    }

    @Override
    public String getName() {return data.getName();}

    @Override
    public InputStream openStream() {
        return security.openStream(data);
    }
}
