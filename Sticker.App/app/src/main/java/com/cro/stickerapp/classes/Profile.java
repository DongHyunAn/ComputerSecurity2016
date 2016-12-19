package com.cro.stickerapp.classes;

import android.content.SharedPreferences;
import android.os.Build;
import android.util.Log;

import com.cro.stickerapp.global.GlobalEngine;

/**
 * Created by Cro on 2016-08-19.
 */
public class Profile {
    private String _playerName;
    private int _profileThumbnailNum;

    public Profile() {
        try {
            SharedPreferences profile = GlobalEngine.getInstance().getSharedPreferences("profile");

            _playerName = profile.getString("playerName", Build.MODEL);
            _profileThumbnailNum = profile.getInt("ThumbnailNum", 0);

            Log.d("LoadProfile...", _playerName + ", " + Integer.toString(_profileThumbnailNum));
        }catch (Exception e)
        {
            _playerName = Build.MODEL;
            _profileThumbnailNum = 0;
        }
        Log.d("LoadProfile...", _playerName + ", " + Integer.toString(_profileThumbnailNum));
    }

    public void makeProfile(String playerName, int profileThumbnailNum){
        _playerName = playerName;
        _profileThumbnailNum = profileThumbnailNum;

        SharedPreferences.Editor editor =GlobalEngine.getInstance().getSharedPreferences("profile").edit();

        editor.putString("playerName", playerName);
        editor.putInt("ThumbnailNum", profileThumbnailNum);

        Log.d("SaveProfile...", _playerName + ", " + Integer.toString(_profileThumbnailNum));

        editor.apply();
    }

    //region Getter
    public String get_playerName() {
        return _playerName;
    }

    public int get_profileImageNum() {
        return _profileThumbnailNum;
    }

    //endregion
}
