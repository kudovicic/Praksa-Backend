using api_praksa.Filters;
using Microsoft.AspNetCore.Mvc;

namespace api_praksa.Controllers
{
    public class SessionTokenAttribute : TypeFilterAttribute
    {
        public SessionTokenAttribute() : base(typeof(SessionTokenFilter))
        {

        }

    }
}
