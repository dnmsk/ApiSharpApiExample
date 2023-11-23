using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiSharp.Models;
using Bybit.Api;
using Bybit.Api.Enums;
using Bybit.Api.Models;

namespace ApiSharpApiExample
{
    public class BybitRest
    {
        private readonly BybitRestApiClient _restApi;
        private readonly ConcurrentDictionary<BybitCategory, AsyncLazy<ConcurrentDictionary<string, LocalSecurityInfo>>> _secInfos = new ConcurrentDictionary<BybitCategory, AsyncLazy<ConcurrentDictionary<string, LocalSecurityInfo>>>();

        public BybitRest(BybitRestApiClient restApi)
        {
            _restApi = restApi;
        }

        public static IEnumerable<BybitCategory> ClassesListLocal => new[] { BybitCategory.Linear, BybitCategory.Inverse, BybitCategory.Option };

        public AsyncLazy<ConcurrentDictionary<string, LocalSecurityInfo>> GetClassCodeSecurities(BybitCategory category) {
            return _secInfos.GetOrAdd(category, classId => {
                switch (category) {
                    case BybitCategory.Spot:
                        return new AsyncLazy<ConcurrentDictionary<string, LocalSecurityInfo>>(() => _restApi.Market.GetSpotInstrumentsAsync()
                            .ContinueWith(t => {
                                var responseResult = t.Result;
                                var map = responseResult.Data.Select(i => {
                                    var n = i.Symbol;
                                    return new LocalSecurityInfo(classId.ToString(), n, n, i.QuoteCoin, i.LotSizeFilter.MinimumOrderQuantity, i.PriceFilter.TickSize);
                                }).ToDictionary(k => k.SecCode, k => k);

                                return new ConcurrentDictionary<string, LocalSecurityInfo>(map);
                            }));
                    case BybitCategory.Linear:
                        return new AsyncLazy<ConcurrentDictionary<string, LocalSecurityInfo>>(() => EnumerableToConcurrentDictionary(
                                WithCursorClassIterator(c => _restApi.Market.GetLinearInstrumentsAsync(cursor: c, limit: 1000), null),
                                i => {
                                    var n = i.Symbol;
                                    return new LocalSecurityInfo(classId.ToString(), n, n, i.QuoteCoin, i.LotSizeFilter.MinimumOrderQuantity, i.PriceFilter.TickSize);
                                }, v => v.SecCode));
                    case BybitCategory.Inverse:
                        return new AsyncLazy<ConcurrentDictionary<string, LocalSecurityInfo>>(() => EnumerableToConcurrentDictionary(
                                WithCursorClassIterator(c => _restApi.Market.GetInverseInstrumentsAsync(cursor: c, limit: 1000), null),
                                i => {
                                    var n = i.Symbol;
                                    return new LocalSecurityInfo(classId.ToString(), n, n, i.QuoteCoin, i.LotSizeFilter.MinimumOrderQuantity, i.PriceFilter.TickSize);
                                }, v => v.SecCode));
                    case BybitCategory.Option:
                        return new AsyncLazy<ConcurrentDictionary<string, LocalSecurityInfo>>(() => EnumerableToConcurrentDictionary(
                                WithCursorClassIterator(c => _restApi.Market.GetOptionInstrumentsAsync(cursor: c, limit: 1000), null),
                                i => {
                                    var n = i.Symbol;
                                    return new LocalSecurityInfo(classId.ToString(), n, n, i.QuoteCoin, i.LotSizeFilter.MinimumOrderQuantity, i.PriceFilter.TickSize);
                                }, v => v.SecCode));
                }

                return null;
            });
        }

        private static async Task<ConcurrentDictionary<K, V>> EnumerableToConcurrentDictionary<K, V, I>(IAsyncEnumerable<I> enumerable, Func<I, V> toVal, Func<V, K> getKey = null) {
            var map = await enumerable.Select(toVal).ToDictionaryAsync(getKey, v => v).ConfigureAwait(false);
            return new ConcurrentDictionary<K, V>(map);
        }

        private static async IAsyncEnumerable<T> WithCursorClassIterator<T>(Func<string, Task<RestCallResult<BybitRestApiCursorResponse<T>>>> cursorFetcher, string initialCursor) {
            var currentCursor = initialCursor;
            do {
                var apiCallResult = await cursorFetcher(currentCursor).ConfigureAwait(false);
                var response = apiCallResult.Data;
                foreach (var r in response.Payload) {
                    yield return r;
                }

                currentCursor = response.NextPageCursor;
            } while (!string.IsNullOrWhiteSpace(currentCursor));
        }
    }
}
