using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using ExampleApplication.Business.Data.Access;
using ExampleApplication.Business.Data.Entities;

namespace ExampleApplication.Controllers
{
    public class HeroController : Controller
    {
        private readonly IHeroRepository _heroRepository;

        public HeroController(IHeroRepository heroRepository)
        {
            _heroRepository = heroRepository;
        }

        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> DestroyEverything()
        {
            await _heroRepository.ResetAsInScorchedEarthKillEverything();
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            var data = _heroRepository.GetAll().ToList();
            return View(data);
        }

        public ActionResult Details(string id)
        {
            var hero = _heroRepository.GetById(id);
            var currentLocation = _heroRepository.GetCurrentLocation(id);
            if (currentLocation == null)
            {
                hero.DispatchStatus = DispatchType.Available;
                hero.DispatchLocation = null;
            }
            else
            {
                hero.DispatchStatus = DispatchType.Dispatched;
                hero.DispatchLocation = currentLocation;
            }
            return View(hero);
        }

        [HttpPost]
        public async Task<ActionResult> Dispatch(string id, string location)
        {
            await _heroRepository.DispatchHeroAsync(id, location);
            return RedirectToAction("Details", new {id});
        }

        [HttpPost]
        public async Task<ActionResult> Recall(string id)
        {
            await _heroRepository.RecallHeroAsync(id);
            return RedirectToAction("Details", new { id });
        }
    }
}