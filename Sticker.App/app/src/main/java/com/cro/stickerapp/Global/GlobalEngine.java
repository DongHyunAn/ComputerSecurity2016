package com.cro.stickerapp.global;

import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.os.Handler;
import android.preference.PreferenceManager;
import android.util.Log;
import android.widget.Toast;

import com.cro.stickerapp.classes.Profile;
import com.cro.stickerapp.classes.NetworkMessageReceiver;
import com.cro.stickerapp.R;

public class GlobalEngine {

    //region Singleton
    private static GlobalEngine _instance = new GlobalEngine();

    public static GlobalEngine getInstance() {
        return _instance;
    }

    private GlobalEngine() {
    }
    //endregion

    public SharedPreferences getSharedPreferences(String xmlFileName) {
        Log.d("loadedClass", _nowActivity.getLocalClassName());
        return _nowActivity.getSharedPreferences(xmlFileName, Context.MODE_PRIVATE);
    }

    private Activity _nowActivity;

    public void setNowActivity(Activity activity) {
        this._nowActivity = activity;
        _isNavigating = false;
    }

    public Activity getNowActivity() {
        return _nowActivity;
    }

    private boolean _isNavigating = false;

    public void requestNavigate(Class<?> cls, int delay) {
        if (_nowActivity.getClass() == cls || _isNavigating) {
            return;
        }
        _isNavigating = true;

        final Activity oldActivity = _nowActivity;
        final Class<?> classType = cls;
        final int delayTime = delay;

        oldActivity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                Handler handler = new Handler();
                handler.postDelayed(new Runnable() {
                    @Override
                    public void run() {
                        oldActivity.startActivity(new Intent(oldActivity, classType));
                        oldActivity.overridePendingTransition(R.anim.fade_in, R.anim.fade_out);
                        oldActivity.finish();
                    }
                }, delayTime);
            }
        });
    }

    private Profile _profile;

    public Profile get_profile()
    {
        if(_profile==null)
        {
            _profile = new Profile();
        }
        return _profile;
    }

    public void sendMessageToActivity(final String message)
    {
        if(_nowActivity instanceof NetworkMessageReceiver)
        {
            _nowActivity.runOnUiThread(new Runnable() {
                @Override
                public void run() {
                    NetworkMessageReceiver receiver = (NetworkMessageReceiver)_nowActivity;
                    receiver.receiveMessageFromServer(message);
                }
            });
        }
    }

    public void finishApp(final String message)
    {
        GlobalNetworkService.getInstance().sendMessageToServer("Player_connectionClose");

        if(_nowActivity!=null)
        {
            _nowActivity.runOnUiThread(new Runnable() {
                @Override
                public void run() {
                    if(message!=null) {
                        Toast.makeText(_nowActivity, message, Toast.LENGTH_SHORT).show();
                    }
                    _nowActivity.finish();
                }
            });
        }

        try
        {
            Thread.sleep(2);
            GlobalNetworkService.getInstance().getTCPClient().closeConnect();
        }catch (Exception e)
        {
            // do nothing
        }

        android.os.Process.killProcess(android.os.Process.myPid());
    }
}
