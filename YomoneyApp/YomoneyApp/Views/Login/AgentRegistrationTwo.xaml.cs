using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YomoneyApp.ViewModels.Countries;
using YomoneyApp.ViewModels.Login;

namespace YomoneyApp.Views.Login
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AgentRegistrationTwo : ContentPage
    {
        AccountViewModel viewModel;
       
        public AgentRegistrationTwo()
        {
            InitializeComponent();
            BindingContext = viewModel = new AccountViewModel(this);            

            Province.SelectedIndexChanged += async (sender, e) =>
            {
                try
                {
                    if (viewModel.FormProvince != Province.SelectedItem.ToString())
                    {
                        viewModel.FormProvince = Province.SelectedItem.ToString();

                        Town.Items.Clear();

                        var towns = await viewModel.ExecuteLoadTownsCommand(viewModel.FormProvince);
                        
                        foreach (var town in towns)
                        {
                            Town.Items.Add(town.Name.Trim());
                        }                            

                        Supervisor.Items.Clear();

                        var supervisors = await viewModel.ExecuteLoadSupervisorsCommand(viewModel.FormProvince);

                        foreach (var supervisor in supervisors)
                        {
                            Supervisor.Items.Add(supervisor.Firstname.Trim() + " " + supervisor.Lastname.Trim());
                        }                           
                    }                    

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }                
            };

            Town.SelectedIndexChanged += async (sender, e) =>
            {
                try
                {
                    if (Town.SelectedIndex > 0)
                    {
                        viewModel.FormTown = Town.SelectedItem.ToString();

                        await viewModel.ExecuteStoreTownIdCommand(viewModel.FormTown);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }                
            };

            Supervisor.SelectedIndexChanged += async (sender, e) =>
            {
                try
                {
                    if (Supervisor.SelectedIndex > 0)
                    {
                        viewModel.FormSupervisor = Supervisor.SelectedItem.ToString();

                        await viewModel.ExecuteStoreSupervisorCommand(viewModel.FormSupervisor);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            };

            Nationality.SelectedIndexChanged += async (sender, e) =>
            {
                try
                {
                    if (Nationality.SelectedIndex > 0)
                    {
                        viewModel.FormNationality = Nationality.SelectedItem.ToString();

                        await viewModel.ExecuteStoreNationalityCommand(viewModel.FormNationality);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            };
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();          

            try
            {
                if (viewModel.FormProvinceId == 0)
                {
                    var provinces = await viewModel.ExecuteLoadProvincesCommand();
                    Province.Items.Clear();
                    foreach (var province in provinces)
                        Province.Items.Add(province.Name.Trim());
                }               

                if (viewModel.FormNationalityId == 0)
                {
                    var nationalities = await viewModel.ExecuteLoadNationalitiesCommand();
                    Nationality.Items.Clear();
                    foreach (var nationality in nationalities)
                        Nationality.Items.Add(nationality.Name.Trim());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }            
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SignIn());
        }
    }
}