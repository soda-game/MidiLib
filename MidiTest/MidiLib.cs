using System;
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
            public int leanNum;
            public NoteType type;
            public int color;
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

            }

        }

        void HeaderTestLog(HeaderChunkData h)
        {
            Console.WriteLine(
                "チャンクID：" + (char)h.chunkID[0] + (char)h.chunkID[1] + (char)h.chunkID[2] + (char)h.chunkID[3] + "\n" +
                "データ長：" + h.dataLength + "\n" +
                "フォーマット：" + h.format + "\n" +
                "トラック数：" + h.tracks + "\n" +
                "分解能：" + h.timeBase);
        }
    }
}
