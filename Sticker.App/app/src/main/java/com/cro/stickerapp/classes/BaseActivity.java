package com.cro.stickerapp.classes;

import android.app.Activity;
import android.app.AlertDialog;
import android.content.DialogInterface;
import android.os.Bundle;
import android.view.WindowManager;

import com.cro.stickerapp.global.GlobalEngine;
import com.cro.stickerapp.global.GlobalNetworkService;

public class BaseActivity extends Activity {
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        GlobalEngine.getInstance().setNowActivity(this);
        getWindow().addFlags(WindowManager.LayoutParams.FLAG_KEEP_SCREEN_ON);
    }

    AlertDialog _connectedDialog;

    @Override
    protected void onPause() {
        super.onPause();

        if(_connectedDialog !=null) {
            _connectedDialog.dismiss();
            _connectedDialog = null;
        }
//        if(GlobalNetworkService.getInstance().getTCPClient()!=null)
//        {
//            if(_connectedDialog !=null) {
//                _connectedDialog.dismiss();
//                _connectedDialog = null;
//            }
//
//            GlobalEngine.getInstance().finishApp("어플리케이션이 종료되었습니다. 서버와의 연결이 해제되었습니다.");
//        }
    }

    @Override
    public void onBackPressed() {
        AlertDialog.Builder connectedBuilder = new AlertDialog.Builder(this);

        connectedBuilder.setPositiveButton("예", new DialogInterface.OnClickListener() {
            @Override
            public void onClick(DialogInterface dialogInterface, int i) {
                GlobalEngine.getInstance().finishApp("Sticker 연결을 종료합니다.");
            }
        });

        connectedBuilder.setNegativeButton("아니오", new DialogInterface.OnClickListener() {
            @Override
            public void onClick(DialogInterface dialogInterface, int i) {
                // do nothing
            }
        });

        connectedBuilder.setMessage("정말 종료하시겠습니까?").setTitle("Sticker");
        _connectedDialog = connectedBuilder.create();
        _connectedDialog.show();
    }
}
