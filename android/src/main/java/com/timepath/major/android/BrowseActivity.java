package com.timepath.major.android;

import android.app.Activity;
import android.app.AlertDialog;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.content.pm.ResolveInfo;
import android.net.Uri;
import android.os.AsyncTask;
import android.os.Bundle;
import android.view.Menu;
import android.view.View;
import android.widget.AdapterView;
import android.widget.AdapterView.OnItemClickListener;
import android.widget.AdapterView.OnItemLongClickListener;
import android.widget.ListView;
import android.widget.TextView;
import android.widget.Toast;
import com.timepath.major.ProtoConnection.Callback;
import com.timepath.major.proto.Messages.File;
import com.timepath.major.proto.Messages.File.FileType;
import com.timepath.major.proto.Messages.ListRequest;
import com.timepath.major.proto.Messages.ListResponse;
import com.timepath.major.proto.Messages.Meta.Builder;

import java.io.FileInputStream;
import java.io.IOException;
import java.io.OutputStream;
import java.net.URLConnection;
import java.text.MessageFormat;
import java.util.ArrayList;
import java.util.List;

/**
 * @author TimePath
 */
public class BrowseActivity extends Activity {

    public static final String ROOT = BrowseActivity.class.getName() + ".ROOT";
    public static final File   BACK = File.newBuilder().setName("..").setType(FileType.DIRECTORY).build();

    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        Intent intent = getIntent();
        final String root = intent.getStringExtra(ROOT);
        this.setContentView(R.layout.activity_main);
        TextView cdir = (TextView) this.findViewById(R.id.cdir);
        cdir.setText(root);
        ListView listview = (ListView) this.findViewById(R.id.files);
        final ArrayList<File> list = new ArrayList<>();
        list.add(BACK);
        list.addAll(App.getInstance().getCache().select(root));
        final FileAdapter adapter = new FileAdapter(this, list);
        listview.setAdapter(adapter);
        final Activity a = this;
        new AsyncTask<Void, Void, Void>() {
            @Override
            protected Void doInBackground(final Void... params) {
                final ReliableProtoConnection conn = App.getInstance().getConnection();
                conn.addListener(new Object() {
                    @Callback
                    void listing(final ListResponse l, Builder response) throws IOException {
                        list.clear();
                        list.add(BACK);
                        list.addAll(l.getFileList());
                        App.getInstance().getCache().delete(root);
                        for(File file : l.getFileList()) {
                            App.getInstance().getCache().insert(root, file);
                        }
                        a.runOnUiThread(new Runnable() {
                            public void run() {
                                adapter.notifyDataSetChanged();
                            }
                        });
                        conn.removeListener(this);
                    }
                });
                try {
                    conn.write(conn.newBuilder().setListRequest(ListRequest.newBuilder().setPath(root)).build());
                } catch(IOException e) {
                    e.printStackTrace();
                }
                return null;
            }
        }.execute();
        listview.setOnItemClickListener(new OnItemClickListener() {
            @Override
            public void onItemClick(final AdapterView<?> parent, final View view, final int position, final long id) {
                final File file = (File) parent.getItemAtPosition(position);
                final Activity a = BrowseActivity.this;
                view.animate().setDuration(200).translationXBy(50).withEndAction(new Runnable() {
                    @Override
                    public void run() {
                        view.setTranslationX(view.getTranslationX() - 50);
                    }
                });
                if(file.getType() == FileType.DIRECTORY) {
                    a.startActivity(new Intent(a, BrowseActivity.class) {{
                        putExtra(BrowseActivity.ROOT, root + file.getName() + "/");
                    }});
                    overridePendingTransition(R.anim.right_slide_in, R.anim.left_slide_out);
                } else { // TODO: download
                    try {
                        OutputStream outputStream = openFileOutput("test.txt", MODE_WORLD_READABLE);
                        outputStream.write("Hello world!".getBytes("UTF-8"));
                        outputStream.close();
                        java.io.File temp = getFileStreamPath("test.txt");
                        String mime = URLConnection.guessContentTypeFromStream(new FileInputStream(temp));
                        if(mime == null) mime = URLConnection.guessContentTypeFromName(temp.getName());
                        Intent intent = new Intent(Intent.ACTION_VIEW);
                        intent.setDataAndType(Uri.fromFile(temp), mime);
                        PackageManager pm = getPackageManager();
                        List<ResolveInfo> apps = pm.queryIntentActivities(intent, 0);
                        if(apps.size() > 0) {
                            startActivity(intent);
                        } else {
                            Toast.makeText(BrowseActivity.this,
                                           MessageFormat.format(getString(R.string.file_unsupported), temp),
                                           Toast.LENGTH_SHORT).show();
                        }
                    } catch(IOException e) {
                        e.printStackTrace();
                    }
                }
            }
        });
        listview.setOnItemLongClickListener(new OnItemLongClickListener() {
            @Override
            public boolean onItemLongClick(final AdapterView<?> parent,
                                           final View view,
                                           final int position,
                                           final long id)
            {
                if(position == 0) return false;
                final TextView textView = new TextView(BrowseActivity.this);
                textView.setText(parent.getItemAtPosition(position).toString());
                new AlertDialog.Builder(BrowseActivity.this).setTitle(getString(R.string.fileinfo_title))
                                                            .setView(textView)
                                                            .setNeutralButton(R.string.ok, null)
                                                            .show();
                return true;
            }
        });
    }

    @Override
    public void onBackPressed() {
        super.onBackPressed();
        this.finish();
        overridePendingTransition(R.anim.left_slide_in, R.anim.right_slide_out);
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        return false;
    }
}
