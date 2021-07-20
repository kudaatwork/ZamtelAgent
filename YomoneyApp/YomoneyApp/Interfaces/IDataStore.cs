using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace YomoneyApp
{
    public interface IDataStore
    {
        Task Init();
        Task<MenuItem> AddSelectedAsync(MenuItem store);
        Task<IEnumerable<MenuItem>> GetSelectedAsync();
        Task<bool> RemoveSelectedAsync(MenuItem store);
        Task<IEnumerable<Store>> GetStoresAsync();
        Task<Store> AddStoreAsync(Store store);
        Task<bool> RemoveStoreAsync(Store store);
        Task<Store> UpdateStoreAsync(Store store);
        Task SyncStoresAsync();
        Task SyncFeedbacksAsync();
    }
}

