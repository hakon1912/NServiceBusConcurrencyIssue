using System.Threading.Tasks;
using System.Web.Http;
using NServiceBus;
using IfInsurance.Messages.Commands;

namespace Api.Controllers
{
    [RoutePrefix("api")]
    public class SomeController : ApiController
    {
        private readonly IEndpointInstance _endpointInstance;

        public SomeController(IEndpointInstance endpointInstance)
        {
            _endpointInstance = endpointInstance;
        }

        [Route]
        public async Task<IHttpActionResult> Post()
        {
            await _endpointInstance.Send(new SomeCommand()).ConfigureAwait(false);
            return Ok();
        }
    }
}
