using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//らいぶらりっぽい
using MidiLib;

namespace MidiTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string filePath = "C:/Users/K019G1032/Desktop/testvs/New Unity Project/Assets/sound/testDo.mid";
            MidiSystem midi = new MidiSystem();

            //midi読み込み
            midi.ReadMidi(filePath);

            //試しに頑張ったやつ
            {
                //byte[] all_midiData = midi.All_MidiData(filePath);

                ////チャンク
                //ChunkData ChunkData = new ChunkData();

                //ChunkData.tracks = midi.ToInit16(all_midiData, (int)MidiSystem.HeaderIndex.TRACKS);
                //ChunkData.timebase = midi.ToInit16(all_midiData, (int)MidiSystem.HeaderIndex.TIME_BASE);


                ////データ解析
                //List<Notes> notes = new List<Notes>();
                //List<Temps> temps = new List<Temps>();

                //for (int t = 0; t < ChunkData.tracks; t++)
                //{
                //    ChunkData.track_dateLan = midi.ToInit32(all_midiData, (int)MidiSystem.TrackIndex.LENGTH);

                //    //ループするとき引き継ぎたいやつ
                //    uint eventTime = 0;
                //    byte statusByte = 0;
                //    byte melody = 0; //音色

                //    for (int i = (int)MidiSystem.TrackIndex.DATA; i < ChunkData.track_dateLan;)
                //    {
                //        //デルタタイム
                //        uint delta = 0;
                //        while (true)
                //        {
                //            byte one_delta = all_midiData[i++];
                //            delta |= one_delta & (uint)0x7f;

                //            if ((one_delta & 0x80) == 0) break;

                //            delta = delta << 7;
                //        }
                //        eventTime += delta;

                //        //ランニングステータス
                //        if (all_midiData[i] < 0x80)
                //        {
                //        }
                //        else
                //        {
                //            statusByte = all_midiData[i++];
                //        }

                //        //ステータスバイト
                //        byte dataByte0, dataByte1, dataByte2, dataByte3; //入っているデータが違うので名前は適当

                //        //ステータスバイト分岐 無駄に多い
                //        Console.WriteLine(Convert.ToString(statusByte, 16));
                //        //--MIDIイベント--
                //        if (statusByte >= 0x80 && statusByte <= 0xef)
                //        {
                //            switch (statusByte & 0xf0) //下４にはチャンネル番号が入っているため、0に揃える
                //            {
                //                case 0x90: //ノートオン 音の始まり
                //                    dataByte0 = all_midiData[i++]; //音階
                //                    dataByte1 = all_midiData[i++]; //音の強さ ベロシティ値


                //                    //ノート情報まとめる noteが被るのでかっこ
                //                    {
                //                        Notes note = new Notes();
                //                        note.eventTime = (int)eventTime;
                //                        note.leanNum = (int)dataByte0;
                //                        note.color = melody;

                //                        //ベロ値でオンオフを送ってくる奴に対応
                //                        if (dataByte1 > 0) //音が鳴っていたら 音の始まり
                //                            note.type = NoteType.ON;
                //                        else
                //                            note.type = NoteType.OFF;

                //                        notes.Add(note);
                //                    }
                //                    break;

                //                case 0x80: //オフノート 音の終わり
                //                    dataByte0 = all_midiData[i++]; //音階
                //                    dataByte1 = all_midiData[i++]; //ベロ値

                //                    {
                //                        Notes note = new Notes();
                //                        note.eventTime = (int)eventTime;
                //                        note.leanNum = (int)dataByte0;
                //                        note.color = melody;
                //                        note.type = NoteType.OFF;

                //                        notes.Add(note);
                //                    }
                //                    break;

                //                case 0xc0: //プログラムチェンジ 音色を変える
                //                    i++;
                //                    melody = all_midiData[i++];
                //                    break;

                //                case 0xa0: //キープッシャー
                //                    i += 2;
                //                    break;
                //                case 0xb0: //コンチェ
                //                    i += 2;
                //                    break;
                //                case 0xd0: //チェンネルプレッシャー
                //                    i += 1;
                //                    break;
                //                case 0xe0: //ピッチベンド
                //                    i += 2;
                //                    break;
                //            }
                //        }

                //        //--システムエクスクルーシブイベント--
                //        else if (statusByte == 0x70 || statusByte == 0x7f)
                //        {
                //            byte dataLen = all_midiData[i++];
                //            i += dataLen;
                //        }

                //        //--メタイベ--
                //        else if (statusByte == 0xff)
                //        {
                //            byte eveNum = all_midiData[i++];
                //            byte dataLen = all_midiData[i++];

                //            switch (eveNum)
                //            {
                //                case 0x51:
                //                    {
                //                        Temps temp = new Temps();
                //                        temp.eventTime = (int)eventTime;

                //                        //3byte固定 4分音符の長さをマイクロ秒で
                //                        uint te = 0;
                //                        te |= all_midiData[i++];
                //                        te <<= 8;
                //                        te |= all_midiData[i++];
                //                        te <<= 8;
                //                        te |= all_midiData[i++];

                //                        //BPM計算 = 60秒のマクロ秒/4分音符のマイクロ秒
                //                        temp.bpm = 60000000 / (float)t;
                //                        //小数点第１位切り捨て
                //                        temp.bpm = (float)Math.Floor(temp.bpm * 10) / 10;
                //                        //tick値=60/分解能*1000
                //                        temp.tick = (60 / temp.bpm / ChunkData.timebase * 1000);

                //                        temps.Add(temp);
                //                    }
                //                    break;

                //                default:
                //                    i += dataLen; //メタはデータ長で全てとばせる 書くの面倒だった
                //                    break;
                //            }
                //        }
                //    }
                //}
                ////for(int i = 0; i <notes.Count; i++)
                ////{
                ////    Console.WriteLine(Convert.ToString( notes[i].eventTime,16));
                ////    Console.WriteLine(Convert.ToString( notes[i].leanNum,16));
                ////}
            }

            Console.ReadKey();
        }


    }
}
