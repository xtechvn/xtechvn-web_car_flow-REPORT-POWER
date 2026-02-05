using Entities.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Repositories.IRepositories;
using Utilities;
using Utilities.Contants;
using WEB.CMS.Customize;
using WEB.CMS.Services;

namespace WEB.CMS.Controllers
{
    [CustomAuthorize]
    public class PlanController : Controller
    {
        private readonly IMenuRepository _MenuRepository;
        private readonly IConfiguration _configuration;
        private readonly IAllCodeRepository _allCodeRepository;
        private readonly IPlanRepository _planRepository;


        public PlanController(IMenuRepository menuRepository, IConfiguration configuration, IAllCodeRepository allCodeRepository , IPlanRepository planRepository)
        {
            _MenuRepository = menuRepository;
            _configuration = configuration;
            _allCodeRepository = allCodeRepository;
            _planRepository = planRepository;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Search(int? year)
        {
            year ??= DateTime.Now.Year;

            var data = await _planRepository.GetAll(year, null, null);
            return View(data);
        }




        public async Task<IActionResult> AddOrUpdate(int id)
        {
            // 1) Load dropdown chi nhánh từ AllCode
            var branches = _allCodeRepository.GetListByType(AllCodeType.BRANCH);
            ViewBag.Branches = branches;

            // 2) Model mặc định (Create)
            var model = new PlanUpsertViewModel
            {
                YearValue = DateTime.Now.Year,
                MonthValue = DateTime.Now.Month
            };
            model.Items.Add(new PlanItemVM()); // mặc định 1 dòng

            // 3) Edit
            if (id > 0)
            {
                // id ở đây là WeightId (hoặc PlanId tuỳ bạn lưu)
                var data = await _planRepository.GetForEdit(id);
                if (data == null) return NotFound();

                // Nếu GetForEdit trả về 1 item thì ok:
                model = data;

                // Nếu dự án bạn muốn edit 1 record thì Items chỉ có 1 dòng.
                if (model.Items == null || model.Items.Count == 0)
                    model.Items = new List<PlanItemVM> { new PlanItemVM() };
            }

            return View(model);
        }



        [HttpPost]
        public async Task<IActionResult> AddOrUpdatePlan(PlanUpsertViewModel model)
        {
            try
            {
                long result = 0;
                string actionName = model.MonthId > 0 ? "Cập nhật" : "Thêm mới";

                // validate cơ bản
                if (model.MonthValue < 1 || model.MonthValue > 12)
                    return Ok(new { isSuccess = false, message = "Tháng phải từ 1 đến 12" });

                if (model.Items == null || model.Items.Count == 0)
                    return Ok(new { isSuccess = false, message = "Vui lòng nhập ít nhất 1 chi nhánh và trọng lượng" });

                var dupBranch = model.Items.Where(x => x.Type > 0).GroupBy(x => x.Type).Any(g => g.Count() > 1);
                if (dupBranch)
                    return Ok(new { isSuccess = false, message = "Chi nhánh bị trùng, vui lòng chọn lại" });

                // user
                var username = User?.Identity?.Name;

                if (model.MonthId > 0)
                    result = await _planRepository.UpdatePlan(model, username);
                else
                    result = await _planRepository.CreatePlan(model, username);

                // validate trùng tháng chỉ áp dụng khi CREATE
                if (result == -3)
                    return Ok(new { isSuccess = false, message = $"Tháng {model.MonthValue}/{model.YearValue} đã tồn tại Plan" });

                if (result > 0)
                    return Ok(new { isSuccess = true, message = $"{actionName} Plan thành công" });

                return Ok(new { isSuccess = false, message = $"{actionName} Plan thất bại" });
            }
            catch (Exception ex)
            {
                return Ok(new { isSuccess = false, message = ex.Message });
            }
        }


        [HttpPost]
        [Route("/plan/delete-weight")]
        public async Task<IActionResult> DeleteWeight(int id)
        {
            try
            {
                var result = await _planRepository.DeleteWeight(id);

                if (result == -1)
                    return Ok(new { isSuccess = false, message = "Không tìm thấy dữ liệu cần xóa" });

                if (result > 0)
                    return Ok(new { isSuccess = true, message = "Xóa thành công" });

                return Ok(new { isSuccess = false, message = "Xóa thất bại" });
            }
            catch (Exception ex)
            {
                return Ok(new { isSuccess = false, message = ex.Message });
            }
        }

    }
}
