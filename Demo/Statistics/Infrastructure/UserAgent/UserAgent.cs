namespace Demo.Statistics.Infrastructure.UserAgent
{
    public class UserAgent
    {
        private readonly string _userAgent;

        private ClientBrowser _browser;
        public ClientBrowser Browser => _browser ?? (_browser = new ClientBrowser(_userAgent));

        private ClientOs _os;
        public ClientOs Os => _os ?? (_os = new ClientOs(_userAgent));

        public UserAgent(string userAgent) => _userAgent = userAgent;
    }
}
