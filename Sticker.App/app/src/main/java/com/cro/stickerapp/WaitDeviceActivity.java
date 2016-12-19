package com.cro.stickerapp;

import android.app.AlertDialog;
import android.content.DialogInterface;
import android.os.Bundle;
import android.os.Handler;
import android.util.Log;
import android.widget.ImageView;
import android.widget.TextView;
import android.widget.Toast;

import com.bumptech.glide.Glide;
import com.bumptech.glide.load.DecodeFormat;
import com.bumptech.glide.load.engine.DiskCacheStrategy;
import com.cro.stickerapp.classes.BaseActivity;
import com.cro.stickerapp.global.GlobalEngine;
import com.cro.stickerapp.global.GlobalNetworkService;

public class WaitDeviceActivity extends BaseActivity {

    Handler _handler;
    AlertDialog _connectedDialog;

    TextView _tv_stateMessage;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_wait_device);

        initializeView();

        if(!GlobalNetworkService.getInstance().isConnectionSuccess()) {
            waitDevice();
        }else
        {
            setStateMessage("장치와 연결되었습니다. 서버의 응답을 기다리는 중입니다.");
        }
    }

    private void initializeView() {
        ImageView img_background = (ImageView) findViewById(R.id.img_waitdevice_background);
        final ImageView img_loading = (ImageView) findViewById(R.id.img_waitdevice_loading);
        _tv_stateMessage = (TextView) findViewById(R.id.tv_waitdevice_stateText);

        Glide.with(this).load(R.drawable.background).asBitmap().format(DecodeFormat.PREFER_ARGB_8888).diskCacheStrategy(DiskCacheStrategy.SOURCE).centerCrop().into(img_background);
        Glide.with(this).load(R.drawable.loading).override(94,94).into(img_loading);

        _handler = new Handler();

        new Thread() {
            @Override
            public void run() {
                try{
                    while(WaitDeviceActivity.this.equals(GlobalEngine.getInstance().getNowActivity())) {
                        _handler.post(new Runnable() {
                            @Override
                            public void run() {
                                img_loading.setRotation(img_loading.getRotation() + 30.f);
                            }
                        });
                        Thread.sleep(200);
                    }
                }catch (Exception e)
                {
                    Log.d("WaitDeviceActivity", "Loading animation end");
                }
            }
        }.start();

        setStateMessage("Sticker PC와 연결을 수행중입니다...");
    }

    public void setStateMessage(final String message)
    {
        WaitDeviceActivity.this.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                _tv_stateMessage.setText(message);
            }
        });
    }

    @Override
    protected void onPause() {
        closeDialog();
        super.onPause();
    }

    private void closeDialog()
    {
        if(_connectedDialog !=null) {
            _connectedDialog.dismiss();
            _connectedDialog = null;
        }
    }

    private void waitDevice() {
        GlobalNetworkService.getInstance().initUDPBroadcastMessageReceiver(new GlobalNetworkService.NetworkConnectionCallback<String>() {
            @Override
            public void connectionFinished(String result) {
                if (result == null) {
                    _handler.post(new Runnable() {
                        @Override
                        public void run() {
                            Toast.makeText(getApplicationContext(), "연결 설정에 실패하였습니다. 프로그램을 종료합니다.", Toast.LENGTH_LONG).show();
                            WaitDeviceActivity.this.finish();
                        }
                    });

                } else  {
                    AlertDialog.Builder connectedBuilder = new AlertDialog.Builder(WaitDeviceActivity.this);

                    connectedBuilder.setPositiveButton("예", new DialogInterface.OnClickListener() {
                        @Override
                        public void onClick(DialogInterface dialogInterface, int i) {
                            GlobalNetworkService.getInstance().initTCPConnection(new GlobalNetworkService.NetworkConnectionCallback<Boolean>() {
                            @Override
                                public void connectionFinished(Boolean result) {
                                    if(!result)
                                    {
                                        _handler.post(new Runnable() {
                                            @Override
                                            public void run() {
                                                Toast.makeText(getApplicationContext(), "연결 설정에 실패하였습니다. 프로그램을 종료합니다.", Toast.LENGTH_LONG).show();
                                                WaitDeviceActivity.this.finish();
                                            }
                                        });
                                    }else
                                    {
                                        _handler.post(new Runnable() {
                                            @Override
                                            public void run() {

                                                setStateMessage("장치와 연결되었습니다. 서버의 응답을 기다리는 중입니다.");
                                            }
                                        });
                                    }
                                }
                            });
                        }
                    });

                    connectedBuilder.setNegativeButton("아니오", new DialogInterface.OnClickListener() {
                        @Override
                        public void onClick(DialogInterface dialogInterface, int i) {
                            _handler.post(new Runnable() {
                                @Override
                                public void run() {
                                    Toast.makeText(getApplicationContext(), "연결 요청을 거절하였습니다. 프로그램을 종료합니다.", Toast.LENGTH_LONG).show();
                                    WaitDeviceActivity.this.finish();
                                }
                            });
                        }
                    });

                    connectedBuilder.setMessage("Sticker 장치(" + result.split("_")[2] + ")를 찾았습니다. 연결하시겠습니까?").setTitle("연결");
                    _connectedDialog = connectedBuilder.create();
                    _connectedDialog.show();
                }
            }
        });
    }
}
