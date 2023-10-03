//namespace NapAdmin.Controller;

//using Fanex.NAP.Client.Attributes;
//using Fanex.NAP.Client.Helpers.Web;
//using Microsoft.AspNetCore.Mvc;

//[NapSuperAuth]
//public class SharedMenuComponent : ViewComponent
//{
//    private readonly INapSession napSession;

//    public SharedMenuComponent(INapSession napSession)
//    {
//        this.napSession = napSession;
//    }

//    public IViewComponentResult Invoke()
//    {
//        ViewBag.MenuItemList = napSession.MenuItemList;

//        return View("~/Views/Shared/_Menu.cshtml");
//    }
//}
