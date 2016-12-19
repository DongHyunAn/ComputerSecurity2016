package com.cro.stickerapp.oneCardModule;

import android.app.AlertDialog;
import android.content.Context;
import android.content.DialogInterface;
import android.graphics.Rect;
import android.os.Bundle;
import android.os.Vibrator;
import android.support.v7.widget.LinearLayoutManager;
import android.support.v7.widget.RecyclerView;
import android.support.v7.widget.helper.ItemTouchHelper;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageButton;
import android.widget.ImageView;
import android.widget.RelativeLayout;
import android.widget.Toast;

import com.bumptech.glide.Glide;
import com.bumptech.glide.load.DecodeFormat;
import com.bumptech.glide.load.engine.DiskCacheStrategy;
import com.cro.stickerapp.R;
import com.cro.stickerapp.classes.BaseActivity;
import com.cro.stickerapp.classes.NetworkMessageReceiver;
import com.cro.stickerapp.global.GlobalEngine;
import com.cro.stickerapp.global.GlobalNetworkService;
import com.cro.stickerapp.global.GlobalService;

import java.util.ArrayList;

import butterknife.BindView;
import butterknife.ButterKnife;
import butterknife.OnClick;

public class OneCardControllerActivity extends BaseActivity implements NetworkMessageReceiver {

    //region ButterKnife
    @BindView(R.id.img_OCcontroller_bg)
    ImageView imgOCcontrollerBg;
    @BindView(R.id.img_OCcontroller_back)
    ImageButton imgOCcontrollerBack;
    @BindView(R.id.img_OCcontroller_penalty)
    ImageButton imgOCcontrollerPenalty;
    @BindView(R.id.ibtn_OCcontroller_shape_s)
    ImageButton ibtnOCcontrollerShapeS;
    @BindView(R.id.ibtn_OCcontroller_shape_h)
    ImageButton ibtnOCcontrollerShapeH;
    @BindView(R.id.ibtn_OCcontroller_shape_c)
    ImageButton ibtnOCcontrollerShapeC;
    @BindView(R.id.ibtn_OCcontroller_shape_d)
    ImageButton ibtnOCcontrollerShapeD;

    @BindView(R.id.rv_OCcontroller_cardList)
    RecyclerView rvOCcontrollerCardList;

    @BindView(R.id.rel_OCcontroller_waitPlayer)
    RelativeLayout relOCcontrollerWaitPlayer;
    @BindView(R.id.rel_OCcontroller_selectShape)
    RelativeLayout relOCcontrollerSelectShape;
    @BindView(R.id.rel_OCcontroller_popup)
    RelativeLayout relOCcontrollerPopup;
    //endregion

    RecyclerViewAdapter _adapter;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_one_card_controller);
        ButterKnife.bind(this);

        initializeView();
    }

    private void initializeView() {
        Glide.with(this).load(R.drawable.oc_background).asBitmap().format(DecodeFormat.PREFER_ARGB_8888).diskCacheStrategy(DiskCacheStrategy.SOURCE).centerCrop().into(imgOCcontrollerBg);
        Glide.with(this).load(R.drawable.oc_back).into(imgOCcontrollerBack);
        imgOCcontrollerPenalty.setImageDrawable(GlobalService.getInstance().stateListDrawableMaker(this, R.drawable.img_oc_penalty_off, R.drawable.img_oc_penalty_on));

        Glide.with(this).load(R.drawable.shape_s).into(ibtnOCcontrollerShapeS);
        Glide.with(this).load(R.drawable.shape_h).into(ibtnOCcontrollerShapeH);
        Glide.with(this).load(R.drawable.shape_c).into(ibtnOCcontrollerShapeC);
        Glide.with(this).load(R.drawable.shape_d).into(ibtnOCcontrollerShapeD);

        this.setState(State.Wait);

        //region RecyclerView Init

        LinearLayoutManager layoutManager = new LinearLayoutManager(this);
        layoutManager.setOrientation(LinearLayoutManager.HORIZONTAL);
        rvOCcontrollerCardList.setLayoutManager(layoutManager);

        rvOCcontrollerCardList.addItemDecoration(new OneCardItemDecoration(200));

        ArrayList<OneCardItem> arrayList = new ArrayList<>();
        _adapter = new RecyclerViewAdapter(arrayList, this);
        rvOCcontrollerCardList.setAdapter(_adapter);

        ItemTouchHelper.Callback callback = new ItemTouchHelperCallback(_adapter);
        ItemTouchHelper helper = new ItemTouchHelper(callback);
        helper.attachToRecyclerView(rvOCcontrollerCardList);

        //endregion
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

    AlertDialog _backConfirmDialog;

    @OnClick({R.id.img_OCcontroller_back, R.id.img_OCcontroller_penalty})
    public void onClick(View view) {
        switch (view.getId()) {
            case R.id.img_OCcontroller_back: {
                AlertDialog.Builder connectedBuilder = new AlertDialog.Builder(OneCardControllerActivity.this);

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
            }
            break;
            case R.id.img_OCcontroller_penalty: {
                if (relOCcontrollerPopup.getVisibility() == View.INVISIBLE) {
                    setState(State.Wait);
                    GlobalNetworkService.getInstance().sendMessageToServer("OneCard_Controller_Penalty");
                }
            }
            break;
        }
    }

    @Override
    public void receiveMessageFromServer(String message) {
        String[] tokenize = message.split("_");

        if (tokenize[0].equals("OneCard") && tokenize[1].equals("Controller")) {
            switch (tokenize[2]) {
                case "drawCard": {
                    _adapter.getmItemList().add(new OneCardItem(tokenize[3]));
                    _adapter.notifyDataSetChanged();
                }
                break;
                case "isYourTurn": {
                    this.setState(State.Play);

                    String topCard = tokenize[3];
                    int penaltyNum = Integer.parseInt(tokenize[4]);

                    for (OneCardItem item : _adapter.getmItemList()) {
                        item.setSubmitable(isSubmitableChecker(item._cardName, topCard, penaltyNum));
                    }

                    _adapter.notifyDataSetChanged();
                }
                break;
                default:
                    break;
            }
        }
    }

    private boolean isSubmitableChecker(String cardName, String topCard, int num) {
        // cardName is joker

        if (cardName.charAt(0) == 'j') {
            return true;
        }

        if(topCard.charAt(0) == 'j' && num==0)
        {
            return true;
        }

        boolean isShapeCollect = (cardName.charAt(0) == topCard.charAt(0));
        boolean isNumCollect = (cardName.charAt(1) == topCard.charAt(1));

        if (num != 0)
        {
            // topCard is joker

            if (topCard.charAt(1) == 'j') {
                return cardName.equals("sa");
            }

            // topCard is a

            if (topCard.charAt(1) == 'a') {
                return (cardName.charAt(1) == '2' && isShapeCollect) || isNumCollect;
            }

            // topCard is 2

            if (topCard.charAt(1) == '2') {
                return (cardName.charAt(1) == 'a' && isShapeCollect) || isNumCollect;
            }
        }

        return isShapeCollect || isNumCollect;
    }

    @OnClick({R.id.ibtn_OCcontroller_shape_s, R.id.ibtn_OCcontroller_shape_h, R.id.ibtn_OCcontroller_shape_c, R.id.ibtn_OCcontroller_shape_d})
    public void onShapeClick(View view) {
        if(_sevenCardName == null)
        {
            GlobalEngine.getInstance().finishApp("게임 중 알 수 없는 에러가 발생하여 Sticker를 종료합니다.");
            return;
        }

        setState(OneCardControllerActivity.State.Wait);

        String message = "OneCard_Controller_Submit_" + _sevenCardName;

        switch (view.getId()) {
            case R.id.ibtn_OCcontroller_shape_s: message += "_s";
                break;
            case R.id.ibtn_OCcontroller_shape_h: message += "_h";
                break;
            case R.id.ibtn_OCcontroller_shape_c: message += "_c";
                break;
            case R.id.ibtn_OCcontroller_shape_d: message += "_d";
                break;
        }
        GlobalNetworkService.getInstance().sendMessageToServer(message);
        _sevenCardName = null;
    }

    enum State {
        Play,
        Wait,
        Shape
    }

    public void setState(State state) {
        switch (state) {
            case Play:
                relOCcontrollerPopup.setVisibility(View.INVISIBLE);
                break;
            case Wait:
                relOCcontrollerPopup.setVisibility(View.VISIBLE);
                relOCcontrollerWaitPlayer.setVisibility(View.VISIBLE);
                relOCcontrollerSelectShape.setVisibility(View.INVISIBLE);
                break;
            case Shape:
                relOCcontrollerPopup.setVisibility(View.VISIBLE);
                relOCcontrollerWaitPlayer.setVisibility(View.INVISIBLE);
                relOCcontrollerSelectShape.setVisibility(View.VISIBLE);
                break;
        }
    }

    String _sevenCardName;

    public void set_sevenCardName(String cardName)
    {
        _sevenCardName = cardName;
    }
}

class OneCardItem {
    Integer _itemResource;
    boolean _isSubmitable;
    String _cardName;

    public String get_cardName() {
        return _cardName;
    }

    public Integer getItemResource() {
        return _itemResource;
    }

    public void setItemResource(String str) {
        _cardName = str;
        this._itemResource = GlobalService.getInstance().getResourceByString("card_" + str);
    }

    public boolean isSubmitable() {
        return _isSubmitable;
    }

    public void setSubmitable(boolean submitable) {
        _isSubmitable = submitable;
    }

    public OneCardItem(String itemResource) {
        setItemResource(itemResource);
        this._isSubmitable = false;
    }
}

interface ItemTouchHelperAdapter {
    void onItemDismiss(int position);
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
        int dragFlags = ItemTouchHelper.START | ItemTouchHelper.END;
        int swipeFlags = ItemTouchHelper.UP; //| ItemTouchHelper.DOWN
        return makeMovementFlags(dragFlags, swipeFlags);
    }

    @Override
    public boolean onMove(RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder, RecyclerView.ViewHolder target) {
        return true;
    }

    @Override
    public void onSwiped(RecyclerView.ViewHolder viewHolder, int direction) {
        mAdapter.onItemDismiss(viewHolder.getAdapterPosition());
    }
}

class OneCardItemDecoration extends RecyclerView.ItemDecoration {
    private int mOffsetPx;

    public OneCardItemDecoration(int offsetPx) {
        mOffsetPx = offsetPx;
    }

    @Override
    public void getItemOffsets(Rect outRect, View view, RecyclerView parent, RecyclerView.State state) {
        super.getItemOffsets(outRect, view, parent, state);

        int itemPosition = parent.getChildAdapterPosition(view);
        int itemCount = state.getItemCount();

        if (parent.getAdapter() instanceof RecyclerViewAdapter) {
            RecyclerViewAdapter adapter = (RecyclerViewAdapter) parent.getAdapter();
            if (!adapter.getmItemListAtPosition(itemPosition).isSubmitable()) {
                outRect.bottom = -150;
            }
        }

        if (itemPosition != 0) {
            outRect.left = -300;
        }

        if (itemCount <= 5) {
            return;
        }

        if (itemPosition == 0) {
            outRect.left = mOffsetPx;
        } else if (itemPosition > 0 && itemPosition == itemCount - 1) {
            outRect.right = mOffsetPx;
        }
    }
}

class RecyclerViewAdapter extends RecyclerView.Adapter<RecyclerViewAdapter.RecyclerViewHolder> implements ItemTouchHelperAdapter {

    ArrayList<OneCardItem> mItemList;
    OneCardControllerActivity mActivity;

    public RecyclerViewAdapter(ArrayList<OneCardItem> mItemList, OneCardControllerActivity mActivity) {
        this.mItemList = mItemList;
        this.mActivity = mActivity;
    }

    public ArrayList<OneCardItem> getmItemList() {
        return mItemList;
    }

    public OneCardItem getmItemListAtPosition(int pos) {
        try {
            return mItemList.get(pos);
        } catch (Exception e) {
            e.printStackTrace();
            return null;
        }
    }

    @Override
    public RecyclerViewHolder onCreateViewHolder(ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(mActivity).inflate(R.layout.item_oc_card, parent, false);
        return new RecyclerViewHolder(view);
    }

    @Override
    public void onBindViewHolder(RecyclerViewHolder holder, int position) {
        try {
            Glide.with(mActivity).load(mItemList.get(position).getItemResource()).into(holder.itemOcCard);
        } catch (Exception e) {
            // do nothings...
        }
    }

    @Override
    public int getItemCount() {
        return mItemList.size();
    }

    @Override
    public void onItemDismiss(int position) {

        OneCardItem item = mItemList.get(position);

        if (!item.isSubmitable()) {

            mActivity.runOnUiThread(new Runnable() {
                @Override
                public void run() {
                    Toast.makeText(mActivity, "그 카드는 낼 수 없습니다.", Toast.LENGTH_SHORT).show();
                }
            });

            notifyDataSetChanged();

            return;
        }

        // is submitable?
        if(item.get_cardName().charAt(1) == '7')
        {
            mActivity.set_sevenCardName(item.get_cardName());
            mActivity.setState(OneCardControllerActivity.State.Shape);
        }else
        {
            mActivity.setState(OneCardControllerActivity.State.Wait);
            GlobalNetworkService.getInstance().sendMessageToServer("OneCard_Controller_Submit_" + item.get_cardName());
        }

        mItemList.remove(position);
        //notifyItemRemoved(position);

        notifyDataSetChanged();

        Vibrator vibe = (Vibrator) mActivity.getSystemService(Context.VIBRATOR_SERVICE);
        vibe.vibrate(100);
    }

    public static class RecyclerViewHolder extends RecyclerView.ViewHolder {

        @BindView(R.id.item_oc_card)
        ImageView itemOcCard;

        public RecyclerViewHolder(View itemView) {
            super(itemView);
            ButterKnife.bind(this, itemView);
        }
    }
}