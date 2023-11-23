using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bybit.Api;
using Bybit.Api.Enums;
using Bybit.Api.Helpers.Public;

namespace ApiSharpApiExample
{
    public class BybitWs
    {
        private readonly BybitStreamClient _wsApi;

        public BybitWs(BybitStreamClient wsApi)
        {
            _wsApi = wsApi;
        }

        public async Task InitSection(BybitCategory category, IEnumerable<string> secCodes, Action<BybitTradeStream> onTrade) {
            foreach (var secCode in secCodes) {
                    await _wsApi.SubscribeToTradesAsync(category, secCode, data => {
                        var trade = data.Data;
                        onTrade(trade);
                    }).ConfigureAwait(false);
                }
        }
    }
}
