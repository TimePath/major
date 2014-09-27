package com.timepath.major.android;

import android.app.Activity;
import android.app.AlertDialog;
import android.content.DialogInterface;
import android.content.Intent;
import android.content.SharedPreferences.Editor;
import android.os.AsyncTask;
import android.os.Bundle;
import android.text.InputType;
import android.view.Menu;
import android.view.MenuInflater;
import android.view.MenuItem;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.EditText;
import android.widget.Toast;

/**
 * @author TimePath
 */
public class LoginActivity extends Activity {

    private String address = App.getInstance().getSettings().getString(Settings.HOST, "192.168.1.30");
    private int    port    = App.getInstance().getSettings().getInt(Settings.PORT, 9001);

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_login);
        Button b = (Button) this.findViewById(R.id.login);
        b.setOnClickListener(new OnClickListener() {
            @Override
            public void onClick(final View v) {
                final Activity a = LoginActivity.this;
                Toast.makeText(a, getString(R.string.connecting), Toast.LENGTH_SHORT).show();
                new AsyncTask<Void, Void, Void>() {
                    @Override
                    protected Void doInBackground(final Void... params) {
                        App.getInstance().connect(address, port);
                        a.startActivity(new Intent(a, BrowseActivity.class) {{
                            putExtra(BrowseActivity.ROOT, "/");
                        }});
                        return null;
                    }
                }.execute();
            }
        });
    }

    @Override
    public boolean onCreateOptionsMenu(final Menu menu) {
        MenuInflater inflater = getMenuInflater();
        inflater.inflate(R.menu.main, menu);
        return true;
    }

    @Override
    public boolean onOptionsItemSelected(final MenuItem item) {
        switch(item.getItemId()) {
            case R.id.action_settings:
                final EditText input = new EditText(this);
                input.setText(address);
                input.setInputType(InputType.TYPE_CLASS_PHONE);
                AlertDialog.Builder builder = new AlertDialog.Builder(this);
                builder.setTitle(getString(R.string.settings_title))
                       .setView(input)
                       .setPositiveButton(getString(R.string.ok), new DialogInterface.OnClickListener() {
                           @Override
                           public void onClick(DialogInterface dialog, int which) {
                               final Editor editor = App.getInstance().getSettings().edit();
                               address = input.getText().toString();
                               editor.putString(Settings.HOST, address);
                               editor.commit();
                           }
                       })
                       .setNegativeButton(getString(R.string.cancel), new DialogInterface.OnClickListener() {
                           @Override
                           public void onClick(DialogInterface dialog, int which) {
                               dialog.cancel();
                           }
                       })
                       .show();
                return true;
            default:
                return super.onOptionsItemSelected(item);
        }
    }
}

