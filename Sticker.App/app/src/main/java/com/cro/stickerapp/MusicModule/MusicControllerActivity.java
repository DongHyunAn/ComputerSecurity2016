package com.cro.stickerapp.musicModule;

import android.content.Context;

import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.graphics.drawable.BitmapDrawable;

import android.graphics.drawable.Drawable;
import android.os.Vibrator;

import android.os.Bundle;
import android.util.Log;
import android.view.View;
import android.widget.ImageButton;
import android.widget.ImageView;
import android.widget.SeekBar;
import android.widget.TextView;

import com.bumptech.glide.Glide;
import com.cro.stickerapp.classes.BaseActivity;
import com.cro.stickerapp.classes.NetworkMessageReceiver;
import com.cro.stickerapp.global.GlobalNetworkService;
import com.cro.stickerapp.global.GlobalService;
import com.cro.stickerapp.R;

/**
 * An example full-screen activity that shows and hides the system UI (i.e.
 * status bar and navigation/system bar) with user interaction.
 */
public class MusicControllerActivity extends BaseActivity implements NetworkMessageReceiver, View.OnClickListener{
    Bitmap[] repeatBitmap = new Bitmap[2];
    Bitmap[] shuffleBitmap = new Bitmap[2];
    Bitmap[] playBitmap = new Bitmap[2];

    ImageView iv_repeat;
    ImageView iv_shuffle;
    ImageView iv_play;

    int playCount = 0;
    int repeatCount = 0;
    int shuffleCount = 0;
    boolean isPlaying = false;
    TextView tv_title;
    TextView tv_album;
    TextView tv_endtime;
    TextView tv_starttime;

    SeekBar sb_playbar;
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_music_controller);

        ImageButton ibtn_before = (ImageButton) findViewById(R.id.ibtn_music_before);
        ImageButton ibtn_listup = (ImageButton) findViewById(R.id.ibtn_music_listup);
        ImageButton ibtn_listdown = (ImageButton) findViewById(R.id.ibtn_music_listdown);
        ImageButton ibtn_next = (ImageButton) findViewById(R.id.ibtn_music_next);
        ImageButton ibtn_stop = (ImageButton) findViewById(R.id.ibtn_music_stop);

        ImageButton ibtn_back = (ImageButton) findViewById(R.id.ibtn_music_back);

        Glide.with(this).load(R.drawable.rps_btn_back).into(ibtn_back);

        ibtn_back.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                String message = "Sticker_Command_c";
                GlobalNetworkService.getInstance().sendMessageToServer(message);

                Vibrator vibe = (Vibrator) getSystemService(Context.VIBRATOR_SERVICE);
                vibe.vibrate(100);
            }
        });

        ImageView iv_background = (ImageView)findViewById(R.id.img_music_background);
        ImageView iv_listicon = (ImageView)findViewById(R.id.iv_music_icon);
        iv_play = (ImageView) findViewById(R.id.iv_music_play);
        iv_repeat = (ImageView) findViewById(R.id.iv_music_repeat);
        iv_shuffle = (ImageView) findViewById(R.id.iv_music_shuffle);


        tv_title = (TextView) findViewById(R.id.tv_music_title);
        tv_album = (TextView) findViewById(R.id.tv_music_album);
        tv_starttime = (TextView) findViewById(R.id.tv_music_starttime);
        tv_endtime = (TextView) findViewById(R.id.tv_music_endtime);

        sb_playbar = (SeekBar) findViewById(R.id.sb_music_playbar);
        SeekBar sb_soundbar = (SeekBar) findViewById(R.id.sb_music_soundbar);

        ibtn_stop.setImageDrawable(GlobalService.getInstance().stateListDrawableMaker(this, R.drawable.music_stop, R.drawable.music_stop_touch));
        ibtn_next.setImageDrawable(GlobalService.getInstance().stateListDrawableMaker(this, R.drawable.music_next, R.drawable.music_next_touch));
        ibtn_before.setImageDrawable(GlobalService.getInstance().stateListDrawableMaker(this, R.drawable.music_back, R.drawable.music_back_touch));
        ibtn_listup.setImageDrawable(GlobalService.getInstance().stateListDrawableMaker(this, R.drawable.music_list_up, R.drawable.music_list_up_touch));
        ibtn_listdown.setImageDrawable(GlobalService.getInstance().stateListDrawableMaker(this, R.drawable.music_list_down, R.drawable.music_list_down_touch));


        Glide.with(this).load(R.drawable.music_list_icon).centerCrop().into(iv_listicon);
        Glide.with(this).load(R.drawable.music_background).centerCrop().into(iv_background);
        Glide.with(this).load(R.drawable.music_play).centerCrop().into(iv_play);

        Drawable dp = getResources().getDrawable(R.drawable.music_play_slide);
        Bitmap bitmap = ((BitmapDrawable)dp).getBitmap();
        Drawable newdp = new BitmapDrawable(getResources(), Bitmap.createScaledBitmap(bitmap, bitmap.getWidth()/3, bitmap.getHeight()/3, true ));
        sb_playbar.setThumb(newdp);

        Drawable ds = getResources().getDrawable(R.drawable.music_sound_slide);
        Bitmap bitmap2 = ((BitmapDrawable)ds).getBitmap();
        Drawable newds = new BitmapDrawable(getResources(), Bitmap.createScaledBitmap(bitmap2, bitmap2.getWidth()/3, bitmap2.getHeight()/3, true ));
        sb_soundbar.setThumb(newds);

        repeatBitmap[0] = BitmapFactory.decodeResource(this.getResources(), R.drawable.music_repeat);
        repeatBitmap[1] = BitmapFactory.decodeResource(this.getResources(), R.drawable.music_one_repeat);

        shuffleBitmap[0] = BitmapFactory.decodeResource(this.getResources(), R.drawable.music_shuffle);
        shuffleBitmap[1] = BitmapFactory.decodeResource(this.getResources(), R.drawable.music_shuffle_selected);

        playBitmap[0] = BitmapFactory.decodeResource(this.getResources(), R.drawable.music_play);
        playBitmap[1] = BitmapFactory.decodeResource(this.getResources(), R.drawable.music_pause);


        tv_starttime.setText("00:00");
        tv_endtime.setText("00:00");

        ibtn_before.setOnClickListener(this);
        ibtn_listdown.setOnClickListener(this);
        ibtn_listup.setOnClickListener(this);
        ibtn_next.setOnClickListener(this);
        ibtn_stop.setOnClickListener(this);
        iv_play.setOnClickListener(this);
        iv_repeat.setOnClickListener(this);
        iv_shuffle.setOnClickListener(this);

        sb_soundbar.setOnSeekBarChangeListener(new SeekBar.OnSeekBarChangeListener() {

            @Override
            public void onProgressChanged(SeekBar seekBar, int i, boolean b) {
                String message = "Music_seekbar_sound_"+i;
                GlobalNetworkService.getInstance().sendMessageToServer(message);
            }

            @Override
            public void onStartTrackingTouch(SeekBar seekBar) {

            }

            @Override
            public void onStopTrackingTouch(SeekBar seekBar) {
                GlobalNetworkService.getInstance().sendMessageToServer("Music_seekbar_sound_"+seekBar.getProgress());
            }
        });

        sb_playbar.setOnSeekBarChangeListener(new SeekBar.OnSeekBarChangeListener() {
            @Override
            public void onProgressChanged(SeekBar seekBar, int i, boolean b) {
                if(isPlaying==false) {
                    String message = "Music_seekbar_player_" + i;
                    GlobalNetworkService.getInstance().sendMessageToServer(message);
                    Log.d("a", message);
                }
            }

            @Override
            public void onStartTrackingTouch(SeekBar seekBar) {

            }

            @Override
            public void onStopTrackingTouch(SeekBar seekBar) {

            }
        });

    }

    @Override
    public void onClick(View view) {
        String message = "Music_";
        switch (view.getId())
        {
            case R.id.ibtn_music_before:
                message += "b";
                break;
            case R.id.iv_music_play:
                playCount++;
                Log.e("error",playCount+"a");
                if(playCount%2==0)
                {
                    Glide.with(this).load(R.drawable.music_play).centerCrop().into(iv_play);
                }
                else
                {
                    Glide.with(this).load(R.drawable.music_pause).centerCrop().into(iv_play);
                }
                message += "p";
                break;

            case R.id.ibtn_music_stop:
                Glide.with(this).load(R.drawable.music_play).centerCrop().into(iv_play);
                playCount = 0;
                message += "st";
                break;

            case R.id.ibtn_music_next:
                message += "n";
                break;

            case R.id.ibtn_music_listup:
                message += "lu";
                break;
            case R.id.ibtn_music_listdown:
                message += "ld";
                break;

            case R.id.iv_music_repeat:
                repeatCount++;
                iv_repeat.setImageBitmap(repeatBitmap[repeatCount%2]);
                message += "r";
                break;

            case R.id.iv_music_shuffle:
                message += "su";
                shuffleCount++;
                iv_shuffle.setImageBitmap(shuffleBitmap[shuffleCount%2]);
                break;
        }
        GlobalNetworkService.getInstance().sendMessageToServer(message);
        Vibrator vibe = (Vibrator) getSystemService(Context.VIBRATOR_SERVICE);
        vibe.vibrate(100);
    }

    @Override
    public void receiveMessageFromServer(String message) {
        String[] tokenize = message.split("_");
        if(tokenize[0].equals("Music") )
        {
            if(tokenize[1].equals("Controll"))
            {
                tv_title.setText(tokenize[2]);
                tv_album.setText(tokenize[3]);
                tv_endtime.setText(tokenize[4]);
            }
            else if(tokenize[1].equals("Timer"))
            {
                isPlaying = true;
                sb_playbar.setProgress(Integer.parseInt(tokenize[2]));
                sb_playbar.setMax(Integer.parseInt(tokenize[3]));
                tv_starttime.setText((tokenize[4]));
                isPlaying = false;
            }
        }
    }
}
