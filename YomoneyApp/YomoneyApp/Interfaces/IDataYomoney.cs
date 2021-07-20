using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using YomoneyApp.Models.Work;

namespace YomoneyApp
{
    public interface IDataYomoney
    {
        
        Task<YoContact> GetContactsAsyncById(int Id);
        Task<UserCredentials> GetCredentialsAsyncByName(string Name);
        Task<List<YoContact>> GetContactsAsync();
        Task<bool> AddContactAsync(YoContact contact);
        Task<bool> AddCredentialsAsync(UserCredentials user);
        Task<bool> RemoveContactAsync(int Id);
        Task<bool> UpdateContactAsync(YoContact contact);
        Task<List<YoContact>> QueryContactsAsync(Func<YoContact,bool> predicate);
    }
}

