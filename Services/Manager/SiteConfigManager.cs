using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLModel.Models;
using System.Web;
using ViewModels;
using SQLModel;
using System.IO;
using AutoMapper;
using ViewModels.DBModels;
using System.Configuration;
using Utilities;

namespace Services.Manager
{
    public class SiteConfigManager : ISiteConfigManager
    {
        readonly SQLRepository<SiteConfig> _sqlrepository;
        readonly SQLRepository<SiteFlow> _siteflowsqlrepository;
        readonly SQLRepository<Img> _imgsqlrepository;
        readonly SQLRepository<AdminFunctionAuth> _functioninputsqlrepository;
        readonly SQLRepository<AdminFunction> _functionsqlrepository;
        readonly SQLRepository<Menu> _menusqlrepository;
        readonly SQLRepository<Users> _usersqlrepository;
        readonly SQLRepository<GroupUser> _groupsqlrepository;
        readonly SQLRepository<VerifyData> _verifydatasqlrepository;
        readonly SQLRepository<Users> _Usersqlrepository;
        SQLRepositoryInstances _sqlinstance;
        public SiteConfigManager(SQLRepositoryInstances sqlinstance)
        {
            _sqlinstance = sqlinstance;
               _sqlrepository = sqlinstance.SiteConfig;
            _imgsqlrepository = sqlinstance.Img;
            _functionsqlrepository = sqlinstance.AdminFunction;
            _functioninputsqlrepository = sqlinstance.AdminFunctionAuth;
            _menusqlrepository = sqlinstance.Menu;
            _usersqlrepository = sqlinstance.Users;
            _groupsqlrepository = sqlinstance.GroupUser;
            _siteflowsqlrepository = sqlinstance.SiteFlow;
            _verifydatasqlrepository = sqlinstance.VerifyData;
            _Usersqlrepository = sqlinstance.Users;
        }
        public IEnumerable<SiteConfig> GetAll()
        {
            return _sqlrepository.GetAll();
        }

        #region Save
        public string Save(SiteConfigModel model, System.Web.HttpPostedFileBase uploadfile, string updateuser)
        {
            model.LoginDescContent = HttpUtility.UrlDecode(model.LoginDescContent);
            if (uploadfile != null)
            {
                var root = HttpContext.Current.Request.PhysicalApplicationPath;
                model.LogoUploadFileNameOrg = model.LogoUploadFile.FileName.Split('\\').Last();
                var uploadfilepath = ConfigurationManager.AppSettings["UploadFile"];
                if (uploadfilepath.IsNullorEmpty())
                {
                    uploadfilepath = HttpContext.Current.Request.PhysicalApplicationPath + "\\UploadFile";
                }
                var newfilename = DateTime.Now.Ticks + "_" + model.LogoUploadFileNameOrg;
                var path = root + "\\UploadImage\\Config\\" + newfilename;
                if (System.IO.Directory.Exists(root + "\\UploadImage\\Config\\") == false)
                {
                    System.IO.Directory.CreateDirectory(root + "\\UploadImage\\Config\\");
                }
                model.LogoUploadFilePath = path;
                model.LogoUploadFile.SaveAs(path);
                model.LogoUploadFileName = newfilename;

            }
            if (model.ID >= 0)
            {
                var oldmodel = _sqlrepository.GetByWhere(new SiteConfig()
                {
                    ID = model.ID
                });
                if (oldmodel.Count() > 0)
                {
                    var oldroot = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\UploadImage\\Config\\";
                    //刪除舊檔案
                    if (model.LogoUploadFileName.IsNullorEmpty())
                    {
                        if (System.IO.File.Exists(oldmodel.First().LogoUploadFilePath))
                        {
                            System.IO.File.Delete(oldmodel.First().LogoUploadFilePath);
                        }
                        model.LogoUploadFilePath = "";
                        model.LogoUploadFileName = "";
                    }
                    else
                    {
                        if (oldmodel.First().LogoUploadFileName != model.LogoUploadFileName)
                        {
                            if (System.IO.File.Exists(oldmodel.First().LogoUploadFilePath))
                            {
                                System.IO.File.Delete(oldmodel.First().LogoUploadFilePath);
                            }
                        }
                    }
                    var savemodel = oldmodel.First();
                    savemodel.Comp_Name = model.Comp_Name;
                    savemodel.Login_Title = model.Login_Title;
                    savemodel.Page_Title = model.Page_Title;
                    savemodel.UpdateDatetime = DateTime.Now;
                    savemodel.UpdateUser = updateuser;
                    savemodel.LoginDescContent = model.LoginDescContent;
                    savemodel.LogoUploadFileName = model.LogoUploadFileName;
                    savemodel.LogoUploadFilePath = model.LogoUploadFilePath;
                    savemodel.LogoUploadFileNameOrg = model.LogoUploadFileNameOrg;
                    var r = _sqlrepository.Update(savemodel);
                    if (r < 0) { return "作業失敗"; }
                }
                else
                {
                    var r = _sqlrepository.Create(new SiteConfig()
                    {
                        Comp_Name = model.Comp_Name,
                        Login_Title = model.Login_Title,
                        Page_Title = model.Page_Title,
                        UpdateDatetime = DateTime.Now,
                        UpdateUser = updateuser,
                        LoginDescContent = model.LoginDescContent == null ? "" : model.LoginDescContent,
                        LogoUploadFileName = model.LogoUploadFileName,
                        LogoUploadFilePath = model.LogoUploadFilePath,
                        LogoUploadFileNameOrg = model.LogoUploadFileNameOrg,
                        Port = 25,
                        IsAuthMailServer = false
                    });
                    if (r < 0) { return "作業失敗"; }
                }
                // _sqlrepository.Update(model);
            }
            else
            {
                var r = _sqlrepository.Create(new SiteConfig()
                {
                    Comp_Name = model.Comp_Name,
                    Login_Title = model.Login_Title,
                    Page_Title = model.Page_Title,
                    UpdateDatetime = DateTime.Now,
                    UpdateUser = updateuser,
                     LoginDescContent = model.LoginDescContent==null?"" : model.LoginDescContent,
                    LogoUploadFileName = model.LogoUploadFileName,
                    LogoUploadFilePath = model.LogoUploadFilePath,
                    LogoUploadFileNameOrg = model.LogoUploadFileNameOrg,
                    Port = 25,
                    IsAuthMailServer = false
                });
                if (r < 0) { return "儲存失敗"; }
            }

            return "儲存成功";
        }
        #endregion

        #region Where
        public IEnumerable<SiteConfig> Where(SiteConfig model)
        {
            return _sqlrepository.GetByWhere(model);
        }
        #endregion

        #region GetAdminFunctionModel
        public AdminFunctionModel GetAdminFunctionModel(string groupid,string langid)
        {
            if (string.IsNullOrEmpty(groupid)) { groupid = "-1"; }
            var input = _functioninputsqlrepository.GetByWhere("GroupID=@1 and LangID=@2", new object[] { groupid, langid }).ToArray();
            var data = _functionsqlrepository.GetAll();
            var model = new AdminFunctionModel();
            model.GroupID = groupid;
            var groupdata = _groupsqlrepository.GetByWhere("ID=@1", new object[] { groupid });
            if (groupdata.Count() > 0) { model.GroupName = groupdata.First().Group_Name; }
            model.AdminFixFunctionInput = input.Where(v=>v.Type==0).ToList();
            model.AdminMenuFunctionInput = input.Where(v => v.Type ==1).ToList();
            if (data.Count() > 0)
            {
                var group = data.GroupBy(v => v.GroupID.Value);
                foreach (var g in group) {
                    model.AdminFunctionList.Add(g.Key, g.ToList());
                }
            }
            var menu = _menusqlrepository.GetByWhere("Status=@1 and LangID=@2", new object[] { true, langid });
            model.MenuList = menu.OrderBy(v=>v.MenuLevel).ThenBy(v=>v.Sort).ToList();
            model.UsersList = _usersqlrepository.GetByWhere("Group_ID=@1", new object[] { groupid }).ToList();
            return model;
        }
        #endregion

        #region GroupAuthSave
        public void GroupAuthSave(string languageID, string groupid, Dictionary<string, string> inputdata,string groupname,string account, Dictionary<string, string> oldlist)
        {
            var r = 0;
            if (groupid != "-1")
            {
                _groupsqlrepository.Update("Group_Name=@1", "ID=@2", new object[] { groupname, groupid });
            }
            else {
                var maxcount = _groupsqlrepository.GetDataCaculate("Max(Sort)");
                if (maxcount <= 0) { maxcount = 1; }
                var model = new GroupUser()
                {
                    Enabled = true,
                    Group_Name = groupname,
                    Readonly = false,
                    Seo_Manage = true,
                    Sort = maxcount + 1,
                    UpdateDatetime = DateTime.Now,
                    UpdateUser = account
                };
                _groupsqlrepository.Create(model);
                groupid = model.ID.ToString();
            }
            _functioninputsqlrepository.DelDataUseWhere("GroupID=@1 AND LangID=@2", new object[] { groupid,  languageID });
            var functiondata = _functionsqlrepository.GetAll();
            foreach (var key in inputdata.Keys) {
                var karr = key.Split('_');
                if (inputdata[key] == "true") {

                    var type = karr[0] == "fix" ? 0 : 1;
                    if (type == 0)
                    {
                        var fdata = functiondata.Where(v => v.ID == int.Parse(karr[1]));
                        _functioninputsqlrepository.Create(new AdminFunctionAuth()
                        {
                            GroupID = int.Parse(groupid),
                            ItemID = int.Parse(karr[1]),
                            LangID = int.Parse(languageID),
                            Type = type,
                            GID = fdata.Count() > 0 ? fdata.First().GroupID.Value : 0
                        });
                    }
                    else
                    {
                        _functioninputsqlrepository.Create(new AdminFunctionAuth()
                        {
                            GroupID = int.Parse(groupid),
                            ItemID = int.Parse(karr[1]),
                            LangID = int.Parse(languageID),
                            Type = type,
                            GID = 0
                        });
                    }
                }
            }
            if (oldlist != null) {
              
                foreach (var key in oldlist.Keys)
                {
                    if (key != languageID) {
                        _functioninputsqlrepository.DelDataUseWhere("GroupID=@1 AND LangID=@2", new object[] { groupid, key });
                        var karr = oldlist[key].Split('^');
                        foreach (var auth in karr) {
                            var autharr = auth.Split('_');
                            var type = autharr[0] == "fix" ? 0 : 1;
                            if (type == 0)
                            {
                                var fdata = functiondata.Where(v => v.ID == int.Parse(autharr[1]));
                                _functioninputsqlrepository.Create(new AdminFunctionAuth()
                                 {
                                     GroupID = int.Parse(groupid),
                                     ItemID = int.Parse(autharr[1]),
                                     LangID = int.Parse(key),
                                     Type = type,
                                      GID = fdata.Count()>0? fdata.First().GroupID.Value:0
                                 });
                            }
                            else
                            {
                                _functioninputsqlrepository.Create(new AdminFunctionAuth()
                                {
                                    GroupID = int.Parse(groupid),
                                    ItemID = int.Parse(autharr[1]),
                                    LangID = int.Parse(key),
                                    Type = type,
                                    GID =0
                                 });
                            }

                        }
                    }
                }
            }
        }
        #endregion

        #region GetSiteConfigModel
        public SiteConfigModel GetSiteConfigModel()
        {
            var model = _sqlrepository.GetAll();
            if (model.Count() > 0)
            {
                var config = new MapperConfiguration(cfg => cfg.CreateMap<SiteConfig, SiteConfigModel>());
                var mapper = config.CreateMapper();
                var cmodel = mapper.Map<SiteConfigModel>(model.First());
                return cmodel;
            }
            else
            {
                return new SiteConfigModel();
            }
        }
        #endregion

        #region GetSiteFlow
        public SiteFlow GetSiteFlow()
        {
            var SiteFlow = _siteflowsqlrepository.GetAll();
            if (SiteFlow.Count() > 0)
            {
                return SiteFlow.First();
            }
            else {
                return new SiteFlow();
            }
        }
        #endregion

        #region SaveMailServer
        public string SaveMailServer(SiteConfigModel model, string updateuser)
        {
            model.MailServerIP = model.MailServerIP == null ? "" : model.MailServerIP;
            model.EMailAccount = model.EMailAccount == null ? "" : model.EMailAccount;
            model.EMailPassword = model.EMailPassword == null ? "" : model.EMailPassword;
            if (model.ID >= 0)
            {
                
                var oldmodel = _sqlrepository.GetByWhere(new SiteConfig()
                {
                    ID = model.ID
                });
                if (oldmodel.Count() > 0)
                {  
                     var r = _sqlrepository.Update("MailServerIP=@1,Port=@2,EMailAccount=@3,EMailPassword=@4,IsAuthMailServer=@5", 
                         "ID=@6",new object[] { model.MailServerIP, model.Port, model.EMailAccount ,
                             model.EMailPassword, model.IsAuthMailServer,model.ID });
                    if (r < 0) { return "作業失敗"; }
                }
                else
                {
                    var r = _sqlrepository.Create(new SiteConfig()
                    {
                        Port = model.Port,
                        IsAuthMailServer = model.IsAuthMailServer,
                        MailServerIP = model.MailServerIP,
                        EMailAccount = model.EMailAccount,
                        EMailPassword = model.EMailPassword
                    });
                    if (r < 0) { return "作業失敗"; }
                }
            }
            else
            {
                var r = _sqlrepository.Create(new SiteConfig()
                {
                    Port = model.Port,
                    IsAuthMailServer = model.IsAuthMailServer,
                    MailServerIP= model.MailServerIP,
                     EMailAccount=model.EMailAccount,
                      EMailPassword=model.EMailPassword
                });
                if (r < 0) { return "儲存失敗"; }
            }

            return "儲存成功";
        }
        #endregion

        #region SaveQuestionnaireFinishDesc
        public string SaveQuestionnaireFinishDesc(int ID,string QuestionnaireFinishDesc, string updateuser)
        {
            if (ID >= 0)
            {
                var oldmodel = _sqlrepository.GetByWhere(new SiteConfig()
                {
                    ID = ID
                });
                if (oldmodel.Count() > 0)
                {
                    var r = _sqlrepository.Update("QuestionnaireFinishDesc=@1",
                        "ID=@2", new object[] { QuestionnaireFinishDesc, ID });
                    if (r < 0) { return "作業失敗"; }
                }
                else
                {
                    var r = _sqlrepository.Create(new SiteConfig()
                    {
                        QuestionnaireFinishDesc = QuestionnaireFinishDesc
                    });
                    if (r < 0) { return "作業失敗"; }
                }
            }
            else
            {
                var r = _sqlrepository.Create(new SiteConfig()
                {
                    QuestionnaireFinishDesc = QuestionnaireFinishDesc
                });
                if (r < 0) { return "儲存失敗"; }
            }

            return "儲存成功";
        }
        #endregion


        #region PagingVerify
        public Paging<VerifyDataResult> PagingVerify(SearchModelBase model)
        {
            var Paging = new Paging<VerifyDataResult>();
            var onepagecnt = model.Limit;
            var whereobj = new List<string>();
            var wherestr = new List<string>();
            wherestr.Add("LangID=@1 ");
            whereobj.Add(model.LangId);
            if (model.Search != "")
            {
                if (string.IsNullOrEmpty(model.Key) == false)
                {
                    wherestr.Add("VerifyStatus=@2");
                    whereobj.Add(model.Key);
                }
            }
            var str = string.Join(" and ", wherestr);
            var data = _verifydatasqlrepository.GetDataUseWhereRange(str, whereobj.ToArray(), model.Offset, model.Limit, model.Sort);
            Paging.total = _verifydatasqlrepository.GetCountUseWhere(str, whereobj.ToArray());
            var vbutton = "";
            foreach (var d in data)
            {
                var modelname = "";
                if (d.ModelID == 1) { modelname = "圖文編輯"; }
                else if (d.ModelID == 2) { modelname = "訊息模組"; }
                else if (d.ModelID == 3) { modelname = "活動管理"; }
                else if (d.ModelID == 4) { modelname = "文件下載"; }
                else if (d.ModelID == 5) { modelname = "網站導覽"; }
                else if (d.ModelID ==11) { modelname = "表單管理"; }
                else if (d.ModelID == 18) { modelname = "影音管理"; }
                else if (d.ModelID == 17) { modelname = "大事記要模組"; }
                else if (d.ModelID == 19) { modelname = "專利管理"; }
                if (string.IsNullOrEmpty(model.Key)|| model.Key=="0") {
                    vbutton = "<button class='btn blue verifyok' value='" + d.ModelID + "_" + d.ModelMainID + "_" + d.ModelItemID + "'>通過</button> " +
                    "<button class='btn grey-mint verifyrefuse' value='" + d.ModelID + "_" + d.ModelMainID + "_" + d.ModelItemID + "'>未通過</button>";
                }
                else if (model.Key=="1")
                {
                    vbutton = "<button class='btn grey-mint verifyrefuse' value='" + d.ModelID + "_" + d.ModelMainID + "_" + d.ModelItemID + "'>未通過</button>";
                }
                else if (model.Key == "2")
                {
                    //vbutton = "<button class='btn blue verifyok' value='" + d.ModelID + "_" + d.ModelMainID + "_" + d.ModelItemID + "'>通過</button> " ;
                    vbutton = "";
                }
                Paging.rows.Add(new VerifyDataResult()
                {
                  Name= modelname,
                   Status=d.VerifyStatus==0?"審核中" : (d.VerifyStatus == 1 ? "已通過" : "未通過"),
                   Title= d.ModelName,
                   UpdateStatus=d.ModelStatus==1?"新增":"修改",
                   UpdateTime=d.UpdateDateTime==null?"":d.UpdateDateTime.Value.ToString("yyyy/MM/dd HH:mm:ss"),
                   UpdateUser=d.UpdateUser,
                   TitleLink="<a href='#'  modid='"+ d .ModelID+"'  mainid='"+d.ModelMainID+"' itemid='"+d.ModelItemID+"' class='verifyview'>"+ d.ModelName +"</a>",
                    OptionHtml= vbutton
                });
            }
            return Paging;
        }

        #endregion

        #region SetVerifyOK
        public string SetVerifyOK(string id,string account)
        {
            if (string.IsNullOrEmpty(id)) { return "設定失敗,查無設定資料"; }
            var admin = _Usersqlrepository.GetByWhere("Account=@1", new object[] { account });
            var idarr = id.Split('_');
            if (idarr.Length<3) { return "設定失敗,資料來源不足"; }
            var r = _verifydatasqlrepository.Update("VerifyStatus=1,VerifyDateTime=@1,VerifyUser=@2,VerifyName=@3", "ModelID=@4 and ModelMainID=@5 and ModelItemID=@6"
                , new object[] { DateTime.Now, account, admin.Count() == 0 ? "" : admin.First().User_Name,idarr[0], idarr[1], idarr[2] });
            if (idarr[0] == "1") {
                r = _sqlinstance.PageIndexItem.Update("IsVerift=1", "ModelID=@1 and ItemID=@2", new object[] { idarr[1], idarr[2] });
            }
            else if (idarr[0] == "2")
            {
                r = _sqlinstance.MessageItem.Update("IsVerift=1", "ModelID=@1 and ItemID=@2", new object[] { idarr[1], idarr[2] });
            }
            else if (idarr[0] == "3")
            {
                r = _sqlinstance.ActiveItem.Update("IsVerift=1", "ModelID=@1 and ItemID=@2", new object[] { idarr[1], idarr[2] });
            }
            else if (idarr[0] == "4")
            {
                r = _sqlinstance.FileDownloadItem.Update("IsVerift=1", "ModelID=@1 and ItemID=@2", new object[] { idarr[1], idarr[2] });
            }
            else if (idarr[0] == "11")
            {
                r = _sqlinstance.ModelFormMain.Update("IsVerift=1", "ID=@1", new object[] { idarr[1]});
            }
            else if (idarr[0] == "17")
            {
                r = _sqlinstance.EventListItem.Update("IsVerift=1", "ModelID=@1 and ItemID=@2", new object[] { idarr[1], idarr[2] });
            }
            else if (idarr[0] == "7")
            {
                r = _sqlinstance.ArticleItem.Update("IsVerift=1", "ModelID=@1 and ItemID=@2", new object[] { idarr[1], idarr[2] });
            }
            return "設定完成";
        }
        #endregion

        #region SetVerifyRefuse
        public string SetVerifyRefuse(string id,string account)
        {
            if (string.IsNullOrEmpty(id)) { return "設定失敗,查無設定資料"; }
            var admin = _Usersqlrepository.GetByWhere("Account=@1", new object[] { account });
            var idarr = id.Split('_');
            if (idarr.Length < 3) { return "設定失敗,資料來源不足"; }
            var r = _verifydatasqlrepository.Update("VerifyStatus=2,VerifyDateTime=@1,VerifyUser=@2,VerifyName=@3", "ModelID=@4 and ModelMainID=@5 and ModelItemID=@6"
                , new object[] { DateTime.Now, account, admin.Count() == 0 ? "" : admin.First().User_Name,idarr[0], idarr[1], idarr[2]  });
            return "設定完成";
        }
        #endregion
        
    }
}
