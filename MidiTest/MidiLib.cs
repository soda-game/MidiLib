﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiLib
{
    class MidiSystem
    {
        //チャンクデータ
        struct HeaderChunkData
        {
            public byte[] chunkID; //MThd
            public int dataLength; //ヘッダの長さ
            public short format; //フォーマット
            public short tracks; //トラック数
            public short timeBase; //分解能
        }
        struct TrackChunkData
        {
            public byte[] chunkID; //MTrk
            public int dataLength; //トラックのデータ長
            public byte[] data; //演奏データ
        }

        //音げーに必要なものたち
        enum NoteType
        {
            ON, OFF
        }
        struct NoteData
        {
            public int eventTime;
            public int leanNum; //音階
            public NoteType type;
            public int Instrument; //楽器
        }
        List<NoteData> noteDataList = new List<NoteData>();

        struct TempData
        {
            public int eventTime;
            public float bpm;
            public float tick;
        }
        List<TempData> tempDataList = new List<TempData>();

        //main
        public void ReadMidi(string filePath)
        {
            noteDataList.Clear();
            tempDataList.Clear();

            //ファイル読み込み 読み込み終わるまで出ない!
            using (var file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (var reader = new BinaryReader(file))
            {
                //-------- ヘッダ解析 -------
                var headerChunk = new HeaderChunkData();

                //チャンクID
                headerChunk.chunkID = reader.ReadBytes(4);
                //リトルエンディアンなら逆に
                if (BitConverter.IsLittleEndian)
                {
                    //データ長
                    var bytePick = reader.ReadBytes(4);
                    Array.Reverse(bytePick);
                    headerChunk.dataLength = BitConverter.ToInt32(bytePick, 0);

                    //フォーマット
                    bytePick = reader.ReadBytes(2);
                    Array.Reverse(bytePick);
                    headerChunk.format = BitConverter.ToInt16(bytePick, 0);

                    //トラック数
                    bytePick = reader.ReadBytes(2);
                    Array.Reverse(bytePick);
                    headerChunk.tracks = BitConverter.ToInt16(bytePick, 0);

                    //分解能
                    bytePick = reader.ReadBytes(2);
                    Array.Reverse(bytePick);
                    headerChunk.timeBase = BitConverter.ToInt16(bytePick, 0);
                }
                else
                {
                    //データ長
                    headerChunk.dataLength = BitConverter.ToInt32(reader.ReadBytes(4), 0);
                    //フォーマット
                    headerChunk.format = BitConverter.ToInt16(reader.ReadBytes(2), 0);
                    //トラック数
                    headerChunk.tracks = BitConverter.ToInt16(reader.ReadBytes(2), 0);
                    //分解能
                    headerChunk.timeBase = BitConverter.ToInt16(reader.ReadBytes(2), 0);
                }
                //***ヘッダーテスト用
                HeaderTestLog(headerChunk);


                //-------- トラック解析 -------
                TrackChunkData[] trackChunks = new TrackChunkData[headerChunk.tracks]; //トラック数分 作る

                //トラック数分回す
                for (int i = 0; i < trackChunks.Length; i++)
                {
                    //チャンクID
                    trackChunks[i].chunkID = reader.ReadBytes(4);

                    if (BitConverter.IsLittleEndian)
                    {
                        //データ長
                        var bytePick = reader.ReadBytes(4);
                        Array.Reverse(bytePick);
                        trackChunks[i].dataLength = BitConverter.ToInt32(bytePick, 0);
                    }
                    else
                    {
                        //データ長
                        trackChunks[i].dataLength = BitConverter.ToInt32(reader.ReadBytes(4), 0);
                    }

                    //演奏データ
                    trackChunks[i].data = reader.ReadBytes(trackChunks[i].dataLength);
                    //***トラックテスト用
                    TrackTestLog(trackChunks[i]);
                    //演奏データ解析へ
                    TrackDataAnaly(trackChunks[i].data, headerChunk);
                }

            }

            //テンポ確認用
            TempTestLog(tempDataList);
            //***Notes確認用
            NoteTestLog(noteDataList);
            

        }

        void TrackDataAnaly(byte[] data, HeaderChunkData header)
        {
            //トラック内で引き継ぎたいもの
            uint eventTime = 0; //開始からの時間
            byte statusByte = 0; //FFとか入る
            uint Instrument = 0; //楽器

            //データ分
            for (int i = 0; i < data.Length;)
            {
                //---デルタタイム---
                uint delta = 0;
                while (true)
                {
                    byte bytePick = data[i++];
                    delta |= bytePick & (uint)0x7f;

                    if ((bytePick & 0x80) == 0) break;

                    delta = delta << 7;
                }
                eventTime += delta;

                //---ランニングステータス---
                if (data[i] < 0x80)
                {
                    //***
                }
                else
                {
                    statusByte = data[i++];
                }

                //---ステータスバイト---

                //ステバ分岐 
                //--Midiイベント--
                if (statusByte >= 0x80 & statusByte <= 0xef)
                {
                    switch (statusByte & 0xf0)
                    {
                        case 0x90://ノートオン
                            {
                                byte leanNum = data[i++]; //音階
                                byte velocity = data[i++]; //音の強さ

                                //ノート情報まとめる 
                                NoteData noteData = new NoteData();
                                noteData.eventTime = (int)eventTime;
                                noteData.leanNum = (int)leanNum;
                                noteData.Instrument = (int)Instrument;

                                //ベロ値でオンオフを送ってくる奴に対応
                                if (velocity > 0) //音が鳴っていたらオン
                                    noteData.type = NoteType.ON;
                                else
                                    noteData.type = NoteType.OFF;

                                noteDataList.Add(noteData);
                            }
                            break;

                        case 0x80: //ノートオフ
                            {
                                byte leanNum = data[i++];
                                byte velocity = data[i++];

                                NoteData noteData = new NoteData();
                                noteData.eventTime = (int)eventTime;
                                noteData.leanNum = (int)leanNum;
                                noteData.Instrument = (int)Instrument;
                                noteData.type = NoteType.OFF; //オフしか来ない

                                noteDataList.Add(noteData);
                            }
                            break;

                        case 0xc0: //プログラムチェンジ　音色 楽器を変える
                            Instrument = data[i++];
                            break;

                        case 0xa0: //キープッシャー
                            i += 2;
                            break;
                        case 0xb0: //コンチェ
                            i += 2;
                            break;
                        case 0xd0: //チェンネルプレッシャー
                            i += 1;
                            break;
                        case 0xe0: //ピッチベンド
                            i += 2;
                            break;
                    }
                }

                //--システムエクスクルーシブイベント--
                else if (statusByte == 0x70 || statusByte == 0x7f)
                {
                    byte dataLen = data[i++];
                    i += dataLen;
                }

                //--メタイベ--
                else if (statusByte == 0xff)
                {
                    byte eveNum = data[i++];
                    byte dataLen = data[i++]; //可変長***

                    switch (eveNum)
                    {
                        case 0x51:
                            {
                                TempData tempData = new TempData();
                                tempData.eventTime = (int)eventTime;

                                //3byte固定 4分音符の長さをマイクロ秒で
                                uint temp = 0;
                                temp |= data[i++];
                                temp <<= 8;
                                temp |= data[i++];
                                temp <<= 8;
                                temp |= data[i++];

                                //BPM計算 = 60秒のマクロ秒/4分音符のマイクロ秒
                                tempData.bpm = 60000000 / (float)temp;
                                //小数点第１位切り捨て
                                tempData.bpm = (float)Math.Floor(tempData.bpm * 10) / 10;
                                //tick値=60/分解能*1000
                                tempData.tick = (60 / tempData.bpm / header.timeBase * 1000);
                                tempDataList.Add(tempData);
                            }
                            break;

                        default:
                            i += dataLen; //メタはデータ長で全てとばせる 書くの面倒だった
                            break;
                    }
                }
            }

        }

        void HeaderTestLog(HeaderChunkData h)
        {
            Console.WriteLine(
                "チャンクID：" + (char)h.chunkID[0] + (char)h.chunkID[1] + (char)h.chunkID[2] + (char)h.chunkID[3] + "\n" +
                "データ長：" + h.dataLength + "\n" +
                "フォーマット：" + h.format + "\n" +
                "トラック数：" + h.tracks + "\n" +
                "分解能：" + h.timeBase + "\n");
        }
        void TrackTestLog(TrackChunkData t)
        {
            Console.WriteLine(
                 "チャンクID：" + (char)t.chunkID[0] + (char)t.chunkID[1] + (char)t.chunkID[2] + (char)t.chunkID[3] + "\n" +
                 "データ長：" + t.dataLength+"\n");
        }

        void NoteTestLog(List<NoteData> nList)
        {
            foreach (NoteData n in nList)
            {
                Console.WriteLine(
                    "発生時間:" + n.eventTime + "\n" +
                    "音階:" + n.leanNum + "\n" +
                    "タイプ:" + n.type.ToString() + "\n" +
                    "楽器:" + n.Instrument + "\n");
            }
        }

        void TempTestLog(List<TempData> tList)
        {
            foreach (TempData t in tList)
            {
                Console.WriteLine(
                "発生時間:" + t.eventTime + "\n" +
                "BPM値:" + t.bpm + "\n" +
                "Tick値:" + t.tick + "\n");
            }
        }
    }
}
