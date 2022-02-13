using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TSM.Core.LocalStorage;

namespace TSM.Logic.ItemLookup
{
    public class ItemLookup
    {
        private const string wowheadUrl = "https://www.wowhead.com/item={0}&xml";
        private SemaphoreSlim lookupSemaphore = new(10);

        public async Task LookupItemsAsync(IEnumerable<string> itemIds, IDataStore dataStore)
        {
            using HttpClient httpClient = new();
            var tasks = itemIds.Select(x => LookupItemIDAsync(x, httpClient));

            await Task.WhenAll(tasks).ContinueWith(async x => await dataStore.StoreItemNames(x.Result.Where(x => 
                !string.IsNullOrWhiteSpace(x.Key)).ToDictionary(x => x.Key, x => x.Value)));

            //await dataStore.StoreItemNames(tasks.Select(x => x.Result).Where(x => !string.IsNullOrWhiteSpace(x.Key)).ToDictionary(x => x.Key, x => x.Value));
        }

        private async Task<KeyValuePair<string, string>> LookupItemIDAsync(string x, HttpClient httpClient)
        {
            try
            {
                await lookupSemaphore.WaitAsync();

                if (x[0] == 'i')
                {
                    var request = await httpClient.GetAsync(new Uri(string.Format(wowheadUrl, GetWowheadItemID(x))));

                    if (request.IsSuccessStatusCode)
                    {
                        XmlSerializer xmlSerializer = new(typeof(ItemModel));
                        ItemModel model = xmlSerializer.Deserialize(request.Content.ReadAsStream()) as ItemModel;
                        return new KeyValuePair<string, string>(x, model.Item.Name);
                    }
                    else
                    {
                        System.Diagnostics.Debugger.Break();
                    }
                }

                return default;
            }
            finally
            {
                lookupSemaphore.Release();
            }
        }

        private static string GetWowheadItemID(string id)
        {
            id = id[2..];
            return id.Substring(0, id.IndexOf(':') > 0 ? id.IndexOf(':') : id.Length);
        }
    }
}
