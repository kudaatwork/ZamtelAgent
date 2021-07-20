using System;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Linq;
using MvvmHelpers;
using YomoneyApp.Helpers;

namespace YomoneyApp
{
    public class OppotunityViewModel : ViewModelBase
    {
        readonly IDataStore dataStore;
        public ObservableRangeCollection<Store> Stores { get; set; }
        public ObservableRangeCollection<Grouping<string, Store>> StoresGrouped { get; set; }
        public bool ForceSync { get; set; }
        public OppotunityViewModel(Page page) : base(page)
        {
            //Title = "Locations";
            dataStore = DependencyService.Get<IDataStore>();
            Stores = new ObservableRangeCollection<Store>();
            StoresGrouped = new ObservableRangeCollection<Grouping<string, Store>>();
        }
        public Action<Store> ItemSelected { get; set; }

        public async Task DeleteStore(Store store)
        {
            if (IsBusy)
                return;
            IsBusy = true;
            try
            {
                await dataStore.RemoveStoreAsync(store);
                Stores.Remove(store);
                Sort();
            }
            catch (Exception ex)
            {
                await page.DisplayAlert("Oh Oooh :(", $"Unable to remove {store?.Name ?? "Unknown"}, please try again: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }



        }

        private Command forceRefreshCommand;
        public Command ForceRefreshCommand
        {
            get
            {
                return forceRefreshCommand ??
                    (forceRefreshCommand = new Command(async () =>
                    {
                        ForceSync = true;
                        await ExecuteGetOppotunitiesCommand();
                    }));
            }
        }

        private Command getOppotunitiesCommand;
        public Command GetOppotunitiesCommand
        {
            get
            {
                return getOppotunitiesCommand ??
                    (getOppotunitiesCommand = new Command(async () => await ExecuteGetOppotunitiesCommand(), () => { return !IsBusy; }));
            }
        }

        private async Task ExecuteGetOppotunitiesCommand()
        {
            if (IsBusy)
                return;

            if (ForceSync)
             //Settings.LastSync = DateTime.Now.AddDays(-30);

            IsBusy = true;
            GetOppotunitiesCommand.ChangeCanExecute();
            var showAlert = false;
            try
            {
                Stores.Clear();

                var stores = await dataStore.GetStoresAsync();

                Stores.ReplaceRange(stores);


                Sort();
            }
            catch (Exception ex)
            {
                showAlert = true;

            }
            finally
            {
                IsBusy = false;
                GetOppotunitiesCommand.ChangeCanExecute();
            }

            if (showAlert)
                await page.DisplayAlert("Oh Oooh :(", "Unable to gather oppotunities.", "OK");


        }

        private void Sort()
        {

            StoresGrouped.Clear();
            var sorted = from store in Stores
                         orderby store.Country, store.City
                         group store by store.Country into storeGroup
                         select new Grouping<string, Store>(storeGroup.Key, storeGroup);

            StoresGrouped.ReplaceRange(sorted);
        }
    }

}

