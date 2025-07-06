using System.Linq;
using AutomatedSearch.Model;

namespace AutomatedSearch.ViewModel
{
    public partial class ViewModel
    {
        public bool AddAccount(User user)
        {
            if (AppData.Accounts.FirstOrDefault(ac => ac.Username == user.Username) != null)
            {
                SendMessage("This account is aready added!");
                return false;
            }

            AppData.Accounts.Add(user);
            if (!SaveToFile())
            {
                return false;
            }

            return true;
        }

        public bool RemoveAccount(User user)
        {
            bool res = AppData.Accounts.Remove(user);
            if (res)
            {
                if (!SaveToFile())
                {
                    return false;
                }
            }
            else
            {
                SendMessage("This account doesn't exist!");
            }

            return res;
        }

    }
}
