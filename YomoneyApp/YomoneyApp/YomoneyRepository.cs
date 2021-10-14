using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YomoneyApp.Models.Work;

namespace YomoneyApp
{
   public class YomoneyRepository:  IDataYomoney 
   {
        private readonly DatatbaseContext db;

        public YomoneyRepository(string dbPath)
        {
            db = new DatatbaseContext(dbPath);
        }

        #region Account Credential
        public async Task<bool> AddCredentialsAsync(UserCredentials user)
        {
            try
            {
                var traking = await db.UserCredentials.AddAsync(user);
                await db.SaveChangesAsync();
                var isAdded = traking.State = EntityState.Added;
               
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<UserCredentials> GetCredentialsAsyncByName(string Name)
        {
            try
            {
                var userz = await db.UserCredentials.ToListAsync();
                var contact = userz.Where(u => u.Phone == Name).FirstOrDefault();
                return contact;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        #endregion

        #region Contact Details
        public async Task<bool> AddContactAsync(YoContact contact)
        {
            try
            {
                var cnt = 0;
                var c = db.YoContacts.Count();
                if (c > 0)
                {
                    var count = db.YoContacts.Last();
                    if (count != null)
                        cnt = count.Id;
                }
                contact.Id = cnt + 1;
                if (string.IsNullOrEmpty(contact.Skills))
                    contact.Skills = "no skills added";
                var traking = await db.YoContacts.AddAsync(contact);
                await db.SaveChangesAsync();
                var isAdded = traking.State = EntityState.Added;
                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }

        public async Task<bool> AddRangeContactAsync(List<YoContact> contact)
        {
            try
            {
                var cnt = 1;
                var co = db.YoContacts.Count();
                if (co > 0)
                {
                    var count = db.YoContacts.Last();

                    if (count != null)
                        cnt = count.Id;
                }
                foreach (var c in contact)
                {
                   var w = db.YoContacts.Where(u => u.Phone == c.Phone).FirstOrDefault();
                    if (w == null)
                    {
                        c.Id = cnt;
                        if (string.IsNullOrEmpty(c.Skills))
                            c.Skills = "no skills added";
                        cnt++;
                    }
                }
                await db.YoContacts.AddRangeAsync(contact);
                await db.SaveChangesAsync();
                
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<List<YoContact>> GetContactsAsync()
        {
            try
            {
                List<YoContact> contacts = await db.YoContacts.ToListAsync();
                return contacts;
            }
            catch(Exception e)
            {
                return null;
            }
        }

        public async Task<YoContact> GetContactsAsyncById(int Id)
        {
            try
            {
                var contact = await db.YoContacts.FindAsync(Id);
                return contact;
            }
            catch(Exception e)
            {
                return null;
            }
        }

        public async Task<List<YoContact>> QueryContactsAsync(Func<YoContact, bool> predicate)
        {
            try
            {
                var contacts = db.YoContacts.Where(predicate);
                return contacts.ToList();
            }
            catch(Exception e)
            {
                return null;
            }
        }

        public async Task<bool> RemoveContactAsync(int Id) 
        {
            try
            {
                var contact = await db.YoContacts.FindAsync(Id);
                var tracking = db.Remove(contact);
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception e )
            {
                return false;

            }
        }

        public async Task<bool> UpdateContactAsync(YoContact contact)
        {
            try
            {
                var tracking = db.YoContacts.Update(contact);
                await db.SaveChangesAsync();
                return true;

            }
            catch (Exception e)
            {
                return false;
            }
        }
        #endregion

        #region Message Details
        public async Task<bool> AddMessageAsync(ChatMessage msg)
        {
            try
            {
                long cnt = 0;
                var c = db.YoMessages.Count();
              
                msg.Id = c + 1;
                
                var traking = await db.YoMessages.AddAsync(msg);
                await db.SaveChangesAsync();
                var isAdded = traking.State = EntityState.Added;
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<List<ChatMessage>> GetMessagesAsync()
        {
            try
            {
                List<ChatMessage> messages = await db.YoMessages.ToListAsync();
                return messages;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<List<ChatMessage>> QueryChatMessageAsync(Func<ChatMessage, bool> predicate)
        {
            try
            {
                var contacts =  db.YoMessages.Where(predicate);
                return contacts.ToList();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<bool> RemoveMessaesAsync(int Id)
        {
            try
            {
                var contact = await db.YoMessages.FindAsync(Id);
                var tracking = db.Remove(contact);
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                return false;

            }
        }

        public async Task<bool> UpdateMessagesAsync(ChatMessage contact)
        {
            try
            {
                var tracking = db.YoMessages.Update(contact);
                await db.SaveChangesAsync();
                return true;

            }
            catch (Exception e)
            {
                return false;
            }
        }
        #endregion
    }
}
