package com.cro.stickerapp.baseModule;

import android.content.Context;
import android.os.Bundle;
import android.os.Vibrator;
import android.view.View;
import android.widget.ImageButton;
import android.widget.ImageView;

import com.bumptech.glide.Glide;
import com.bumptech.glide.load.DecodeFormat;
import com.bumptech.glide.load.engine.DiskCacheStrategy;
import com.cro.stickerapp.R;
import com.cro.stickerapp.classes.BaseActivity;
import com.cro.stickerapp.global.GlobalNetworkService;
import com.cro.stickerapp.global.GlobalService;

import butterknife.BindView;
import butterknife.ButterKnife;
import butterknife.OnClick;

public class BaseControllerActivity extends BaseActivity {

    //region ButterKnife

    @BindView(R.id.img_base_background)
    ImageView imgBaseBackground;
    @BindView(R.id.ibtn_base_up)
    ImageButton ibtnBaseUp;
    @BindView(R.id.ibtn_base_left)
    ImageButton ibtnBaseLeft;
    @BindView(R.id.ibtn_base_right)
    ImageButton ibtnBaseRight;
    @BindView(R.id.ibtn_base_down)
    ImageButton ibtnBaseDown;
    @BindView(R.id.ibtn_base_cancel)
    ImageButton ibtnBaseCancel;
    @BindView(R.id.ibtn_base_ok)
    ImageButton ibtnBaseOk;
    @BindView(R.id.ibtn_base_logo)
    ImageButton ibtnBaseLogo;

    //endregion

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_base_controller);
        ButterKnife.bind(this);

        initializeView();
    }

    protected void initializeView() {
        Glide.with(this).load(R.drawable.background).asBitmap().format(DecodeFormat.PREFER_ARGB_8888).diskCacheStrategy(DiskCacheStrategy.SOURCE).centerCrop().into(imgBaseBackground);

        ibtnBaseUp.setImageDrawable(GlobalService.getInstance().stateListDrawableMaker(this, R.drawable.btn_arrow, R.drawable.btn_arrow_over));
        ibtnBaseDown.setImageDrawable(GlobalService.getInstance().stateListDrawableMaker(this, R.drawable.btn_arrow, R.drawable.btn_arrow_over));
        ibtnBaseLeft.setImageDrawable(GlobalService.getInstance().stateListDrawableMaker(this, R.drawable.btn_arrow, R.drawable.btn_arrow_over));
        ibtnBaseRight.setImageDrawable(GlobalService.getInstance().stateListDrawableMaker(this, R.drawable.btn_arrow, R.drawable.btn_arrow_over));

        ibtnBaseOk.setImageDrawable(GlobalService.getInstance().stateListDrawableMaker(this, R.drawable.btn_okay, R.drawable.btn_okay_over));
        ibtnBaseCancel.setImageDrawable(GlobalService.getInstance().stateListDrawableMaker(this, R.drawable.btn_cancel, R.drawable.btn_cancel_over));

        Glide.with(this).load(R.drawable.btn_back).into(ibtnBaseLogo);
    }

    @OnClick({R.id.ibtn_base_up, R.id.ibtn_base_left, R.id.ibtn_base_right, R.id.ibtn_base_down, R.id.ibtn_base_cancel, R.id.ibtn_base_ok, R.id.ibtn_base_logo})
    public void onClick(View view) {
        String message = "Sticker_Command_";
        switch (view.getId()) {
            case R.id.ibtn_base_up:
                message += "u";
                break;
            case R.id.ibtn_base_down:
                message += "d";
                break;
            case R.id.ibtn_base_left:
                message += "l";
                break;
            case R.id.ibtn_base_right:
                message += "r";
                break;
            case R.id.ibtn_base_ok:
                message += "s";
                break;
            case R.id.ibtn_base_cancel:
                message += "c";
                break;
            case R.id.ibtn_base_logo:
                message += "logo";
                break;
        }

        GlobalNetworkService.getInstance().sendMessageToServer(message);

        Vibrator vibe = (Vibrator) getSystemService(Context.VIBRATOR_SERVICE);
        vibe.vibrate(100);
    }
}
