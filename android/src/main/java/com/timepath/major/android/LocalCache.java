package com.timepath.major.android;

import android.content.ContentValues;
import android.content.Context;
import android.database.Cursor;
import android.database.sqlite.SQLiteDatabase;
import android.database.sqlite.SQLiteOpenHelper;
import com.timepath.major.proto.Messages.File;
import com.timepath.major.proto.Messages.File.FileType;

import java.util.LinkedList;
import java.util.List;

/**
 * @author TimePath
 */
public class LocalCache extends SQLiteOpenHelper {

    private static final String   TABLE_FILES      = "files";
    private static final String   COLUMN_ID        = "_id";
    private static final String   COLUMN_ANCESTOR  = "parent";
    private static final String   COLUMN_NAME      = "name";
    private static final String   COLUMN_TYPE      = "type";
    private static final String   DATABASE_CREATE  = "create table " + TABLE_FILES + "(" //
                                                     + COLUMN_ID + " integer primary key autoincrement" //
                                                     + ", " + COLUMN_ANCESTOR + " text not null" //
                                                     + ", " + COLUMN_NAME + " text not null" //
                                                     + ", " + COLUMN_TYPE + " integer not null" //
                                                     + ");";
    private static final String[] COLUMNS_ALL      = {
            COLUMN_ID, COLUMN_ANCESTOR, COLUMN_NAME, COLUMN_TYPE
    };
    private static final String   DATABASE_NAME    = "files.db";
    private static final int      DATABASE_VERSION = 1;
    private SQLiteDatabase database;

    public LocalCache(Context context) {
        super(context, DATABASE_NAME, null, DATABASE_VERSION);
    }

    public void open() {
        database = getWritableDatabase();
    }

    public void close() {
        database.close();
    }

    @Override
    public void onCreate(SQLiteDatabase database) {
        database.execSQL(DATABASE_CREATE);
    }

    @Override
    public void onUpgrade(SQLiteDatabase db, int oldVersion, int newVersion) {
        db.execSQL("DROP TABLE IF EXISTS " + TABLE_FILES);
        onCreate(db);
    }

    /**
     * Store information about a file
     *
     * @param parent
     *         the parent directory
     * @param file
     *         the file object
     *
     * @return the row ID of the newly inserted row, or -1 if an error occurred
     */
    public long insert(String parent, File file) {
        ContentValues values = new ContentValues();
        values.put(COLUMN_ANCESTOR, parent);
        values.put(COLUMN_NAME, file.getName());
        values.put(COLUMN_TYPE, file.getType().getNumber());
        return database.insert(TABLE_FILES, null, values);
    }

    public void delete(String parent) {
        database.delete(TABLE_FILES, COLUMN_ANCESTOR + "=?", new String[] { parent });
    }

    public List<File> select(String parent) {
        List<File> fileList = new LinkedList<>();
        Cursor cursor = database.query(TABLE_FILES,
                                       COLUMNS_ALL,
                                       COLUMN_ANCESTOR + "=?",
                                       new String[] { parent },
                                       null,
                                       null,
                                       null);
        cursor.moveToFirst();
        while(!cursor.isAfterLast()) {
            File file = parse(cursor);
            fileList.add(file);
            cursor.moveToNext();
        }
        cursor.close();
        return fileList;
    }

    private File parse(final Cursor cursor) {
        return File.newBuilder()
                   .setName(cursor.getString(cursor.getColumnIndex(COLUMN_NAME)))
                   .setType(FileType.valueOf(cursor.getInt(cursor.getColumnIndex(COLUMN_TYPE))))
                   .build();
    }
}
