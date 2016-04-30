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

namespace WcfService1
{

    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“IService1”。
    [ServiceContract]
    public interface IService1
    {
        [OperationContract, WebInvoke(Method = "GET", UriTemplate = "register/{call}/{nicheng}/{mypassword}",
             BodyStyle = WebMessageBodyStyle.WrappedRequest,
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json)]
        classregister register(string call, string nicheng, string mypassword);



        [OperationContract, WebInvoke(Method = "GET", UriTemplate = "log/{qqcall}/{password}",
             BodyStyle = WebMessageBodyStyle.WrappedRequest,
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json)]
        classlog log(string qqcall, string password);



        [OperationContract, WebInvoke(Method = "POST", UriTemplate = "uploadicon",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
       ResponseFormat = WebMessageFormat.Json,
       RequestFormat = WebMessageFormat.Json)]
        yesno uploadicon(Stream iconimg);



        [OperationContract, WebInvoke(Method = "POST", UriTemplate = "uploadbackground",
          BodyStyle = WebMessageBodyStyle.WrappedRequest,
     ResponseFormat = WebMessageFormat.Json,
     RequestFormat = WebMessageFormat.Json)]
        yesno uploadbackground(Stream bkg);



        [OperationContract, WebInvoke(Method = "GET", UriTemplate = "seehomepage",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
       ResponseFormat = WebMessageFormat.Xml,
       RequestFormat = WebMessageFormat.Xml)]
        IList<muandardata> seehomepage();


        [OperationContract, WebInvoke(Method = "GET", UriTemplate = "seehomeart/{arid}",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
       ResponseFormat = WebMessageFormat.Xml,
       RequestFormat = WebMessageFormat.Xml)]
        muandardata seehomeart(string arid);


        [OperationContract, WebInvoke(Method = "GET", UriTemplate = "seehomemus/{muid}",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
       ResponseFormat = WebMessageFormat.Xml,
       RequestFormat = WebMessageFormat.Xml)]
        muandardata seehomemus(string muid);


        [OperationContract, WebInvoke(Method = "GET", UriTemplate = "commandar/{userid}/{artid}/{content}",
             BodyStyle = WebMessageBodyStyle.WrappedRequest,
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json)]
        yesno commandar(string userid, string artid, string content);


        [OperationContract, WebInvoke(Method = "GET", UriTemplate = "commandmu/{userid}/{artid}/{content}",
             BodyStyle = WebMessageBodyStyle.WrappedRequest,
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json)]
        yesno commandmu(string userid, string artid, string content);

        [OperationContract, WebInvoke(Method = "GET", UriTemplate = "zanart/{userid}/{artid}",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
       ResponseFormat = WebMessageFormat.Json,
       RequestFormat = WebMessageFormat.Json)]
        yesno zanart(string userid, string artid);

        [OperationContract, WebInvoke(Method = "GET", UriTemplate = "zanmus/{userid}/{artid}",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
       ResponseFormat = WebMessageFormat.Json,
       RequestFormat = WebMessageFormat.Json)]
        yesno zanmus(string userid, string artid);


        [OperationContract, WebInvoke(Method = "GET", UriTemplate = "zhuanfaart/{userid}/{artid}",
           BodyStyle = WebMessageBodyStyle.WrappedRequest,
      ResponseFormat = WebMessageFormat.Json,
      RequestFormat = WebMessageFormat.Json)]
        yesno zhuanfaart(string userid, string artid);

        [OperationContract, WebInvoke(Method = "GET", UriTemplate = "zhuanfamus/{userid}/{musid}",
          BodyStyle = WebMessageBodyStyle.WrappedRequest,
     ResponseFormat = WebMessageFormat.Json,
     RequestFormat = WebMessageFormat.Json)]
        yesno zhuanfamus(string userid, string musid);

        [OperationContract, WebInvoke(Method = "GET", UriTemplate = "userlist",
         BodyStyle = WebMessageBodyStyle.WrappedRequest,
    ResponseFormat = WebMessageFormat.Json,
    RequestFormat = WebMessageFormat.Json)]
       IList<classuserlist> userlist();

        [OperationContract, WebInvoke(Method = "GET", UriTemplate = "fans/{userid}/{fanslist}",
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
   ResponseFormat = WebMessageFormat.Json,
   RequestFormat = WebMessageFormat.Json)]
        yesno fans(string userid, string fanslist);


        [OperationContract, WebInvoke(Method = "GET", UriTemplate = "sendweibo/{userid}/{weibo}",
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
   ResponseFormat = WebMessageFormat.Json,
   RequestFormat = WebMessageFormat.Json)]
        classreturnid sendweibo(string userid, string weibo);

        [OperationContract, WebInvoke(Method = "POST", UriTemplate = "sendweibopoc",
       BodyStyle = WebMessageBodyStyle.WrappedRequest,
  ResponseFormat = WebMessageFormat.Json,
  RequestFormat = WebMessageFormat.Json)]
        yesno sendweibopoc(Stream pweiboic);


        [OperationContract, WebInvoke(Method = "GET", UriTemplate = "weibozhuan/{userid}/{weiboid}",
       BodyStyle = WebMessageBodyStyle.WrappedRequest,
  ResponseFormat = WebMessageFormat.Json,
  RequestFormat = WebMessageFormat.Json)]
        yesno weibozhuan(string userid, string weiboid);

        [OperationContract, WebInvoke(Method = "GET", UriTemplate = "seedynamic/{userid}/{page}",
      BodyStyle = WebMessageBodyStyle.WrappedRequest,
      ResponseFormat = WebMessageFormat.Xml,
      RequestFormat = WebMessageFormat.Xml)]
        IList<classdynamic> seedynamic(string userid,string page);


        [OperationContract, WebInvoke(Method = "GET", UriTemplate = "weibocomment/{userid}/{weiboid}/{content}",
      BodyStyle = WebMessageBodyStyle.WrappedRequest,
 ResponseFormat = WebMessageFormat.Json,
 RequestFormat = WebMessageFormat.Json)]
        yesno weibocomment(string userid, string weiboid,string content);

        [OperationContract, WebInvoke(Method = "GET", UriTemplate = "weibozhan/{userid}/{weiboid}",
      BodyStyle = WebMessageBodyStyle.WrappedRequest,
 ResponseFormat = WebMessageFormat.Json,
 RequestFormat = WebMessageFormat.Json)]
        yesno weibozhan(string userid, string weiboid);


        [OperationContract, WebInvoke(Method = "GET", UriTemplate = "seemessage/{userid}",
      BodyStyle = WebMessageBodyStyle.WrappedRequest,
 ResponseFormat = WebMessageFormat.Xml,
 RequestFormat = WebMessageFormat.Xml)]
        IList<classmessage> seemessage(string userid);

        [OperationContract, WebInvoke(Method = "GET", UriTemplate = "seelike/{userid}",
    BodyStyle = WebMessageBodyStyle.WrappedRequest,
ResponseFormat = WebMessageFormat.Xml,
RequestFormat = WebMessageFormat.Xml)]
        IList<classzan> seelike(string userid);

        [OperationContract, WebInvoke(Method = "GET", UriTemplate = "seeband",
    BodyStyle = WebMessageBodyStyle.WrappedRequest,
ResponseFormat = WebMessageFormat.Json,
RequestFormat = WebMessageFormat.Json)]
        IList<banduser> seeband();


        [OperationContract, WebInvoke(Method = "GET", UriTemplate = "seevideplist/{bandid}",
  BodyStyle = WebMessageBodyStyle.WrappedRequest,
ResponseFormat = WebMessageFormat.Json,
RequestFormat = WebMessageFormat.Json)]
        IList<videolist> seevideplist(string bandid);

        [OperationContract, WebInvoke(Method = "GET", UriTemplate = "watchvideo/{videoid}",
 BodyStyle = WebMessageBodyStyle.WrappedRequest,
ResponseFormat = WebMessageFormat.Json,
RequestFormat = WebMessageFormat.Json)]
        yesno watchvideo(string videoid);

        [OperationContract, WebInvoke(Method = "GET", UriTemplate = "commentvideo/{videoid}/{userid}/{content}",
 BodyStyle = WebMessageBodyStyle.WrappedRequest,
ResponseFormat = WebMessageFormat.Json,
RequestFormat = WebMessageFormat.Json)]
        yesno commentvideo(string videoid, string userid, string content);

        [OperationContract, WebInvoke(Method = "GET", UriTemplate = "zanvideo/{videoid}/{userid}",
BodyStyle = WebMessageBodyStyle.WrappedRequest,
ResponseFormat = WebMessageFormat.Json,
RequestFormat = WebMessageFormat.Json)]
        yesno zanvideo(string videoid, string userid);


        [OperationContract, WebInvoke(Method = "GET", UriTemplate = "seetopweek",
BodyStyle = WebMessageBodyStyle.WrappedRequest,
ResponseFormat = WebMessageFormat.Json,
RequestFormat = WebMessageFormat.Json)]
        IList<topvideo> seetopweek();

        [OperationContract, WebInvoke(Method = "GET", UriTemplate = "seetopmonth",
BodyStyle = WebMessageBodyStyle.WrappedRequest,
ResponseFormat = WebMessageFormat.Json,
RequestFormat = WebMessageFormat.Json)]
        IList<topvideo> seetopmonth();

        [OperationContract, WebInvoke(Method = "GET", UriTemplate = "seetoptotal",
BodyStyle = WebMessageBodyStyle.WrappedRequest,
ResponseFormat = WebMessageFormat.Json,
RequestFormat = WebMessageFormat.Json)]
        IList<topvideo> seetoptotal();


        [OperationContract, WebInvoke(Method = "GET", UriTemplate = "seewbcomment/{weiboid}",
BodyStyle = WebMessageBodyStyle.WrappedRequest,
ResponseFormat = WebMessageFormat.Json,
RequestFormat = WebMessageFormat.Json)]
        IList<comment> seewbcomment(string weiboid);

        [OperationContract, WebInvoke(Method = "GET", UriTemplate = "seemucomment/{musicid}",
BodyStyle = WebMessageBodyStyle.WrappedRequest,
ResponseFormat = WebMessageFormat.Json,
RequestFormat = WebMessageFormat.Json)]
        IList<comment> seemucomment(string musicid);

        [OperationContract, WebInvoke(Method = "GET", UriTemplate = "seeartcomment/{artid}",
BodyStyle = WebMessageBodyStyle.WrappedRequest,
ResponseFormat = WebMessageFormat.Json,
RequestFormat = WebMessageFormat.Json)]
        IList<comment> seeartcomment(string artid);

        [OperationContract, WebInvoke(Method = "GET", UriTemplate = "seevdcomment/{videoid}",
BodyStyle = WebMessageBodyStyle.WrappedRequest,
ResponseFormat = WebMessageFormat.Json,
RequestFormat = WebMessageFormat.Json)]
        IList<comment> seevdcomment(string videoid);


        [OperationContract, WebInvoke(Method = "GET", UriTemplate = "lookfor/{loginname}",
BodyStyle = WebMessageBodyStyle.WrappedRequest,
ResponseFormat = WebMessageFormat.Json,
RequestFormat = WebMessageFormat.Json)]
        lookforuserclass lookfor(string loginname);


        [OperationContract, WebInvoke(Method = "GET", UriTemplate = "cancelfans/{userid1}/{userid2}",
       BodyStyle = WebMessageBodyStyle.WrappedRequest,
  ResponseFormat = WebMessageFormat.Json,
  RequestFormat = WebMessageFormat.Json)]
        yesno cancelfans(string userid1, string userid2);

        [OperationContract, WebInvoke(Method = "GET", UriTemplate = "problem/{title}/{details}",
       BodyStyle = WebMessageBodyStyle.WrappedRequest,
  ResponseFormat = WebMessageFormat.Json,
  RequestFormat = WebMessageFormat.Json)]
        yesno problem(string title, string details);


        [OperationContract, WebInvoke(Method = "GET", UriTemplate = "inform/{weiboid}",
     BodyStyle = WebMessageBodyStyle.WrappedRequest,
ResponseFormat = WebMessageFormat.Json,
RequestFormat = WebMessageFormat.Json)]
        yesno inform(string weiboid);
    }
}
