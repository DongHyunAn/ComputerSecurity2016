package com.cro.stickerapp.oneCardModule;

import android.app.AlertDialog;
import android.content.DialogInterface;
import android.os.Bundle;
import android.os.Handler;
import android.support.v7.app.AppCompatActivity;
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

public class OneCardWaitActivity extends BaseActivity implements NetworkMessageReceiver {

    //region ButterKnife
    @BindView(R.id.img_OCwait_background)
    ImageView imgOCwaitBackground;
    @BindView(R.id.img_OCwait_loading)
    ImageView imgOCwaitLoading;
    @BindView(R.id.img_OCwait_back)
    ImageButton imgOCwaitBack;
    @BindView(R.id.ibtn_OCwait_start)
    ImageButton ibtnOCwaitStart;
    @BindView(R.id.rel_OCwait_startLayout)
    RelativeLayout relOCwaitStartLayout;
    //endregion

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_one_card_wait);

        ButterKnife.bind(this);
        initializeView();
    }

    private void initializeView() {
        Glide.with(this).load(R.drawable.oc_background).asBitmap().format(DecodeFormat.PREFER_ARGB_8888).diskCacheStrategy(DiskCacheStrategy.SOURCE).centerCrop().into(imgOCwaitBackground);
        Glide.with(this).load(R.drawable.loading).into(imgOCwaitLoading);
        Glide.with(this).load(R.drawable.oc_back).into(imgOCwaitBack);
        Glide.with(this).load(R.drawable.oc_btn_back).into(ibtnOCwaitStart);

        relOCwaitStartLayout.setAlpha(0);
        ibtnOCwaitStart.setEnabled(false);

        final Handler handler = new Handler();

        new Thread() {
            @Override
            public void run() {
                try {
                    while (OneCardWaitActivity.this.equals(GlobalEngine.getInstance().getNowActivity())) {
                        handler.post(new Runnable() {
                            @Override
                            public void run() {
                                imgOCwaitLoading.setRotation(imgOCwaitLoading.getRotation() + 30.f);
                            }
                        });
                        Thread.sleep(200);
                    }
                } catch (Exception e) {
                    // do nothing...
                }
            }
        }.start();
    }

    AlertDialog _backConfirmDialog;

    @OnClick({R.id.img_OCwait_back, R.id.ibtn_OCwait_start})
    public void onClick(View view) {
        switch (view.getId()) {
            case R.id.img_OCwait_back: {
                    AlertDialog.Builder connectedBuilder = new AlertDialog.Builder(OneCardWaitActivity.this);

                    connectedBuilder.setPositiveButton("예", new DialogInterface.OnClickListener() {
                        @Override
                        public void onClick(DialogInterface dialogInterface, int i) {
                            GlobalNetworkService.getInstance().sendMessageToServer("OneCard_Exit");
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
            case R.id.ibtn_OCwait_start:
                    GlobalNetworkService.getInstance().sendMessageToServer("OneCard_GameStart");
                break;
        }
    }

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

        if (tokenize[0].equals("OneCard") && tokenize[1].equals("Wait")) {
            switch (tokenize[2]) {
                case "Ready": {
                    if (!ibtnOCwaitStart.isEnabled()) {
                        relOCwaitStartLayout.setAlpha(1);
                        ibtnOCwaitStart.setEnabled(true);
                    }
                }break;
                case "Wait": {
                    if (ibtnOCwaitStart.isEnabled()) {
                        relOCwaitStartLayout.setAlpha(0);
                        ibtnOCwaitStart.setEnabled(false);
                    }
                }break;
            }
        }
    }
}
