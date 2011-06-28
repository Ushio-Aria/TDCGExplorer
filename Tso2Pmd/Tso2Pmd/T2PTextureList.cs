using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;

using TDCG;

namespace Tso2Pmd
{
    public class T2PTextureList
    {
        // テクスチャBitmapのリスト（同一イメージの重複を許さない）
        List<Bitmap> bmps = new List<Bitmap>();
        // tso番号+テクスチャ名より、bmp_listへのインデックス
        // （Bitmapはアドレスの情報しか持たないので，実体は重複している可能性あり）
        Dictionary<string, Bitmap> bmap = new Dictionary<string, Bitmap>();
        // bmp_listより、出力ファイル名へのインデックス
        Dictionary<Bitmap, string> file_names = new Dictionary<Bitmap, string>();

        // テクスチャを指定し、Bitmapとして記憶し、参照可能なようにインデックス
        // をつける。ただし、以前記憶したものの中に同じBitmapがあるなら、新規の
        // Bitmapは作成せず、インデックスのみつける。
        public void Add(TSOTex tex, int tso_id)
        {
            if (tex.width == 0 || tex.height == 0)
                return;

            Bitmap bmp = new Bitmap(tex.width, tex.height);
            SetBitmapBytes(bmp, tex.data);

            // bmp_listと比較して、同じものがあればそれのアドレスのみ参照しておく
            foreach (Bitmap tmp_bmp in bmps)
            {
                if (EqualBitmaps(bmp, tmp_bmp))
                {
                    bmap.Add(tso_id.ToString() + "-" + tex.Name, tmp_bmp);
                    return;
                }
            }

            // 同じものがなければ、新規にbmp_listにBitmapを追加
            bmps.Add(bmp);
            bmap.Add(tso_id.ToString() + "-" + tex.Name, bmp);

            // 同時にこれを出力するときのファイル名を作成
            file_names.Add(bmp, "t" + (bmps.Count - 1).ToString("000") + ".bmp");
        }

        /// <summary>
        /// 全てのビットマップを書き出します。
        /// </summary>
        /// <param name="dest_path">出力先パス</param>
        /// <param name="spheremap_used">スフィアマップを使うか</param>
        public void Save(string dest_path, bool spheremap_used)
        {
            foreach (Bitmap bmp in bmps)
            {
                if (bmp.Width == 256 && bmp.Height == 16)
                {
                    // テクスチャがtoonテクスチャなら加工（最適化）してから、書き出す
                    Bitmap toon_bmp = TurnBitmap(bmp);
                    toon_bmp.Save(dest_path + "/" + file_names[bmp],
                        System.Drawing.Imaging.ImageFormat.Bmp);

                    // 色飛び補完用のスフィアマップを書き出す
                    if (spheremap_used)
                    {
                        Bitmap sphere_bmp = MakeSphereBitmap(bmp);
                        string sphere_file_name = Path.ChangeExtension(file_names[bmp], ".sph");
                        sphere_bmp.Save(dest_path + "/" + sphere_file_name,
                            System.Drawing.Imaging.ImageFormat.Bmp);
                    }
                }
                else
                {
                    bmp.Save(dest_path + "/" + file_names[bmp],
                        System.Drawing.Imaging.ImageFormat.Bmp);
                }
            }
        }

        // ビットマップを得る
        public Bitmap GetBitmap(int tso_id, string tex_name)
        {
            Bitmap bmp;
            bmap.TryGetValue(tso_id.ToString() + "-" + tex_name, out bmp);
            return bmp;
        }

        // テクスチャを出力したファイル名を得る
        public string GetFileName(int tso_id, string tex_name)
        {
            Bitmap bmp; 
            bmap.TryGetValue(tso_id.ToString() + "-" + tex_name, out bmp);
            if (bmp == null) return null;

            string str;
            file_names.TryGetValue(bmp, out str);
            return str;
        }

        // byte[]をBitmapに変換する
        private void SetBitmapBytes(Bitmap bmp, byte[] bytes)
        {
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            System.Drawing.Imaging.BitmapData bmpData =
                bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                bmp.PixelFormat);

            // Bitmapの先頭アドレスを取得
            IntPtr ptr = bmpData.Scan0;

            // Bitmapへコピー
            Marshal.Copy(bytes, 0, ptr, bytes.Length);

            bmp.UnlockBits(bmpData);
        }

        // ２つのBitmapが等しいか判定する
        private bool EqualBitmaps(Bitmap bmp1, Bitmap bmp2)
        {
            if (bmp1.Size != bmp2.Size)
                return false;

            for (int x = 0; x < bmp1.Width; x++)
                for (int y = 0; y < bmp1.Height; y++)
                    if (bmp1.GetPixel(x, y) != bmp2.GetPixel(x, y)) 
                        return false;

            return true;
        }

        // toonテクスチャを最適化する
        private Bitmap TurnBitmap(Bitmap bmp1)
        {
            if (bmp1.Width == 256 && bmp1.Height == 16)
            {
                Bitmap bmp2 = new Bitmap(16, 250);

                for (int i = 0; i < 250; i++)
                {
                    Color c = bmp1.GetPixel(i, 0);

                    for (int j = 0; j < 16; j++)
                        bmp2.SetPixel(j, 250 - (i + 1), c);
                }

                return bmp2;
            }
            else
            {
                return bmp1;
            }
        }

        // toonテクスチャより、色とび補完用のスフィアマップを作成する
        private Bitmap MakeSphereBitmap(Bitmap bmp1)
        {
            Bitmap bmp2 = new Bitmap(1, 1);

            bmp2.SetPixel(0, 0, bmp1.GetPixel(249, 0));

            return bmp2;
        }
    }
}
