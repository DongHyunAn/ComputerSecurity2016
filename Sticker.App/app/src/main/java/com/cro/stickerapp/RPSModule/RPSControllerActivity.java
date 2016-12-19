package com.cro.stickerapp.rpsModule;

import android.app.Activity;
import android.app.AlertDialog;
import android.content.Context;
import android.content.DialogInterface;
import android.os.Bundle;
import android.os.Vibrator;
import android.support.v7.widget.LinearLayoutManager;
import android.support.v7.widget.RecyclerView;
import android.support.v7.widget.helper.ItemTouchHelper;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageButton;
import android.widget.ImageView;
import android.widget.RelativeLayout;

import com.bumptech.glide.Glide;
import com.bumptech.glide.load.DecodeFormat;
import com.bumptech.glide.load.engine.DiskCacheStrategy;
import com.cro.stickerapp.R;
import com.cro.stickerapp.classes.BaseActivity;
import com.cro.stickerapp.classes.NetworkMessageReceiver;
import com.cro.stickerapp.global.GlobalNetworkService;
import com.cro.stickerapp.global.GlobalService;

import java.util.ArrayList;

import butterknife.BindView;
import butterknife.ButterKnife;

public class RPSControllerActivity extends BaseActivity implements NetworkMessageReceiver {

    //region ButterKnife
    @BindView(R.id.img_RPScontroller_background)
    ImageView imgRPScontrollerBackground;
    @BindView(R.id.img_RPScontroller_logo)
    ImageView imgRPScontrollerLogo;
    @BindView(R.id.img_RPScontroller_back)
    ImageButton imgRPScontrollerBack;

    @BindView(R.id.img_RPScontroller_popup_bg)
    ImageView imgRPScontrollerPopupBg;
    @BindView(R.id.ibtn_RPScontroller_popup_out)
    ImageButton ibtnRPScontrollerPopupOut;
    @BindView(R.id.ibtn_RPScontroller_popup_retry)
    ImageButton ibtnRPScontrollerPopupRetry;
    @BindView(R.id.rel_RPScontroller_popup)
    RelativeLayout relRPScontrollerPopup;
    @BindView(R.id.img_RPScontroller_popup2_bg)
    ImageView imgRPScontrollerPopup2Bg;
    @BindView(R.id.rel_RPScontroller_popup2)
    RelativeLayout relRPScontrollerPopup2;
    @BindView(R.id.rel_RPScontroller_blackPanel)
    RelativeLayout relRPScontrollerBlackPanel;

    @BindView(R.id.img_RPScontroller_arrow)
    ImageView imgRPScontrollerArrow;
    @BindView(R.id.rv_RPScontroller_cardList)
    RecyclerView rvRPScontrollerCardList;
    //endregion

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_rpscontroller);
        ButterKnife.bind(this);

        initializeView();
    }

    private void initializeView() {
        Glide.with(this).load(R.drawable.rps_img_background).asBitmap().format(DecodeFormat.PREFER_ARGB_8888).diskCacheStrategy(DiskCacheStrategy.SOURCE).centerCrop().into(imgRPScontrollerBackground);
        Glide.with(this).load(R.drawable.rps_img_popup).centerCrop().into(imgRPScontrollerPopupBg);
        Glide.with(this).load(R.drawable.rps_img_popup2).centerCrop().into(imgRPScontrollerPopup2Bg);
        Glide.with(this).load(R.drawable.rps_img_logo).centerCrop().into(imgRPScontrollerLogo);
        Glide.with(this).load(R.drawable.rps_btn_back).centerCrop().into(imgRPScontrollerBack);
        Glide.with(this).load(R.drawable.rps_img_arrow).centerCrop().into(imgRPScontrollerArrow);

        ibtnRPScontrollerPopupOut.setImageDrawable(GlobalService.getInstance().stateListDrawableMaker(this, R.drawable.rps_btn_box, R.drawable.rps_btn_box_select));
        ibtnRPScontrollerPopupRetry.setImageDrawable(GlobalService.getInstance().stateListDrawableMaker(this, R.drawable.rps_btn_box, R.drawable.rps_btn_box_select));

        View.OnClickListener popupListener = new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                if (view.getId() == R.id.ibtn_RPScontroller_popup_out) {
                    GlobalNetworkService.getInstance().sendMessageToServer("RPS_Exit");
                } else if (view.getId() == R.id.ibtn_RPScontroller_popup_retry) {
                    GlobalNetworkService.getInstance().sendMessageToServer("RPS_Replay");
                }
            }
        };
        ibtnRPScontrollerPopupOut.setOnClickListener(popupListener);
        ibtnRPScontrollerPopupRetry.setOnClickListener(popupListener);

        imgRPScontrollerBack.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                AlertDialog.Builder connectedBuilder = new AlertDialog.Builder(RPSControllerActivity.this);

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
            }
        });

        this.setState(RPSControllerState.Controller);

        LinearLayoutManager layoutManager = new LinearLayoutManager(this);
        layoutManager.setOrientation(LinearLayoutManager.HORIZONTAL);
        rvRPScontrollerCardList.setLayoutManager(layoutManager);

        ArrayList<Integer> arrayList = new ArrayList<>();
        arrayList.add(R.drawable.rps_img_rock);
        arrayList.add(R.drawable.rps_img_scissor);
        arrayList.add(R.drawable.rps_img_paper);

        RecyclerViewAdapter adapter = new RecyclerViewAdapter(arrayList, this);
        rvRPScontrollerCardList.setAdapter(adapter);

        ItemTouchHelper.Callback callback = new ItemTouchHelperCallback(adapter);
        ItemTouchHelper helper = new ItemTouchHelper(callback);
        helper.attachToRecyclerView(rvRPScontrollerCardList);
    }

    AlertDialog _backConfirmDialog;

    @Override
    protected void onPause() {
        if (_backConfirmDialog != null) {
            _backConfirmDialog.dismiss();
            _backConfirmDialog = null;
        }

        super.onPause();
    }

    public enum RPSControllerState {
        None,
        Controller,
        Wait,
        Popup
    }

    RPSControllerState _controllerState;

    @Override
    public void receiveMessageFromServer(String message) {
        String[] tokenize = message.split("_");

        if (tokenize[0].equals("RPS") && tokenize[1].equals("Controller")) {
            switch (tokenize[2]) {
                case "Controller": {
                    setState(RPSControllerState.Controller);
                }break;
                case "Popup": {
                    setState(RPSControllerState.Popup);
                }break;
                default:
                    break;
            }
        }
    }

    public void setState(RPSControllerState state) {
        switch (state) {
            case Controller: {
                relRPScontrollerBlackPanel.setVisibility(View.INVISIBLE);
                relRPScontrollerBlackPanel.setClickable(false);

                ArrayList<Integer> arrayList = new ArrayList<>();
                arrayList.add(R.drawable.rps_img_rock);
                arrayList.add(R.drawable.rps_img_scissor);
                arrayList.add(R.drawable.rps_img_paper);

                if(rvRPScontrollerCardList.getAdapter() instanceof RecyclerViewAdapter)
                {
                    RecyclerViewAdapter adapter = (RecyclerViewAdapter)rvRPScontrollerCardList.getAdapter();
                    adapter.setNewList(arrayList);
                }
            }
            break;
            case Wait: {
                relRPScontrollerBlackPanel.setClickable(true);
                relRPScontrollerBlackPanel.setVisibility(View.VISIBLE);

                relRPScontrollerPopup.setVisibility(View.INVISIBLE);
                relRPScontrollerPopup2.setVisibility(View.VISIBLE);
            }
            break;
            case Popup: {
                relRPScontrollerBlackPanel.setClickable(true);
                relRPScontrollerBlackPanel.setVisibility(View.VISIBLE);

                relRPScontrollerPopup.setVisibility(View.VISIBLE);
                relRPScontrollerPopup2.setVisibility(View.INVISIBLE);
            }
            break;
            case None:
            default:
                break;
        }
        _controllerState = state;
    }
}

interface ItemTouchHelperAdapter {
    void onItemDismiss(int position);

    void onItemMove(int fromPosition, int toPosition);
}

class ItemTouchHelperCallback extends ItemTouchHelper.Callback {
    ItemTouchHelperAdapter mAdapter;

    public ItemTouchHelperCallback(ItemTouchHelperAdapter mAdapter) {
        this.mAdapter = mAdapter;
    }

    @Override
    public boolean isLongPressDragEnabled() {
        return false;
    }

    @Override
    public boolean isItemViewSwipeEnabled() {
        return true;
    }

    @Override
    public int getMovementFlags(RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder) {
        int dragFlags =  ItemTouchHelper.START | ItemTouchHelper.END;
        int swipeFlags = ItemTouchHelper.UP; //| ItemTouchHelper.DOWN
        return makeMovementFlags(dragFlags, swipeFlags);
    }

    @Override
    public boolean onMove(RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder, RecyclerView.ViewHolder target) {
        mAdapter.onItemMove(viewHolder.getAdapterPosition(), target.getAdapterPosition());
        return true;
    }

    @Override
    public void onSwiped(RecyclerView.ViewHolder viewHolder, int direction) {
        mAdapter.onItemDismiss(viewHolder.getAdapterPosition());
    }
}

class RecyclerViewAdapter extends RecyclerView.Adapter<RecyclerViewAdapter.RecyclerViewHolder> implements ItemTouchHelperAdapter {

    ArrayList<Integer> mItemList;
    RPSControllerActivity mActivity;

    public RecyclerViewAdapter(ArrayList<Integer> mItemList, RPSControllerActivity mActivity) {
        this.mItemList = mItemList;
        this.mActivity = mActivity;
    }

    public void setNewList(ArrayList<Integer> list)
    {
        mItemList.clear();
        this.mItemList = list;
        this.notifyDataSetChanged();
    }

    @Override
    public RecyclerViewHolder onCreateViewHolder(ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(mActivity).inflate(R.layout.item_card, parent, false);
        return new RecyclerViewHolder(view);
    }

    @Override
    public void onBindViewHolder(RecyclerViewHolder holder, int position) {
        try
        {
            Glide.with(mActivity).load(mItemList.get(position)).into(holder.ivItemCard);
        }catch (Exception e)
        {
            // do nothings...
        }
    }

    @Override
    public int getItemCount() {
        return mItemList.size();
    }

    @Override
    public void onItemDismiss(int position) {
        String message = "";
        switch (mItemList.get(position)) {
            case R.drawable.rps_img_rock:
                message = "RPS_Card_R";
                break;
            case R.drawable.rps_img_scissor:
                message = "RPS_Card_S";
                break;
            case R.drawable.rps_img_paper:
                message = "RPS_Card_P";
                break;
            default:
                break;
        }

        if(message.equals(""))
        {
            return;
        }
        mItemList.remove(position);
        notifyItemRemoved(position);

        mActivity.setState(RPSControllerActivity.RPSControllerState.Wait);

        GlobalNetworkService.getInstance().sendMessageToServer(message);

        Vibrator vibe = (Vibrator) mActivity.getSystemService(Context.VIBRATOR_SERVICE);
        vibe.vibrate(100);
    }

    @Override
    public void onItemMove(int fromPosition, int toPosition) {
//        Integer fromItem = mItemList.get(fromPosition);
//        Integer toItem = mItemList.get(toPosition);
//        mItemList.set(fromPosition, toItem);
//        mItemList.set(toPosition, fromItem);
//
//        notifyItemMoved(fromPosition, toPosition);
    }

    public static class RecyclerViewHolder extends RecyclerView.ViewHolder {

        @BindView(R.id.iv_item_card)
        ImageView ivItemCard;

        public RecyclerViewHolder(View itemView) {
            super(itemView);
            ButterKnife.bind(this, itemView);
        }
    }
}