using Microsoft.AspNetCore.Rewrite;
using NLog;
using System.Text.RegularExpressions;
using ILogger = NLog.ILogger;

namespace ngaoda
{
    public class CustomRewriteRule() : IRule
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        public void ApplyRule(RewriteContext context)
        {
            try
            {
                var request = context.HttpContext.Request;
                var path = request.Path.ToString();

                var query = request.QueryString.ToString();

                // Pattern match URL
                var regexPattern = @"\/resources\/(.+)\.(jpg|jpeg|png)";
                //var conditionPattern = @"w=(\d+)&h=(\d+)&q=(\d+)&dpr=(\d+)&fit=(\w+)";
                //var conditionPattern = @"(?:w=(\d+)|h=(\d+)|q=(\d+)|dpr=(\d+)|fit=(\w+))";
                var conditionPattern = @"(?:w=(\d+)|h=(\d+)|q=(\d+)|dpr=(\d+)|fit=(\w+))(?:&|$)";


                var match = Regex.Match(path, regexPattern);
                var conditionMatches = Regex.Matches(query, conditionPattern);


                //if (match.Success && conditionMatch.Success)
                //{
                //    //var imagePath = match.Groups[1].Value + "." + match.Groups[3].Value;
                //    var width = conditionMatch.Groups[1].Value;
                //    var height = conditionMatch.Groups[2].Value;
                //    var quality = conditionMatch.Groups[3].Value;
                //    var dpr = conditionMatch.Groups[4].Value;
                //    var fit = conditionMatch.Groups[5].Value;

                if (match.Success && conditionMatches.Count > 0)
                {
                    var width = "";
                    var height = "";
                    var quality = "";
                    var dpr = "";
                    var fit = "";

                    foreach (Match conditionMatch in conditionMatches)
                    {
                        if (conditionMatch.Groups[1].Success)
                        {
                            width = conditionMatch.Groups[1].Value;
                        }
                        else if (conditionMatch.Groups[2].Success)
                        {
                            height = conditionMatch.Groups[2].Value;
                        }
                        else if (conditionMatch.Groups[3].Success)
                        {
                            quality = conditionMatch.Groups[3].Value;
                        }
                        else if (conditionMatch.Groups[4].Success)
                        {
                            dpr = conditionMatch.Groups[4].Value;
                        }
                        else if (conditionMatch.Groups[5].Success)
                        {
                            fit = conditionMatch.Groups[5].Value;
                        }
                    }


                    // Rewrite path internally
                    context.HttpContext.Request.Path = "/ImageProcessing"; // nhảy sang api trong ImageProcessingController
                    context.HttpContext.Request.QueryString = new QueryString($"?path={path.Substring(1)}&w={width}&h={height}&q={quality}&dpr={dpr}&fit={fit}");

                    Logger.Info("url-image-matched");
                }
                //context.Result = RuleResult.SkipRemainingRules; // Bỏ qua các quy tắc rewrite còn lại

            }
            catch (Exception ex)
            {
                // Xử lý exception tại đây
                // Ví dụ: ghi log exception
                Console.WriteLine($"Exception in ApplyRule middleware: {ex.Message}");

                // Rethrow để cho phép ngoại lệ tiếp tục lan ra nếu cần thiết
                throw;
            }

        }
    }
}
