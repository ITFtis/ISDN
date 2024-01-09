using SQLModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModels;

namespace Services.Interface
{
    public interface ILoginManager
    {
          string[] GetCaptchImage();
          AdminMemberModel ValidateUser(string account,string password);
        string ForgetPassword(string email);
    }
}
