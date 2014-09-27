package com.timepath.major.android;

import android.content.Context;
import android.content.Intent;
import android.content.pm.ResolveInfo;
import android.graphics.drawable.Drawable;
import android.net.Uri;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.ImageView;
import android.widget.TextView;
import com.timepath.major.proto.Messages.File;
import com.timepath.major.proto.Messages.File.FileType;

import java.net.URLConnection;
import java.util.List;

/**
 * @author TimePath
 */
public class FileAdapter extends ArrayAdapter<File> {

    private final Context    context;
    private final List<File> values;

    public FileAdapter(Context context, List<File> values) {
        super(context, R.layout.filelayout, values);
        this.context = context;
        this.values = values;
    }

    @Override
    public View getView(final int position, final View convertView, final ViewGroup parent) {
        View rowView = convertView;
        if(rowView == null) {
            LayoutInflater inflater = (LayoutInflater) context.getSystemService(Context.LAYOUT_INFLATER_SERVICE);
            rowView = inflater.inflate(R.layout.filelayout, parent, false);
            ViewHolder viewHolder = new ViewHolder();
            viewHolder.text = (TextView) rowView.findViewById(R.id.label);
            viewHolder.image = (ImageView) rowView.findViewById(R.id.icon);
            viewHolder.file = values.get(position);
            rowView.setTag(viewHolder);
        }
        ViewHolder holder = (ViewHolder) rowView.getTag();
        holder.text.setText(holder.file.getName());
        holder.image.setImageDrawable(getIcon(holder.file));
        return rowView;
    }

    Drawable getIcon(File f) {
        Intent intent = new Intent(Intent.ACTION_VIEW);
        if(f.getType() == FileType.FILE) {
            intent.setDataAndType(Uri.fromParts("file", f.getName(), null),
                                  URLConnection.guessContentTypeFromName(f.getName()));
        } else {
            intent.setType("inode/directory");
        }
        final List<ResolveInfo> matches = context.getPackageManager().queryIntentActivities(intent, 0);
        for(ResolveInfo match : matches) {
            final Drawable drawable = match.loadIcon(context.getPackageManager());
            if(drawable != null) {
                return drawable;
            }
        }
        return null;
    }

    static class ViewHolder {

        public TextView  text;
        public ImageView image;
        public File      file;
    }
}
