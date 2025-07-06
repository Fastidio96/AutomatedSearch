using AutomatedSearch.Model;

namespace AutomatedSearch
{
    public class Costants
    {
        public const string URL_LOGIN = "https://login.live.com/";
        public const string URL_HOST_LOGGED = "account.microsoft.com";
        public const string URL_POINTS_INFOS = "https://rewards.bing.com/pointsbreakdown";
        public const string URL_REWARDS = "https://rewards.bing.com/";
        public const string URL_LOGOUT = "https://rewards.bing.com/Signout";

        public const string URL_SEARCH_FORMAT = "https://www.bing.com/search?q=what+is+";

        public const string JS_PATH_BALANCE_POINTS = "#balanceToolTipDiv > p > mee-rewards-counter-animation > span";
        public const string JS_PATH_ADDITIONAL_INFO = "#rewards-header > div > div.l_header_right > a.redirect_link.additional_info";
        public const string JS_PATH_SEARCHES = "#userPointsBreakdown > div > div:nth-child(2) > div > div:nth-child(1) > div > div.pointsDetail > mee-rewards-user-points-details > div > div > div > div > p.pointsDetail.c-subheading-3.ng-binding";

        /// <summary>
        /// Must be 32 bytes length => En\conding UTF-8
        /// </summary>
        public const string KEY = @"@#adawd84484wdafeeefeSDFS333eER!";
    }
}
