using Hangfire.Context;
using Microsoft.AspNetCore.Mvc;

namespace Hangfire.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ProductContext _context;
        private readonly ILogger<ProductController> _logger;

        public ProductController(
            ProductContext context,
            ILogger<ProductController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("")]
        public IActionResult Default()
        {
            var data = _context.GetRandom();
            _logger.LogInformation($"Default job executed. Data: {data.Name}");

            return Ok();
        }

        [HttpGet("hangfire/fire-and-forget")]
        public IActionResult HangfireFireAndForget()
        {
            var jobId = BackgroundJob.Enqueue(() => HangfireFireAndForgetJob());

            return Ok($"Job ID: {jobId}. Fire-and-forget job scheduled.");
        }

        [HttpGet("hangfire/delayed")]
        public IActionResult HangfireDelayed()
        {
            var jobId = BackgroundJob.Schedule(() => HangfireDelayedJob(), TimeSpan.FromMinutes(1));

            return Ok($"Job ID: {jobId}. Delayed job scheduled.");
        }

        [HttpGet("hangfire/recurring")]
        public IActionResult HangfireRecurring()
        {
            RecurringJob.AddOrUpdate("recurring-job", () => HangfireRecurringJob(), Cron.Minutely);

            return Ok("Recurring job scheduled.");
        }

        [HttpGet("hangfire/continuation")]
        public IActionResult HangfireContinuation()
        {
            var parentJobId = BackgroundJob.Enqueue(() => HangfireContinuationParentJob());
            BackgroundJob.ContinueWith(parentJobId, () => HangfireContinuationChildJob());

            return Ok("Continuation job scheduled.");
        }

        [NonAction]
        public void HangfireFireAndForgetJob()
        {
            var data = _context.GetRandom();
            _logger.LogInformation($"Fire-and-forget job executed! Data: {data?.Name}");
        }

        [NonAction]
        public void HangfireDelayedJob()
        {
            var data = _context.GetRandom();
            _logger.LogInformation($"Delayed job executed! Data: {data?.Name}");
        }

        [NonAction]
        public void HangfireRecurringJob()
        {
            var data = _context.GetRandom();
            _logger.LogInformation($"Recurring job executed! Data: {data?.Name}");
        }

        [NonAction]
        public void HangfireContinuationParentJob()
        {
            var parentData = _context.GetRandom();
            _logger.LogInformation($"Parent job executed! Data: {parentData?.Name}");
        }

        [NonAction]
        public void HangfireContinuationChildJob()
        {
            var continuationData = _context.GetRandom();
            _logger.LogInformation($"Continuation job executed! Data: {continuationData?.Name}");
        }
    }
}