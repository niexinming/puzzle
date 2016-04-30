using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.IO;
using System.Drawing;
using System.Data.Sql;
using System.ServiceModel.Web;
using System.Data;
using System.Data.SqlClient;
using System.ServiceModel.Channels;
using System.Net;
using System.Web.Configuration;

namespace WcfService1
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“Service1”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 Service1.svc 或 Service1.svc.cs，然后开始调试。
    public class Service1 : IService1
    {
        public classregister register(string call, string nicheng, string mypassword)///用户注册
        {
            classregister cr = new classregister();
            string connstr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["SqlConnStr"].ConnectionString;////数据库连接字符串
            SqlConnection con = new SqlConnection(connstr);
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = con;

            try
            {
                con.Open();
            }
            catch
            {
                cr.userid = -1;
                return cr;
            }
            cmd.CommandText = "SELECT UserID FROM dbo.T_Users WHERE LoginName=N'" + call + "'";///查询是否有相同注册号的人
            SqlDataReader dr = cmd.ExecuteReader();
            int temp = 0, flag = 0;
            while (dr.Read())
            {
                if(!dr.IsDBNull(0))
                temp = dr.GetInt32(0);
                flag++;
            }
            if (flag > 0)//如果有相同注册号的人就返回-1
            {
                cr.userid = -1;
                return cr;
            }
            dr.Close();
            // cmd.ExecuteNonQuery();
          
          /*  cmd.ExecuteNonQuery();

            cmd.CommandText = "SELECT UserID FROM dbo.T_Users WHERE LoginName=N'" + call + "'";//查找用户账号对应的用户id
            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                if(!dr.IsDBNull(0))
                cr.userid = dr.GetInt32(0);
            }
            con.Close();
            return cr;//返回用户id */

            //有户登录返回用户id
            string CommandText = "INSERT INTO dbo.T_Users(LoginName,Name,PWD)VALUES(N'" + call + "',N'" + nicheng + "',N'" + mypassword + "') SELECT CAST(scope_identity() AS int);";//如果没有相同注册号的人就将用户数据插入用户数据
            SqlCommand cmd1 = new SqlCommand(CommandText, con);

              try
            {
            cr.userid = (Int32)cmd1.ExecuteScalar();//返回自增列
            }
               catch(Exception e)
               {
                   string tempe = e.ToString();
                   cr.userid = -1;  //返回用户id
                   return cr;
               }
            con.Close();
            return cr;
        }

        public classlog log(string qqcall, string password)///用户登录
        {
            classlog cl = new classlog();
            string connstr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["SqlConnStr"].ConnectionString;////数据库连接字符串
            SqlConnection con = new SqlConnection(connstr);
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = con;

            try
            {
                con.Open();
            }
            catch
            {
                cl.myuserid = -1;
                return cl;
            }
            cmd.CommandText = "SELECT LoginName,PWD FROM dbo.T_Users WHERE LoginName=N'" + qqcall + "'";//选取用户的注册号和密码
            SqlDataReader dr = cmd.ExecuteReader();
            dr.Read();
            try
            {
                dr.GetString(0);//看用户名是否存在
            }
            catch
            {
                cl.myuserid = -1;//不存在就返回假
                return cl;
            }
            if (dr.GetString(0) == qqcall && dr.GetString(1) == password)//看输入的用户名与密码是否匹配
            {
                dr.Close();
                cmd.CommandText = "SELECT UserID,ICON,Skip,VipFlag,UploadNumber FROM dbo.T_Users WHERE LoginName=N'" + qqcall + "'";
                dr = cmd.ExecuteReader();//如果登录成功就返回
                dr.Read();
                if(!dr.IsDBNull(0))
                cl.myuserid = dr.GetInt32(0);//用户名
                if (!dr.IsDBNull(1))
                cl.usertouxiang = dr.GetString(1);//用户头像
                if (!dr.IsDBNull(2))
                cl.userback = dr.GetString(2);//用户背景
                if (!dr.IsDBNull(3))
                cl.vipflag = dr.GetInt32(3);//用户vip标志
                if (!dr.IsDBNull(4))
                cl.shangchangshu = dr.GetInt32(4);//用户上传数
                con.Close();
                return cl;
            }
            else
            {
                cl.myuserid = -1;
                con.Close();
                return cl;
            }
        }

        public yesno uploadicon(Stream iconimg)//上传头像
        {
            yesno ul = new yesno();
            WebHeaderCollection headerCollection = WebOperationContext.Current.IncomingRequest.Headers;
            //获得http头编码
            string item = "userid";
            string value = headerCollection.Get(item);///从http头中读取用户id
                                                  
            string connstr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["SqlConnStr"].ConnectionString;////数据库连接字符串
            SqlConnection con = new SqlConnection(connstr);
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = con;

            try
            {
                con.Open();//打开数据库
            }
            catch
            {
                ul .issuccess=-1;
                return ul;
            }
            cmd.CommandText = "UPDATE dbo.T_Users SET ICON = N'" + value + ".jpg' WHERE UserID=" + value + ";";//将用户头像名插入到对应的用户表中
            SqlDataReader dr = cmd.ExecuteReader();//执行插入

            try
            {
                Image ima = Image.FromStream(iconimg);///上传用户的头像图片
                ima.Save(System.Web.Configuration.WebConfigurationManager.AppSettings["icon"].ToString() + value + ".jpg");///设置保存位置
            }

            catch
            {
                //捕捉错误，转换成为字节流
                ul.issuccess = -1;
                return ul;
            }
            ul.issuccess = 1;
            con.Close();
            return ul;
        }


        public yesno uploadbackground(Stream bkg)///上传用户背景
        {
            yesno ul = new yesno();
            WebHeaderCollection headerCollection = WebOperationContext.Current.IncomingRequest.Headers;  //获得http头编码
          
            string item = "userid";
            string value = headerCollection.Get(item);//获取用户头部信息



            try
            {
                //创建文件
                using (FileStream outputStream = new FileStream(System.Web.Configuration.WebConfigurationManager.AppSettings["background"].ToString() + value + ".jpg", FileMode.OpenOrCreate, FileAccess.Write))//创建图片文件
                {
                    ///网络流转字符流
                    // 我们不用对两个流对象进行读写，只要复制流就OK  
                    bkg.CopyTo(outputStream);
                    ////写入文件，清除缓冲区
                    outputStream.Flush();

                }
            }

            catch 
            {
                //捕捉错误，转换成为字节流
                ul.issuccess = -1;
            }
            //发送成功的信息，转换成字节流
            string connstr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["SqlConnStr"].ConnectionString;////数据库连接字符串
            SqlConnection con = new SqlConnection(connstr);
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = con;

            try
            {
                con.Open();//打开用数据库
            }
            catch
            {
                
            }
            cmd.CommandText = "UPDATE dbo.T_Users SET Skip = N'" + value + ".jpg' WHERE UserID=" + value + ";";//将用户背景名插入到对应的用户表中
            SqlDataReader dr = cmd.ExecuteReader();//执行sql语句
            ul.issuccess = 1;
            con.Close();
            return ul;
        }

        public IList<muandardata> seehomepage()///看首页
        {

            IList<muandardata> list = new List<muandardata>();//音乐和文章列表
            string connstr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["SqlConnStr"].ConnectionString;////数据库连接字符串
            SqlConnection con = new SqlConnection(connstr);//数据库连接
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = con;

            try
            {
                con.Open();//打开数据库
            }
            catch
            {
                muandardata ap = new muandardata();
                ap.arid = -1;//失败返回的数据
                list.Add(ap);
                return list;
            }
            cmd.CommandText = @"SELECT tm.MusicID, tm.UploadTime, tm.Name, tm.MusicPIC,ta.ArticleID,ta.Title
                              FROM PUZZLE.dbo.T_Music tm JOIN PUZZLE.dbo.T_Article ta ON tm.UploadTime=ta.UploadTime ORDER BY ta.UploadTime";///选取音乐表里面的音乐id，音乐上传时间，音乐名，音乐图片，文章表里面的文章id,文章标题 按照上传时间排序
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                muandardata ap = new muandardata();
                if(!dr.IsDBNull(0))
                ap.muid = dr.GetInt32(0);//音乐id
                if (!dr.IsDBNull(1))
                ap.muupdtime = dr.GetDateTime(1);//上传时间
                if (!dr.IsDBNull(2))
                ap.muname = dr.GetString(2);//音乐名
                if (!dr.IsDBNull(3))
                ap.mupic = dr.GetString(3);//音乐图片名
                if (!dr.IsDBNull(4))
                ap.arid = dr.GetInt32(4);//文章id
                if (!dr.IsDBNull(5))
                ap.arttitle = dr.GetString(5);//文章标题
                list.Add(ap);
            }
            con.Close();
            return list;//返回列表

        }

        public muandardata seehomeart(string arid)//看首页文章
        {
           
            muandardata ar = new muandardata();//音乐和文章的数据
            string connstr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["SqlConnStr"].ConnectionString;////数据库连接字符串

            SqlConnection con11 = new SqlConnection(connstr);
            SqlCommand cmd11 = new SqlCommand();
            cmd11.Connection = con11;
            con11.Open();
            cmd11.CommandText = "SELECT *FROM PUZZLE.dbo.T_Article ta WHERE ta.ArticleID=" + arid;  //检查看的文章是否存在
            SqlDataReader dr11 = cmd11.ExecuteReader();
            dr11.Read();
            try
            {
                if (dr11.IsDBNull(0))
                {
                    ar.arid = -1;
                    con11.Close();
                    return ar;
                }
            }
            catch
            {
                ar.arid= -1;
                con11.Close();
                return ar;
            }
            con11.Close();


            SqlConnection con = new SqlConnection(connstr);
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = con;

            try
            {
                con.Open();//打开数据库
            }
            catch
            {
              
            }
            cmd.CommandText = @"SELECT  ta.UploadTime, ta.Title, ta.Details, ta.TranspondNumber,ta.CommentNumber, ta.ZanNumber,ta.ArticleID
                                 FROM PUZZLE.dbo.T_Article ta WHERE ta.ArticleID=" +arid+";";//从文章表里面选取文章上传时间，文章标题，文章内容，转发数，评论数，赞数
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                if(!dr.IsDBNull(0))
                ar.artupdtime = dr.GetDateTime(0);//文章上传时间
                if (!dr.IsDBNull(1))
                ar.arttitle = dr.GetString(1);//文章标题
                if (!dr.IsDBNull(2))
                ar.artdatailes = dr.GetString(2);//文章内容
                if (!dr.IsDBNull(3))
                ar.arttranspondnum = dr.GetInt32(3);//文章转发数
                if (!dr.IsDBNull(4))
                ar.artcommentnu = dr.GetInt32(4);//文章评论数
                if (!dr.IsDBNull(5))
                ar.artzannum = dr.GetInt32(5);///文章赞数
                if (!dr.IsDBNull(6))
                   ar.arid = dr.GetInt32(6);
            }
            con.Close();
            return ar;
        }

        public  muandardata seehomemus(string muid)//看首页音乐
        {

            muandardata ar = new muandardata();//音乐和文章的数据
            string connstr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["SqlConnStr"].ConnectionString;////数据库连接字符串

            SqlConnection con11 = new SqlConnection(connstr);
            SqlCommand cmd11 = new SqlCommand();
            cmd11.Connection = con11;
            con11.Open();
            cmd11.CommandText = "SELECT *FROM PUZZLE.dbo.T_Music tmu WHERE tmu.MusicID=" + muid;  //检查看的音乐是否存在
            SqlDataReader dr11 = cmd11.ExecuteReader();
            dr11.Read();
            try
            {
                if (dr11.IsDBNull(0))
                {
                    ar.muid = -1;
                    con11.Close();
                    return ar;
                }
            }
            catch
            {
                ar.muid = -1;
                con11.Close();
                return ar;
            }
            con11.Close();


            SqlConnection con = new SqlConnection(connstr);
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = con;

            try
            {
                con.Open();
            }
            catch
            {

            }
            cmd.CommandText = @"SELECT tm.UploadTime,tm.Music,tm.MusicPIC,tm.Introduce,tm.CommentNumber,tm.TranspondNumber,tm.ZanNumber, tm.MusicID,tm.Name
                               FROM PUZZLE.dbo.T_Music tm WHERE tm.MusicID=" + muid + ";";//在音乐表里面选择上传时间，音乐名，音乐图片，音乐介绍，评论数，转发数，赞数,音乐路径
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                if(!dr.IsDBNull(0))
                ar.muupdtime = dr.GetDateTime(0);//音乐上传时间
                if (!dr.IsDBNull(1))
                ar.mupath = dr.GetString(1);//音乐路径
                if (!dr.IsDBNull(2))
                ar.mupic = dr.GetString(2);//音乐图片
                if (!dr.IsDBNull(3))
                ar.muintro = dr.GetString(3);//音乐介绍
                if (!dr.IsDBNull(4))
                ar.mucommentnu = dr.GetInt32(4);//音乐评论数
                if (!dr.IsDBNull(5))
                ar.mutranspondnum = dr.GetInt32(5);//音乐转发数
                if (!dr.IsDBNull(6))
                ar.muzannum = dr.GetInt32(6);//音乐赞数
                if (!dr.IsDBNull(7))
                 ar.muid = dr.GetInt32(7);//音乐id
                if (!dr.IsDBNull(8))
                 ar.muname = dr.GetString(8);//音乐名

            }
            con.Close();
            return ar;
        }

        public  yesno commandar(string userid,string artid,string content)///评论文章
        {
            yesno issu = new yesno();
            string connstr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["SqlConnStr"].ConnectionString;////数据库连接字符串


            #region 检查存在性
            SqlConnection con11 = new SqlConnection(connstr);
            SqlCommand cmd11 = new SqlCommand();
            cmd11.Connection = con11;
            con11.Open();
            cmd11.CommandText = "SELECT *FROM PUZZLE.dbo.T_Article ta WHERE ta.ArticleID=" + artid;  //检查转发的文章是否存在
            SqlDataReader dr11 = cmd11.ExecuteReader();
            dr11.Read();
            try
            {
                if (dr11.IsDBNull(0))
                {
                    issu.issuccess = -1;
                    con11.Close();
                    return issu;
                }
            }
            catch
            {
                issu.issuccess = -1;
                con11.Close();
                return issu;
            }
            dr11.Close();

            //检查用户是否存在
            cmd11.Connection = con11;
            cmd11.CommandText = "SELECT *FROM PUZZLE.dbo.T_Users tu WHERE tu.UserID=" + userid;
            dr11 = cmd11.ExecuteReader();
            dr11.Read();
            try
            {
                if (dr11.IsDBNull(0))
                {
                    issu.issuccess = -1;
                    con11.Close();
                    return issu;
                }
            }
            catch (Exception e)
            {
                string tempe = e.ToString();
                issu.issuccess = -1;
                con11.Close();
                return issu;
            }
            con11.Close();
            #endregion


            SqlConnection con = new SqlConnection(connstr);
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = con;

            try
            {
                con.Open();
            }
            catch
            {
                issu.issuccess = -1;//失败返回-1
                return issu;
            }
            cmd.CommandText = @"INSERT INTO PUZZLE.dbo.T_ArticleComment(Details, ArticleID, UserID) VALUES(N'"+content+"',"+artid+","+userid+");";///插入评论内容，文章id,用户id
            SqlDataReader dr = cmd.ExecuteReader();
            issu.issuccess=1;//成功返回1
            con.Close();
            return issu;
        }


        public yesno commandmu(string userid, string muid, string content)//评论音乐
        {
            yesno issu = new yesno();
            string connstr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["SqlConnStr"].ConnectionString;////数据库连接字符串

            #region 检查存在性
            SqlConnection con11 = new SqlConnection(connstr);
            SqlCommand cmd11 = new SqlCommand();
            cmd11.Connection = con11;
            con11.Open();
            cmd11.CommandText = "SELECT *FROM PUZZLE.dbo.T_Music tmu WHERE tmu.MusicID=" + muid;  //检查评论的音乐是否存在
            SqlDataReader dr11 = cmd11.ExecuteReader();
            dr11.Read();
            try
            {
                if (dr11.IsDBNull(0))
                {
                    issu.issuccess = -1;
                    con11.Close();
                    return issu;
                }
            }
            catch
            {
                issu.issuccess = -1;
                con11.Close();
                return issu;
            }
            dr11.Close();

            //检查用户是否存在
            cmd11.Connection = con11;
            cmd11.CommandText = "SELECT *FROM PUZZLE.dbo.T_Users tu WHERE tu.UserID=" + userid;
            dr11 = cmd11.ExecuteReader();
            dr11.Read();
            try
            {
                if (dr11.IsDBNull(0))
                {
                    issu.issuccess = -1;
                    con11.Close();
                    return issu;
                }
            }
            catch (Exception e)
            {
                string tempe = e.ToString();
                issu.issuccess = -1;
                con11.Close();
                return issu;
            }
            con11.Close();
            #endregion


            SqlConnection con = new SqlConnection(connstr);
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = con;

            try
            {
                con.Open();
            }
            catch
            {
                issu.issuccess = -1;//失败返回-1
                return issu;
            }
            cmd.CommandText = @"INSERT INTO PUZZLE.dbo.T_MusicComment(Details, MusicID, UserID) VALUES(N'" + content + "'," + muid + "," + userid + ");";///插入评论内容，音乐id,用户id
            SqlDataReader dr = cmd.ExecuteReader();
            issu.issuccess = 1;//成功返回1
            con.Close();
            return issu;
        }

        public yesno zanart(string userid, string artid)//赞文章
        {
            yesno issu = new yesno();
            string connstr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["SqlConnStr"].ConnectionString;////数据库连接字符串

            #region 检查存在性
            SqlConnection con11 = new SqlConnection(connstr);
            SqlCommand cmd11 = new SqlCommand();
            cmd11.Connection = con11;
            con11.Open();
            cmd11.CommandText = "SELECT *FROM PUZZLE.dbo.T_Article ta WHERE ta.ArticleID=" + artid;  //检查赞的文章是否存在
            SqlDataReader dr11 = cmd11.ExecuteReader();
            dr11.Read();
            try
            {
                if (dr11.IsDBNull(0))
                {
                    issu.issuccess = -1;
                    con11.Close();
                    return issu;
                }
            }
            catch
            {
                issu.issuccess = -1;
                con11.Close();
                return issu;
            }
            dr11.Close();

            //检查用户是否存在
            cmd11.Connection = con11;
            cmd11.CommandText = "SELECT *FROM PUZZLE.dbo.T_Users tu WHERE tu.UserID=" + userid;
            dr11 = cmd11.ExecuteReader();
            dr11.Read();
            try
            {
                if (dr11.IsDBNull(0))
                {
                    issu.issuccess = -1;
                    con11.Close();
                    return issu;
                }
            }
            catch (Exception e)
            {
                string tempe = e.ToString();
                issu.issuccess = -1;
                con11.Close();
                return issu;
            }
            con11.Close();
            #endregion




            SqlConnection con = new SqlConnection(connstr);
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = con;

            try
            {
                con.Open();
            }
            catch
            {
                issu.issuccess = -1;//失败返回-1
                return issu;
            }
            cmd.CommandText = "INSERT INTO PUZZLE.dbo.T_Zan(TypeFlag, TypeID, UserID) VALUES(1," + artid + "," + userid + ")" +  //在赞表中插入【文章、微博、音乐、视频标识】：1，文章id,用户id
                                "UPDATE PUZZLE.dbo.T_Article SET ZanNumber = ZanNumber+1 WHERE ArticleID=" + artid + ";";  //文章表中找到对应文章id,并且文章的赞数+1
            SqlDataReader dr = cmd.ExecuteReader();
            issu.issuccess = 1;//成功返回1
            con.Close();
            
            return issu;

        }

        public yesno zanmus(string userid, string muid)//赞音乐
        {
            yesno issu = new yesno();
            string connstr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["SqlConnStr"].ConnectionString;////数据库连接字符串


            #region 检查存在性
            SqlConnection con11 = new SqlConnection(connstr);
            SqlCommand cmd11 = new SqlCommand();
            cmd11.Connection = con11;
            con11.Open();
            cmd11.CommandText = "SELECT *FROM PUZZLE.dbo.T_Music tmu WHERE tmu.MusicID=" + muid;  //检查赞的音乐是否存在
            SqlDataReader dr11 = cmd11.ExecuteReader();
            dr11.Read();
            try
            {
                if (dr11.IsDBNull(0))
                {
                    issu.issuccess = -1;
                    con11.Close();
                    return issu;
                }
            }
            catch
            {
                issu.issuccess = -1;
                con11.Close();
                return issu;
            }
            dr11.Close();

            //检查用户是否存在
            cmd11.Connection = con11;
            cmd11.CommandText = "SELECT *FROM PUZZLE.dbo.T_Users tu WHERE tu.UserID=" + userid;
            dr11 = cmd11.ExecuteReader();
            dr11.Read();
            try
            {
                if (dr11.IsDBNull(0))
                {
                    issu.issuccess = -1;
                    con11.Close();
                    return issu;
                }
            }
            catch (Exception e)
            {
                string tempe = e.ToString();
                issu.issuccess = -1;
                con11.Close();
                return issu;
            }
            con11.Close();
            #endregion

            SqlConnection con = new SqlConnection(connstr);
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = con;

            try
            {
                con.Open();
            }
            catch
            {
                issu.issuccess = -1;//失败返回-1
                return issu;
            }
            cmd.CommandText = "INSERT INTO PUZZLE.dbo.T_Zan(TypeFlag, TypeID, UserID) VALUES(2," + muid + "," + userid + ")" +  //在赞表中插入【文章、微博、音乐、视频标识】：2，文章id,用户id
                                "UPDATE PUZZLE.dbo.T_Music SET ZanNumber = ZanNumber+1 WHERE MusicID=" + muid + ";";  //音乐表中找到对应音乐id,并且音乐的赞数+1
            SqlDataReader dr = cmd.ExecuteReader();
            issu.issuccess = 1;//成功就返回1
            con.Close();
            return issu;

        }

        public yesno zhuanfaart(string userid, string artid)//转发文章
        {
            yesno issu = new yesno();
            string connstr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["SqlConnStr"].ConnectionString;////数据库连接字符串

            #region 检查存在性
            SqlConnection con11 = new SqlConnection(connstr);
            SqlCommand cmd11 = new SqlCommand();
            cmd11.Connection = con11;
            con11.Open();
            cmd11.CommandText = "SELECT *FROM PUZZLE.dbo.T_Article ta WHERE ta.ArticleID=" + artid;  //检查转发的文章是否存在
            SqlDataReader dr11 = cmd11.ExecuteReader();
            dr11.Read();
            try
            {
                if (dr11.IsDBNull(0))
                {
                    issu.issuccess = -1;
                    con11.Close();
                    return issu;
                }
            }
            catch
            {
                issu.issuccess = -1;
                con11.Close();
                return issu;
            }
            dr11.Close();

            //检查用户是否存在
            cmd11.Connection = con11;
            cmd11.CommandText = "SELECT *FROM PUZZLE.dbo.T_Users tu WHERE tu.UserID=" + userid;  
             dr11 = cmd11.ExecuteReader();
             dr11.Read();
             try
             {
                 if (dr11.IsDBNull(0))
                 {
                     issu.issuccess = -1;
                     con11.Close();
                     return issu;
                 }
             }
             catch(Exception e)
             {
                 string tempe = e.ToString();
                 issu.issuccess = -1;
                 con11.Close();
                 return issu;
             }
            con11.Close();
            #endregion


            SqlConnection con = new SqlConnection(connstr);
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = con;

            try
            {
                con.Open();
            }
            catch
            {
                issu.issuccess = 0;//失败返回0
                return issu;
            }
            cmd.CommandText = "INSERT INTO PUZZLE.dbo.T_WeiBo(UserID, TranspondFlag, TypeFlag, TypeID)VALUES("+userid+",1,1,"+artid+");";//在微博表里面插入用户id，转发标识（1代表转发），转发类型标志(1代表文章），转发的文章id
            SqlDataReader dr = cmd.ExecuteReader();
            issu.issuccess = 1;//成功返回1
            con.Close();
            return issu;

        }


        public yesno zhuanfamus(string userid, string musid)//转发音乐
        {
            yesno issu = new yesno();
            string connstr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["SqlConnStr"].ConnectionString;////数据库连接字符串


            #region 检查存在性
            SqlConnection con11 = new SqlConnection(connstr);
            SqlCommand cmd11 = new SqlCommand();
            cmd11.Connection = con11;
            con11.Open();
            cmd11.CommandText = "SELECT *FROM PUZZLE.dbo.T_Music tmu WHERE tmu.MusicID=" + musid;  //检查转发的音乐是否存在
            SqlDataReader dr11 = cmd11.ExecuteReader();
            dr11.Read();
            try
            {
                if (dr11.IsDBNull(0))
                {
                    issu.issuccess = -1;
                    con11.Close();
                    return issu;
                }
            }
            catch
            {
                issu.issuccess = -1;
                con11.Close();
                return issu;
            }
            dr11.Close();

            //检查用户是否存在
            cmd11.Connection = con11;
            cmd11.CommandText = "SELECT *FROM PUZZLE.dbo.T_Users tu WHERE tu.UserID=" + userid;
            dr11 = cmd11.ExecuteReader();
            dr11.Read();
            try
            {
                if (dr11.IsDBNull(0))
                {
                    issu.issuccess = -1;
                    con11.Close();
                    return issu;
                }
            }
            catch (Exception e)
            {
                string tempe = e.ToString();
                issu.issuccess = -1;
                con11.Close();
                return issu;
            }
            con11.Close();
            #endregion

            SqlConnection con = new SqlConnection(connstr);
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = con;

            try
            {
                con.Open();
            }
            catch
            {
                issu.issuccess = 0;//失败返回0
                return issu;
            }
            cmd.CommandText = "INSERT INTO PUZZLE.dbo.T_WeiBo(UserID, TranspondFlag, TypeFlag, TypeID)VALUES(" + userid + ",1,2," + musid + ");";  //在微博表里面插入用户id，转发标识（1代表转发），转发类型标志(2）代表音乐，转发的音乐id
            SqlDataReader dr = cmd.ExecuteReader();//执行数据库指令
            issu.issuccess = 1;//成功返回1
            con.Close();
            return issu;
        }

        public IList<classuserlist> userlist()//获取用户列表
        {
            IList<classuserlist> list = new List<classuserlist>();
            string connstr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["SqlConnStr"].ConnectionString;////数据库连接字符串
            SqlConnection con = new SqlConnection(connstr);
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = con;

            try
            {
                con.Open();
            }
            catch
            {
                list = null;//不成功返回空
                return list;
            }
            cmd.CommandText = @"SELECT tu.UserID, tu.Name, tu.ICON, tu.VipFlag FROM PUZZLE.dbo.T_Users tu";//从用户表里面获取所有用户的用户id，用户昵称，用户头像，用户vip标志
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                classuserlist ap = new classuserlist();
                if (!dr.IsDBNull(0))
                ap.userid = dr.GetInt32(0);//用户id
                if (!dr.IsDBNull(1))
                ap.nicheng = dr.GetString(1);//用户昵称
                if (!dr.IsDBNull(2))
                ap.icon = dr.GetString(2);//用户头像
                if (!dr.IsDBNull(3))
                ap.vipflag = dr.GetInt32(3);//用户vip标志
                list.Add(ap);
            }
            con.Close();
            return list;//成功返回用户列表

        }

        public yesno fans(string userid, string fanslist)///用户添加关注的人，其中userid是用户id，fanslist是关注的用户id列表，每个用户用逗号隔开
       {
           yesno issu = new yesno();
          
           string connstr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["SqlConnStr"].ConnectionString;////数据库连接字符串

           #region 检查存在性
           SqlConnection con11 = new SqlConnection(connstr);
           SqlCommand cmd11 = new SqlCommand();
           
           con11.Open();
         
           //检查用户是否存在
           cmd11.Connection = con11;
           cmd11.CommandText = "SELECT *FROM PUZZLE.dbo.T_Users tu WHERE tu.UserID=" + userid;
           SqlDataReader dr11 = cmd11.ExecuteReader();
           dr11.Read();
           try
           {
               if (dr11.IsDBNull(0))
               {
                   issu.issuccess = -1;
                   con11.Close();
                   return issu;
               }
           }
           catch (Exception e)
           {
               string tempe = e.ToString();
               issu.issuccess = -1;
               con11.Close();
               return issu;
           }
           con11.Close();
           #endregion


           SqlConnection con = new SqlConnection(connstr);
           SqlCommand cmd = new SqlCommand();

           cmd.Connection = con;

           try
           {
               con.Open();
           }
           catch
           {
               issu.issuccess = -1;//失败返回-1
               con.Close();
               return issu;
           }




           string[] arrTemp = fanslist.Split(',');//将带逗号的用户字符串分割开来，并且保存在字符串数组里面
           for (int i = 0; i < arrTemp.Length; i++)
           {
               SqlConnection con2 = new SqlConnection(connstr);
               SqlCommand cmd2 = new SqlCommand();

               cmd2.Connection = con2;
               con2.Open();
               cmd2.CommandText = "SELECT *FROM PUZZLE.dbo.T_UserRelation tur WHERE tur.User1=" + userid + " AND tur.User2=" + arrTemp[i];//检查用户是否重复关注

               SqlDataReader dr2 = cmd2.ExecuteReader();
               dr2.Read();
               try
               {
                   int tempi = dr2.GetInt32(2);///如果数据为空，就会抛出异常
                   issu.issuccess = -1;//如果重复关注就返回-1
                   return issu;
               }
               catch
               {
                   ///如果没有就跳过
               }
               try
               {
                   SqlCommand cmd1 = new SqlCommand("UPDATE PUZZLE.dbo.T_UserRelation SET RelationFlag = 1  WHERE User1=" + arrTemp[i] + " and user2=" + userid + ";", con);//将用户关注列表插入用户关系表中，首先看是否是互相关注（找重复的id对）
                   int count = cmd1.ExecuteNonQuery();//返回受影响行数
                   if (count > 0)
                   {
                       continue;//如果存相同的用户对，则忽略
                   }
                   cmd.CommandText = "INSERT INTO PUZZLE.dbo.T_UserRelation(User1,User2,RelationFlag)VALUES(" + userid + "," + arrTemp[i] + ",0)";//否则插入新的用户关系
                 
                   SqlDataReader dr = cmd.ExecuteReader();
                   dr.Close();
                    }
               catch(Exception e)
               {
                   string temp = e.ToString();
                   issu.issuccess = -1;
                   return issu;
               }
             
           }
           issu.issuccess = 1;//成功返回1
           con.Close();
           return issu;
       }

         public classreturnid sendweibo(string userid, string weibo)//发微博
       {
           classreturnid weiboid = new classreturnid();
           string str = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["SqlConnStr"].ConnectionString;////数据库连接字符串


           #region 检查存在性
           SqlConnection con11 = new SqlConnection(str);
           SqlCommand cmd11 = new SqlCommand();

           con11.Open();

           //检查用户是否存在
           cmd11.Connection = con11;
           cmd11.CommandText = "SELECT *FROM PUZZLE.dbo.T_Users tu WHERE tu.UserID=" + userid;
           SqlDataReader dr11 = cmd11.ExecuteReader();
           dr11.Read();
           try
           {
               if (dr11.IsDBNull(0))
               {
                   weiboid.myid = -1;
                   con11.Close();
                   return weiboid;
               }
           }
           catch (Exception e)
           {
               string tempe = e.ToString();
               weiboid.myid = -1;
               con11.Close();
               return weiboid;
           }
           con11.Close();
           #endregion


           SqlConnection conn = new SqlConnection(str);
           string CommandText = "INSERT INTO PUZZLE.dbo.T_WeiBo(Details, UserID, TranspondFlag)VALUES(N'"+weibo+"',"+userid+",0) SELECT CAST(scope_identity() AS int);";//在微博表里面插入微博内容，用户id，和转发标志为0（表明为1）返回插入的微博id
           SqlCommand cmd = new SqlCommand(CommandText, conn);
           try
           {
               conn.Open();
           }
           catch
           {
               weiboid.myid = -1;
               return weiboid;
           }
           weiboid.myid= (Int32)cmd.ExecuteScalar();//返回自增列
           conn.Close();
         
           return weiboid;//返回插入的微博id
       }

         public yesno sendweibopoc(Stream pweiboic)//发微博图片
       {
           string connstr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["SqlConnStr"].ConnectionString;////数据库连接字符串
           yesno ul = new yesno();
           WebHeaderCollection headerCollection = WebOperationContext.Current.IncomingRequest.Headers;
           //获得http头编码
           string item1 = "number";//微博图片序号，每条微博只能发9张图片
           string item2 = "weiboid";//微博id
           string xuhao = headerCollection.Get(item1);//每条微博图片序号
           string myweiboid = headerCollection.Get(item2);//每条微博id

           #region 检查存在性
           SqlConnection con11 = new SqlConnection(connstr);
           SqlCommand cmd11 = new SqlCommand();

           con11.Open();

           //检查微博是否存在
           cmd11.Connection = con11;
           cmd11.CommandText = "SELECT *FROM PUZZLE.dbo.T_WeiBo twb WHERE twb.WeiBoID=" + myweiboid;
           SqlDataReader dr11 = cmd11.ExecuteReader();
           dr11.Read();
           try
           {
               if (dr11.IsDBNull(0))
               {
                   ul.issuccess = -1;
                   con11.Close();
                   return ul;
               }
           }
           catch (Exception e)
           {
               string tempe = e.ToString();
               ul.issuccess = -1;
               con11.Close();
               return ul;
           }
           con11.Close();
           #endregion
         
          
         
           SqlConnection con = new SqlConnection(connstr);
           SqlCommand cmd = new SqlCommand();

           cmd.Connection = con;

           try
           {
               con.Open();
           }
           catch(Exception e)
           {
               string nie = e.ToString();
           }

           SqlDataReader dr = cmd.ExecuteReader();
           ///检查微博是否存在
           cmd.CommandText = "SELECT *FROM PUZZLE.dbo.T_WeiBo twb WHERE twb.WeiBoID=" + myweiboid; //检查微博是否存在
           dr = cmd.ExecuteReader();
           try
           {
               if (dr.IsDBNull(0))
               {
                   ul.issuccess = -1;
                   con.Close();
                   return ul;
               }
           }
           catch
           {
               ul.issuccess = -1;
               con.Close();
               return ul;
           }
           dr.Close();

           cmd.CommandText = "UPDATE PUZZLE.dbo.T_WeiBo SET IMG" + xuhao + "=N'" + myweiboid +"+"+xuhao+".jpg' WHERE WeiBoID=" + myweiboid + ";";//插入相对应序号和博客id的微博图片
           dr = cmd.ExecuteReader();


//接受流，保存为图片
           try
           {
               //创建文件
               using (FileStream outputStream = new FileStream(System.Web.Configuration.WebConfigurationManager.AppSettings["weibopic"].ToString() + myweiboid +"+"+ xuhao + ".jpg", FileMode.OpenOrCreate, FileAccess.Write))
               {
                   ///网络流转字符流
                   // 我们不用对两个流对象进行读写，只要复制流就OK  
                   pweiboic.CopyTo(outputStream);
                   ////写入文件，清除缓冲区
                   outputStream.Flush();

               }
           }

           catch
           {
               //失败返回-1
               ul.issuccess = -1;
               return ul;
           }
           ul.issuccess = 1;
           con.Close();
           return ul;
       }

         public yesno weibozhuan(string userid, string weiboid)//转发微博
         {
             yesno ul = new yesno();
             string connstr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["SqlConnStr"].ConnectionString;////数据库连接字符串

             SqlConnection con11 = new SqlConnection(connstr);
             SqlCommand cmd11 = new SqlCommand();
             cmd11.Connection = con11;
             con11.Open();

             //检查微博是否存在
             cmd11.CommandText = "SELECT *FROM PUZZLE.dbo.T_WeiBo tbv WHERE tbv.WeiBoID=" + weiboid;  //检查转发的微博是否存在
             SqlDataReader dr11 = cmd11.ExecuteReader();
             dr11.Read();
             try
             {
                 if (dr11.IsDBNull(0))
                 {
                     ul.issuccess = -1;
                     con11.Close();
                     return ul;
                 }
             }
             catch
             {
                 ul.issuccess = -1;
                 con11.Close();
                 return ul;
             }
             dr11.Close();
             ///检查用户是否存在
             cmd11.CommandText = "SELECT  tu.UserID FROM PUZZLE.dbo.T_Users tu WHERE tu.UserID=" + userid; //检查用户是否存在
             dr11 = cmd11.ExecuteReader();
             dr11.Read();
             try
             {
                 if (dr11.IsDBNull(0))
                 {
                     ul.issuccess = -1;
                     con11.Close();
                     return ul;
                 }
             }
             catch
             {
                 ul.issuccess = -1;
                 con11.Close();
                 return ul;
             }
             con11.Close();



             SqlConnection con = new SqlConnection(connstr);
             SqlCommand cmd = new SqlCommand();

             cmd.Connection = con;
             try
             {
                 con.Open();
             }
             catch (Exception e)
             {
                 string nie = e.ToString();
             }
             ////存储过程：在微博表中插入用户id，转发标志为1（表示为转发），转发类型为0（表示为微博），和转发的微博id；然后保存新插入的微博id
             ////在微博表中找到被转发的微博，微博转发数+1
             ////从微博表中找到作者id
             ///在消息表里面插入作者id，视频微博标志0（0为微博），被转发的微博id，评论赞转发标志2（2为转发），评论赞转发id为新插入的微博id，已读标志为0
             /*
             存储过程
             USE [PUZZLE]
             GO
             SET ANSI_NULLS ON
             GO
             SET QUOTED_IDENTIFIER ON
             GO
             ALTER PROCEDURE [dbo].[weibozhuan]
             @userid INT,
             @weiboid INT
             AS
             
             BEGIN
             	declare @zuozheid INT
             	DECLARE @zfweiboid INT
             	INSERT INTO PUZZLE.dbo.T_WeiBo(UserID,TranspondFlag,TypeFlag, TypeID)VALUES(@userid,1,0,@weiboid) set @zfweiboid=@@identity
             	UPDATE PUZZLE.dbo.T_WeiBo SET TranspondNumber = TranspondNumber+1 WHERE WeiBoID=@weiboid 
             	SELECT @zuozheid=twb.UserID FROM PUZZLE.dbo.T_WeiBo twb WHERE twb.WeiBoID=@weiboid
             	INSERT INTO PUZZLE.dbo.T_Message(ToUserID, TypeFlag, TypeID, ActionFlag,ActionID,IsRead)VALUES(@zuozheid,0,@weiboid,2,@zfweiboid,0)
             END
             

             */

             cmd.CommandText = "EXEC weibozhuan " + userid + "," + weiboid;//执行存储过程
             SqlDataReader dr = cmd.ExecuteReader();

             ul.issuccess = 1;//成功返回1
             con.Close();
             return ul;
         }


         public IList<classdynamic> seedynamic(string userid, string page)///看动态,page为页数
         {

             ///选择查看的页数
             string choisepage = "";//页数
             switch (page)
             {
                 case "0": choisepage = "0 AND 5 "; break;
                 case "1": choisepage = "5 AND 10"; break;
                 case "2": choisepage = "10 AND 15"; break;
                 case "3": choisepage = "15 AND 20"; break;
                 case "4": choisepage = "20 AND 25"; break;
                 case "5": choisepage = "25 AND 30"; break;
                 case "6": choisepage = "30 AND 35"; break;
                 case "7": choisepage = "35 AND 40"; break;
                 case "8": choisepage = "40 AND 45"; break;
                 case "9": choisepage = "45 AND 50"; break;
                 case "10": choisepage = "50 AND 55"; break;
                    
             }


             string connstr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["SqlConnStr"].ConnectionString;////数据库连接字符串
              IList<classdynamic> cdnclist = new List<classdynamic>();

              #region 检查存在性
              SqlConnection con11 = new SqlConnection(connstr);
              SqlCommand cmd11 = new SqlCommand();

              con11.Open();

              //检查用户是否存在
              cmd11.Connection = con11;
              cmd11.CommandText = "SELECT *FROM PUZZLE.dbo.T_Users tu WHERE tu.UserID=" + userid;
              SqlDataReader dr11 = cmd11.ExecuteReader();
              dr11.Read();
              try
              {
                  if (dr11.IsDBNull(0))
                  {
                      cdnclist = null;
                      con11.Close();
                      return cdnclist;
                  }
              }
              catch (Exception e)
              {
                  string tempe = e.ToString();
                  cdnclist = null;
                  con11.Close();
                  return cdnclist;
              }
              con11.Close();
              #endregion


              string userlist = "";///关注的用户的用户id

            SqlConnection conuli = new SqlConnection(connstr);
             SqlCommand cmduli = new SqlCommand();

             cmduli.Connection = conuli;

             try
             {
                 conuli.Open();
             }
             catch
             {
                 cdnclist = null;
                 return cdnclist;
             }

          //获取用户关注对象的用户id
             cmduli.CommandText = @"SELECT tur.User2,tur.User1 FROM PUZZLE.dbo.T_UserRelation tur WHERE (tur.User1 = " + userid + " AND tur.RelationFlag IN (0,1)) OR (tur.User2 =" + userid + " AND tur.RelationFlag IN(1,2));";
             SqlDataReader druli = cmduli.ExecuteReader();
            ///以上sql语句只得到了用户与被关注用户的关系对，要选出不同于本用户的被关注用户
             while (druli.Read())
             {
                 int tempuser1 = druli.GetInt32(0);
                 int tempuser2 = druli.GetInt32(1);
                 if (tempuser1.ToString() != userid)
                 {
                     userlist = userlist + tempuser1.ToString() + ",";
                 }
                 else
                 {
                     userlist = userlist + tempuser2.ToString() + ",";
                 }
             }
             userlist = userlist.Substring(0, userlist.Length - 1);//去掉字符串最后的“,”号

             conuli.Close();


           
           
             SqlConnection con = new SqlConnection(connstr);
             SqlCommand cmd = new SqlCommand();

             cmd.Connection = con;

             try
             {
                 con.Open();
             }
             catch
             {
                 cdnclist = null;
                 return cdnclist;
             }

             //联合用户表，用户关系表，微博表中查找关注用户id，关注用户头像，微博内容，发表时间，转发标志（1：转发，0：原创），文章音乐标志（微博0 文章1 音乐2），文章、微博、音乐ID，图片1，2，3，4，5，6，7，8，9，微博赞数，微博转发数，微博评论数 ，最后进行分页传输，根据请求就行分页

             /*
               sql分页函数 ROW_NUMBER()
             例子：
                       select * from (
                              select *,ROW_NUMBER() OVER (ORDER BY FlightsDetailID) as rank from tbl_FlightsDetail
                       )  as t where t.rank between 3000001 and 3000010
                       
                       
             */
             cmd.CommandText = @"SELECT *
                    FROM(SELECT 
                    tu.Name,tu.ICON,tu.VipFlag,twb.Details,twb.DTime,twb.TranspondFlag,twb.TypeFlag,twb.TypeID,twb.IMG1, twb.IMG2,
                                                     twb.IMG3, twb.IMG4, twb.IMG5, twb.IMG6, twb.IMG7, twb.IMG8, twb.IMG9,
                                                     twb.CommentNumber, twb.TranspondNumber, twb.ZanNumber,tu.UserID,
                     ROW_NUMBER() OVER(ORDER BY twb.DTime DESC)
                    AS row FROM 
                     PUZZLE.dbo.T_WeiBo twb JOIN PUZZLE.dbo.T_Users tu ON twb.UserID=tu.UserID WHERE twb.UserID IN ("+userlist+"))"
                   + " AS t WHERE t.row BETWEEN " + choisepage;
             SqlDataReader dr = cmd.ExecuteReader();

             while (dr.Read())
             {
                 int typeid = -1;
                 int typeflag = -1;

                 classdynamic ap = new classdynamic();
                
              if(!dr.IsDBNull(0))
                 ap.nicheng = dr.GetString(0);//用户昵称

              if (!dr.IsDBNull(1))
                 ap.icon = dr.GetString(1);//用户头像

              if (!dr.IsDBNull(2)) 
                 ap.vipflag = dr.GetInt32(2);//用户vip标志

              if (!dr.IsDBNull(3))
                 ap.datails = dr.GetString(3);//微博内容
              if (!dr.IsDBNull(4))
                 ap.Dtime = dr.GetDateTime(4);//发表时间
              if (!dr.IsDBNull(5))
                 ap.zhuanfaflag =Convert.ToInt32(dr.GetBoolean(5));//转发标志
              if (!dr.IsDBNull(6))
                  typeflag = dr.GetInt32(6);//转发类型
              if (!dr.IsDBNull(7))
                 typeid = dr.GetInt32(7) ;//转发内容id
              if (!dr.IsDBNull(8))
                 ap.img1 = dr.GetString(8);//图片1
              if (!dr.IsDBNull(9))
                  ap.img2 = dr.GetString(9);//图片2
              if (!dr.IsDBNull(10))
                  ap.img3 = dr.GetString(10);//图片3
              if (!dr.IsDBNull(11))
                  ap.img4 = dr.GetString(11);//图片4
              if (!dr.IsDBNull(12))
                  ap.img5 = dr.GetString(12);//图片5
              if (!dr.IsDBNull(13))
                  ap.img6 = dr.GetString(13);//图片6
              if (!dr.IsDBNull(14))
                  ap.img7 = dr.GetString(14);//图片7
              if (!dr.IsDBNull(15))
                  ap.img8 = dr.GetString(15);//图片8
              if (!dr.IsDBNull(16))
                  ap.img9 = dr.GetString(16);//图片9
              if (!dr.IsDBNull(17))
                 ap.commentnum = dr.GetInt32(17);//评论数
              if (!dr.IsDBNull(18))
                 ap.transpondnum = dr.GetInt32(18);//转发数
              if (!dr.IsDBNull(19))
                 ap.zannum = dr.GetInt32(19);//赞数
              if (!dr.IsDBNull(20))
                  ap.userid=dr.GetInt32(20);
                
                 if (ap.zhuanfaflag == 1)//如果转发标志为1，则找转发的类型
                 {

                     switch (typeflag)
                     {
                         case 0://转发类型为0，则找相应的微博
                             {
                                 ap.zhuanwb = new zhuanweibo();
                                 SqlConnection conw = new SqlConnection(connstr);
                                 SqlCommand cmdw = new SqlCommand();
                                 cmdw.Connection = conw;
                                 conw.Open();
                                 //找原微博
                                 cmdw.CommandText = @"SELECT twb.Details, twb.DTime, twb.IMG1, twb.IMG2, twb.IMG3, twb.IMG4, twb.IMG5,
                                                   twb.IMG6, twb.IMG7, twb.IMG8, twb.IMG9,
                                                   twb.TranspondNumber, twb.ZanNumber,twb.WeiBoID
                                                   FROM PUZZLE.dbo.T_WeiBo twb WHERE WeiBoID=" + typeid+";";
                                 SqlDataReader drw = cmdw.ExecuteReader();
                                  drw.Read();
                                  if (!drw.IsDBNull(0))
                                      ap.zhuanwb.wbdatails = drw.GetString(0);//微博内容
                                  if (!drw.IsDBNull(1))
                                      ap.zhuanwb.wbweibotime = drw.GetDateTime(1);//微博发表时间
                                  if (!drw.IsDBNull(2))
                                      ap.zhuanwb.wbimg1 = drw.GetString(2);//微博图片1
                                  if (!drw.IsDBNull(3))
                                      ap.zhuanwb.wbimg2 = drw.GetString(3);//微博图片2
                                  if (!drw.IsDBNull(4))
                                      ap.zhuanwb.wbimg3 = drw.GetString(4);//微博图片3
                                  if (!drw.IsDBNull(5))
                                      ap.zhuanwb.wbimg4 = drw.GetString(5);//微博图片4
                                  if (!drw.IsDBNull(6))
                                      ap.zhuanwb.wbimg5 = drw.GetString(6);//微博图片5
                                  if (!drw.IsDBNull(7))
                                      ap.zhuanwb.wbimg6 = drw.GetString(7);//微博图片6
                                  if (!drw.IsDBNull(8))
                                      ap.zhuanwb.wbimg7 = drw.GetString(8);//微博图片7
                                  if (!drw.IsDBNull(9))
                                      ap.zhuanwb.wbimg8 = drw.GetString(9);//微博图片8
                                  if (!drw.IsDBNull(10))
                                      ap.zhuanwb.wbimg9 = drw.GetString(10);//微博图片9
                                  if (!drw.IsDBNull(11))
                                      ap.zhuanwb.wbTranspondNum = drw.GetInt32(11);//转发数
                                  if (!drw.IsDBNull(12))
                                      ap.zhuanwb.wbznanum = drw.GetInt32(12);//赞数

                             
                                  conw.Close();
                                 break;
                             }
                         case 1: //转发类型为1，则找相应的文章
                             {
                                 ap.zhuanart = new zhuanarticle();
                                 SqlConnection cona = new SqlConnection(connstr);
                                 SqlCommand cmda = new SqlCommand();
                                 cmda.Connection = cona;
                                 cona.Open();
                                 cmda.CommandText = @"SELECT ta.UploadTime, ta.Title, ta.Details,
                                                   ta.TranspondNumber, ta.ZanNumber
                                                   FROM PUZZLE.dbo.T_Article ta WHERE ta.ArticleID=" + typeid + ";";
                                 SqlDataReader dra = cmda.ExecuteReader();
                                 dra.Read();
                                 if (!dra.IsDBNull(0))
                                     ap.zhuanart.artupdtime = dra.GetDateTime(0);//文章上传时间
                                 if (!dra.IsDBNull(1))
                                     ap.zhuanart.arttitle = dra.GetString(1);//文章标题
                                 if (!dra.IsDBNull(2))
                                     ap.zhuanart.artdatailes = dra.GetString(2);//文章内容
                                 if (!dra.IsDBNull(3))
                                     ap.zhuanart.artTranspondNum = dra.GetInt32(3);//转发数
                                 if (!dra.IsDBNull(4))
                                     ap.zhuanart.artzannum = dra.GetInt32(4);//赞数
                                
                                cona.Close();
                                 break; 
                             }
                         case 2: //转发类型为2，则找相应的音乐
                             {
                                 ap.zhuanmu = new zhuanmusic();
                                 SqlConnection conm = new SqlConnection(connstr);
                                 SqlCommand cmdm = new SqlCommand();
                                 cmdm.Connection = conm;
                                 conm.Open();
                                 cmdm.CommandText = @"SELECT tm.UploadTime, tm.Music, tm.MusicPIC, tm.Introduce, tm.TranspondNumber,
                                                    tm.ZanNumber
                                                    FROM PUZZLE.dbo.T_Music tm WHERE tm.MusicID=" + typeid + ";";
                                 SqlDataReader drm = cmdm.ExecuteReader();
                                 dr.Read();
                                 if (!drm.IsDBNull(0))
                                     ap.zhuanmu.muupdtime = drm.GetDateTime(0);///音乐上传时间
                                 if (!drm.IsDBNull(1))
                                     ap.zhuanmu.muname = drm.GetString(1);//音乐名称
                                 if (!drm.IsDBNull(2))
                                     ap.zhuanmu.mupic = drm.GetString(2);//音乐图片
                                 if (!drm.IsDBNull(3))
                                     ap.zhuanmu.muintro = drm.GetString(3);//音乐介绍
                                 if (!drm.IsDBNull(4))
                                     ap.zhuanmu.muTranspondNum = drm.GetInt32(4);//转发数
                                 if (!drm.IsDBNull(5))
                                     ap.zhuanmu.muzannum = drm.GetInt32(5);//赞数
                                 conm.Close();
                                 break; 
                             }
                     }
                     
                 }
                 cdnclist.Add(ap);
             }
        
             con.Close();
             return cdnclist;
         }

        public yesno weibocomment(string userid, string weiboid,string content)//评论微博
        {
            yesno ul = new yesno();
            string connstr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["SqlConnStr"].ConnectionString;////数据库连接字符串

            SqlConnection con11 = new SqlConnection(connstr);
            SqlCommand cmd11 = new SqlCommand();
            cmd11.Connection = con11;
            con11.Open();
            cmd11.CommandText = "SELECT *FROM PUZZLE.dbo.T_WeiBo tbv WHERE tbv.WeiBoID=" + weiboid;  //检查评论的微博是否存在
            SqlDataReader dr11 = cmd11.ExecuteReader();
            dr11.Read();
            try
            {
                if (dr11.IsDBNull(0))
                {
                    ul.issuccess = -1;
                    con11.Close();
                    return ul;
                }
            }
            catch
            {
                ul.issuccess = -1;
                con11.Close();
                return ul;
            }
            dr11.Close();
            //检查用户是否存在
            cmd11.CommandText = "SELECT  tu.UserID FROM PUZZLE.dbo.T_Users tu WHERE tu.UserID=" + userid; //检查用户是否存在
            dr11 = cmd11.ExecuteReader();
            dr11.Read();
            try
            {
                if (dr11.IsDBNull(0))
                {
                    ul.issuccess = -1;
                    con11.Close();
                    return ul;
                }
            }
            catch
            {
                ul.issuccess = -1;
                con11.Close();
                return ul;
            }
            con11.Close();


            SqlConnection con = new SqlConnection(connstr);
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = con;
            try
            {
                con.Open();
            }
            catch (Exception e)
            {
                string nie = e.ToString();
                ul.issuccess = -1;
                return ul;
            }
            ///在微博评论表里面插入：评论内容，微博id，用户id ，然后获得评论id
            ///在微博表中找找作者id
            ///消息表中插入作者id，视频博客标识0，视频或博客id，评论、赞标识0（评论0 赞1 转发2），评论id，已读标志为0
            /*
            存储过程
           CREATE PROCEDURE weibocomment
           @userid INT,
           @weiboid INT,
           @content VARCHAR(MAX)
           AS
           BEGIN
           	DECLARE @commentid INT
           	declare @zuozheid INT
           	INSERT INTO PUZZLE.dbo.T_WeiboComment(Details, WeiBoID, UserID)VALUES(@content,@weiboid,@userid)SET @commentid=@@IDENTITY
           	SELECT @zuozheid=twb.UserID FROM PUZZLE.dbo.T_WeiBo twb WHERE twb.WeiBoID=@weiboid
           	INSERT INTO PUZZLE.dbo.T_Message(ToUserID, TypeFlag, TypeID, ActionFlag,ActionID, IsRead)VALUES(@zuozheid,0,@weiboid,0, @commentid,0)
           	UPDATE PUZZLE.dbo.T_WeiBo SET CommentNumber = CommentNumber+1 WHERE WeiBoID=@weiboid 	
           END
           GO

            */

            cmd.CommandText = "EXEC weibocomment " + userid + "," + weiboid + "," + content;//执行存储过程
            SqlDataReader dr = cmd.ExecuteReader();

            ul.issuccess = 1;///成功返回1
            con.Close();
            return ul;
        }


        public yesno weibozhan(string userid, string weiboid)///微博赞
        {
            yesno ul = new yesno();
            string connstr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["SqlConnStr"].ConnectionString;////数据库连接字符串


            SqlConnection con11 = new SqlConnection(connstr);
            SqlCommand cmd11 = new SqlCommand();
            cmd11.Connection = con11;
            con11.Open();
            cmd11.CommandText = "SELECT *FROM PUZZLE.dbo.T_WeiBo tbv WHERE tbv.WeiBoID=" + weiboid;  //检查赞的微博是否存在
            SqlDataReader dr11 = cmd11.ExecuteReader();
            dr11.Read();
            try
            {
                if (dr11.IsDBNull(0))
                {
                    ul.issuccess = -1;
                    con11.Close();
                    return ul;
                }
            }
            catch
            {
                ul.issuccess = -1;
                con11.Close();
                return ul;
            }
            dr11.Close();
            //检查用户是否存在
            cmd11.CommandText = "SELECT  tu.UserID FROM PUZZLE.dbo.T_Users tu WHERE tu.UserID=" + userid;
            dr11 = cmd11.ExecuteReader();
            dr11.Read();
            try
            {
                if (dr11.IsDBNull(0))
                {
                    ul.issuccess = -1;
                    con11.Close();
                    return ul;
                }
            }
            catch
            {
                ul.issuccess = -1;
                con11.Close();
                return ul;
            }
            con11.Close();




            SqlConnection con = new SqlConnection(connstr);
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = con;
            try
            {
                con.Open();
            }
            catch (Exception e)
            {
                string nie = e.ToString();
                ul.issuccess = -1;
                return ul;
            }
            ///赞表中插入文章、微博、音乐、视频标识0（微博0，文章1，音乐2，视频3），微博id，用户id，保存新插入赞表的id
            ///在微博表通过微博id中查找作者id
            ///消息表中插入：作者id，视频博客标识：0（博客0 视频1），博客id，评论、赞标识：1（评论0 赞1 转发2），新插入赞表的id，已读标志0
            ///微博表中赞数+1
            /*
            存储过程
          USE [PUZZLE]
          GO
          SET ANSI_NULLS ON
          GO
          SET QUOTED_IDENTIFIER ON
          GO
           ALTER PROCEDURE [dbo].[weibozan]
                     @userid INT,
                     @weiboid INT
                     AS
                     BEGIN
                     	declare @zuozheid INT
                     	DECLARE @zanid INT
                     	INSERT INTO PUZZLE.dbo.T_Zan(TypeFlag, TypeID, UserID)VALUES(0,@weiboid,@userid) set @zanid=@@identity
                     	SELECT @zuozheid=twb.UserID FROM PUZZLE.dbo.T_WeiBo twb WHERE twb.WeiBoID=@weiboid
                     	INSERT INTO PUZZLE.dbo.T_Message(ToUserID, TypeFlag, TypeID, ActionFlag,ActionID, IsRead)VALUES(@zuozheid,0,@weiboid,1,@zanid,0)
                      UPDATE PUZZLE.dbo.T_WeiBo SET ZanNumber = ZanNumber+1 WHERE WeiBoID=@weiboid 
                     END

            */

            cmd.CommandText = "EXEC weibozan " + userid + "," + weiboid;
            SqlDataReader dr = cmd.ExecuteReader();

            ul.issuccess = 1;
            con.Close();
            return ul;
        }

        public   IList<classmessage> seemessage(string userid)///看消息
        {
            IList<classmessage> messlist = new List<classmessage>();
             string connstr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["SqlConnStr"].ConnectionString;////数据库连接字符串


             #region 检查存在性
             SqlConnection con11 = new SqlConnection(connstr);
             SqlCommand cmd11 = new SqlCommand();

             con11.Open();

             //检查用户是否存在
             cmd11.Connection = con11;
             cmd11.CommandText = "SELECT *FROM PUZZLE.dbo.T_Users tu WHERE tu.UserID=" + userid;
             SqlDataReader dr11 = cmd11.ExecuteReader();
             dr11.Read();
             try
             {
                 if (dr11.IsDBNull(0))
                 {
                     messlist=null;
                     con11.Close();
                     return messlist;
                 }
             }
             catch (Exception e)
             {
                 string tempe = e.ToString();
                 messlist = null;
                 con11.Close();
                 return messlist;
             }
             con11.Close();
             #endregion



            SqlConnection con = new SqlConnection(connstr);
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = con;

            try
            {
                con.Open();
            }
            catch
            {
                messlist = null;
                return messlist;
            }
            cmd.CommandText = @"SELECT tm.TypeFlag, tm.TypeID, tm.ActionFlag, tm.ActionID
                        FROM PUZZLE.dbo.T_Message tm WHERE tm.IsRead=0 AND tm.ToUserID=" + userid                    ////在消息表中查找未读的和对应用户id，选取视频博客标识，视频或博客id，评论、赞标识，评论、赞、转发ID
                        +"  UPDATE  PUZZLE.dbo.T_Message SET IsRead = 1 WHERE ToUserID="+userid;                     ////在消息表中将未读改为已读
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                string vidoweiboid=null;
                string actionid=null;

                classmessage cmes = new classmessage();
                if (!dr.IsDBNull(0))
                cmes.vidoweiflag = dr.GetInt32(0);//视频博客标识
                if (!dr.IsDBNull(1))
                    vidoweiboid = dr.GetInt32(1).ToString();//视频或博客id
                if (!dr.IsDBNull(2))
                    cmes.actionflag = dr.GetInt32(2);//视频或博客id，评论、赞标识
                if (!dr.IsDBNull(3))
                    actionid = dr.GetInt32(3).ToString();//评论、赞、转发ID


                if (cmes.vidoweiflag == 0)//博客（博客0 视频1）
                {
                    cmes.wbme = new weibomessage();
                    SqlConnection conwb = new SqlConnection(connstr);
                    SqlCommand cmdwb = new SqlCommand();

                    cmdwb.Connection = conwb;
                    conwb.Open();
                    cmdwb.CommandText = @"SELECT twb.WeiBoID, twb.Details, twb.IMG1, twb.IMG2, twb.IMG3, twb.IMG4, twb.IMG5,
                                          twb.IMG6, twb.IMG7, twb.IMG8, twb.IMG9
                                          FROM PUZZLE.dbo.T_WeiBo twb WHERE twb.WeiBoID=" + vidoweiboid;
                    SqlDataReader drwb = cmdwb.ExecuteReader();
                    drwb.Read();
                    if (!drwb.IsDBNull(0))
                    cmes.wbme.wbweiboid = drwb.GetInt32(0);
                    if (!drwb.IsDBNull(1))
                    cmes.wbme.wbdetail = drwb.GetString(1);
                    if (!drwb.IsDBNull(2))
                    cmes.wbme.img1 = drwb.GetString(2);
                    if (!drwb.IsDBNull(3))
                    cmes.wbme.img2 = drwb.GetString(3);
                    if (!drwb.IsDBNull(4))
                    cmes.wbme.img3 = drwb.GetString(4);
                    if (!drwb.IsDBNull(5))
                    cmes.wbme.img4 = drwb.GetString(5);
                    if (!drwb.IsDBNull(6))
                    cmes.wbme.img5 = drwb.GetString(6);
                    if (!drwb.IsDBNull(7))
                    cmes.wbme.img6 = drwb.GetString(7);
                    if (!drwb.IsDBNull(8))
                    cmes.wbme.img7 = drwb.GetString(8);
                    if (!drwb.IsDBNull(9))
                    cmes.wbme.img8 = drwb.GetString(9);
                    if (!drwb.IsDBNull(10))
                    cmes.wbme.img9 = drwb.GetString(10);
                    conwb.Close();
                    if (cmes.actionflag == 0)///评论0
                    {
                        cmes.wbp = new weiboping();
                        SqlConnection conwbping = new SqlConnection(connstr);
                        SqlCommand cmdwbping = new SqlCommand();

                        cmdwbping.Connection = conwbping;
                        conwbping.Open();
                        cmdwbping.CommandText = @"SELECT  tvcm.Details,tvcm.DTime,tu.Name,tu.ICON FROM PUZZLE.dbo.T_WeiboComment tvcm LEFT JOIN PUZZLE.dbo.T_Users tu ON tvcm.UserID=tu.UserID WHERE tvcm.CommentID=" + actionid;
                        SqlDataReader drwbping = cmdwbping.ExecuteReader();
                        drwbping.Read();
                        if (!drwbping.IsDBNull(0))
                        cmes.wbp.wbpingdetails= drwbping.GetString(0);
                        if (!drwbping.IsDBNull(1))
                        cmes.wbp.wbpingupdate = drwbping.GetDateTime(1);
                        if (!drwbping.IsDBNull(2)) 
                        cmes.wbp.nicehng = drwbping.GetString(2);
                        if (!drwbping.IsDBNull(3))
                         cmes.wbp.usericno = drwbping.GetString(3);
                        conwbping.Close();
                    }


                    if (cmes.actionflag == 1)/// 赞1 
                    {
                        cmes.wbz = new weibozan();
                        SqlConnection conwbz = new SqlConnection(connstr);
                        SqlCommand cmdwbz = new SqlCommand();

                        cmdwbz.Connection = conwbz;
                        conwbz.Open();
                        cmdwbz.CommandText = @"SELECT tu.Name,tu.ICON FROM PUZZLE.dbo.T_Zan tz  JOIN PUZZLE.dbo.T_Users tu ON tz.UserID=tu.UserID WHERE tz.ZanID=" + actionid + "AND tz.TypeFlag=0";
                        SqlDataReader drwbz = cmdwbz.ExecuteReader();
                        drwbz.Read();
                        if (!drwbz.IsDBNull(0))
                        cmes.wbz.nicehng = drwbz.GetString(0);
                        if (!drwbz.IsDBNull(1))
                        cmes.wbz.usericno = drwbz.GetString(1);
                        conwbz.Close();
                    }

                    if (cmes.actionflag == 2)// 转发2
                    {
                        cmes.wbzhuan = new weibozhuan();
                        SqlConnection conwbzhuan = new SqlConnection(connstr);
                        SqlCommand cmdwbzhuan = new SqlCommand();

                        cmdwbzhuan.Connection = conwbzhuan;
                        conwbzhuan.Open();
                        cmdwbzhuan.CommandText = @"SELECT tu.Name,tu.ICON FROM PUZZLE.dbo.T_WeiBo tw JOIN PUZZLE.dbo.T_Users tu ON tw.UserID=tu.UserID WHERE tw.WeiBoID=" + actionid;
                        SqlDataReader drwbzhuan = cmdwbzhuan.ExecuteReader();
                        drwbzhuan.Read();
                        if (!drwbzhuan.IsDBNull(0))
                            cmes.wbzhuan.nicehng = drwbzhuan.GetString(0);
                        if (!drwbzhuan.IsDBNull(1))
                            cmes.wbzhuan.usericno = drwbzhuan.GetString(1);
                        conwbzhuan.Close();
                    }
                }
                else//视频（博客0 视频1）
                {

                    if (cmes.actionflag == 0)//评论0 
                    {
                        cmes.viping = new videoping();
                        SqlConnection convdping = new SqlConnection(connstr);
                        SqlCommand cmdvdping = new SqlCommand();

                        cmdvdping.Connection = convdping;
                        convdping.Open();
                        cmdvdping.CommandText = @"SELECT tvd.Details,tvd.DTime,tu.Name,tu.ICON FROM PUZZLE.dbo.T_VideoComment tvd JOIN PUZZLE.dbo.T_Users tu ON tvd.UserID=tu.UserID WHERE tvd.VideoID=" + actionid;
                        SqlDataReader drvdping = cmdvdping.ExecuteReader();
                        drvdping.Read();
                        if (!drvdping.IsDBNull(0))
                            cmes.viping.videtails = drvdping.GetString(0);
                        if (!drvdping.IsDBNull(1))
                            cmes.viping.videdate = drvdping.GetDateTime(1);
                        if (!drvdping.IsDBNull(2))
                            cmes.viping.nicehng = drvdping.GetString(2);
                        if (!drvdping.IsDBNull(3))
                            cmes.viping.usericno = drvdping.GetString(3);
                        convdping.Close();
                    }
                    if (cmes.actionflag == 1)// 赞1
                    {
                        cmes.vdz = new videozan();
                        SqlConnection convdz = new SqlConnection(connstr);
                        SqlCommand cmdvdz = new SqlCommand();

                        cmdvdz.Connection = convdz;
                        convdz.Open();
                        cmdvdz.CommandText = @"SELECT tu.Name,tu.ICON FROM PUZZLE.dbo.T_Zan tz  JOIN PUZZLE.dbo.T_Users tu ON tz.UserID=tu.UserID WHERE tz.ZanID=" + actionid + "AND tz.TypeFlag=3";
                        SqlDataReader drvdz = cmdvdz.ExecuteReader();
                        drvdz.Read();
                        if (!drvdz.IsDBNull(0))
                            cmes.vdz.nicehng = drvdz.GetString(0);
                        if (!drvdz.IsDBNull(1))
                            cmes.vdz.usericno = drvdz.GetString(1);
                        convdz.Close();
                    }
                    if (cmes.actionflag == 2)/// 转发2
                    {
                        cmes.vdzhuan = new videozhuan();
                        SqlConnection convdzhuan = new SqlConnection(connstr);
                        SqlCommand cmdvdzhuan = new SqlCommand();

                        cmdvdzhuan.Connection = convdzhuan;
                        convdzhuan.Open();
                        cmdvdzhuan.CommandText = @"SELECT tu.Name,tu.ICON FROM PUZZLE.dbo.T_WeiBo tw JOIN PUZZLE.dbo.T_Users tu ON tw.UserID=tu.UserID WHERE tw.WeiBoID=" + actionid;
                        SqlDataReader drvdzhuan = cmdvdzhuan.ExecuteReader();
                        drvdzhuan.Read();
                        if (!drvdzhuan.IsDBNull(0))
                            cmes.vdzhuan.nicehng = drvdzhuan.GetString(0);
                        if (!drvdzhuan.IsDBNull(1))
                            cmes.vdzhuan.usericno = drvdzhuan.GetString(1);
                        convdzhuan.Close();
                    }
                }
                messlist.Add(cmes);
            }
            con.Close();///关闭数据库
            return messlist;//返回列表
        }

        public IList<classzan> seelike(string userid)///看喜欢
        {
            IList<classzan> zanlist = new List<classzan>();
             string connstr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["SqlConnStr"].ConnectionString;////数据库连接字符串

             #region 检查存在性
             SqlConnection con11 = new SqlConnection(connstr);
             SqlCommand cmd11 = new SqlCommand();

             con11.Open();

             //检查用户是否存在
             cmd11.Connection = con11;
             cmd11.CommandText = "SELECT *FROM PUZZLE.dbo.T_Users tu WHERE tu.UserID=" + userid;
             SqlDataReader dr11 = cmd11.ExecuteReader();
             dr11.Read();
             try
             {
                 if (dr11.IsDBNull(0))
                 {
                     zanlist = null;
                     con11.Close();
                     return zanlist;
                 }
             }
             catch (Exception e)
             {
                 string tempe = e.ToString();
                 zanlist = null;
                 con11.Close();
                 return zanlist;
             }
             con11.Close();
             #endregion



            SqlConnection con = new SqlConnection(connstr);
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = con;

            try
            {
                con.Open();
            }
            catch
            {
                zanlist = null;
                return zanlist;
            }
            cmd.CommandText = @"SELECT tz.TypeFlag, tz.TypeID, tz.DTime FROM PUZZLE.dbo.T_Zan tz WHERE tz.UserID=" + userid;
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                classzan clz = new classzan();
                int typeid = -1;
                if (!dr.IsDBNull(0)) 
                clz.typeflag = dr.GetInt32(0);
                if (!dr.IsDBNull(1))
                 typeid = dr.GetInt32(1);
                if (!dr.IsDBNull(2))
                 clz.dtime = dr.GetDateTime(2);


                if (clz.typeflag == 0)
                {
                    clz.zwb = new zanweibo();
                    SqlConnection conwb = new SqlConnection(connstr);
                    SqlCommand cmdwb = new SqlCommand();
                    cmdwb.Connection = conwb;
                    conwb.Open();
                    cmdwb.CommandText = @"SELECT tw.WeiBoID, tw.Details, tw.DTime, tw.IMG1, tw.IMG2, tw.IMG3, tw.IMG4, tw.IMG5, tw.IMG6,
                                          tw.IMG7, tw.IMG8, tw.IMG9
                                          FROM PUZZLE.dbo.T_WeiBo tw WHERE tw.WeiBoID=" + typeid;
                    SqlDataReader drwb = cmdwb.ExecuteReader();
                    drwb.Read();
                    if(!drwb.IsDBNull(0))
                    clz.zwb.wbweiboid = drwb.GetInt32(0);
                    if (!drwb.IsDBNull(1))
                    clz.zwb.wbdetail = drwb.GetString(1);
                    if (!drwb.IsDBNull(2))
                     clz.zwb.wbuptime = drwb.GetDateTime(2);
                    if (!drwb.IsDBNull(3))
                      clz.zwb.img1 = drwb.GetString(3);
                    if (!drwb.IsDBNull(4))
                        clz.zwb.img2 = drwb.GetString(4);
                    if (!drwb.IsDBNull(5))
                        clz.zwb.img3 = drwb.GetString(5);
                    if (!drwb.IsDBNull(6))
                        clz.zwb.img4 = drwb.GetString(6);
                    if (!drwb.IsDBNull(7))
                        clz.zwb.img5 = drwb.GetString(7);
                    if (!drwb.IsDBNull(8))
                        clz.zwb.img6 = drwb.GetString(8);
                    if (!drwb.IsDBNull(9))
                        clz.zwb.img7 = drwb.GetString(9);
                    if (!drwb.IsDBNull(10))
                        clz.zwb.img8 = drwb.GetString(10);
                    if (!drwb.IsDBNull(11))
                        clz.zwb.img9 = drwb.GetString(11);
                    conwb.Close();
                }

                if (clz.typeflag == 1)
                {
                    clz.zwz = new zanwenzhang();
                    SqlConnection conart = new SqlConnection(connstr);
                    SqlCommand cmdart = new SqlCommand();
                    cmdart.Connection = conart;
                    conart.Open();
                    cmdart.CommandText = @"SELECT ta.ArticleID, ta.UploadTime, ta.Title, ta.Details
                                         FROM PUZZLE.dbo.T_Article ta WHERE ta.ArticleID=" + typeid;
                    SqlDataReader drart = cmdart.ExecuteReader();
                    drart.Read();
                    if(!drart.IsDBNull(0))
                    clz.zwz.wenzhangid = drart.GetInt32(0);
                    if (!drart.IsDBNull(1))
                        clz.zwz.aruptime = drart.GetDateTime(1);
                    if (!drart.IsDBNull(2))
                        clz.zwz.artitle = drart.GetString(2);
                    if (!drart.IsDBNull(3))
                        clz.zwz.ardetail = drart.GetString(3);
                    conart.Close();

                }

                if (clz.typeflag == 2)
                {
                    clz.zmu = new zanmusic();
                    SqlConnection conmu = new SqlConnection(connstr);
                    SqlCommand cmdmu = new SqlCommand();
                    cmdmu.Connection = conmu;
                    conmu.Open();
                    cmdmu.CommandText = @"SELECT tm.MusicID,tm.UploadTime, tm.Music, tm.MusicPIC, tm.Introduce
                                          FROM PUZZLE.dbo.T_Music tm WHERE tm.MusicID=" + typeid;
                    SqlDataReader drmu = cmdmu.ExecuteReader();
                    drmu.Read();
                    if(!drmu.IsDBNull(0))
                    clz.zmu.muid = drmu.GetInt32(0);
                    if (!drmu.IsDBNull(1))
                        clz.zmu.muuptime = drmu.GetDateTime(1);
                    if (!drmu.IsDBNull(2))
                        clz.zmu.muname = drmu.GetString(2);
                    if (!drmu.IsDBNull(3))
                        clz.zmu.mupic = drmu.GetString(3);
                    if (!drmu.IsDBNull(4))
                        clz.zmu.muintroduce = drmu.GetString(4);
                    conmu.Close();
                }

                if (clz.typeflag == 3)
                {
                    clz.zvd = new zanvideo();
                    SqlConnection convd = new SqlConnection(connstr);
                    SqlCommand cmdvd = new SqlCommand();
                    cmdvd.Connection = convd;
                    convd.Open();
                    cmdvd.CommandText = @"SELECT tbv.VideoID,tbv.Video ,tbv.UploadTime, tbv.Title
                                         FROM PUZZLE.dbo.T_BandVideo tbv WHERE tbv.VideoID=" + typeid;
                    SqlDataReader drvd = cmdvd.ExecuteReader();
                    drvd.Read();
                    if(!drvd.IsDBNull(0))
                    clz.zvd.videoid = drvd.GetInt32(0);
                    if (!drvd.IsDBNull(1))
                        clz.zvd.videoname = drvd.GetString(1);
                    if (!drvd.IsDBNull(2))
                        clz.zvd.videouptime = drvd.GetDateTime(2);
                    if (!drvd.IsDBNull(3))
                        clz.zvd.videoTitle = drvd.GetString(3);
                    convd.Close();
                }
                zanlist.Add(clz);
            }
            con.Close();
            return zanlist;
        }

       public IList<banduser> seeband()///看乐队
        {
            IList<banduser> bandlist = new List<banduser>();
             string connstr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["SqlConnStr"].ConnectionString;////数据库连接字符串
            SqlConnection con = new SqlConnection(connstr);
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = con;

            try
            {
                con.Open();
            }
            catch
            {
                bandlist = null;
                return bandlist;
            }
            cmd.CommandText = @"SELECT tu.UserID,tu.Name,tu.ICON,tu.VipFlag,tu.UploadNumber,tu.Sign FROM PUZZLE.dbo.T_Users tu WHERE tu.VipFlag=3";
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                banduser bu = new banduser();
                if(!dr.IsDBNull(0))
                bu.bandid = dr.GetInt32(0);
                if (!dr.IsDBNull(1))
                    bu.nicheng = dr.GetString(1);
                if (!dr.IsDBNull(2))
                    bu.icon = dr.GetString(2);
                if (!dr.IsDBNull(3))
                bu.vipflag = dr.GetInt32(3);
                if (!dr.IsDBNull(4))
                    bu.uploadnu = dr.GetInt32(4);
                if (!dr.IsDBNull(5))
                    bu.sign = dr.GetString(5);
                bandlist.Add(bu);
            }
            con.Close();
            return bandlist;
        }

       public IList<videolist> seevideplist(string bandid)//看视频列表
       {
           IList<videolist> vdlist = new List<videolist>();
              string connstr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["SqlConnStr"].ConnectionString;////数据库连接字符串

              #region 检查存在性
              SqlConnection con11 = new SqlConnection(connstr);
              SqlCommand cmd11 = new SqlCommand();

              con11.Open();

              //检查用户是否存在
              cmd11.Connection = con11;
              cmd11.CommandText = "SELECT *FROM PUZZLE.dbo.T_Users tu WHERE tu.UserID=" + bandid;
              SqlDataReader dr11 = cmd11.ExecuteReader();
              dr11.Read();
              try
              {
                  if (dr11.IsDBNull(0))
                  {
                      vdlist = null;
                      con11.Close();
                      return vdlist;
                  }
              }
              catch (Exception e)
              {
                  string tempe = e.ToString();
                  vdlist = null;
                  con11.Close();
                  return vdlist;
              }
              con11.Close();
              #endregion

            SqlConnection con = new SqlConnection(connstr);
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = con;

            try
            {
                con.Open();
            }
            catch
            {
                vdlist = null;
                return vdlist;
            }
            cmd.CommandText = @"SELECT tbv.Video, tbv.UploadTime, tbv.ZanNumber, tbv.CommentNumber, tbv.Title,
       tbv.Thumbnail,tbv.VideoID
  FROM PUZZLE.dbo.T_BandVideo tbv WHERE tbv.UserID=" + bandid;
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                videolist vdl = new videolist();
                if (!dr.IsDBNull(0))
                    vdl.videoname = dr.GetString(0);
                if (!dr.IsDBNull(1))
                    vdl.videuptime = dr.GetDateTime(1);
                if (!dr.IsDBNull(2))
                    vdl.zannumm = dr.GetInt32(2);
                if (!dr.IsDBNull(3))
                    vdl.commentnum = dr.GetInt32(3);
                if (!dr.IsDBNull(4))
                    vdl.videoTitle = dr.GetString(4);
                if (!dr.IsDBNull(5))
                    vdl.videoThumbnail = dr.GetString(5);
                if (!dr.IsDBNull(6))
                    vdl.videoid = dr.GetInt32(6);
                vdlist.Add(vdl);

            }
            con.Close();
            return vdlist;
       }

     public  yesno watchvideo(string videoid)///用户看视频
       {
           yesno wv = new yesno();
           string connstr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["SqlConnStr"].ConnectionString;////数据库连接字符串

           SqlConnection con11 = new SqlConnection(connstr);
           SqlCommand cmd11 = new SqlCommand();
           cmd11.Connection = con11;
           con11.Open();
           cmd11.CommandText = "SELECT *FROM PUZZLE.dbo.T_BandVideo tbv WHERE tbv.VideoID=" + videoid;  //检查视频是否存在
           SqlDataReader dr11 = cmd11.ExecuteReader();
           dr11.Read();
           try
           {
               if (dr11.IsDBNull(0))
               {
                   wv.issuccess = -1;
                   con11.Close();
                   return wv;
               }
           }
           catch
           {
               wv.issuccess = -1;
               con11.Close();
               return wv;
           }
           con11.Close();




           SqlConnection con = new SqlConnection(connstr);
           SqlCommand cmd = new SqlCommand();

           cmd.Connection = con;

           try
           {
               con.Open();
           }
           catch
           {
               wv.issuccess = -1;
               return wv;
           }
           /*存储过程
          CREATE PROCEDURE watchvideo
          @videoid INT
          AS
          BEGIN
          	DECLARE @mycount INT=-1
          	SELECT @mycount=COUNT(*) FROM PUZZLE.dbo.T_VideoPlayTimes tvpt WHERE tvpt.VodeoID=@videoid 
          	IF @mycount<1
          	BEGIN
          		INSERT INTO PUZZLE.dbo.T_VideoPlayTimes(VodeoID,Times)VALUES(@videoid,1)
          	END
          	ELSE
          	BEGIN
          		UPDATE PUZZLE.dbo.T_VideoPlayTimes SET Times = Times+1 WHERE VodeoID=@videoid
          	END
          	
          END
          GO
           
            */
           cmd.CommandText = @"EXEC PUZZLE.dbo.watchvideo " + videoid;
           cmd.ExecuteReader();
           wv.issuccess = 1;
           con.Close();
           return wv;
       }

     public yesno commentvideo(string videoid, string userid, string content)///评论视频
     {
         yesno wv = new yesno();
         string connstr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["SqlConnStr"].ConnectionString;////数据库连接字符串

         #region 检查存在性
         SqlConnection con11 = new SqlConnection(connstr);
         SqlCommand cmd11 = new SqlCommand();
         cmd11.Connection = con11;
         con11.Open();
         cmd11.CommandText = "SELECT *FROM PUZZLE.dbo.T_BandVideo tbv WHERE tbv.VideoID=" + videoid;  //检查评论的视频是否存在
         SqlDataReader dr11 = cmd11.ExecuteReader();
         dr11.Read();
         try
         {
             if (dr11.IsDBNull(0))
             {
                 wv.issuccess = -1;
                 con11.Close();
                 return wv;
             }
         }
         catch
         {
             wv.issuccess = -1;
             con11.Close();
             return wv;
         }
         dr11.Close();

         cmd11.CommandText = "SELECT  tu.UserID FROM PUZZLE.dbo.T_Users tu WHERE tu.UserID=" + userid; //检查用户是否存在
         dr11 = cmd11.ExecuteReader();
         dr11.Read();
         try
         {
             if (dr11.IsDBNull(0))
             {
                 wv.issuccess = -1;
                 con11.Close();
                 return wv;
             }
         }
         catch
         {
             wv.issuccess = -1;
             con11.Close();
             return wv;
         }

         con11.Close();
         #endregion

         SqlConnection con = new SqlConnection(connstr);
         SqlCommand cmd = new SqlCommand();

         cmd.Connection = con;

         try
         {
             con.Open();
         }
         catch
         {
             wv.issuccess = -1;
             return wv;
         }
         /*存储过程
     USE [PUZZLE]
       GO
       SET ANSI_NULLS ON
       GO
       SET QUOTED_IDENTIFIER ON
       GO
       ALTER PROCEDURE [dbo].[commentvideo]
       @userid INT,
       @videoid INT,
       @content VARCHAR(MAX)
       AS
       BEGIN
       	DECLARE @commentvideoid INT
       	DECLARE @zuozheid INT
       	INSERT INTO PUZZLE.dbo.T_VideoComment(Details, VideoID, UserID) VALUES(@content,@videoid,@userid) SET @commentvideoid=@@IDENTITY
       	SELECT @zuozheid=tbv.UserID FROM PUZZLE.dbo.T_BandVideo tbv WHERE tbv.VideoID=@videoid
       	INSERT INTO PUZZLE.dbo.T_Message(ToUserID, TypeFlag, TypeID, ActionFlag,ActionID, IsRead)VALUES(@zuozheid,1,@videoid,0,@commentvideoid,0)
       	UPDATE PUZZLE.dbo.T_BandVideo SET CommentNumber = CommentNumber+1 WHERE VideoID=@videoid
       END

           
          */
         cmd.CommandText = @"EXEC PUZZLE.dbo.commentvideo " + userid + "," + videoid + "," + content;
         cmd.ExecuteReader();
         wv.issuccess = 1;
         con.Close();
         return wv;
     }

     public yesno zanvideo(string videoid, string userid)//赞视频
     {
         yesno wv = new yesno();
         string connstr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["SqlConnStr"].ConnectionString;////数据库连接字符串

         SqlConnection con11 = new SqlConnection(connstr);
         SqlCommand cmd11 = new SqlCommand();
         cmd11.Connection = con11;
         con11.Open();
         cmd11.CommandText = "SELECT *FROM PUZZLE.dbo.T_BandVideo tbv WHERE tbv.VideoID=" + videoid;  //检查赞的视频是否存在
         SqlDataReader dr11 = cmd11.ExecuteReader();
         dr11.Read();
         try
         {
             if (dr11.IsDBNull(0))
             {
                 wv.issuccess = -1;
                 con11.Close();
                 return wv;
             }
         }
         catch
         {
             wv.issuccess = -1;
             con11.Close();
             return wv;
         }
         dr11.Close();
         cmd11.CommandText = "SELECT  tu.UserID FROM PUZZLE.dbo.T_Users tu WHERE tu.UserID=" + userid; //检查用户是否存在
         dr11 = cmd11.ExecuteReader();
         dr11.Read();
         try
         {
             if (dr11.IsDBNull(0))
             {
                 wv.issuccess = -1;
                 con11.Close();
                 return wv;
             }
         }
         catch
         {
             wv.issuccess = -1;
             con11.Close();
             return wv;
         }
         con11.Close();


         SqlConnection con = new SqlConnection(connstr);
         SqlCommand cmd = new SqlCommand();

         cmd.Connection = con;

         try
         {
             con.Open();
         }
         catch
         {
             wv.issuccess = -1;
             return wv;
         }
         /*存储过程
      CREATE PROCEDURE zanvideo
        @videoid INT,
        @userid INT
        AS
        BEGIN
        	DECLARE @newzanid INT
        	DECLARE @zuozheid INT 
        	INSERT INTO PUZZLE.dbo.T_Zan(TypeFlag, TypeID, UserID)VALUES(3,@videoid,@userid) SET @newzanid=@@IDENTITY
        	SELECT @zuozheid=tbv.UserID FROM PUZZLE.dbo.T_BandVideo tbv WHERE tbv.VideoID=@videoid
             INSERT INTO PUZZLE.dbo.T_Message(ToUserID, TypeFlag, TypeID, ActionFlag,ActionID, IsRead)VALUES(@zuozheid,1,@videoid,1,@newzanid,0)
             UPDATE PUZZLE.dbo.T_BandVideo SET ZanNumber  = ZanNumber+1 WHERE VideoID=@videoid
        END
                   
                  */
         cmd.CommandText = @"EXEC PUZZLE.dbo.zanvideo " + videoid + "," + userid;
         cmd.ExecuteReader();
         wv.issuccess = 1;
         con.Close();
         return wv;
     }

     public IList<topvideo> seetopweek()///看视频周排行
     {
         IList<topvideo> weeklist = new List<topvideo>();
          string connstr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["SqlConnStr"].ConnectionString;////数据库连接字符串
            SqlConnection con = new SqlConnection(connstr);
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = con;

            try
            {
                con.Open();
            }
            catch
            {
                weeklist = null;
                return weeklist;
            }
            cmd.CommandText = @"SELECT TOP 20 tu.Name,tu.ICON,tbv.Video, tbv.UploadTime,tbv.WeekTimes, tbv.ZanNumber, tbv.Title, tbv.Thumbnail
                  FROM PUZZLE.dbo.T_Users tu JOIN PUZZLE.dbo.T_BandVideo tbv ON tu.UserID=tbv.UserID ORDER BY tbv.WeekTimes desc";
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                topvideo tvd = new topvideo();
                if (!dr.IsDBNull(0))
                    tvd.nicheng = dr.GetString(0);
                if (!dr.IsDBNull(1))
                    tvd.icon = dr.GetString(1);
                if (!dr.IsDBNull(2))
                    tvd.videoname = dr.GetString(2);
                if (!dr.IsDBNull(3))
                    tvd.update = dr.GetDateTime(3);
                if (!dr.IsDBNull(4))
                    tvd.topweek = dr.GetInt32(4);
                if (!dr.IsDBNull(5))
                    tvd.zannum = dr.GetInt32(5);
                if (!dr.IsDBNull(6))
                    tvd.videotitle = dr.GetString(6);
                if (!dr.IsDBNull(7))
                    tvd.videopic = dr.GetString(7);
                weeklist.Add(tvd);
            }
            con.Close();
            return weeklist;
     }

     public IList<topvideo> seetopmonth()////看视频月排行
     {
         IList<topvideo> weeklist = new List<topvideo>();
         string connstr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["SqlConnStr"].ConnectionString;////数据库连接字符串
         SqlConnection con = new SqlConnection(connstr);
         SqlCommand cmd = new SqlCommand();

         cmd.Connection = con;

         try
         {
             con.Open();
         }
         catch
         {
             weeklist = null;
             return weeklist;
         }
         cmd.CommandText = @"SELECT TOP 20 tu.Name,tu.ICON,tbv.Video, tbv.UploadTime, tbv.MonthTimes, tbv.ZanNumber, tbv.Title, tbv.Thumbnail
                  FROM PUZZLE.dbo.T_Users tu JOIN PUZZLE.dbo.T_BandVideo tbv ON tu.UserID=tbv.UserID ORDER BY  tbv.MonthTimes desc";
         SqlDataReader dr = cmd.ExecuteReader();

         while (dr.Read())
         {
             topvideo tvd = new topvideo();
             if(!dr.IsDBNull(0))
             tvd.nicheng = dr.GetString(0);
             if (!dr.IsDBNull(1))
             tvd.icon = dr.GetString(1);
             if (!dr.IsDBNull(2))
             tvd.videoname = dr.GetString(2);
             if (!dr.IsDBNull(3))
             tvd.update = dr.GetDateTime(3);
             if (!dr.IsDBNull(4))
             tvd.topmonth = dr.GetInt32(4);
             if (!dr.IsDBNull(5))
             tvd.zannum = dr.GetInt32(5);
             if (!dr.IsDBNull(6))
             tvd.videotitle = dr.GetString(6);
             if (!dr.IsDBNull(7))
             tvd.videopic = dr.GetString(7);
             weeklist.Add(tvd);
         }
         con.Close();
         return weeklist;
     }

     public IList<topvideo> seetoptotal()///看视频总排行
     {
         IList<topvideo> weeklist = new List<topvideo>();
         string connstr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["SqlConnStr"].ConnectionString;////数据库连接字符串
         SqlConnection con = new SqlConnection(connstr);
         SqlCommand cmd = new SqlCommand();

         cmd.Connection = con;

         try
         {
             con.Open();
         }
         catch
         {
             weeklist = null;
             return weeklist;
         }
         cmd.CommandText = @"SELECT TOP 20 tu.Name,tu.ICON,tbv.Video, tbv.UploadTime, tbv.TotalTimes, tbv.ZanNumber, tbv.Title, tbv.Thumbnail
                  FROM PUZZLE.dbo.T_Users tu JOIN PUZZLE.dbo.T_BandVideo tbv ON tu.UserID=tbv.UserID ORDER BY  tbv.TotalTimes desc";
         SqlDataReader dr = cmd.ExecuteReader();

         while (dr.Read())
         {
             topvideo tvd = new topvideo();
             if (!dr.IsDBNull(0))
                 tvd.nicheng = dr.GetString(0);
             if (!dr.IsDBNull(1))
                 tvd.icon = dr.GetString(1);
             if (!dr.IsDBNull(2))
                 tvd.videoname = dr.GetString(2);
             if (!dr.IsDBNull(3))
                 tvd.update = dr.GetDateTime(3);
             if (!dr.IsDBNull(4))
                 tvd.toptotil = dr.GetInt32(4);
             if (!dr.IsDBNull(5))
                 tvd.zannum = dr.GetInt32(5);
             if (!dr.IsDBNull(6))
                 tvd.videotitle = dr.GetString(6);
             if (!dr.IsDBNull(7))
                 tvd.videopic = dr.GetString(7);
             weeklist.Add(tvd);
         }
         con.Close();
         return weeklist;
     }

     public IList<comment> seewbcomment(string weiboid)///看微博评论
     {
         IList<comment> comlist = new List<comment>();
         string connstr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["SqlConnStr"].ConnectionString;////数据库连接字符串


         SqlConnection con11 = new SqlConnection(connstr);
         SqlCommand cmd11 = new SqlCommand();
         cmd11.Connection = con11;
         con11.Open();
         cmd11.CommandText = "SELECT *FROM PUZZLE.dbo.T_WeiboComment twbc WHERE twbc.CommentID=" + weiboid;  //检查看的微博评论是否存在
         SqlDataReader dr11 = cmd11.ExecuteReader();
         dr11.Read();
         try
         {
             if (dr11.IsDBNull(0))
             {
                 comlist = null;
                 con11.Close();
                 return comlist;
             }
         }
         catch
         {
             comlist = null;
             con11.Close();
             return comlist;
         }
         con11.Close();



         SqlConnection con = new SqlConnection(connstr);
         SqlCommand cmd = new SqlCommand();

         cmd.Connection = con;

         try
         {
             con.Open();
         }
         catch
         {
             comlist = null;
             return comlist;
         }
         cmd.CommandText = @"SELECT tu.Name,tu.ICON,twbc.Details,twbc.DTime FROM PUZZLE.dbo.T_Users tu JOIN PUZZLE.dbo.T_WeiboComment twbc ON tu.UserID=twbc.UserID WHERE twbc.WeiBoID=" + weiboid;
         SqlDataReader dr = cmd.ExecuteReader();

         while (dr.Read())
         {
             comment com = new comment();
             com.nicheng = dr.GetString(0);
             com.icon = dr.GetString(1);
             com.detail = dr.GetString(2);
             com.dtime = dr.GetDateTime(3);
             comlist.Add(com);
         }
         con.Close();
         return comlist;
     }

     public IList<comment> seemucomment(string musicid)//看音乐评论
     {

         IList<comment> comlist = new List<comment>();
         string connstr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["SqlConnStr"].ConnectionString;////数据库连接字符串

         SqlConnection con11 = new SqlConnection(connstr);
         SqlCommand cmd11 = new SqlCommand();
         cmd11.Connection = con11;
         con11.Open();
         cmd11.CommandText = "SELECT *FROM PUZZLE.dbo.T_MusicComment twbc WHERE twbc.CommentID=" + musicid;  //检查看的音乐评论是否存在
         SqlDataReader dr11 = cmd11.ExecuteReader();
         dr11.Read();
         try
         {
             if (dr11.IsDBNull(0))
             {
                 comlist = null;
                 con11.Close();
                 return comlist;
             }
         }
         catch
         {
             comlist = null;
             con11.Close();
             return comlist;
         }
         con11.Close();

         SqlConnection con = new SqlConnection(connstr);
         SqlCommand cmd = new SqlCommand();

         cmd.Connection = con;

         try
         {
             con.Open();
         }
         catch
         {
             comlist = null;
             return comlist;
         }
         cmd.CommandText = @"SELECT tu.Name,tu.ICON,twbc.Details,twbc.DTime FROM PUZZLE.dbo.T_Users tu JOIN PUZZLE.dbo.T_MusicComment twbc ON tu.UserID=twbc.UserID WHERE twbc.MusicID=" + musicid;
         SqlDataReader dr = cmd.ExecuteReader();

         while (dr.Read())
         {
             comment com = new comment();
             com.nicheng = dr.GetString(0);
             com.icon = dr.GetString(1);
             com.detail = dr.GetString(2);
             com.dtime = dr.GetDateTime(3);
             comlist.Add(com);
         }
         con.Close();
         return comlist;
     }

     public IList<comment> seeartcomment(string artid)//看文章评论
     {
         IList<comment> comlist = new List<comment>();
         string connstr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["SqlConnStr"].ConnectionString;////数据库连接字符串

         SqlConnection con11 = new SqlConnection(connstr);
         SqlCommand cmd11 = new SqlCommand();
         cmd11.Connection = con11;
         con11.Open();
         cmd11.CommandText = "SELECT *FROM PUZZLE.dbo.T_ArticleComment twbc WHERE twbc.CommentID=" + artid;  //检查看的文章评论是否存在
         SqlDataReader dr11 = cmd11.ExecuteReader();
         dr11.Read();
         try
         {
             if (dr11.IsDBNull(0))
             {
                 comlist = null;
                 con11.Close();
                 return comlist;
             }
         }
         catch
         {
             comlist = null;
             con11.Close();
             return comlist;
         }
         con11.Close();


         SqlConnection con = new SqlConnection(connstr);
         SqlCommand cmd = new SqlCommand();

         cmd.Connection = con;

         try
         {
             con.Open();
         }
         catch
         {
             comlist = null;
             return comlist;
         }
         cmd.CommandText = @"SELECT tu.Name,tu.ICON,twbc.Details,twbc.DTime FROM PUZZLE.dbo.T_Users tu JOIN PUZZLE.dbo.T_ArticleComment twbc ON tu.UserID=twbc.UserID WHERE twbc.ArticleID=" + artid;
         SqlDataReader dr = cmd.ExecuteReader();

         while (dr.Read())
         {
             comment com = new comment();
             com.nicheng = dr.GetString(0);
             com.icon = dr.GetString(1);
             com.detail = dr.GetString(2);
             com.dtime = dr.GetDateTime(3);
             comlist.Add(com);
         }
         con.Close();
         return comlist;
     }

     public IList<comment> seevdcomment(string videoid)///看视频评论
     {
         IList<comment> comlist = new List<comment>();
         string connstr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["SqlConnStr"].ConnectionString;////数据库连接字符串

         SqlConnection con11 = new SqlConnection(connstr);
         SqlCommand cmd11 = new SqlCommand();
         cmd11.Connection = con11;
         con11.Open();
         cmd11.CommandText = "SELECT *FROM PUZZLE.dbo.T_VideoComment twbc WHERE twbc.CommentID=" + videoid;  //检查看的文章评论是否存在
         SqlDataReader dr11 = cmd11.ExecuteReader();
         dr11.Read();
         try
         {
             if (dr11.IsDBNull(0))
             {
                 comlist = null;
                 con11.Close();
                 return comlist;
             }
         }
         catch
         {
             comlist = null;
             con11.Close();
             return comlist;
         }
         con11.Close();


         SqlConnection con = new SqlConnection(connstr);
         SqlCommand cmd = new SqlCommand();

         cmd.Connection = con;

         try
         {
             con.Open();
         }
         catch
         {
             comlist = null;
             return comlist;
         }
         cmd.CommandText = @"SELECT tu.Name,tu.ICON,twbc.Details,twbc.DTime FROM PUZZLE.dbo.T_Users tu JOIN PUZZLE.dbo.T_VideoComment twbc ON tu.UserID=twbc.UserID WHERE twbc.VideoID=" + videoid;
         SqlDataReader dr = cmd.ExecuteReader();

         while (dr.Read())
         {
             comment com = new comment();
             com.nicheng = dr.GetString(0);
             com.icon = dr.GetString(1);
             com.detail = dr.GetString(2);
             com.dtime = dr.GetDateTime(3);
             comlist.Add(com);
         }
         con.Close();
         return comlist;
     }

     public lookforuserclass lookfor(string loginname)///查找用户
     {
         lookforuserclass lfu=new lookforuserclass();
          string connstr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["SqlConnStr"].ConnectionString;////数据库连接字符串
         SqlConnection con = new SqlConnection(connstr);
         SqlCommand cmd = new SqlCommand();

         cmd.Connection = con;

         try
         {
             con.Open();
         }
         catch
         {
             lfu = null;
             return lfu;
         }
         cmd.CommandText = @"SELECT tu.UserID,tu.Name, tu.ICON, tu.VipFlag
  FROM PUZZLE.dbo.T_Users tu WHERE LoginName='" + loginname+"'";
         SqlDataReader dr = cmd.ExecuteReader();

         while (dr.Read())
         {
             if(!dr.IsDBNull(0))
             lfu.userid = dr.GetInt32(0);
             if (!dr.IsDBNull(1))
                 lfu.nicheng = dr.GetString(1);
             if (!dr.IsDBNull(2))
                 lfu.icon = dr.GetString(2);
             if (!dr.IsDBNull(3))
                 lfu.vipflag = dr.GetInt32(3);

         }
         return lfu;
     }

     public yesno cancelfans(string userid1, string userid2)///取消关注
     {
         yesno issu = new yesno();
         string connstr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["SqlConnStr"].ConnectionString;////数据库连接字符串

         #region 检查存在性
         SqlConnection con11 = new SqlConnection(connstr);
         SqlCommand cmd11 = new SqlCommand();

         con11.Open();

         //检查用户是否存在
         cmd11.Connection = con11;
         cmd11.CommandText = "SELECT *FROM PUZZLE.dbo.T_Users tu WHERE tu.UserID=" + userid1;
         SqlDataReader dr11 = cmd11.ExecuteReader();
         dr11.Read();
         try
         {
             if (dr11.IsDBNull(0))
             {
                 issu.issuccess = -1;
                 con11.Close();
                 return issu;
             }
         }
         catch (Exception e)
         {
             string tempe = e.ToString();
             issu.issuccess = -1;
             con11.Close();
             return issu;
         }
         con11.Close();
         #endregion


         SqlConnection con = new SqlConnection(connstr);
         SqlCommand cmd = new SqlCommand();

         cmd.Connection = con;

         try
         {
             con.Open();
         }
         catch
         {
             issu.issuccess = -1;
             con.Close();
             return issu;
         }
         /*存储过程
          CREATE PROCEDURE cancelfans
               @user1 INT,
               @user2 INT
               AS
               BEGIN
               	DECLARE @flag INT
               	DECLARE @relationid INT
               	DECLARE @tempuser1 INT 
               	DECLARE @tempuser2 INT 
               	SELECT @tempuser1=tur.User1,@tempuser2=tur.User2, @relationid=tur.RelationID, @flag=tur.RelationFlag FROM PUZZLE.dbo.T_UserRelation tur WHERE (tur.User1=@user1 AND tur.user2=@user2) OR (tur.User1=@user2 AND tur.user2=@user1)
               	IF @flag=0
               	BEGIN
               		DELETE PUZZLE.dbo.T_UserRelation  WHERE RelationID=@relationid
               	END
               	IF @flag=1
               	BEGIN
               		IF @user1=@tempuser1
               		BEGIN
               			UPDATE PUZZLE.dbo.T_UserRelation SET RelationFlag = 2 WHERE RelationID=@relationid
               		END
               		ELSE
               		BEGIN
               			UPDATE PUZZLE.dbo.T_UserRelation SET RelationFlag = 0 WHERE RelationID=@relationid
               		END
               	END
               END
               GO
          */
         cmd.CommandText = @"EXEC puzzle.dbo.cancelfans "+userid1+","+userid2;
         SqlDataReader dr = cmd.ExecuteReader();
         issu.issuccess = 1;
         con.Close();
         return issu;
     }


     public yesno problem(string title, string details)///反馈问题
     {
         yesno issu = new yesno();
         string connstr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["SqlConnStr"].ConnectionString;////数据库连接字符串
         SqlConnection con = new SqlConnection(connstr);
         SqlCommand cmd = new SqlCommand();

         cmd.Connection = con;

         try
         {
             con.Open();
         }
         catch
         {
             issu.issuccess = -1;
             con.Close();
             return issu;
         }
         cmd.CommandText = @"INSERT INTO puzzle.dbo.T_Problem(Title, Details,IsRead)VALUES(N'"+title+"',N'"+details+"',0)";
         SqlDataReader dr = cmd.ExecuteReader();
         issu.issuccess = 1;
         con.Close();
         return issu;
     }
     public yesno inform(string weiboid)//举报微博
     {
         yesno issu = new yesno();
         string connstr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["SqlConnStr"].ConnectionString;////数据库连接字符串

         SqlConnection con11 = new SqlConnection(connstr);
         SqlCommand cmd11 = new SqlCommand();
         cmd11.Connection = con11;
         con11.Open();
         cmd11.CommandText = "SELECT *FROM PUZZLE.dbo.T_WeiBo tbv WHERE tbv.WeiBoID=" + weiboid;  //检查被举报的微博是否存在
         SqlDataReader dr11 = cmd11.ExecuteReader();
         dr11.Read();
         try
         {
             if (dr11.IsDBNull(0))
             {
                 issu.issuccess = -1;
                 con11.Close();
                 return issu;
             }
         }
         catch
         {
             issu.issuccess = -1;
             con11.Close();
             return issu;
         }
         con11.Close();

         SqlConnection con = new SqlConnection(connstr);
         SqlCommand cmd = new SqlCommand();

         cmd.Connection = con;

         try
         {
             con.Open();
         }
         catch
         {
             issu.issuccess = -1;
             con.Close();
             return issu;
         }
         /*
         CREATE PROCEDURE inform
         @weiboid INT
         AS
         BEGIN
         	DECLARE @TranspondFlag INT 
         	DECLARE @zhuanid INT
         	SELECT @TranspondFlag=twb.TranspondFlag, @zhuanid= twb.TypeID FROM PUZZLE.dbo.T_WeiBo twb WHERE twb.WeiBoID=@weiboid
         	IF @TranspondFlag=1
         	BEGIN
         		INSERT INTO PUZZLE.dbo.T_Inform(WeiBoID, IsDispose) VALUES(@zhuanid,0)
         	END
         	ELSE
         	BEGIN
         		INSERT INTO PUZZLE.dbo.T_Inform(WeiBoID, IsDispose) VALUES(@weiboid,0)
         	END
         END
                   */
         cmd.CommandText = @"EXEC PUZZLE.dbo.inform "+weiboid;
         SqlDataReader dr = cmd.ExecuteReader();
         issu.issuccess = 1;
         con.Close();
         return issu;
     }                        
    }
}






