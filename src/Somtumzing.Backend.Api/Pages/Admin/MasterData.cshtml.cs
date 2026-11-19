using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Somtumzing.Backend.Api.Pages.Admin
{
    [Authorize(Policy = "BackOffice")]
    public class MasterDataModel : PageModel
    {
        public void OnGet()
        {
            // ทำงานเมื่อหน้าแผงควบคุมโหลดครั้งแรก
        }
    }
}
