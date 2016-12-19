package com.cro.stickerapp.global;

import android.app.Activity;
import android.content.Context;
import android.graphics.Bitmap;
import android.graphics.Color;
import android.graphics.drawable.BitmapDrawable;
import android.graphics.drawable.Drawable;
import android.graphics.drawable.StateListDrawable;
import android.text.Editable;
import android.text.TextWatcher;
import android.util.Log;
import android.view.KeyEvent;
import android.view.ViewGroup;
import android.view.inputmethod.EditorInfo;
import android.view.inputmethod.InputMethodManager;
import android.widget.EditText;
import android.widget.RelativeLayout;
import android.widget.TextView;

import com.bumptech.glide.Glide;
import com.bumptech.glide.request.animation.GlideAnimation;
import com.bumptech.glide.request.target.SimpleTarget;
import com.cro.stickerapp.R;
import com.cro.stickerapp.Secure.AES128;

import java.lang.reflect.Field;

public class GlobalService {

    //region Singleton
    private static GlobalService _instance = new GlobalService();

    public static GlobalService getInstance() {
        return _instance;
    }

    private GlobalService() {
    }
    //endregion

    public StateListDrawable stateListDrawableMaker(final Activity activity, Integer normalDrawable, Integer pressedDrawable)
    {
        final StateListDrawable state = new StateListDrawable();

        Glide.with(activity).load(pressedDrawable).asBitmap().into(new SimpleTarget<Bitmap>() {
            @Override
            public void onResourceReady(Bitmap resource, GlideAnimation<? super Bitmap> glideAnimation) {
                state.addState(new int[]{ android.R.attr.state_pressed }, new BitmapDrawable(activity.getResources(), resource));
            }
        });

        Glide.with(activity).load(normalDrawable).asBitmap().into(new SimpleTarget<Bitmap>() {
            @Override
            public void onResourceReady(Bitmap resource, GlideAnimation<? super Bitmap> glideAnimation) {
                state.addState(new int[]{ -android.R.attr.state_pressed }, new BitmapDrawable(activity.getResources(), resource));
            }
        });

        return state;
    }

    public void requestKeyboardService()
    {
        new Thread(){
            @Override
            public void run() {
                KeyboardService keyboardService = new KeyboardService(GlobalEngine.getInstance().getNowActivity());
                int count = 0;
                try{
                    while(count++ < 10)
                    {
                        Log.d("GlobalService", "keyboard Loading..");

                        if(keyboardService.isEnable())
                        {
                            keyboardService.OpenKeyboard();
                            break;
                        }
                        Thread.sleep(500);
                    }
                }catch (Exception e) {
                    // do nothings
                }
            }
        }.start();
    }

    public class KeyboardService
    {
        private EditText _editText;
        private Activity _activity;
        private Boolean _isEnable;

        public KeyboardService(Activity activity) {
            _activity = activity;
            _isEnable = false;

            _activity.runOnUiThread(new Runnable() {
                @Override
                public void run() {
                    initialize();
                }
            });
        }

        public Boolean isEnable()
        {
            return _isEnable;
        }

        private void initialize() {
            try
            {
                if(_editText==null)
                {
                    _editText = new EditText(_activity);
                    RelativeLayout.LayoutParams layoutParams = new RelativeLayout.LayoutParams(
                            RelativeLayout.LayoutParams.WRAP_CONTENT,
                            RelativeLayout.LayoutParams.WRAP_CONTENT);
                    _editText.setLayoutParams(layoutParams);

                    _editText.setBackgroundColor(Color.TRANSPARENT);
                    _editText.setTextColor(Color.TRANSPARENT);
                    _editText.setCursorVisible(false);

                    _editText.setEnabled(false);
                    _editText.setFocusable(true);

                    ViewGroup root = (ViewGroup)_activity.getWindow().getDecorView();
                    root.addView(_editText);

                    _editText.addTextChangedListener(new TextWatcher() {
                        //region Unused Method
                        @Override
                        public void beforeTextChanged(CharSequence charSequence, int i, int i1, int i2) {

                        }
                        @Override
                        public void onTextChanged(CharSequence charSequence, int i, int i1, int i2) {
                        }
                        //endregion

                        @Override
                        public void afterTextChanged(Editable editable) {
                            String encryptEditable = "";

                            try {
                                if(editable.toString().equals("") || editable.toString().equals(" ") || editable.toString() == null || editable.toString().equals(null)) {}
                                else {
                                    encryptEditable = AES128.encrypt(editable.toString(), AES128.key);
                                    System.out.println("editable : " + editable.toString());
                                    System.out.println("encrypt : " + encryptEditable);
                                }
                            } catch(Exception e) {
                                Log.d("GlobalNetworkService", "Profile Encrypt Error");
                            }
                            GlobalNetworkService.getInstance().sendMessageToServer("Sticker_Text_" + encryptEditable);
                        }
                    });

                    _editText.setOnEditorActionListener(new TextView.OnEditorActionListener() {
                        @Override
                        public boolean onEditorAction(TextView textView, int i, KeyEvent keyEvent) {
                            if(i== EditorInfo.IME_ACTION_DONE || i==EditorInfo.IME_NULL || keyEvent.getKeyCode() == KeyEvent.KEYCODE_ENTER) {
                                CloseKeyboard();
                                GlobalNetworkService.getInstance().sendMessageToServer("Sticker_Command_s");
                            }
                            return false;
                        }
                    });
                }
                _isEnable = true;
            }catch (Exception e)
            {
                _editText = null;
                _isEnable = false;
            }
        }

        private void OpenKeyboard() {
            if(_editText!=null) {
                _activity.runOnUiThread(new Runnable() {
                    @Override
                    public void run() {
                        _editText.setEnabled(true);
                        _editText.setText("");
                        _editText.requestFocus();

                        InputMethodManager imm = (InputMethodManager) _activity.getSystemService(Context.INPUT_METHOD_SERVICE);
                        imm.showSoftInput(_editText, InputMethodManager.SHOW_FORCED);
                    }
                });
            }
        }

        private void CloseKeyboard() {
            if(_editText!=null) {
                _activity.runOnUiThread(new Runnable() {
                    @Override
                    public void run() {
                        _editText.setEnabled(false);
                        InputMethodManager imm = (InputMethodManager) _activity.getSystemService(Context.INPUT_METHOD_SERVICE);
                        imm.hideSoftInputFromWindow(_editText.getWindowToken(), 0);
                    }
                });
            }
        }
    }

    public Integer getResourceByString(String str)
    {
        try {
            Class res = R.drawable.class;
            Field idField = res.getDeclaredField(str);
            return idField.getInt(null);
        } catch (Exception e) {
            e.printStackTrace();
            return -1;
        }
    }
}
