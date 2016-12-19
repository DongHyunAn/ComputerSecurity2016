package com.cro.stickerapp.Secure;

import android.util.Base64;

import javax.crypto.Cipher;
import javax.crypto.spec.IvParameterSpec;
import javax.crypto.spec.SecretKeySpec;

/**
 * Created by DongHyun on 2016-12-18.
 */

public class AES128 {


    /**
     * hex to byte[] : 16진수 문자열을 바이트 배열로 변환한다.
     * @param hex    hex string
     * @return
     */
    public static byte[] hexToByteArray(String hex) {

        if (hex == null || hex.length() == 0) {
            return null;
        }

        byte[] ba = new byte[hex.length() / 2];

        for (int i = 0; i < ba.length; i++) {
            ba[i] = (byte) Integer.parseInt(hex.substring(2 * i, 2 * i + 2), 16);
        }

        return ba;
    }
    /**
     * byte[] to hex : unsigned byte(바이트) 배열을 16진수 문자열로 바꾼다.
     * @param ba        byte[]
     * @return
     */
    public static String byteArrayToHex(byte[] ba) {

        if (ba == null || ba.length == 0) {
            return null;
        }

        StringBuffer sb = new StringBuffer(ba.length * 2);
        String hexNumber;

        for (int x = 0; x < ba.length; x++) {
            hexNumber = "0" + Integer.toHexString(0xff & ba[x]);
            sb.append(hexNumber.substring(hexNumber.length() - 2));
        }

        return sb.toString();
    }

    /** 암호화 키 16자리 */
    public static String key = "fe8025947de7cd71";
    /**
     * AES 방식의 암호화
     *
     * @param message 암호화 대상 문자열
     * @return String 암호화 된 문자열
     * @throws Exception
     */
    public static String encrypt(String text, String key) throws Exception
    {
        Cipher cipher = Cipher.getInstance("AES/CBC/PKCS5Padding");
        byte[] keyBytes= new byte[16];
        byte[] b= key.getBytes("UTF-8");
        int len= b.length;
        if (len > keyBytes.length) len = keyBytes.length;
        System.arraycopy(b, 0, keyBytes, 0, len);
        SecretKeySpec keySpec = new SecretKeySpec(keyBytes, "AES");
        IvParameterSpec ivSpec = new IvParameterSpec(keyBytes);
        cipher.init(Cipher.ENCRYPT_MODE,keySpec,ivSpec);

        byte[] results = cipher.doFinal(text.getBytes("UTF-8"));
//               BASE64Encoder encoder = new BASE64Encoder();
//               return encoder.encode(results);

        return Base64.encodeToString(results, 0);
    }

    /**
     * AES 방식의 복호화
     *
     * @param encrypted 복호화 대상 문자열
     * @return String 복호화 된 문자열
     * @throws Exception
     */
    public static String decrypt(String encrypted) throws Exception {

        // use key coss2
        SecretKeySpec skeySpec = new SecretKeySpec(key.getBytes(), "AES");

        Cipher cipher = Cipher.getInstance("AES");
        cipher.init(Cipher.DECRYPT_MODE, skeySpec);

        byte[] original = cipher.doFinal(hexToByteArray(encrypted));

        String originalString = new String(original);

        return originalString;
    }
}
