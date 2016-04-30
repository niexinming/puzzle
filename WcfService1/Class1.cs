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
    [DataContract]
    public class classregister
    {
        [DataMember]
        public int userid { get; set; }//注册类：用户id
    }

    [DataContract]
    public class classlog
    {
        [DataMember]
        public int myuserid { get; set; }//用户id
        [DataMember]
        public string usertouxiang { get; set; }//用户头像
        [DataMember]
        public string userback { get; set; }//用户背景
        [DataMember]
        public int vipflag { set; get; }//vip标识
        [DataMember]
        public int shangchangshu { set; get; }//上传数

    }

    [DataContract]
    public class yesno
    {
        [DataMember]
        public int issuccess { set; get; }//判断是否成功上传
    }

    [DataContract]
    public class muandardata
    {
        [DataMember]
        public int arid { set; get; }
        [DataMember]
        public string arttitle { set; get; }
        [DataMember]
        public string artdatailes { get; set; }
        [DataMember]
        public int artcommentnu { get; set; }
        [DataMember]
        public int arttranspondnum { set; get; }
        [DataMember]
        public int artzannum { set; get; }
        [DataMember]
        public DateTime artupdtime { set; get; }

        [DataMember]
        public int muid { set; get; }
        [DataMember]
        public string muname { set; get; }
        [DataMember]
        public string mupath { set; get; }
        [DataMember]
        public string mupic { get; set; }
        [DataMember]
        public int mucommentnu { get; set; }
        [DataMember]
        public int mutranspondnum { set; get; }
        [DataMember]
        public int muzannum { set; get; }
        [DataMember]
        public DateTime muupdtime { set; get; }
        [DataMember]
        public string muintro { get; set; }

    }

    [DataContract]
    public class classuserlist
    {
        [DataMember]
        public int userid{get;set;}
        [DataMember]
        public string nicheng{get;set;}
        [DataMember]
        public string icon{set;get;}
        [DataMember]
        public int vipflag{ set; get; }
     }

    [DataContract]
    public class classreturnid
    {
        [DataMember]
        public int myid { get; set; }
    }

    #region 动态数据契约
    [DataContract]
    public class classdynamic
    {
        [DataMember]
       public int userid{get;set;}
        [DataMember]
        public string nicheng { get; set; }
        [DataMember]
        public string icon { get; set; }
        [DataMember]
        public int vipflag { get; set; }
        [DataMember]
        public int weiboid { get; set; }
        [DataMember]
        public string datails { get; set; }
        [DataMember]
        public string img1 { get; set; }
        [DataMember]
        public string img2 { get; set; }
        [DataMember]
        public string img3 { get; set; }
        [DataMember]
        public string img4 { get; set; }
        [DataMember]
        public string img5 { get; set; }
        [DataMember]
        public string img6 { get; set; }
        [DataMember]
        public string img7 { get; set; }
        [DataMember]
        public string img8 { get; set; }
        [DataMember]
        public string img9 { get; set; }
        [DataMember]
        public DateTime Dtime { get; set; }
        [DataMember]
        public int commentnum { get; set; }
        [DataMember]
        public int transpondnum { set; get; }
        [DataMember]
        public int zannum { get; set; }
        [DataMember]
        public int zhuanfaflag { get; set; }


        [DataMember]
        public zhuanarticle zhuanart { get; set; }

        [DataMember]
        public zhuanmusic zhuanmu { get; set; }
        [DataMember]
        public zhuanweibo zhuanwb { get; set; }
       
       
    }

    [DataContract]
    public class zhuanarticle
    {
        [DataMember]
        public string arttitle { set; get; }
        [DataMember]
        public string artdatailes { get; set; }
        [DataMember]
        public DateTime artupdtime { set; get; }
        [DataMember]
        public int artTranspondNum { set; get; }
        [DataMember]
        public int artzannum { get; set; }
        

    }
    [DataContract]
    public class zhuanmusic
    {
        [DataMember]
        public string muname { set; get; }
        [DataMember]
        public string mupic { get; set; }
        [DataMember]
        public DateTime muupdtime { set; get; }
        [DataMember]
        public string muintro { get; set; }
        [DataMember]
        public int muTranspondNum { set; get; }
        [DataMember]
        public int muzannum { get; set; }
    }


    [DataContract]
    public class zhuanweibo
    {
        [DataMember]
        public int wbweiboid { get; set; }
        [DataMember]
        public string wbdatails { get; set; }
        [DataMember]
        public string wbimg1 { get; set; }
        [DataMember]
        public string wbimg2 { get; set; }
        [DataMember]
        public string wbimg3 { get; set; }
        [DataMember]
        public string wbimg4 { get; set; }
        [DataMember]
        public string wbimg5 { get; set; }
        [DataMember]
        public string wbimg6 { get; set; }
        [DataMember]
        public string wbimg7 { get; set; }
        [DataMember]
        public string wbimg8 { get; set; }
        [DataMember]
        public string wbimg9 { get; set; }
        [DataMember]
        public DateTime wbweibotime { get; set; }
        [DataMember]
        public int wbTranspondNum { get; set; }
        [DataMember]
        public int wbznanum { get; set; }
    }
    #endregion

    #region 消息表数据契约
    [DataContract]
    public class classmessage
    {
        
        [DataMember]
        public int typeflag { get; set; }
        [DataMember]
        public int vidoweiflag { get; set; }
        [DataMember]
        public int actionflag { get; set;}
        [DataMember]
        public int actionid { get; set; }
        [DataMember]
        public weibomessage wbme { get; set; }
        [DataMember]
        public weiboping wbp { get; set; }
      
        [DataMember]
        public weibozan wbz { get; set; }
        [DataMember]
        public weibozhuan wbzhuan { get; set; }

          [DataMember]
        public videomessage vime { get; set; }
        [DataMember]
        public videoping viping { get; set; }
        [DataMember]
        public videozan vdz { get; set; }
        [DataMember]
        public videozhuan vdzhuan { get; set; }

    }

    [DataContract]
    public class weibomessage
    {
        [DataMember]
        public int wbweiboid { get; set; }
        [DataMember]
        public string wbdetail { get; set; }
        [DataMember]
        public string img1 { get; set; }
        [DataMember]
        public string img2 { get; set; }
        [DataMember]
        public string img3 { get; set; }
        [DataMember]
        public string img4 { get; set; }
        [DataMember]
        public string img5 { get; set; }
        [DataMember]
        public string img6 { get; set; }
        [DataMember]
        public string img7 { get; set; }
        [DataMember]
        public string img8 { get; set; }
        [DataMember]
        public string img9 { get; set; }
    }

    [DataContract]
    public class weiboping
    {
        [DataMember]
        public string nicehng { get; set; }
        [DataMember]
        public string usericno { get; set; }
        [DataMember]
        public string wbpingdetails { get; set; }
        [DataMember]
        public DateTime wbpingupdate { set; get; }
    }

    [DataContract]
    public class weibozan
    {
         [DataMember]
        public string nicehng { get; set; }
        [DataMember]
        public string usericno { get; set; }
       
    }

    [DataContract]
    public class weibozhuan
    {
        [DataMember]
        public string nicehng { get; set; }
        [DataMember]
        public string usericno { get; set; }

    }

    [DataContract]
    public class videomessage
    {
        [DataMember]
        public int viid { get; set; }
        [DataMember]
        public string viname { get; set; }
        [DataMember]
        public DateTime viupdata { get; set; }
    }

    [DataContract]
    public class videozan
    {
        [DataMember]
        public string nicehng { get; set; }
        [DataMember]
        public string usericno { get; set; }
    }

    public class videozhuan
    {
        [DataMember]
        public string nicehng { get; set; }
        [DataMember]
        public string usericno { get; set; }
    }

    [DataContract]
    public class videoping
    {
        [DataMember]
        public string nicehng { get; set; }
        [DataMember]
        public string usericno { get; set; }
        [DataMember]
        public string videtails { get; set; }
        [DataMember]
        public DateTime videdate { get; set; }
    }
#endregion
    
     #region 赞表数据契约
    [DataContract]
    public class classzan
    {
       
        [DataMember]
        public int typeflag { get; set; }
        [DataMember]
        public DateTime dtime { get; set; }
        [DataMember]
        public zanweibo zwb { get; set; }
        [DataMember]
        public zanwenzhang zwz{get;set;}
        [DataMember]
        public zanmusic zmu { get; set; }
        [DataMember]
        public zanvideo zvd { get; set; }
    }
    [DataContract]
    public class zanweibo
    {
        [DataMember]
        public int wbweiboid { get; set; }
        [DataMember]
        public string wbdetail { get; set; }
        [DataMember]
        public DateTime wbuptime { get; set; }
        [DataMember]
        public string img1 { get; set; }
        [DataMember]
        public string img2 { get; set; }
        [DataMember]
        public string img3 { get; set; }
        [DataMember]
        public string img4 { get; set; }
        [DataMember]
        public string img5 { get; set; }
        [DataMember]
        public string img6 { get; set; }
        [DataMember]
        public string img7 { get; set; }
        [DataMember]
        public string img8 { get; set; }
        [DataMember]
        public string img9 { get; set; }
    }
    [DataContract]
    public class zanwenzhang
    {
        [DataMember]
        public int wenzhangid { get; set; }
        [DataMember]
        public string artitle { get; set; }
        [DataMember]
        public string ardetail { get; set; }
        [DataMember]
        public DateTime aruptime { get; set; }
    }
    [DataContract]
    public class zanmusic
    {
        [DataMember]
        public int muid { get; set; }
        [DataMember]
        public DateTime muuptime { get; set; }
        [DataMember]
        public string muname { get; set; }
        [DataMember]
        public string mupic{get;set;}
        [DataMember]
        public string muintroduce{get;set;}
    }
    [DataContract]
    public class zanvideo
    {
        [DataMember]
        public int videoid { get; set; }
        [DataMember]
        public string videoTitle { get; set; }
        [DataMember]
        public string videoname { get; set; }
        [DataMember]
        public DateTime videouptime { get; set; }
    

    }
    #endregion

    [DataContract]
    public class banduser
    {
        [DataMember]
        public int bandid { get; set; }
        [DataMember]
        public string nicheng { get; set; }
        [DataMember]
        public string icon { get; set; }
        [DataMember]
        public int vipflag { get; set; }
        [DataMember]
        public int uploadnu { get; set; }
        [DataMember]
        public string sign { get; set; }
    }


    [DataContract]
    public class videolist
    {
        [DataMember]
        public  int videoid{get;set;}
        [DataMember]
        public  string videoname{get;set;}
        [DataMember]
        public DateTime videuptime { get; set; }
        [DataMember]
        public int zannumm { get; set; }
        [DataMember]
        public int commentnum { get; set; }
        [DataMember]
        public string videoTitle { get; set; }
        [DataMember]
        public string videoThumbnail { get; set; }
    }

    [DataContract]
    public class topvideo
    {
        [DataMember]
        public string nicheng { get; set; }
        [DataMember]
        public string icon { get; set; }
        [DataMember]
        public string videoname { get; set; }
        [DataMember]
        public string videotitle { get; set; }
        [DataMember]
        public string videopic { get; set; }
        [DataMember]
        public DateTime update { get; set; }
        [DataMember]
        public int zannum { get; set; }
        [DataMember]
        public int topweek { get; set; }
        [DataMember]
        public int topmonth { get; set; }
        [DataMember]
        public int toptotil { get; set; }
    }

    [DataContract]
    public class comment
    {
        [DataMember]
        public string nicheng { get; set; }
        [DataMember]
        public string icon { get; set; }
        [DataMember]
        public string detail { get; set; }
        [DataMember]
        public DateTime dtime { get; set; }
    }

    [DataContract]
    public class lookforuserclass
    {
        [DataMember]
        public int userid { get; set; }//用户id
        [DataMember]
        public string nicheng { get; set; }//用户昵称
        [DataMember]
        public string icon { get; set; }
        [DataMember]
        public int vipflag { get; set; }
    }
}

