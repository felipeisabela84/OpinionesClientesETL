using Microsoft.Extensions.Logging;

namespace OpinionesClientesETL.DATA
{
    public class LoggerService
    {
        private readonly ILogger<LoggerService> _logger;

        public LoggerService(ILogger<LoggerService> logger)
        {
            _logger = logger;
        }

        public void LogInfo(string message)
        {
            _logger.LogInformation($"[ETL INFO]: {message}");
        }

        public void LogError(string message, Exception ex = null)
        {
            _logger.LogError(ex, $"[ETL ERROR]: {message}");
        }
    }
}