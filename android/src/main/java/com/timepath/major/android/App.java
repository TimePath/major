package com.timepath.major.android;

import android.app.Application;
import android.content.SharedPreferences;

/**
 * @author TimePath
 */
public class App extends Application {

    private static final String PREFS_NAME = "com.timepath.major.android";
    private static App                     instance;
    private        SharedPreferences       settings;
    private        ReliableProtoConnection connection;
    private        LocalCache              cache;

    public static App getInstance() {
        return instance;
    }

    public ReliableProtoConnection getConnection() {
        return connection;
    }

    @Override
    public void onCreate() {
        super.onCreate();
        instance = this;
        settings = getSharedPreferences(PREFS_NAME, MODE_PRIVATE);
        cache = new LocalCache(this);
        cache.open();
    }

    public SharedPreferences getSettings() {
        return settings;
    }

    public void connect(final String address, final int port) {
        connection = new ReliableProtoConnection(address, port);
    }

    public LocalCache getCache() {
        return cache;
    }
}
