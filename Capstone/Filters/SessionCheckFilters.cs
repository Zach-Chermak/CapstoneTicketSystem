using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Capstone.Filters
{
    public class SessionCheckFilters
    {

        //define a class names "SessionCheckFilter" that inherits from 
        //"ActionFilterAttribute"
        public class SessionCheckFilter : ActionFilterAttribute
        {
            //this code defies an action filter (SessionCheckFilter) that 
            //checks whether a user is logged in by verifitying the 
            //"User" session variable

            //Override the "OnActionExecuting" method which executes before 
            //an action method is called

            public override void OnActionExecuting(ActionExecutingContext context)
            {
                if (context.HttpContext.Session.GetString("User") == null)
                {
                    context.Result = new RedirectResult("/Login/Index");
                }
                else
                {
                    context.Result = new RedirectResult("/Ticket/Index");
                }
            }
        }
    }
}
