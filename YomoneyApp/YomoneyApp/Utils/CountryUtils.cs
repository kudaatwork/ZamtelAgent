using PhoneNumbers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using YomoneyApp.Models;
using YomoneyApp.Services;
using YomoneyApp.ViewModels.Countries;

namespace YomoneyApp.Utils
{
    public static class CountryUtils
    {
        /// <summary>
        /// Gets the list of countries based on ISO 3166-1
        /// </summary>
        /// <returns>Returns the list of countries based on ISO 3166-1</returns>
        public static List<CountriesModel> GetCountriesByIso3166()
        {
            var countries = new List<CountriesModel>();

            CountriesAPIService services = new CountriesAPIService();

            var countriesList = services.GetCountriesList();

            //foreach (var culture in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
            //{
            //    var info = new RegionInfo(culture.LCID);
            //    if (countries.All(p => p.Name != info.Name))
            //        countries.Add(info);
            //}

            if (countriesList != null)
            {
                var countris = countriesList.OrderBy(p => p.name).ToList();

                return countris;
            }
            else
            {
                return null;
            }            
        }

        /// <summary>
        /// Get Country Model by Country Name
        /// </summary>
        /// <param name="countryName">English Name of Country</param>
        /// <returns>Complete Country Model with Region, Flag, Name and Code</returns>
        public static async Task<CountryModel> GetCountryModelByName(string countryName)
        {
            //var phoneNumberUtil = PhoneNumberUtil.GetInstance();
            var isoCountries = GetCountriesByIso3166();
            var countryInfo = isoCountries.FirstOrDefault(c => c.name == countryName);
            
            return countryInfo != null
                ? new CountryModel
                {
                    CountryCode = countryInfo.callingCodes[0],
                    CountryName = countryInfo.name,
                    FlagUrl = countryInfo.flags.png,
                }
                : new CountryModel
                {
                    CountryName = countryName
                };
        }

    }
}
