using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Drawing;
using System.ServiceModel;
using System.Xml;
using System.Web;
namespace WcfwebHttpClient
{
    class Program
    {
        //发送图片
        void sendpic()
        {

          



            //发送协议初始化
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://119.29.5.178/Service1.svc/uploadicon");
            request.Method = "POST";
            request.ContentType = "text/plain";
           // request.Headers.Add("weiboid", "2");///添加头信息
            request.Headers.Add("userid", "100");///添加头信息
            try
            {
                //上传文件

                try
                {

                    string fileName = @"C:\Users\niexinming\Desktop\1.jpg";
                    FileStream fs = null;
                    fs = new FileStream(fileName, FileMode.Open, System.IO.FileAccess.Read, FileShare.ReadWrite);
                ///在图片的二进制码中插入用户信息
                    int streamLength = (int)fs.Length;
                    byte[] buffer = new byte[streamLength];
                    /*    
                                        byte[] data = new byte[4];
                                        UInt32 myid = 1234565;
                                        data = BitConverter.GetBytes(myid);
                                        for (int xixi = 0; xixi < 4; xixi++)
                                            buffer[xixi] = data[xixi];
                       */
                    fs.Read(buffer, 0, streamLength);
                                      
                    //发送图片和用户信息
                    request.ContentLength = buffer.Length;
                    Stream requestStram = request.GetRequestStream();
                    requestStram.Write(buffer, 0, buffer.Length);
                    requestStram.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    Console.WriteLine("失败");
                    Console.Read();
                    return;
                }
                Console.WriteLine("完成");




                //收到回复
                Stream getStream = request.GetResponse().GetResponseStream();

                byte[] resultByte = new byte[1000];
                getStream.Read(resultByte, 0, resultByte.Length);

                Console.WriteLine(Encoding.UTF8.GetString(resultByte));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            Console.ReadLine();
        }
        ///登陆
        void log()
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://192.168.1.104/Service1.svc/nie/niexinming/993025/踩着冬啊啊啊啊将计就计急急急呵呵呵斤计较经济界斤斤计较兵哥哥呵呵呵呵哦哦换个号换个苟富贵反广告费对法");
            request.Method = "GET";
            request.ContentType = "text/plain";
            Stream getStream = request.GetResponse().GetResponseStream();
            //收到回复
            byte[] resultByte = new byte[200];
            getStream.Read(resultByte, 0, resultByte.Length);

            Console.WriteLine(Encoding.UTF8.GetString(resultByte));


            Console.ReadLine();
        }
        ///发送大量文字
        void sendtext()
        {
            try
            {
                string strPost = "小妞,给大爷笑一个,不笑,那大爷给你笑一个,我和超人唯一的区别就是我把内裤穿里边了." +
                                 "我的饭量你是知道的，而且我也不爱吃烤鸭，所以吃了四只我就吃不下去了，我就说:实在" +
                                 "不能吃了，待会儿回家还要吃饭呢。天堂这儿还有一牌子：天堂周围四百米严禁摆摊！上帝" +
                                 "坐那正抽烟呢。上帝说，必须好好招待，这么些年好容易有个说相声的上了天堂了。一巴掌" +
                                 "宽护心毛，还纹一条带鱼。《金瓶梅》里唐僧取经那会儿…… 三十多岁没结婚，北京的媒婆" +
                                 "界就轰动了！骑着脖子拉屎，拉干的我拨下去，拉稀的我擦干了，可是他，骑着脖子拉痢疾！" +
                                 "说相声讲究说学逗唱，当你有口吃就不能说相声。，比如你报时，如果你有口吃就出现这样情况" +
                                 "。现在播报北....北...北京...时.. 时....时间...七....七..七点，靠，我一看表都八点半了  ";
                byte[] buffer = Encoding.UTF8.GetBytes(strPost);
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://localhost:49038/Service1.svc/read");
                request.Method = "POST";
                request.ContentType = "text/plain";

                request.ContentLength = buffer.Length;
                Stream requestStram = request.GetRequestStream();
                requestStram.Write(buffer, 0, buffer.Length);
                requestStram.Close();
                ////收到回复
                Stream getStream = request.GetResponse().GetResponseStream();

                byte[] resultByte = new byte[buffer.Length];
                getStream.Read(resultByte, 0, resultByte.Length);

                Console.WriteLine(Encoding.UTF8.GetString(resultByte));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            Console.ReadLine();

        }
        ///得到用户状态的xml数据
        void getdy()
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://localhost:49038/Service1.svc/getdy");
            request.Method = "GET";
            request.ContentType = "text/plain";
            Stream getStream = request.GetResponse().GetResponseStream();
            //收到回复
            byte[] resultByte = new byte[200];
            getStream.Read(resultByte, 0, resultByte.Length);

            Console.WriteLine(Encoding.UTF8.GetString(resultByte));


            Console.ReadLine();
        }
        ///获取下载的地址信息
        public void downlond()
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://localhost:49038/Service1.svc/download/1");
            request.Method = "GET";
            request.ContentType = "text/plain";
            Stream getStream = request.GetResponse().GetResponseStream();
            //收到回复
            byte[] resultByte = new byte[200];

            getStream.Read(resultByte, 0, resultByte.Length);

            string uil = Encoding.UTF8.GetString(resultByte);
            ////转换格式，从xml字符串转到url
            uil = uil.Trim();



            Console.WriteLine(uil);
            WebClient wc = new WebClient();
            string file = "下载.jpg";
            wc.DownloadFile(uil, file);
        }
        ///发送http头部消息
        void readhead()
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://192.16.1.104/Service1.svc/readhead");
            request.Method = "GET";
            request.ContentType = "text/plain";
            request.Headers.Add("userid", "123456");
            //收到回复
            Stream getStream = request.GetResponse().GetResponseStream();

            byte[] resultByte = new byte[200];
            getStream.Read(resultByte, 0, resultByte.Length);

            Console.WriteLine(Encoding.UTF8.GetString(resultByte));


            Console.ReadLine();
        }

        void seedy()
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://localhost:2437/Service1.svc/seedynamic/101");
            request.Method = "GET";
            request.ContentType = "text/plain";
        
            //收到回复
            Stream getStream = request.GetResponse().GetResponseStream();

            byte[] resultByte = new byte[20000];
            getStream.Read(resultByte, 0, resultByte.Length);

            Console.WriteLine(Encoding.UTF8.GetString(resultByte));


            Console.ReadLine();
        }
        static void Main(string[] args)
        {

            Program a = new Program();
            a.sendpic();


        }
    }
}
