using System;
using System.Threading.Tasks;
using Bybit.Api;
using Bybit.Api.Helpers.Public;

namespace ApiSharpApiExample
{
    public static class Program
    {
        public static async Task Main()
        {
            var rest = new BybitRest(new BybitRestApiClient(new BybitRestApiClientOptions
            {
                RawResponse = true,
            }));
            var ws = new BybitWs(new BybitStreamClient(new BybitStreamClientOptions
            {
                RawResponse = true,
                SubscriptionsCombineTarget = 100,
                AutoReconnect = true,
                NoDataTimeout = TimeSpan.FromSeconds(25)
            }));

            foreach (var bybitCategory in BybitRest.ClassesListLocal)
            {
                var secCodes = await rest.GetClassCodeSecurities(bybitCategory);
                await ws.InitSection(bybitCategory, secCodes.Keys, OnTradeReceived).ConfigureAwait(false);
            }

            while (true)
            {
                await Task.Delay(1000);
            }
        }


        private static void OnTradeReceived(BybitTradeStream trade)
        {

        }
    }
}
