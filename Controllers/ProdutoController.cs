using Hangfire.Context;
using Hangfire.Models;
using Microsoft.AspNetCore.Mvc;

namespace Hangfire.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProdutoController : ControllerBase
    {
        private readonly ProdutoContext _context;

        public ProdutoController(ProdutoContext context)
        {
            _context = context;
        }
       
        [HttpPost("")]
        public IActionResult DefaultAdd([FromBody] Produto product)
        {
            var data = _context.Add(product).Entity;
            return Ok($"Requisição padrão para adicionar um produto. Data: {data.Nome}");
        }

        [HttpGet("")]
        public IActionResult DefaultGet()
        {
            var data = _context.GetRandom();
            return Ok($"Requisição padrão para ler um produto. Data: {data.Nome}");
        }

        [HttpPost("fire-and-forget")]
        public IActionResult AddFireAndForget([FromBody] Produto product)
        {
            var jobId = BackgroundJob.Enqueue(() => _context.Add(product));
            return Ok($"Job ID: {jobId}. Fire-and-forget job para adicionar um produto agendado.");
        }

        [HttpGet("fire-and-forget")]
        public IActionResult GetFireAndForget()
        {
            var jobId = BackgroundJob.Enqueue(() => _context.GetRandom());
            return Ok($"Job ID: {jobId}. Fire-and-forget job para ler um produto agendado.");
        }

        [HttpPost("delayed")]
        public IActionResult AddDelayed([FromBody] Produto product)
        {
            var jobId = BackgroundJob.Schedule(() => _context.Add(product), TimeSpan.FromMinutes(1));
            return Ok($"Job ID: {jobId}. Delayed job para adicionar um produto foi agendado para daqui 1 minuto.");
        }

        [HttpGet("delayed")]
        public IActionResult GetDelayed()
        {
            var jobId = BackgroundJob.Schedule(() => _context.GetRandom(), TimeSpan.FromMinutes(1));
            return Ok($"Job ID: {jobId}. Delayed job para ler um produto foi agendado para daqui 1 minuto.");
        }

        [HttpPost("recurring")]
        public IActionResult AddRecurring([FromBody] Produto product)
        {
            RecurringJob.AddOrUpdate("add-product-recurring", () => _context.Add(product), Cron.Minutely);
            return Ok("Recurring job para adicionar um produto agendado para cada minuto.");
        }

        [HttpGet("recurring")]
        public IActionResult GetRecurring()
        {
            RecurringJob.AddOrUpdate("recurring-job", () => _context.GetRandom(), Cron.Minutely);
            return Ok("Recurring job para ler um produto agendado para cada minuto.");
        }

        [HttpPost("continuation")]
        public IActionResult AddContinuation([FromBody] Produto product)
        {
            var parentJobId = BackgroundJob.Enqueue(() => _context.Add(new Produto { Nome = "Produto novo" }));
            BackgroundJob.ContinueWith(parentJobId, () => _context.Add(product));
            return Ok("Continuation job para adicionar um produto foi agendado.");
        }

        [HttpGet("continuation")]
        public IActionResult GetContinuation()
        {
            var parentJobId = BackgroundJob.Enqueue(() => _context.GetRandom());
            BackgroundJob.ContinueWith(parentJobId, () => _context.GetRandom());
            return Ok("Continuation job para ler um produto foi agendado.");
        }
    }
}