using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TakiClient
{
    class ServerData
    {
        public FrmUser frm;//הגדרת אובייקט של הטופס שבתוכו יתנהל המשחק/תוכנה
        public Thread tcpThd;//טרנד של הלקוח
        public TcpClient tcpclnt;// מאזין של הלקוח
        public NetworkStream stm; //קורא רצף של מידע
        public byte[] readBuffer;//מערך של בתים שקורא מידע מהשרת
        public byte[] writeBuffer;//מערך של מידע ששולח מידע לשרת

        public ServerData(FrmUser frm) // זהו בנאי שמקבל אובייקט של הטופס שמנהל את כל התהליך של העברת וקבלת כל הנתונים מהטופס
        {
            this.frm = frm;
        }

        public void WriteToServer(string strMessage)
        {
            var encord = new UTF8Encoding();//הגדרת אובייקט
            writeBuffer = encord.GetBytes(strMessage);//המרה לבייט
            if (stm != null)
                stm.Write(writeBuffer, 0, writeBuffer.Length);//אם המאזין הופעל אז תשלח את המידע
        }

        public bool ConnectServer(string user, string ip, int port)
        {
            try
            {
                tcpclnt = new TcpClient();//מאתחל את המאזין
                tcpclnt.Connect(ip.Trim(), port);//מחברים אותו לכתובות האייפי והפורט
                stm = tcpclnt.GetStream();//ככה אנחנו מקבלים מידע מן המאזין שלנו
                WriteToServer(user);//
                tcpThd = new Thread(new ThreadStart(ReadSocket));
                tcpThd.Start();
                return true;
            }
            catch (Exception)
            {
                MessageBox.Show("השרת לא מחובר", "שגיאה", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return false;
        }
        public void ReadSocket()
        {
            int ret;
            while (true)// הלולאה אינסופית
            {
                try
                {
                    readBuffer = new Byte[1024];//מערך מטיפוס בייט של מידע שהשרת ישלח לנו ישמר בתוך האובייקט readBuffer
                    ret = stm.Read(readBuffer, 0, readBuffer.Length);//קורא מהשרת ושומר ב readBuffer
                    string msg = Encoding.UTF8.GetString(readBuffer);
                    Console.WriteLine(msg);
                    if (readBuffer[0] == '$')
                    {
                     //   GetPic(readBuffer, ret);
                    }
                    else
                    {
                        //FrmUser.mess = Encoding.UTF8.GetString(readBuffer);
                        //FrmUser.mess = FrmUser.mess.Substring(0, ret);
                        //frm.Invoke(FrmUser.myDelegate);// כל מה שאני מוסיפה לאפס אני רוצה שהוא יוכל להשתלב יחד עם התהליך
                    }

                    //myDelegate הוא אובייקט שהוגדר בטופס המשחק
                }
                catch (Exception)
                {
                    break;
                }
            }


        }

        public void OnClosing()// סוגר את כל התהליכים ומאזין ברגע שלקוח מתנתק 
        {
            Thread.Sleep(1000);//השעייה של שניה
            if (tcpThd != null && tcpThd.IsAlive) tcpThd.Abort();
            if (stm != null) stm.Close();
            if (tcpclnt != null) tcpclnt.Close();
        }
    }
}
