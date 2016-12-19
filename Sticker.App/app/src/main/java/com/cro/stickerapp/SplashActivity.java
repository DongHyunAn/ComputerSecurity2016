package com.cro.stickerapp;

import android.os.Bundle;
import android.widget.ImageView;
import com.bumptech.glide.Glide;
import com.bumptech.glide.load.DecodeFormat;
import com.bumptech.glide.load.engine.DiskCacheStrategy;
import com.cro.stickerapp.classes.BaseActivity;
import com.cro.stickerapp.global.GlobalEngine;
import com.cro.stickerapp.oneCardModule.OneCardControllerActivity;

public class SplashActivity extends BaseActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_splash);

        initializeView();
        GlobalEngine.getInstance().requestNavigate(WaitDeviceActivity.class, 1500);
    }

    private void initializeView() {
        ImageView img_background = (ImageView) findViewById(R.id.img_splash_background);
        ImageView img_logo = (ImageView) findViewById(R.id.img_splash_logo);

        Glide.with(this).load(R.drawable.img_logo_background).asBitmap().format(DecodeFormat.PREFER_ARGB_8888).diskCacheStrategy(DiskCacheStrategy.SOURCE).centerCrop().into(img_background);
        Glide.with(this).load(R.drawable.img_logo).centerCrop().into(img_logo);
    }
}

