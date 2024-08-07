namespace GenAIApp.Utlis
{
    public class RateLimiter
    {
        private readonly TimeSpan _requestInterval;
        private DateTime _nextRequestTime;

        public RateLimiter(TimeSpan requestInterval)
        {
            _requestInterval = requestInterval;
            _nextRequestTime = DateTime.UtcNow;
        }

        public async Task WaitAsync()
        {
            var now = DateTime.UtcNow;
            var waitTime = _nextRequestTime - now;
            if (waitTime > TimeSpan.Zero)
            {
                await Task.Delay(waitTime);
            }
            _nextRequestTime = DateTime.UtcNow + _requestInterval;
        }

    }
}
