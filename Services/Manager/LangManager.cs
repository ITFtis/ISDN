using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLModel.Models;
using System.Web.Mvc;
using ViewModels;
using SQLModel;
using Utilities;
using ViewModels.DBModels;

namespace Services.Manager
{
    public class LangManager : ILangManager
    {
        readonly SQLRepository<Lang> _sqlrepository;
        readonly SQLRepository<ModelLangMain> _modellangsqlrepository;
        readonly SQLRepository<Menu> _menusqlrepository;
        public LangManager(SQLRepositoryInstances sqlinstance)
        {
            _sqlrepository = sqlinstance.Lang;
            _modellangsqlrepository = sqlinstance.ModelLangMain;
            _menusqlrepository = sqlinstance.Menu;
        }

        #region Create
        public int Create(SiteLangModel model, string account)
        {
            var alldata = _sqlrepository.GetByWhere("Deleted=@1", new object[] { false });
            var maxsort = alldata.Max(v => v.Sort) + 1;
            var r = _sqlrepository.Create(new Lang()
            {
                Lang_Name = model.Lang_Name,
                Disp_Name = model.Disp_Name,
                Sub_Domain_Name = model.Sub_Domain_Name,
                Domain_Type = model.Domain_Type,
                Link_Lang_ID = model.Link_Lang_ID,
                Link_Href = model.Link_Href,
                UpdateDatetime = DateTime.Now,
                UpdateUser = account,
                Enabled = true,
                Deleted = false,
                Published = false,
                Sort = maxsort
            });
            return r;
        }
        #endregion

        #region Delete
        public string Delete(string[] idlist, string delaccount, string account, string username)
        {
            try
            {
                var r = 0;
                for (var idx = 0; idx < idlist.Length; idx++)
                {
                    var entity = new Lang();
                    entity.ID = int.Parse(idlist[idx]);
                    entity.Deleted = true;
                    r = _sqlrepository.Delete(entity);
                    if (r <= 0)
                    {
                        NLogManagement.SystemLogInfo("刪除知識庫單元失敗:ID=" + idlist[idx]);
                    }
                }
                var rstr = "";
                if (r >= 0)
                {
                    NLogManagement.SystemLogInfo("刪除多國語系失敗:" + delaccount);
                    rstr = "刪除成功";
                }
                else
                {
                    rstr = "刪除失敗";
                }
                var alldata = _sqlrepository.GetByWhere("Deleted=@1", new object[] { false }).OrderBy(v => v.Sort).ToArray();
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
                NLogManagement.SystemLogInfo("刪除多國語系失敗:" + ex.Message);
                return "刪除失敗";
            }
        }

        #endregion

        #region GetAll
        public IEnumerable<Lang> GetAll()
        {
            return _sqlrepository.GetAll();
        }
        #endregion

        #region GetLangDict
        public string GetLangOption()
        {
            var alllang = _sqlrepository.GetAll();
            var sb = new System.Text.StringBuilder();
            if (alllang == null) { return sb.ToString(); }
            if (alllang.Count() > 0)
            {
                alllang = alllang.Where(v => v.Enabled.Value == true && v.Deleted.Value != true && v.Published == true);
                foreach (var l in alllang)
                {
                    sb.Append("<li><a href='#' langid='" + l.ID.Value + "' class='langchange'>" + l.Disp_Name + "</a></li>");
                }
            }
            return sb.ToString();
        }
        #endregion

        #region GetSelectList
        public IList<SelectListItem> GetSelectList()
        {
            var list = _sqlrepository.GetByWhere("Enabled=@1 and Published=@2 and Deleted=@3", new object[] { true, true, false });
            IList<System.Web.Mvc.SelectListItem> item = new List<System.Web.Mvc.SelectListItem>();
            foreach (var l in list)
            {
                item.Add(new System.Web.Mvc.SelectListItem() { Text = l.Disp_Name, Value = l.ID.ToString() });
            }
            return item;
        }
        #endregion

        #region Paging
        public Paging<Lang> Paging(SearchModelBase model)
        {
            var Paging = new Paging<Lang>();
            var onepagecnt = model.Limit;
            var whereobj = new List<string>();
            var wherestr = new List<string>();
            wherestr.Add("Deleted=0");
            if (model.Search != "")
            {
                if (string.IsNullOrEmpty(model.Search) == false)
                {
                    wherestr.Add("Lang_Name like @1");
                    whereobj.Add("%" + model.Search + "%");
                }
            }
            var str = string.Join(" and ", wherestr);
            Paging.rows = _sqlrepository.GetDataUseWhereRange(str, whereobj.ToArray(), model.Offset, model.Limit, model.Sort);
            Paging.total = _sqlrepository.GetCountUseWhere(str, whereobj.ToArray());
            return Paging;
        }
        #endregion

        #region Update
        public int Update(SiteLangModel model, string account)
        {

            var r = _sqlrepository.Update(new Lang()
            {
                ID = model.ID,
                Lang_Name = model.Lang_Name,
                Disp_Name = model.Disp_Name,
                Sub_Domain_Name = model.Sub_Domain_Name,
                Domain_Type = model.Domain_Type,
                Link_Lang_ID = model.Link_Lang_ID,
                Link_Href = model.Link_Href,
                UpdateDatetime = DateTime.Now,
                UpdateUser = account
            });
            return r;
        }
        #endregion

        #region UpdateSeq
        public string UpdateSeq(int id, int seq, string account, string username)
        {
            try
            {
                var oldmodel = new List<Lang>();
                oldmodel = _sqlrepository.GetByWhere("ID=@1", new object[] { id }).ToList<Lang>();
                if (oldmodel.Count() == 0) { return "更新失敗"; }
                var langid = oldmodel.First().Lang_ID;
                var diff = "";
                diff = diff.TrimStart(',');
                var r = 0;
                if (oldmodel.First().Sort != seq)
                {
                    IList<Lang> mtseqdata = null;
                    mtseqdata = _sqlrepository.GetByWhere("Sort>@1", new object[] { seq }).OrderBy(v => v.Sort).ToArray();
                    var updatetime = DateTime.Now;
                    if (mtseqdata.Count() == 0)
                    {
                        var totalcnt = 0;
                        totalcnt = _sqlrepository.GetAllDataCount();
                        seq = totalcnt;
                    }

                    var qseq = (seq > oldmodel.First().Sort) ? seq : oldmodel.First().Sort;
                    IList<Lang> ltseqdata = null;
                    ltseqdata = _sqlrepository.GetByWhere("Sort<=@1", new object[] { qseq }).OrderBy(v => v.Sort).ToArray();
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
                        tempmodel.UpdateDatetime = updatetime;
                        tempmodel.UpdateUser = account;
                        _sqlrepository.Update(tempmodel);

                    }
                }
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
                NLogManagement.SystemLogInfo("更新多國語系設定排序失敗:" + " error:" + ex.Message);
                return "更新多國語系設定排序失敗:" + " error:" + ex.Message;
            }
        }
        #endregion

        #region SetPublish
        public string SetPublish(string id)
        {
            var r = _sqlrepository.Update("Published=@1", "ID=@2", new object[] { true, id });
            if (r > 0)
            {
                return "發佈成功";
            }
            else
            {
                return "發佈失敗";
            }
        }

        #endregion

        #region GetModelById
        public SiteLangModel GetModelById(string id)
        {
            var model = new SiteLangModel();
            var data = _sqlrepository.GetByWhere("ID=@1", new object[] { id });
            if (data.Count() > 0)
            {
                model.ID = data.First().ID.Value;
                model.Lang_Name = data.First().Lang_Name;
                model.Disp_Name = data.First().Disp_Name;
                model.Sub_Domain_Name = data.First().Sub_Domain_Name;
                model.Domain_Type = data.First().Domain_Type;
                model.Link_Lang_ID = data.First().Link_Lang_ID.Value;
                model.Link_Href = data.First().Link_Href;
            }
            return model;
        }
        #endregion

        //model
        #region PagingMain
        public Paging<ModelLangMain> PagingMain(SearchModelBase model)
        {
            var Paging = new Paging<ModelLangMain>();
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
            Paging.rows = _modellangsqlrepository.GetDataUseWhereRange(str, whereobj.ToArray(), model.Offset, model.Limit, model.Sort);
            Paging.total = _modellangsqlrepository.GetCountUseWhere(str, whereobj.ToArray());
            return Paging;
        }
        #endregion

        #region AddUnit
        public string AddUnit(string name, string langid, string account,ref int newid)
        {
            var alldata = _modellangsqlrepository.GetByWhere("Lang_ID=@1 Order By Sort", new object[] { langid });
            foreach (var mdata in alldata)
            {
                mdata.Sort = mdata.Sort + 1;
                _modellangsqlrepository.Update("Sort=@1", "ID=@2", new object[] { mdata.Sort, mdata.ID });
            }

            var Model = new ModelLangMain();
            Model.Lang_ID = int.Parse(langid);
            Model.ModelID = 6;
            Model.Name = name;
            Model.CreateDate = DateTime.Now;
            Model.UpdateDate = DateTime.Now;
            Model.CreateUser = account;
            Model.UpdateUser = account;
            Model.Sort = 1;
            Model.Status = true;
            var r = _modellangsqlrepository.Create(Model);
            newid = Model.ID;
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
            var r = _modellangsqlrepository.Update("Name=@1", "ID=@2", new object[] { name, id });
            if (r > 0) { return "修改成功"; } else { return "修改失敗"; }
        }
        #endregion

        #region UpdateMainSeq
        public string UpdateMainSeq(int id, int seq, string langid, string account, string username)
        {
            try
            {
                if (seq <= 0) { seq = 1; }
                var oldmodel = _modellangsqlrepository.GetByWhere("ID=@1", new object[] { id });
                if (oldmodel.Count() == 0) { return "更新失敗"; }
                var diff = "";

                diff = diff.TrimStart(',');
                var r = 0;
                if (oldmodel.First().Sort != seq)
                {
                    IList<ModelLangMain> mtseqdata = null;
                    mtseqdata = _modellangsqlrepository.GetByWhere("Sort>@1  and Lang_ID=@2", new object[] { seq, langid }).OrderBy(v => v.Sort).ToArray();

                    var updatetime = DateTime.Now;
                    if (mtseqdata.Count() == 0)
                    {
                        var totalcnt = 0;
                        totalcnt = _modellangsqlrepository.GetCountUseWhere("Lang_ID=@1", new object[] { langid });
                        seq = totalcnt;
                    }

                    var qseq = (seq > oldmodel.First().Sort) ? seq : oldmodel.First().Sort;
                    IList<ModelLangMain> ltseqdata = null;
                    ltseqdata = _modellangsqlrepository.GetByWhere("Sort<=@1  and Lang_ID=@2", new object[] { qseq, langid }).OrderBy(v => v.Sort).ToArray();
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
                        _modellangsqlrepository.Update(tempmodel);
                    }
                }
                //先取出大於目前seq的資料

                oldmodel.First().Sort = seq;
                r = _modellangsqlrepository.Update(oldmodel.First());
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

        #region MainDelete
        public string MainDelete(string[] idlist, string delaccount, string langid, string account, string username)
        {
            try
            {
                var r = 0;
                for (var idx = 0; idx < idlist.Length; idx++)
                {
                    var m = _menusqlrepository.GetByWhere("ModelID=6 and ModelItemID=@1", new object[] { idlist[idx] });
                    if (m.Count() > 0)
                    {
                        return "刪除失敗,刪除的項目使用中..";
                    }
                }


                for (var idx = 0; idx < idlist.Length; idx++)
                {
                    var entity = new ModelLangMain();
                    entity.ID = int.Parse(idlist[idx]);
                    r = _modellangsqlrepository.Delete(entity);
                    if (r <= 0)
                    {
                        NLogManagement.SystemLogInfo("刪除ModelLangMain失敗:ID=" + idlist[idx]);
                    }
                }
                var rstr = "";
                if (r >= 0)
                {
                    NLogManagement.SystemLogInfo("刪除ModelLangMain失敗:" + delaccount);
                    rstr = "刪除成功";
                }
                else
                {
                    rstr = "刪除失敗";
                }
                var alldata = _modellangsqlrepository.GetByWhere("Lang_ID=@1", new object[] { langid }).OrderBy(v => v.Sort).ToArray();
                for (var idx = 1; idx <= alldata.Count(); idx++)
                {
                    var tempmodel = alldata[idx - 1];
                    tempmodel.Sort = idx;
                    _modellangsqlrepository.Update(tempmodel);
                }

                return rstr;
            }
            catch (Exception ex)
            {
                NLogManagement.SystemLogInfo("刪除ModelLangMain失敗:" + ex.Message);
                return "刪除失敗";
            }
        }
        #endregion


        #region GetModelLangById
        public ModelLangMain GetModelLangById(string id)
        {
            var data = _modellangsqlrepository.GetByWhere(new ModelLangMain()
            {
                ID = int.Parse(id)
            });
            if (data.Count() == 0) { return new ModelLangMain(); } else {
                return data.First();
            }
        }
        #endregion

        #region ChangeLangType
        public void ChangeLangType(string mainid, string type, string langid)
        {
            _modellangsqlrepository.Update("UseType=@1 ,UseLangID=@2", "ID=@3", new object[] { type, langid, mainid });
        }
        #endregion

    }
}
