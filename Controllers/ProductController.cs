using Hangfire.Context;
using Hangfire.Models;
using Microsoft.AspNetCore.Mvc;

namespace Hangfire.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly Random _random;
        private readonly ProductContext _context;
        private readonly ILogger<ProductController> _logger;

        public ProductController(
            ProductContext context,
            ILogger<ProductController> logger)
        {
            _random = new Random();
            _context = context;
            _logger = logger;
        }

        [HttpGet("fire-and-forget")]
        public IActionResult FireAndForget()
        {
            // Obter o provedor de escopo de injeção de dependência
            var scopeFactory = HttpContext.RequestServices.GetRequiredService<IServiceScopeFactory>();

            Task.Run(() =>
            {
                // Criar um novo escopo para o trabalho em segundo plano
                using var scope = scopeFactory.CreateScope();
                var scopedContext = scope.ServiceProvider.GetRequiredService<ProductContext>();
                var data = scopedContext.Products.ElementAt(_random.Next(100));
                _logger.LogInformation($"Fire-and-forget job executed! Data: {data.Name}");
            });

            return Ok("Fire-and-forget job scheduled.");
        }

        [HttpGet("hangfire/fire-and-forget")]
        public IActionResult HangfireFireAndForget()
        {
            var jobId = BackgroundJob.Enqueue(() => HangfireFireAndForgetJob());
            return Ok($"Job ID: {jobId}. Fire-and-forget job scheduled.");
        }

        [HttpGet("delayed")]
        public IActionResult Delayed()
        {
            Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromMinutes(1));
                // Acesse o banco de dados aqui
                var data = GetRandom();
                _logger.LogInformation($"Delayed job executed! Data: {data.Name}");
            });
            return Ok("Delayed job scheduled.");
        }

        [HttpGet("hangfire/delayed")]
        public IActionResult HangfireDelayed()
        {
            var jobId = BackgroundJob.Schedule(() => HangfireDelayedJob(), TimeSpan.FromMinutes(1));
            return Ok($"Job ID: {jobId}. Delayed job scheduled.");
        }

        [HttpGet("recurring")]
        public IActionResult Recurring()
        {
            // Obter o provedor de escopo de injeção de dependência
            var scopeFactory = HttpContext.RequestServices.GetRequiredService<IServiceScopeFactory>();

            var timer = new Timer((e) =>
            {
                // Criar um novo escopo para o trabalho em segundo plano
                using (var scope = scopeFactory.CreateScope())
                {
                    // Obter uma nova instância do ProductContext para cada execução do timer
                    var scopedContext = scope.ServiceProvider.GetRequiredService<ProductContext>();
                    var data = scopedContext.Products.ElementAt(_random.Next(100));
                    _logger.LogInformation($"Recurring job executed! Data: {data.Name}");
                }
            }, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));

            return Ok("Recurring job scheduled.");
        }

        [HttpGet("hangfire/recurring")]
        public IActionResult HangfireRecurring()
        {
            RecurringJob.AddOrUpdate("recurring-job", () => HangfireRecurringJob(), Cron.Minutely);
            return Ok("Recurring job scheduled.");
        }

        [HttpGet("continuation")]
        public IActionResult Continuation()
        {
            // Obter o provedor de escopo de injeção de dependência
            var scopeFactory = HttpContext.RequestServices.GetRequiredService<IServiceScopeFactory>();

            Task.Run(() =>
            {
                // Criar um novo escopo para o trabalho em segundo plano (job "pai")
                using (var scope = scopeFactory.CreateScope())
                {
                    var scopedContext = scope.ServiceProvider.GetRequiredService<ProductContext>();
                    var parentData = scopedContext.Products.ElementAt(_random.Next(100));
                    _logger.LogInformation($"Parent job executed! Data: {parentData.Name}");

                    // Executar o trabalho "continuação"
                    Task.Run(() =>
                    {
                        // Criar outro escopo para o trabalho de continuação
                        using (var continuationScope = scopeFactory.CreateScope())
                        {
                            var continuationContext = continuationScope.ServiceProvider.GetRequiredService<ProductContext>();
                            var continuationData = continuationContext.Products.ElementAt(_random.Next(100));
                            _logger.LogInformation($"Continuation job executed! Data: {continuationData.Name}");
                        }
                    });
                }
            });

            return Ok("Continuation job scheduled.");
        }

        [HttpGet("hangfire/continuation")]
        public IActionResult HangfireContinuation()
        {
            var parentJobId = BackgroundJob.Enqueue(() => HangfireContinuationParentJob());
            BackgroundJob.ContinueWith(parentJobId, () => HangfireContinuationChildJob());
            return Ok("Continuation job scheduled.");
        }

        public void HangfireFireAndForgetJob()
        {
            // Acesse o banco de dados aqui
            var data = GetRandom();
            _logger.LogInformation($"Fire-and-forget job executed! Data: {data?.Name}");
        }

        public void HangfireDelayedJob()
        {
            // Acesse o banco de dados aqui
            var data = GetRandom();
            _logger.LogInformation($"Delayed job executed! Data: {data?.Name}");
        }

        public void HangfireRecurringJob()
        {
            // Acesse o banco de dados aqui
            var data = GetRandom();
            _logger.LogInformation($"Recurring job executed! Data: {data?.Name}");
        }

        public void HangfireContinuationParentJob()
        {
            // Acesse o banco de dados aqui
            var parentData = GetRandom();
            _logger.LogInformation($"Parent job executed! Data: {parentData?.Name}");
        }

        public void HangfireContinuationChildJob()
        {
            // Acesse o banco de dados aqui
            var continuationData = GetRandom();
            _logger.LogInformation($"Continuation job executed! Data: {continuationData?.Name}");
        }

        public Product GetRandom()
        {
            return _context.Products.ElementAt(_random.Next(100));
        }
    }
}