using Services.Interface;
using SQLModel;
using SQLModel.Models;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Net.Mail;
using Utilities;
using ViewModels;
using ViewModels.DBModels;

namespace Services.Manager
{
    public class LoginManager : ILoginManager
    {
        readonly SQLRepository<Users> _sqlrepository;
        readonly SQLRepository<GroupUser> _authoritygroupsqlrepository;
        readonly SQLRepository<StudentFormSetting> _formsettingsqlrepository;
        public LoginManager(SQLRepositoryInstances sqlinstance)
        {
            _sqlrepository = sqlinstance.Users;
            _authoritygroupsqlrepository= sqlinstance.GroupUser;
            _formsettingsqlrepository= sqlinstance.StudentFormSetting;
        }
        public string[] GetCaptchImage()
        {
            CaptchaImage _captcha = new CaptchaImage();
            _captcha.LineNoise =CaptchaImage.LineNoiseLevel.None;
            _captcha.BackgroundNoise = CaptchaImage.BackgroundNoiseLevel.None;
            _captcha.TextLength = 4;
            _captcha.Width = 74;
            _captcha.Height = 30;
            Bitmap _bmp = _captcha.RenderImage();
            var base64str = "";
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                _bmp.Save(ms, ImageFormat.Png);
                base64str = string.Format("data:{0};base64,{1}", "image/png", Convert.ToBase64String(ms.ToArray()));
                _bmp.Dispose();
                ms.Dispose();
            }
            return new string[] { _captcha.Text,base64str } ;
        }

        public AdminMemberModel ValidateUser(string account,string password)
        {
            AdminMemberModel _adminuser=null;
            var user = _sqlrepository.GetByWhere("Account=@1 and PWD=@2", new object[] { account , password }).ToArray();
            if (user.Count() > 0) {
                _adminuser = new AdminMemberModel();
                _adminuser.ID= user.First().ID.Value;
                _adminuser.Account = account;
                _adminuser.Status= user.First().Enabled.Value;
                _adminuser.Name = user.First().User_Name;
                var groupname = _authoritygroupsqlrepository.GetByWhere("ID=@1", new object[] { user.First().Group_ID });
                _adminuser.GroupId = user.First().Group_ID.Value;
                if (groupname.Count() == 0)
                {
                    _adminuser.GroupName = "";
                }
                else {
                    _adminuser.GroupName = groupname.First().Group_Name;
                }
            }
            return _adminuser;
        }

        public string ForgetPassword(string email)
        {
            return "密碼已寄出";
        }

    }
}
