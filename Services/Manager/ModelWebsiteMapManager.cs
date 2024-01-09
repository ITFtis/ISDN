using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLModel.Models;
using ViewModels;
using SQLModel;
using Utilities;
using System.Web.Mvc;
using System.Web;
using ViewModels.DBModels;

namespace Services.Manager
{
    public class ModelWebsiteMapManager : IModelWebsiteMapManager
    {
        readonly SQLRepository< ModelWebsiteMapMain> _sqlrepository;
        readonly SQLRepository<SEO> _seosqlrepository;
        readonly SQLRepository<Menu> _menusqlrepository;
        readonly SQLRepository<PageLayout> _pagesqlrepository;
        readonly SQLRepository<WebSiteMapInfo> _webSiteMapInfosqlrepository;
        readonly SQLRepository<Users> _Usersqlrepository;
        public ModelWebsiteMapManager(SQLRepositoryInstances sqlinstance)
        {
            _sqlrepository = sqlinstance.ModelWebsiteMapMain;
            _seosqlrepository= sqlinstance.SEO;
            _menusqlrepository = sqlinstance.Menu;
            _pagesqlrepository = sqlinstance.PageLayout;
            _webSiteMapInfosqlrepository = sqlinstance.WebSiteMapInfo;
            _Usersqlrepository = sqlinstance.Users;
        }

        #region GetAll
        public IEnumerable<ModelWebsiteMapMain> GetAll()
        {
            return _sqlrepository.GetAll();
        }

        #endregion

        #region Paging
        public Paging<ModelWebsiteMapMain> Paging(SearchModelBase model)
        {
            var Paging = new Paging<ModelWebsiteMapMain>();
            var onepagecnt = model.Limit;
            var whereobj = new List<string>();
            var wherestr = new List<string>();
            wherestr.Add("Lang_ID=@1");
            whereobj.Add(model.LangId);
            if (model.Search != "")
            {
                if (string.IsNullOrEmpty(model.Search) == false)
                {
                    wherestr.Add("Name like @2");
                    whereobj.Add("%" + model.Search + "%");
                }
            }
            var str = string.Join(" and ", wherestr);
            Paging.rows = _sqlrepository.GetDataUseWhereRange(str, whereobj.ToArray(), model.Offset, model.Limit, model.Sort);
            Paging.total = _sqlrepository.GetCountUseWhere(str, whereobj.ToArray());
            return Paging;
        }
        #endregion

        #region Where
        public IEnumerable<ModelWebsiteMapMain> Where(ModelWebsiteMapMain model)
        {
            return _sqlrepository.GetByWhere(model);
        }
        #endregion

        #region AddUnit
        public string AddUnit(string name, string langid, string account,ref int newid)
        {
            var alldata = _sqlrepository.GetByWhere("Lang_ID=@1 Order By Sort", new object[] { langid });
            var idx = 2;
            foreach (var mdata in alldata)
            {
                mdata.Sort = idx;
                _sqlrepository.Update("Sort=@1", "ID=@2", new object[] { mdata.Sort, mdata.ID });
                idx += 1;
            }
            var Model = new ModelWebsiteMapMain();
            Model.Lang_ID = int.Parse(langid);
            Model.ModelID = 5;
            Model.Name = name;
            Model.CreateDate = DateTime.Now;
            Model.UpdateDate = DateTime.Now;
            Model.CreateUser = account;
            Model.UpdateUser = account;
            Model.Sort = 1;
            Model.Status = true;
            var r = _sqlrepository.Create(Model);
            newid = Model.ID.Value;
            if (r > 0)
            {
                return "新增成功";
            }
            else
            {
                return "新增失敗";
            }

        }
        #endregion

        #region UpdateUnit
        public string UpdateUnit(string name, string id, string account)
        {
            var r = _sqlrepository.Update("Name=@1", "ID=@2", new object[] { name, id });
            if (r > 0) { return "修改成功"; } else { return "修改失敗"; }
        }
        #endregion

        #region UpdateSeq
        public string UpdateSeq(int id, int seq, string langid, string account, string username)
        {
            try
            {
                if (seq <= 0) { seq = 1; }
                var oldmodel = _sqlrepository.GetByWhere("ID=@1", new object[] { id });
                if (oldmodel.Count() == 0) { return "更新失敗"; }
                var diff = "";

                diff = diff.TrimStart(',');
                var r = 0;
                if (oldmodel.First().Sort != seq)
                {
                    IList<ModelWebsiteMapMain> mtseqdata = null;
                    mtseqdata = _sqlrepository.GetByWhere("Sort>@1   and Lang_ID=@2", new object[] { seq, langid }).OrderBy(v => v.Sort).ToArray();

                    var updatetime = DateTime.Now;
                    if (mtseqdata.Count() == 0)
                    {
                        var totalcnt = 0;
                        totalcnt = _sqlrepository.GetCountUseWhere("Lang_ID=@1", new object[] { langid });
                        seq = totalcnt;
                    }

                    var qseq = (seq > oldmodel.First().Sort) ? seq : oldmodel.First().Sort;
                    IList<ModelWebsiteMapMain> ltseqdata = null;
                    ltseqdata = _sqlrepository.GetByWhere("Sort<=@1   and Lang_ID=@2", new object[] { qseq, langid }).OrderBy(v => v.Sort).ToArray();
                    //更新seq+1
                    var sidx = 0;
                    for (var idx = 1; idx <= ltseqdata.Count(); idx++)
                    {
                        if (idx == seq && seq < oldmodel.First().Sort)
                        {
                            sidx += 1;
                        }
                        var tempmodel = ltseqdata[idx - 1];
                        if (tempmodel.ID == id)
                        {
                            continue;
                        }
                        else
                        {
                            sidx += 1;
                        }
                        tempmodel.Sort = sidx;
                        _sqlrepository.Update(tempmodel);
                    }
                }
                //先取出大於目前seq的資料

                oldmodel.First().Sort = seq;
                r = _sqlrepository.Update(oldmodel.First());
                if (r > 0)
                {
                    return "更新成功";
                }
                else
                {
                    return "更新失敗";
                }

            }
            catch (Exception ex)
            {
                NLogManagement.SystemLogInfo("更新ModelWebsiteMapMain排序失敗:" + " error:" + ex.Message);
                return "更新訊息管理排序失敗:" + " error:" + ex.Message;
            }
        }
        #endregion

        #region Delete
        public string Delete(string[] idlist, string delaccount, string langid, string account, string username)
        {
            try
            {
                var r = 0;
                for (var idx = 0; idx < idlist.Length; idx++)
                {
                    var m = _menusqlrepository.GetByWhere("ModelID=5 and ModelItemID=@1", new object[] { idlist[idx] });
                    if (m.Count() > 0)
                    {
                        return "刪除失敗,刪除的項目使用中..";
                    }
                    var p = _pagesqlrepository.GetByWhere("ModelID=5 and ModelItemID=@1", new object[] { idlist[idx] });
                    if (p.Count() > 0)
                    {
                        return "刪除失敗,刪除的項目使用中..";
                    }
                }
                for (var idx = 0; idx < idlist.Length; idx++)
                {
                    var entity = new ModelWebsiteMapMain();
                    entity.ID = int.Parse(idlist[idx]);
                    r = _sqlrepository.Delete(entity);
                    if (r <= 0)
                    {
                        NLogManagement.SystemLogInfo("刪除ModelWebsiteMapMain失敗:ID=" + idlist[idx]);
                    }
                    else {
                        _webSiteMapInfosqlrepository.DelDataUseWhere("MainID=@1", new object[] { idlist[idx] });

                    }
                }
                var rstr = "";
                if (r >= 0)
                {
                    NLogManagement.SystemLogInfo("刪除ModelWebsiteMapMain失敗:" + delaccount);
                    rstr = "刪除成功";
                }
                else
                {
                    rstr = "刪除失敗";
                }
                var alldata = _sqlrepository.GetByWhere("Lang_ID=@1", new object[] { langid }).OrderBy(v => v.Sort).ToArray();
                for (var idx = 1; idx <= alldata.Count(); idx++)
                {
                    var tempmodel = alldata[idx - 1];
                    tempmodel.Sort = idx;
                    _sqlrepository.Update(tempmodel);
                }

                return rstr;
            }
            catch (Exception ex)
            {
                NLogManagement.SystemLogInfo("刪除ModelWebsiteMapMain失敗:" + ex.Message);
                return "刪除失敗";
            }
        }
        #endregion

        #region GetSEO
        public SEOViewModel GetSEO(string mainid)
        {
            var seodata = _seosqlrepository.GetByWhere("TypeName=@1 and TypeID=@2", new object[] { "WebsiteMap", mainid });
            var model = new SEOViewModel();
            if (seodata.Count() > 0)
            {
                model.ID = seodata.First().ID;
                model.TypeID = seodata.First().TypeID.ToString();
                model.Description = seodata.First().Description;
                model.WebsiteTitle = seodata.First().Title;
                model.Keywords = seodata.Count() == 0 ? new string[10] : new string[] {
                        seodata.First().Keywords1,seodata.First().Keywords2,seodata.First().Keywords3,seodata.First().Keywords4,seodata.First().Keywords5
                    ,seodata.First().Keywords6,seodata.First().Keywords7,seodata.First().Keywords8,seodata.First().Keywords9,seodata.First().Keywords10};
            }
            else
            {
                model.Keywords = new string[10];
                model.TypeID = mainid;
            }
            return model;
        }
        #endregion

        #region SaveUnit
        public string SaveUnit(SEOViewModel model, string LangID, Dictionary<string, string> Column)
        {
            var columnstr = Newtonsoft.Json.JsonConvert.SerializeObject(Column);
            var r = 0;
             r=  _sqlrepository.Update("ColumnDict=@1", "ID=@2", new object[] { columnstr, model.TypeID });
           var seomodel = new SEO()
            {
                Description = model.Description == null ? "" : model.Description,
                Keywords1 = model.Keywords[0],
                Keywords2 = model.Keywords[1],
                Keywords3 = model.Keywords[2],
                Keywords4 = model.Keywords[3],
                Keywords5 = model.Keywords[4],
                Keywords6 = model.Keywords[5],
                Keywords7 = model.Keywords[6],
                Keywords8 = model.Keywords[7],
                Keywords9 = model.Keywords[8],
                Keywords10 = model.Keywords[9],
                Title = model.WebsiteTitle == null ? "" : model.WebsiteTitle,
                TypeName = "WebsiteMap",
                TypeID = int.Parse(model.TypeID),
                Lang_ID = int.Parse(LangID)
            };
        
            if (model.ID == -1)
            {
                r = _seosqlrepository.Create(seomodel);
            }
            else
            {
                seomodel.ID = model.ID;
                r = _seosqlrepository.Update(seomodel);
            }
            if (r > 0)
            {
                return "儲存成功";
            }
            else
            {
                return "儲存失敗";
            }
        }

        #endregion

        #region GetModelByID
        public WebSiteEditModel GetModelByID(string mainid)
        {
            var data = _webSiteMapInfosqlrepository.GetByWhere("MainID=@1", new object[] { mainid });
            var maindata = _sqlrepository.GetByWhere("ID=@1", new object[] { mainid });
            if (data.Count() > 0)
            {
                var model = new WebSiteEditModel()
                {
                    MainID = data.First().MainID.Value,
                     HtmlContent=data.First().HtmlContent
                };
                if (data.First().HotKey.IsNullorEmpty() == false) {
                    model.HotKey = data.First().HotKey.Split(':');
                    model.AreaName = data.First().AreaName.Split(':');
                    model.Intro = data.First().Intro.Split(':');
                }
                model.ModelName = maindata.Count() == 0 ? "" : maindata.First().Name;
                return model;
            }
            else {
                return new WebSiteEditModel() {
                    ModelName = maindata.Count() == 0 ? "" : maindata.First().Name,
                MainID =int.Parse(mainid),
                     AreaName=new string[0],
                      HotKey=new string[0],
                       Intro=new string[0],
                        HtmlContent=""
                };
            }
        }
        #endregion

        #region SaveInfo
        public string SaveInfo(WebSiteEditModel model,string account)
        {
            var savemodel = new WebSiteMapInfo();
            savemodel.HtmlContent = model.HtmlContent;
            savemodel.MainID = model.MainID;
            var admin = _Usersqlrepository.GetByWhere("Account=@1", new object[] { account });
            if (model.AreaName != null) {
                savemodel.HotKey = string.Join(":",model.HotKey);
                savemodel.AreaName = string.Join(":", model.AreaName);
                savemodel.Intro = string.Join(":", model.Intro);
            }
            _webSiteMapInfosqlrepository.DelDataUseWhere("MainID=@1", new object[] { savemodel.MainID });
            savemodel.LangID = model.LangID;
            savemodel.UpdateDatetime = DateTime.Now;
            savemodel.UpdateUser = account;
            savemodel.UpdateName = admin.Count() == 0 ? "" : admin.First().User_Name;

            var r=_webSiteMapInfosqlrepository.Create(savemodel);
            if (r > 0)
            {
                return "設定完成";
            }
            else {
                return "設定失敗";
            }
        } 
        #endregion
    }
}
