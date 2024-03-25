using ESD.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAS.Web.Component
{
    public class BookmarkViewComponent : ViewComponent
    {
        private readonly IModuleService _module;
        private readonly IUserBookMarkServices _userBookMark;
        // Comment
        public BookmarkViewComponent(IModuleService moduleService, IUserBookMarkServices userBookMark)
        {
            _module = moduleService;
            _userBookMark = userBookMark;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = await _module.GetsActive();
            var listBookMark = ((await _userBookMark.GetBookMark())?.Modules) ?? new List<int>();
            var bookMark = model.Where(x => listBookMark.Contains(x.ID)).OrderBy(x => listBookMark.IndexOf(x.ID)).ToList();
            return View(bookMark);
        }

    }
}
