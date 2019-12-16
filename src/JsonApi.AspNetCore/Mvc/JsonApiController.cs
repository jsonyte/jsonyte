using System.Linq;
using System.Threading.Tasks;
using JsonApi.AspNetCore.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JsonApi.AspNetCore.Mvc
{
    public class JsonApiController<T> : ControllerBase
    {
        [HttpGet]
        public virtual async Task<IActionResult> GetAsync()
        {
            var models = GetRepository().GetAsync().ToArray();

            return Ok(models);
        }

        [HttpGet("{id}")]
        public virtual async Task<IActionResult> GetAsync(string id)
        {
            var model = GetRepository().GetAsync(id);

            return Ok(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> PostAsync([FromBody] T model)
        {
            GetRepository().Create(model);

            return Ok();
        }

        [HttpPatch("{id}")]
        public virtual async Task<IActionResult> PatchAsync(string id, [FromBody] T model)
        {
            GetRepository().Update(id, model);

            return Ok();
        }

        [HttpDelete("{id}")]
        public virtual async Task<IActionResult> DeleteAsync(string id)
        {
            GetRepository().Delete(id);

            return Ok();
        }

        private IRepository<T> GetRepository()
        {
            var repository = (IRepository<T>) HttpContext.RequestServices.GetService(typeof(IRepository<T>));

            if (repository == null)
            {
                throw new JsonApiException(StatusCodes.Status500InternalServerError);
            }

            return repository;
        }
    }
}