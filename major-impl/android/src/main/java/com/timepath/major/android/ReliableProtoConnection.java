package com.timepath.major.android;

import com.google.protobuf.MessageLite;
import com.timepath.major.ProtoConnection;
import com.timepath.major.proto.Messages.Meta;
import com.timepath.major.proto.Messages.Meta.Builder;

import java.io.IOException;
import java.net.Socket;
import java.util.Collections;
import java.util.Iterator;
import java.util.LinkedList;
import java.util.List;
import java.util.logging.Level;
import java.util.logging.Logger;

/**
 * Wraps a {@link com.timepath.major.ProtoConnection} in a safe way, and allows for listeners to be added and removed
 * at runtime
 *
 * @author TimePath
 */
public class ReliableProtoConnection extends ProtoConnection {

    private static final Logger            LOG       = Logger.getLogger(ReliableProtoConnection.class.getName());
    final                List<MessageLite> buffer    = Collections.synchronizedList(new LinkedList<MessageLite>());
    private final        List<Object>      listeners = Collections.synchronizedList(new LinkedList<>());
    private final        List<Object>      toRemove  = Collections.synchronizedList(new LinkedList<>());
    private String          address;
    private int             port;
    private ProtoConnection connection;
    private Socket          socket;

    @SuppressWarnings({ "unused", "UnusedParameters" })
    @Deprecated
    protected ReliableProtoConnection(Socket socket) { }

    public ReliableProtoConnection(String address, int port) {
        this.address = address;
        this.port = port;
    }

    @Override
    public void readLoop() throws IOException {
        check();
        connection.readLoop();
        // Connection lost
        connection = null;
    }

    @Override
    public Meta read() throws IOException {
        check();
        return connection.read();
    }

    /**
     * Checks the current connection state, and re-establishes one if neccessary
     *
     * @throws IOException
     */
    protected void check() throws IOException {
        if(connection != null) return;
        // Attempt connection
        socket = new Socket(address, port);
        connection = new ProtoConnection(socket) {
            @Override
            protected void fireCallbacks(final Meta message, final Object listener, final Builder responseBuilder) {
                synchronized(listeners) {
                    for(Iterator<Object> it = listeners.iterator(); it.hasNext(); ) {
                        Object o = it.next();
                        if(toRemove.contains(o)) {
                            it.remove();
                        } else {
                            super.fireCallbacks(message, o, responseBuilder);
                        }
                    }
                }
            }
        };
        new Thread("Proto reader") {
            @Override
            public void run() {
                try {
                    connection.readLoop();
                } catch(IOException e) {
                    LOG.log(Level.WARNING, "Connection lost", e);
                } finally {
                    try {
                        if(socket != null) {
                            socket.close();
                        }
                    } catch(IOException ignored) {
                    }
                }
            }
        }.start();
        // Re-send failed messages
        synchronized(buffer) {
            for(Iterator<MessageLite> it = buffer.iterator(); it.hasNext(); ) {
                connection.write(it.next());
                it.remove();
            }
        }
    }

    /**
     * Registers an object as a listener. Listeners contain methods annotated with {@link
     * com.timepath.major.ProtoConnection.Callback}
     *
     * @param listener
     *         the listener
     */
    public void addListener(final Object listener) {
        listeners.add(listener);
    }

    /**
     * Unregister an object as a listener
     *
     * @param listener
     *         the listener
     */
    public void removeListener(final Object listener) {
        toRemove.add(listener);
    }

    @Override
    public void write(final MessageLite message) throws IOException {
        buffer.add(message);
        for(int i = 0; i < 2; i++) {
            check();
            if(buffer.size() > 0) { // Did not reconnect
                try {
                    connection.write(message);
                    // Sent successfully
                    buffer.remove(0);
                    return;
                } catch(IOException e) { // Got disconnected, try again immediately (for loop)
                    connection = null;
                }
            }
        }
        // Network timed out, try again later
    }
}
