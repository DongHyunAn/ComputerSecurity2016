package com.cro.stickerapp.rpsModule;

import android.app.AlertDialog;
import android.content.DialogInterface;
import android.os.Bundle;
import android.os.Handler;
import android.util.Log;
import android.view.View;
import android.widget.ImageButton;
import android.widget.ImageView;
import android.widget.RelativeLayout;

import com.bumptech.glide.Glide;
import com.bumptech.glide.load.DecodeFormat;
import com.bumptech.glide.load.engine.DiskCacheStrategy;
import com.cro.stickerapp.R;
import com.cro.stickerapp.classes.BaseActivity;
import com.cro.stickerapp.classes.NetworkMessageReceiver;
import com.cro.stickerapp.global.GlobalEngine;
import com.cro.stickerapp.global.GlobalNetworkService;
import com.cro.stickerapp.global.GlobalService;

import butterknife.BindView;
import butterknife.ButterKnife;
import butterknife.OnClick;

public class RPSWaitActivity extends BaseActivity implements NetworkMessageReceiver {

    //region ButterKnife

    @BindView(R.id.img_RPSwait_background)
    ImageView imgRPSwaitBackground;
    @BindView(R.id.img_RPSwait_loading)
    ImageView imgRPSwaitLoading;
    @BindView(R.id.img_RPSwait_logo)
    ImageView imgRPSwaitLogo;

    @BindView(R.id.img_RPSwait_back)
    ImageButton imgRPSwaitBack;
    @BindView(R.id.ibtn_RPSwait_start)
    ImageButton ibtnRPSwaitStart;

    @BindView(R.id.rel_RPSwait_startLayout)
    RelativeLayout relRPSwaitStartLayout;

    //endregion

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_rpswait);
        ButterKnife.bind(this);

        initializeView();
    }

    private void initializeView() {
        Glide.with(this).load(R.drawable.rps_img_background).asBitmap().format(DecodeFormat.PREFER_ARGB_8888).diskCacheStrategy(DiskCacheStrategy.SOURCE).centerCrop().into(imgRPSwaitBackground);
        Glide.with(this).load(R.drawable.loading).centerCrop().into(imgRPSwaitLoading);
        Glide.with(this).load(R.drawable.rps_img_logo).centerCrop().into(imgRPSwaitLogo);
        Glide.with(this).load(R.drawable.rps_btn_back).centerCrop().into(imgRPSwaitBack);

        ibtnRPSwaitStart.setImageDrawable(GlobalService.getInstance().stateListDrawableMaker(this, R.drawable.rps_btn_box, R.drawable.rps_btn_box_select));

        relRPSwaitStartLayout.setAlpha(0);
        ibtnRPSwaitStart.setEnabled(false);

        final Handler handler = new Handler();

        new Thread() {
            @Override
            public void run() {
                try {
                    while (RPSWaitActivity.this.equals(GlobalEngine.getInstance().getNowActivity())) {
                        handler.post(new Runnable() {
                            @Override
                            public void run() {
                                imgRPSwaitLoading.setRotation(imgRPSwaitLoading.getRotation() + 30.f);
                            }
                        });
                        Thread.sleep(200);
                    }
                } catch (Exception e) {
                    Log.d("RPSWaitActivity", "Loading animation end");
                }
            }
        }.start();
    }

    @OnClick({R.id.img_RPSwait_back, R.id.ibtn_RPSwait_start})
    public void onClick(View view) {
        switch (view.getId()) {
            case R.id.img_RPSwait_back: {
                    AlertDialog.Builder connectedBuilder = new AlertDialog.Builder(RPSWaitActivity.this);

                    connectedBuilder.setPositiveButton("예", new DialogInterface.OnClickListener() {
                        @Override
                        public void onClick(DialogInterface dialogInterface, int i) {
                            GlobalNetworkService.getInstance().sendMessageToServer("RPS_Exit");
                        }
                    });

                    connectedBuilder.setNegativeButton("아니오", new DialogInterface.OnClickListener() {
                        @Override
                        public void onClick(DialogInterface dialogInterface, int i) {
                            // do nothing
                        }
                    });

                    connectedBuilder.setMessage("정말 게임을 종료하시겠습니까?").setTitle("Sticker");
                    _backConfirmDialog = connectedBuilder.create();
                    _backConfirmDialog.show();
                } break;
            case R.id.ibtn_RPSwait_start:
                {
                    GlobalNetworkService.getInstance().sendMessageToServer("RPS_GameStart");
                }break;
        }
    }

    AlertDialog _backConfirmDialog;

    @Override
    protected void onPause() {
        closeDialog();
        super.onPause();
    }

    private void closeDialog() {
        if (_backConfirmDialog != null) {
            _backConfirmDialog.dismiss();
            _backConfirmDialog = null;
        }
    }

    @Override
    public void receiveMessageFromServer(String message) {
        String[] tokenize = message.split("_");

        if (tokenize[0].equals("RPS") && tokenize[1].equals("Wait")) {
            switch (tokenize[2]) {
                case "Ready": {
                    if (!ibtnRPSwaitStart.isEnabled()) {
                        relRPSwaitStartLayout.setAlpha(1);
                        ibtnRPSwaitStart.setEnabled(true);
                    }
                }break;
                case "Wait": {
                    if (ibtnRPSwaitStart.isEnabled()) {
                        relRPSwaitStartLayout.setAlpha(0);
                        ibtnRPSwaitStart.setEnabled(false);
                    }
                }break;
            }
        }
    }
}
