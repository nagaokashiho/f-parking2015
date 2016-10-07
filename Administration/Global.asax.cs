using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace Administration
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {

            // アプリケーションのスタートアップで実行するコードです
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

        }

        void Session_OnStart(object sender, EventArgs e)
        {
            Session.RemoveAll();
            //Session.Remove("RoomId");
            //Session.Remove("Rooms");
            //Session.Remove("targetDateList");
            //Session.Remove("targetDateRoom");
            //Session.Remove("ReservationLists");
            //Session.Remove("ReservationRooms");
            //Session.Remove("ReservationsForList");
            //Session.Remove("ReservationsForRoom");
        }

        void Session_OnEnd(object sender, EventArgs e)
        {
            Session.RemoveAll();
            //Session.Remove("RoomId");
            //Session.Remove("Rooms");
            //Session.Remove("targetDateList");
            //Session.Remove("targetDateRoom");
            //Session.Remove("ReservationLists");
            //Session.Remove("ReservationRooms");
            //Session.Remove("ReservationsForList");
            //Session.Remove("ReservationsForRoom");
        }
    }
}