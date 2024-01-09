using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using SQLModel.Models;
using SQLModel;
using ViewModels;
using Utilities;
using ViewModel;
using System.Web;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.HPSF;
using System.Net.Mail;
using System.Threading;
using System.Text;
using ViewModels.DBModels;
using System.Web.Mvc;
using System.Net;

namespace Services.Manager
{
    public class ModelFormManager : IModelFormManager
    {
        readonly SQLRepository<ModelFormMain> _sqlrepository;
        readonly SQLRepository<FormUnitSetting> _unitsqlitemrepository;
        readonly SQLRepository<FormSelItem> _selitemsqlitemrepository;
        readonly SQLRepository<SEO> _seosqlrepository;
        readonly SQLRepository<FormSetting> _formsettingsqlrepository;
        readonly SQLRepository<FormInput> _inputsettingsqlrepository;
        readonly SQLRepository<FormInputNote> _inputnotesqlrepository;
        readonly SQLRepository<Users> _Usersqlrepository;
        ILoginManager _ILoginManager;
        readonly SQLRepository<Menu> _menusqlrepository;
        readonly SQLRepository<PageLayout> _pagesqlrepository;
        readonly SQLRepository<VerifyData> _verifydatasqlrepository;
        readonly SQLRepository<SiteConfig> _siteconfigqlrepository;
        public ModelFormManager(SQLRepositoryInstances sqlinstance)
        {
            _sqlrepository = sqlinstance.ModelFormMain;
            _unitsqlitemrepository = sqlinstance.FormUnitSetting;
            _selitemsqlitemrepository = sqlinstance.FormSelItem;
            _seosqlrepository = sqlinstance.SEO ;
            _formsettingsqlrepository = sqlinstance.FormSetting;
            _ILoginManager = new LoginManager(sqlinstance);
            _inputsettingsqlrepository = sqlinstance.FormInput;
            _menusqlrepository = sqlinstance.Menu;
            _pagesqlrepository = sqlinstance.PageLayout;
            _inputnotesqlrepository = sqlinstance.FormInputNote;
            _Usersqlrepository= sqlinstance.Users;
            _verifydatasqlrepository = sqlinstance.VerifyData;
            _siteconfigqlrepository= sqlinstance.SiteConfig;
        }

        #region GetAll
        public IEnumerable<ModelFormMain> GetAll()
        {
            return _sqlrepository.GetAll();
        }

        #endregion

        #region Paging
        public Paging<ModelFormMain> Paging(SearchModelBase model)
        {
            var Paging = new Paging<ModelFormMain>();
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
        public IEnumerable<ModelFormMain> Where(ModelFormMain model)
        {
            return _sqlrepository.GetByWhere(model);
        }
        #endregion

        #region AddUnit
        public string AddUnit(string name, string langid, string account, ref int newid)
        {
            var alldata = _sqlrepository.GetByWhere("Lang_ID=@1 Order By Sort", new object[] { langid });
            var idx = 2;
            foreach (var mdata in alldata)
            {
                mdata.Sort = idx;
                _sqlrepository.Update("Sort=@1", "ID=@2", new object[] { mdata.Sort, mdata.ID });
                idx += 1;
            }
            var Model = new ModelFormMain();
            Model.Lang_ID = int.Parse(langid);
            Model.ModelID = 11;
            Model.Name = name;
            Model.CreateDate = DateTime.Now;
            Model.UpdateDate = DateTime.Now;
            Model.CreateUser = account;
            Model.UpdateUser = account;
            Model.Sort = 1;
            Model.Status = true;
            //Model.IsVerift = false;
            Model.IsVerift = true;
            var admin = _Usersqlrepository.GetByWhere("Account=@1", new object[] { account });
            Model.CreateName = admin.Count() == 0 ? "" : admin.First().User_Name;
            var r = _sqlrepository.Create(Model);
            //_verifydatasqlrepository.DelDataUseWhere("ModelID=@1 and ModelMainID=@2", new object[] { 11, Model.ID });
            //_verifydatasqlrepository.Create(new VerifyData()
            //{
            //    ModelID = 11,
            //    ModelItemID = Model.ID,
            //    ModelName = name,
            //    ModelMainID = Model.ID,
            //    VerifyStatus = 0,
            //    ModelStatus = 1,
            //    UpdateDateTime = DateTime.Now,
            //    UpdateUser = admin.Count() == 0 ? "" : admin.First().User_Name,
            //    UpdateAccount = account,
            //    LangID = Model.Lang_ID.Value
            //});
            newid = Model.ID;
            //新增name和mail
            _selitemsqlitemrepository.Create(new FormSelItem()
            {
                DefaultText = "",
                Description = "",
                ItemID = Model.ID,
                ItemMode = 1,
                KayName = "Name",
                MustInput = true,
                Status = true,
                Title = "姓名",
                ColumnNum = 50,
                TextLength = 50,
                Sort = 1
            });

            _selitemsqlitemrepository.Create(new FormSelItem()
            {
                DefaultText = "",
                Description = "",
                ItemID = Model.ID,
                ItemMode = 1,
                KayName = "EMail",
                MustInput = true,
                Status = true,
                Title = "EMail",
                ColumnNum = 50,
                TextLength = 50,
                Sort = 2
            });

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
            var admin = _Usersqlrepository.GetByWhere("Account=@1", new object[] { account });
            var uname= admin.Count() == 0 ? "" : admin.First().User_Name;
            var olddata = _sqlrepository.GetByWhere("ID=@1", new object[] { id });
            var r = _sqlrepository.Update("Name=@1,IsVerift=@2,UpdateName=@3", "ID=@4", new object[] { name,false, uname, id });
            //var hasvdata = _verifydatasqlrepository.GetByWhere("ModelID=@1 and ModelMainID=@2", new object[] {
            //       11, id
            //    });
            //if (hasvdata.Count() == 0)
            //{
            //    _verifydatasqlrepository.Create(new VerifyData()
            //    {
            //        ModelID = 11,
            //        ModelItemID =int.Parse( id),
            //        ModelName = name,
            //        ModelMainID = int.Parse(id),
            //        VerifyStatus = 0,
            //        ModelStatus = 2,
            //        UpdateDateTime = DateTime.Now,
            //        UpdateUser = admin.Count() == 0 ? "" : admin.First().User_Name,
            //        UpdateAccount = account,
            //        LangID = olddata.First().Lang_ID.Value
            //    });
            //}
            //else
            //{
            //var r2=    _verifydatasqlrepository.Update("VerifyStatus=0,ModelStatus=2,VerifyDateTime=Null,VerifyUser='',VerifyName='',ModelName=@1,UpdateDateTime=@2,UpdateUser=@3,UpdateAccount=@4",
            //        "ModelID=@5 and ModelMainID=@6", new object[] {
            //               name,DateTime.Now, (admin.Count() == 0 ? "" : admin.First().User_Name),account,11 , id
            //    });
            //}

            if (r > 0) { return "修改成功"; } else { return "修改失敗"; }
        }
        #endregion

        #region UpdateSeq
        public string UpdateSeq(int id, int seq, string langid, string account, string username)
        {
            try
            {
                if (seq <= 0) { seq = 1; }
                var oldmodel = _sqlrepository.GetByWhere("ID=@1 ", new object[] { id });
                if (oldmodel.Count() == 0) { return "更新失敗"; }
                var diff = "";

                diff = diff.TrimStart(',');
                var r = 0;
                if (oldmodel.First().Sort != seq)
                {
                    IList<ModelFormMain> mtseqdata = null;
                    mtseqdata = _sqlrepository.GetByWhere("Sort>@1   and Lang_ID=@2 ", new object[] { seq, langid }).OrderBy(v => v.Sort).ToArray();

                    var updatetime = DateTime.Now;
                    if (mtseqdata.Count() == 0)
                    {
                        var totalcnt = 0;
                        totalcnt = _sqlrepository.GetCountUseWhere("Lang_ID=@1", new object[] { langid });
                        seq = totalcnt;
                    }

                    var qseq = (seq > oldmodel.First().Sort) ? seq : oldmodel.First().Sort;
                    IList<ModelFormMain> ltseqdata = null;
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
                NLogManagement.SystemLogInfo("更新ModelFormMain排序失敗:" + " error:" + ex.Message);
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
                    var m = _menusqlrepository.GetByWhere("ModelID=11 and ModelItemID=@1", new object[] { idlist[idx] });
                    if (m.Count() > 0)
                    {
                        return "刪除失敗,刪除的項目使用中..";
                    }
                    var p = _pagesqlrepository.GetByWhere("ModelID=11 and ModelItemID=@1", new object[] { idlist[idx] });
                    if (p.Count() > 0)
                    {
                        return "刪除失敗,刪除的項目使用中..";
                    }
                }


                var oldroot = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\UploadImage\\ArticleItem\\";
                for (var idx = 0; idx < idlist.Length; idx++)
                {
                    var entity = new ModelFormMain();
                    entity.ID = int.Parse(idlist[idx]);
                    r = _sqlrepository.Delete(entity);
                    if (r <= 0)
                    {
                        NLogManagement.SystemLogInfo("刪除ModelFormMain失敗:ID=" + idlist[idx]);
                    }
                    else
                    {
                        r = _verifydatasqlrepository.DelDataUseWhere("ModelID=@1 and ModelMainID=@2", new object[] { 11, entity.ID });
                        r = _selitemsqlitemrepository.DelDataUseWhere("ItemID=@1", new object[] { entity.ID });
                        r = _formsettingsqlrepository.DelDataUseWhere("ItemID=@1", new object[] { entity.ID });
                        r = _unitsqlitemrepository.DelDataUseWhere("MainID=@1", new object[] { entity.ID });
                        r = _inputsettingsqlrepository.DelDataUseWhere("MainID=@1", new object[] { entity.ID });
                        r = _inputnotesqlrepository.DelDataUseWhere("InputID=@1", new object[] { entity.ID });
                        r = _unitsqlitemrepository.DelDataUseWhere("MainID=@1", new object[] { entity.ID });
                        r = _seosqlrepository.DelDataUseWhere("TypeName='Form' and TypeID=@1", new object[] { entity.ID });
                    }
                }
                var rstr = "";
                if (r >= 0)
                {
                    NLogManagement.SystemLogInfo("刪除ModelFormMain失敗:" + delaccount);
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
                NLogManagement.SystemLogInfo("刪除ModelFormMain失敗:" + ex.Message);
                return "刪除失敗";
            }
        }
        #endregion

        //unit
        #region GetUnitModel
        public FormUnitSettingModel GetUnitModel(string mainid)
        {
            var data = _unitsqlitemrepository.GetByWhere("MainID=@1", new object[] { mainid });
            if (data.Count() > 0)
            {
                return new FormUnitSettingModel()
                {
                    ClassOverview = data.First().ClassOverview,
                    Column1 = data.First().Column1,
                    Column2 = data.First().Column2,
                    Column3 = data.First().Column3,
                    Column4 = data.First().Column4,
                    Column5 = data.First().Column5,
                    Column6 = data.First().Column6,
                    Column7 = data.First().Column7,
                    Column8 = data.First().Column8,
                    Column9 = data.First().Column9,
                    Column10 = data.First().Column10,
                    Column11 = data.First().Column11,
                    Column12 = data.First().Column12,
                    Column13 = data.First().Column13,
                    Column14 = data.First().Column14,
                    Column15 = data.First().Column15,
                    Column16 = data.First().Column16,
                    Column17 = data.First().Column17,
                    Column18 = data.First().Column18,
                    Column19 = data.First().Column19,
                    Column20 = data.First().Column20,
                    IsForward = data.First().IsForward,
                    IsPrint = data.First().IsPrint,
                    IsRSS = data.First().IsRSS,
                    IsShare = data.First().IsShare,
                    MemberAuth = data.First().MemberAuth,
                    MainID = data.First().MainID,
                    ShowCount = data.First().ShowCount,
                    ID = data.First().ID,
                    EMailAuth = data.First().EMailAuth,
                    VIPAuth = data.First().VIPAuth,
                    EnterpriceStudentAuth = data.First().EnterpriceStudentAuth,
                    GeneralStudentAuth = data.First().GeneralStudentAuth
                };
            }
            var model = new FormUnitSettingModel();
            model.MainID = int.Parse(mainid);
            return model;
        }
        #endregion

        #region SetUnitModel
        public string SetUnitModel(FormUnitSettingModel model, string account)
        {
            var newmodel = new FormUnitSetting();
            newmodel.UpdateDatetime = DateTime.Now;
            newmodel.UpdateUser = account;
            var r = 0;
            if (model.ID == -1)
            {
                newmodel.MainID = model.MainID.Value;
                newmodel.ClassOverview = model.ClassOverview;
                newmodel.Column1 = model.Column1;
                newmodel.Column2 = model.Column2;
                newmodel.Column3 = model.Column3;
                newmodel.Column4 = model.Column4;
                newmodel.Column5 = model.Column5;
                newmodel.Column6 = model.Column6;
                newmodel.Column7 = model.Column7;
                newmodel.Column8 = model.Column8;
                newmodel.Column9 = model.Column9;
                newmodel.Column10 = model.Column10;
                newmodel.Column11 = model.Column11;
                newmodel.Column12 = model.Column12;
                newmodel.Column13 = model.Column13;
                newmodel.Column14 = model.Column14;
                newmodel.Column15 = model.Column15;
                newmodel.Column16 = model.Column16;
                newmodel.Column17 = model.Column17;
                newmodel.Column18 = model.Column18;
                newmodel.Column19 = model.Column19;
                newmodel.Column20 = model.Column20;
                newmodel.IsRSS = model.IsRSS;
                newmodel.IsShare = model.IsShare;
                newmodel.IsPrint = model.IsPrint;
                newmodel.IsForward = model.IsForward;
                newmodel.MemberAuth = model.MemberAuth;
                newmodel.ShowCount = model.ShowCount;
                newmodel.VIPAuth = model.VIPAuth;
                newmodel.EMailAuth = model.EMailAuth;
                newmodel.EnterpriceStudentAuth = model.EnterpriceStudentAuth;
                newmodel.GeneralStudentAuth = model.GeneralStudentAuth;
                r = _unitsqlitemrepository.Create(newmodel);
            }
            else
            {
                newmodel.ID = model.ID.Value;
                newmodel.MainID = model.MainID.Value;
                newmodel.ClassOverview = model.ClassOverview;
                newmodel.Column1 = model.Column1 == null ? "" : model.Column1;
                newmodel.Column2 = model.Column2 == null ? "" : model.Column2;
                newmodel.Column3 = model.Column3 == null ? "" : model.Column3;
                newmodel.Column4 = model.Column4 == null ? "" : model.Column4;
                newmodel.Column5 = model.Column5 == null ? "" : model.Column5;
                newmodel.Column6 = model.Column6 == null ? "" : model.Column6;
                newmodel.Column7 = model.Column7 == null ? "" : model.Column7;
                newmodel.Column8 = model.Column8 == null ? "" : model.Column8;
                newmodel.Column9 = model.Column9 == null ? "" : model.Column9;
                newmodel.Column10 = model.Column10 == null ? "" : model.Column10;
                newmodel.Column11 = model.Column11 == null ? "" : model.Column11;
                newmodel.Column12 = model.Column12 == null ? "" : model.Column12;
                newmodel.Column13 = model.Column13 == null ? "" : model.Column13;
                newmodel.Column14 = model.Column14 == null ? "" : model.Column14;
                newmodel.Column15 = model.Column15 == null ? "" : model.Column15;
                newmodel.Column16 = model.Column16 == null ? "" : model.Column16;
                newmodel.Column17 = model.Column17 == null ? "" : model.Column17;
                newmodel.Column18 = model.Column18 == null ? "" : model.Column18;
                newmodel.Column19 = model.Column19 == null ? "" : model.Column19;
                newmodel.Column20 = model.Column20 == null ? "" : model.Column20;
                newmodel.IsRSS = model.IsRSS;
                newmodel.IsShare = model.IsShare;
                newmodel.IsPrint = model.IsPrint;
                newmodel.IsForward = model.IsForward;
                newmodel.MemberAuth = model.MemberAuth;
                newmodel.ShowCount = model.ShowCount;
                newmodel.VIPAuth = model.VIPAuth;
                newmodel.EMailAuth = model.EMailAuth;
                newmodel.EnterpriceStudentAuth = model.EnterpriceStudentAuth;
                newmodel.GeneralStudentAuth = model.GeneralStudentAuth;
                r = _unitsqlitemrepository.Update(newmodel);
            }
            if (r > 0)
            {
                return "修改成功";
            }
            else
            {
                return "修改失敗";
            }

        }


        #endregion

        #region EditSelItem
        public string EditSelItem(FormItemSettingModel model)
        {
            var alldata = _selitemsqlitemrepository.GetByWhere("ItemID=@1", new object[] { model.MainID });
            var maxsort = _selitemsqlitemrepository.GetDataCaculate("Max(Sort)", "ItemID=@1", new object[] { model.MainID });
            if (model.ID < 0)
            {
                var savemodel = new FormSelItem()
                {
                    ItemID = model.MainID,
                    DefaultText = model.DefaultText,
                    SelList = model.SelList,
                    Description = model.Description == null ? "" : model.Description,
                    MustInput = true,
                    Sort = maxsort + 1,
                    Status = true,
                    Title = model.Title,
                    ItemMode = model.ItemMode
                };
                if (model.ColumnNum != null)
                {
                    savemodel.ColumnNum = int.Parse(model.ColumnNum);
                }
                if (model.TextLength != null)
                {
                    savemodel.TextLength = int.Parse(model.TextLength);
                }
                if (model.RowNum != null)
                {
                    savemodel.RowNum = int.Parse(model.RowNum);
                }
                var r = _selitemsqlitemrepository.Create(savemodel);

                if (r > 0) {
                    _siteconfigqlrepository.Update("LastUpdateDate=@1", "", new object[] { DateTime.Now.ToString("yyyy/MM/dd") });
                    return "新增成功"; }
                else
                {
                    return "新增失敗";
                }
            }
            else
            {
                var olddata = _selitemsqlitemrepository.GetByWhere("ID=@1", new object[] { model.ID });
                olddata.First().DefaultText = model.DefaultText == null ? "" : model.DefaultText;
                olddata.First().SelList = model.SelList == null ? "" : model.SelList;
                olddata.First().Description = model.Description == null ? "" : model.Description;
                olddata.First().Title = model.Title == null ? "" : model.Title;
                if (model.ColumnNum != null)
                {
                    olddata.First().ColumnNum = int.Parse(model.ColumnNum);
                }
                if (model.TextLength != null)
                {
                    olddata.First().TextLength = int.Parse(model.TextLength);
                }
                if (model.RowNum != null)
                {
                    olddata.First().RowNum = int.Parse(model.RowNum);
                }
                var r = _selitemsqlitemrepository.Update(olddata.First());

                if (r > 0) {
                    _siteconfigqlrepository.Update("LastUpdateDate=@1", "", new object[] { DateTime.Now.ToString("yyyy/MM/dd") });
                    return "修改成功"; }
                else
                {
                    return "修改失敗";
                }
            }
        }

        #endregion

        #region GetSelItemByID
        public FormItemSettingModel GetSelItemByID(string id)
        {
            var data = _selitemsqlitemrepository.GetByWhere("ID=@1", new object[] { id });
            if (data.Count() > 0)
            {
                var model = new FormItemSettingModel()
                {
                    ID = data.First().ID,
                    Title = data.First().Title,
                    ColumnNum = data.First().ColumnNum == null ? "" : data.First().ColumnNum.ToString(),
                    DefaultText = data.First().DefaultText,
                    Description = data.First().Description == null ? "" : data.First().Description,
                    ItemMode = data.First().ItemMode.Value,
                    MainID = data.First().ItemID,
                    SelList = data.First().SelList,
                    TextLength = data.First().TextLength == null ? "" : data.First().TextLength.ToString(),
                    RowNum = data.First().RowNum == null ? "" : data.First().RowNum.ToString()
                };
                if (model.ItemMode == 1) { model.ItemModeName = "單行輸入"; }
                if (model.ItemMode == 2) { model.ItemModeName = "多行輸入"; }
                if (model.ItemMode == 3) { model.ItemModeName = "單選"; }
                if (model.ItemMode == 4) { model.ItemModeName = "複選"; }
                if (model.ItemMode == 5) { model.ItemModeName = "下拉選單"; }
                return model;
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region PagingSelItem
        public Paging<FormSelItem> PagingSelItem(SearchModelBase model)
        {
            var Paging = new Paging<FormSelItem>();
            var onepagecnt = model.Limit;
            var whereobj = new List<string>();
            var wherestr = new List<string>();
            wherestr.Add("ItemID=@1");
            whereobj.Add(model.ModelID.ToString());
            if (model.Search != "")
            {
                if (string.IsNullOrEmpty(model.Search) == false)
                {
                    wherestr.Add("Name like @2");
                    whereobj.Add("%" + model.Search + "%");
                }
            }
            var str = string.Join(" and ", wherestr);
            Paging.rows = _selitemsqlitemrepository.GetDataUseWhereRange(str, whereobj.ToArray(), model.Offset, model.Limit, model.Sort);
            Paging.total = _selitemsqlitemrepository.GetCountUseWhere(str, whereobj.ToArray());
            return Paging;
        }
        #endregion

        #region SetItemIsMust
        public string SetItemIsMust(string id, bool status, string account, string username)
        {
            try
            {
                var entity = new FormSelItem();
                entity.MustInput = status ? true : false;
                entity.ID = int.Parse(id);
                var r = _selitemsqlitemrepository.Update(entity);
                if (r >= 0)
                {
                    NLogManagement.SystemLogInfo("修改FormSelItem顯示狀態 id=" + id + " 為:" + status);
                    return "更新成功";
                }
                else
                {
                    return "更新失敗";
                }

            }
            catch (Exception ex)
            {
                NLogManagement.SystemLogInfo("修改FormSelItem顯示狀態失敗:" + ex.Message);
                return "更新失敗";
            }
        }
        #endregion

        #region DeleteItem
        public string DeleteItem(string[] idlist, string delaccount, string account, string username)
        {
            try
            {
                var r = 0;
                var itemid = 0;
                for (var idx = 0; idx < idlist.Length; idx++)
                {
                    var entity = new FormSelItem();
                    var olditem = _selitemsqlitemrepository.GetByWhere("ID=@1", new object[] { idlist[idx] });
                    entity.ID = int.Parse(idlist[idx]);
                    if (idx == 0)
                    {
                        itemid = olditem.First().ItemID.Value;
                    }
                    r = _selitemsqlitemrepository.Delete(entity);
                    if (r <= 0)
                    {
                        NLogManagement.SystemLogInfo("刪除FormSelItem失敗:ID=" + idlist[idx]);
                    }
                }
                var rstr = "";
                if (r >= 0)
                {
                    NLogManagement.SystemLogInfo("刪除FormSelItem:" + delaccount);
                    rstr = "刪除成功";
                }
                else
                {
                    rstr = "刪除失敗";
                }
                var alldata = _selitemsqlitemrepository.GetByWhere("ItemID=@1", new object[] { itemid }).OrderBy(v => v.Sort).ToArray();
                for (var idx = 1; idx <= alldata.Count(); idx++)
                {
                    var tempmodel = alldata[idx - 1];
                    _selitemsqlitemrepository.Update("Sort=@1", "ID=@2", new object[] { idx, tempmodel.ID });
                }

                return rstr;
            }
            catch (Exception ex)
            {
                NLogManagement.SystemLogInfo("刪除FormSelItem失敗:" + ex.Message);
                return "刪除失敗";
            }
        }
        #endregion

        #region UpdateItemSeq
        public string UpdateItemSeq(int modelid, int id, int seq, string account, string username)
        {
            try
            {
                if (seq <= 0) { seq = 1; }
                var oldmodel = _selitemsqlitemrepository.GetByWhere("ID=@1", new object[] { id });
                if (oldmodel.Count() == 0) { return "更新失敗"; }
                var diff = "";

                diff = diff.TrimStart(',');
                var r = 0;
                if (oldmodel.First().Sort != seq)
                {
                    IList<FormSelItem> mtseqdata = null;
                    mtseqdata = _selitemsqlitemrepository.GetByWhere("Sort>@1 and ItemID=@2", new object[] { seq, modelid }).OrderBy(v => v.Sort).ToArray();

                    var updatetime = DateTime.Now;
                    if (mtseqdata.Count() == 0)
                    {
                        var totalcnt = 0;
                        totalcnt = _selitemsqlitemrepository.GetCountUseWhere("ItemID=@1", new object[] { modelid });
                        seq = totalcnt;
                    }

                    var qseq = (seq > oldmodel.First().Sort) ? seq : oldmodel.First().Sort;
                    IList<FormSelItem> ltseqdata = null;
                    ltseqdata = _selitemsqlitemrepository.GetByWhere("Sort<=@1 and ItemID=@2", new object[] { qseq, modelid }).OrderBy(v => v.Sort).ToArray();
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
                        _selitemsqlitemrepository.Update(tempmodel);
                    }
                }
                //先取出大於目前seq的資料

                oldmodel.First().Sort = seq;
                r = _selitemsqlitemrepository.Update(oldmodel.First());
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
                NLogManagement.SystemLogInfo("更新FormSelItem排序失敗:" + " error:" + ex.Message);
                return "更新FormSelItem排序失敗:" + " error:" + ex.Message;
            }
        }
        #endregion

        //seo
        #region GetSEO
        public SEOViewModel GetSEO(string mainid)
        {
            var seodata = _seosqlrepository.GetByWhere("TypeName=@1 and TypeID=@2", new object[] { "Form", mainid });
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

        #region SaveSEO
        public string SaveSEO(SEOViewModel model, string LangID)
        {
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
                TypeName = "Form",
                TypeID = int.Parse(model.TypeID),
                Lang_ID = int.Parse(LangID)
            };
            var r = 0;
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

        //
        #region SaveSetting
        public string SaveSetting(FormSettingModel model)
        {
            if (model.ID < 0)
            {
                var r = _formsettingsqlrepository.Create(new FormSetting
                {
                    ItemID = model.ItemID,
                    ReceiveMail = model.ReceiveMail,
                    AdminSenderEMail = model.AdminSenderEMail,
                    AdminSenderName = model.AdminSenderName,
                    AdminSenderTitle = model.AdminSenderTitle,
                    ConfirmContent = model.ConfirmContent,
                    FormDesc = model.FormDesc,
                    SenderEMail = model.SenderEMail,
                    SenderName = model.SenderName,
                    SenderTitle = model.SenderTitle
                });
                if (r > 0)
                {
                    return "設定成功";
                }
                else
                {
                    return "設定失敗";
                }
            }
            else
            {
                var r = _formsettingsqlrepository.Update(new FormSetting
                {
                    ID = model.ID,
                    ReceiveMail = model.ReceiveMail == null ? "" : model.ReceiveMail,
                    AdminSenderEMail = model.AdminSenderEMail == null ? "" : model.AdminSenderEMail,
                    AdminSenderName = model.AdminSenderName == null ? "" : model.AdminSenderName,
                    AdminSenderTitle = model.AdminSenderTitle == null ? "" : model.AdminSenderTitle,
                    ConfirmContent = model.ConfirmContent == null ? "" : model.ConfirmContent,
                    FormDesc = model.FormDesc == null ? "" : model.FormDesc,
                    SenderEMail = model.SenderEMail == null ? "" : model.SenderEMail,
                    SenderName = model.SenderName == null ? "" : model.SenderName,
                    SenderTitle = model.SenderTitle == null ? "" : model.SenderTitle,
                });
                if (r > 0)
                {
                    return "設定成功";
                }
                else
                {
                    return "設定失敗";
                }
            }
        }

        #endregion

        #region GetFormSetting
        public FormSettingModel GetFormSetting(string mainid)
        {
            var model = _formsettingsqlrepository.GetByWhere("ItemID=@1", new object[] { mainid });
            if (model.Count() > 0)
            {
                return new FormSettingModel()
                {
                    ID = model.First().ID,
                    AdminSenderEMail = model.First().AdminSenderEMail,
                    AdminSenderName = model.First().AdminSenderName,
                    AdminSenderTitle = model.First().AdminSenderTitle,
                    ConfirmContent = model.First().ConfirmContent,
                    FormDesc = model.First().FormDesc,
                    ItemID = model.First().ItemID.Value,
                    ReceiveMail = model.First().ReceiveMail,
                    SenderEMail = model.First().SenderEMail,
                    SenderName = model.First().SenderName,
                    SenderTitle = model.First().SenderTitle
                };
            }
            else
            {
                return new FormSettingModel() { ItemID = int.Parse(mainid) };
            }
        }
        #endregion

        #region GetFormList
        public string[] GetFormList(string itemid,IDictionary<string, string> langdict)
        {
            var lists = _selitemsqlitemrepository.GetByWhere("ItemID=@1 Order by  Sort", new object[] { itemid });
            var sb = new StringBuilder();
            var requirestr = "<span class='red'>("+ langdict["mustinputstr"] + ")</span>";
            foreach (var list in lists)
            {
                if (list.ItemMode == 1)
                {
                    sb.Append("<div class='table-row formitem " + (list.MustInput == true ? "require" : "") + "' index='" + list.ID + "' type='1'><div class='form_q'><label for='item_" + list.ID + "'>" 
                        + list.Title + (list.MustInput == true ? requirestr : "")+ "</label></div>");
                    sb.Append("<div class='form_a'><input type='text' id='item_" + list.ID + "'  name='item_" + list.ID + "' class='form-control' maxlength='" + list.TextLength + "' placeholder='" +
                        (list.DefaultText == null ? "" : list.DefaultText) + "' size='" + (list.ColumnNum == null ? "" : list.ColumnNum.Value.ToString()) + "' title='" + list.Title+"'>");
                    sb.Append("<span  id='err_" + list.ID + "' class='error' style='display:none'>" + list.Title + langdict["mustinputstr"] + " ！</span>");
                    sb.Append("<span  id='err2_" + list.ID + "' class='error' style='display:none'></span>");
                    sb.Append("<div class='required'>" + list.Description + "</div></div></div>");
                }
                else if (list.ItemMode == 2)
                {
                    sb.Append("<div class='table-row formitem " + (list.MustInput == true ? "require" : "") + "' index='" + list.ID + "' type='2'><div class='form_q'><label for='item_" + list.ID + "'>" +
                        list.Title + (list.MustInput == true ? requirestr : "") + "</label></div>");
                    var colsstr = list.ColumnNum == null ? "" : "cols='" + list.ColumnNum + "'";
                    sb.Append("<div class='form_a'><textarea rows='" + (list.RowNum == null ? "" : list.RowNum.Value.ToString()) + "'  id ='item_" + list.ID +
                        "' class='form-control'  maxlength='" + list.TextLength + "' type='text' " + colsstr + " title='" + list.Title+ "' name='item_" + list.ID + "'>" +
                       (list.DefaultText == null ? "" : list.DefaultText) + "</textarea>");
                    sb.Append("<span  id='err_" + list.ID + "' class='error' style='display:none'>" + list.Title + langdict["mustinputstr"] + " ！</span>");
                    sb.Append("<div class='required'>" + list.Description + "</div></div></div>");
                }
                else if (list.ItemMode == 3)
                {
                    var items = list.SelList.Split('^');
                    var iidx = 1;
                    sb.Append("<div class='table-row formitem " + (list.MustInput == true ? "require" : "") + "'  index='" + list.ID + "' type='3'><div class='form_q'><label for='"+ iidx+"_item_" + list.ID + "'>" +
                        list.Title + (list.MustInput == true ? requirestr : "") + "</label></div>");
                    sb.Append("<div class='form_a'>");
                    
                    foreach (var item in items)
                    {
                        sb.Append("<label class='mt-radio mt-radio-outline' for='"+ iidx+"_item_" + list.ID+"'><input type='radio' name='item_" + list.ID + "' value='" + item + "' id='"+ iidx+"_item_" + list.ID + "'>" + item + "<span></span></label>");
                        iidx++;
                    }
                    sb.Append("<div id='err_" + list.ID + "' class='error' style='display:none'>" + list.Title + langdict["mustinputstr"] + " ！</div>");
                    sb.Append("<div class='required'>" + list.Description + "</div></div></div>");
                }
                else if (list.ItemMode == 4)
                {
                    var iidx = 1;
                    var items = list.SelList.Split('^');
                    sb.Append("<div class='table-row formitem " + (list.MustInput == true ? "require" : "") + "' index='" + list.ID + "' type='4'><div class='form_q'><label for='"+ iidx+"_item_" + list.ID + "'>"
                    + list.Title + (list.MustInput == true ? requirestr : "") + "</label></div>");
                    sb.Append("<div class='form_a'>");
                    foreach (var item in items)
                    {
                        sb.Append("<label class='mt-checkbox mt-checkbox-outline' for='"+ iidx+"_item_" + list.ID + "'><input type='checkbox' class='checkboxes' value='" + item + "'  id='"+ iidx+"_item_" + 
                            list.ID + "'  name='item_" + list.ID + "'>" + item + "<span></span></label>");
                        iidx++;
                    }
                    sb.Append("<div  id='err_" + list.ID + "' class='error' style='display:none'>" + list.Title + langdict["mustinputstr"] + " ！</div>");
                    sb.Append("<div class='required'>" + list.Description + "</div></div></div>");
                }
                else if (list.ItemMode == 5)
                {
                    var items = list.SelList.Split('^');
                    sb.Append("<div class='table-row formitem " + (list.MustInput == true ? "require" : "") + "' index='" + list.ID + "' type='5'><div class='form_q'><label for='item_" + list.ID + "'>" +
                        list.Title + (list.MustInput == true ? requirestr : "") + "</label></div>");
                    sb.Append("<div class='form_a'><select class='form-control w-auto'  id='item_" + list.ID + "'  name='item_" + list.ID + "'><option value=''>" + langdict["pleaseselectStr"] + "</option>");
                    foreach (var item in items)
                    {
                        sb.Append("<option value='" + item + "'>" + item + "</option>");
                    }
                    sb.Append("</select><div id='err_" + list.ID + "' class='error' style='display:none'>" + list.Title + langdict["mustinputstr"] + " ！</div>");
                    sb.Append("<div class='required'>" + list.Description + "</div></div></div>");
                }

            }
            //
            var imagestrArr = _ILoginManager.GetCaptchImage();
            sb.Append("<div class='table-row' id='div_captch'><div class='form_q'><label for='img_captch'>" + langdict["checkcode"] + requirestr+ "</label></div><div class='form_a'>");
            sb.Append("<input type = 'text'  class='form-control input-small float_left' placeholder='" + langdict["checkcodeinput"]    + "'  id='img_captch' name='img_captch'>");
            sb.Append("<span class='verification_code'>" +
                           "<img src='" + imagestrArr[1] + "' alt='" + langdict["checkcode"] + "' id='image'>" +
                                   "<a href='#div_captch'  id='a_captchRefresh' style='cursor:pointer' title='" + langdict["refresh"] + "'>" +
                                          "<i class='fa fa-refresh'  aria-hidden='true'></i>" + langdict["refresh"] 
                                 + "</a>");
            sb.Append("<a href='javascript:void(0)' data-voice='"+ imagestrArr[0] +  "' title='" + langdict["audioplay"] + "' id='i_voice_play'><i class='fa fa-volume-up'  aria-hidden='true' style='cursor:pointer'>" +
                           "</i><span class='sr-only'>語音撥放 </span></a>");
            sb.Append("</span><div  id='err_captch' class='error' style='display:none'> " + langdict["checkcode"] + langdict["mustinputstr"] + "！</div>" +
           "<div  id='err_captchnomatch' class='error' style='display:none'> " + langdict["CatchStr2"] + "！</div></div></div>");
            
            return new string[] { sb.ToString(), imagestrArr[0] };
        }
        #endregion

        #region SaveForm
        public string SaveForm(string jsonstr, string itemid)
        {
            var tempd = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonstr);
            if (tempd.Count() == 0) { return "無輸入資料"; }
            var name = "";
            var email = "";
            var lists = _selitemsqlitemrepository.GetByWhere("ItemID=@1 Order by  Sort", new object[] { itemid });
            var namekey = lists.Where(v => v.KayName == "Name");
            if (namekey.Count() > 0)
            {
                if (tempd.ContainsKey(namekey.First().ID.ToString()))
                {
                    name = tempd[namekey.First().ID.ToString()];
                }
            }
            else {
                namekey = lists.Where(v => v.Title=="姓名" || v.Title == "聯絡人");
                if (namekey.Count() > 0)
                {
                    if (tempd.ContainsKey(namekey.First().ID.ToString()))
                    {
                        name = tempd[namekey.First().ID.ToString()];
                    }
                }

            }
            var sendbodyarr = new List<string>();
            foreach (var tempkey in tempd.Keys)
            {
                var keydata = lists.Where(v => v.ID == int.Parse(tempkey));
                if (keydata.Count() > 0)
                {
                    sendbodyarr.Add(keydata.First().Title + ":" + tempd[tempkey]);
                }
            }
            var sendbodystr = string.Join("<br>", sendbodyarr);
            var emailkey = lists.Where(v => v.KayName == "EMail");
            if (emailkey.Count() > 0)
            {
                if (tempd.ContainsKey(emailkey.First().ID.ToString()))
                {
                    email = tempd[emailkey.First().ID.ToString()];
                }
            }
            else {
                emailkey = lists.Where(v => v.Title.Replace("-", "").ToLower() == "email");
                if (emailkey.Count() > 0)
                {
                    if (tempd.ContainsKey(emailkey.First().ID.ToString()))
                    {
                        email = tempd[emailkey.First().ID.ToString()];
                    }
                }
            }
            var r = _inputsettingsqlrepository.Create(new FormInput()
            {
                CreateDatetime = DateTime.Now,
                EMail = email,
                JSONStr = jsonstr,
                MainID = int.Parse(itemid),
                Name = name,
                Progress = "0"
            });
            if (r < 0)
            {
                return "輸入失敗";
            }
            else
            {
                try
                {
                    IList<System.Net.Mail.MailMessage> mailmessage = new List<System.Net.Mail.MailMessage>();
                    var host = System.Web.Configuration.WebConfigurationManager.AppSettings["smtphost"];
                    var mailfrom = System.Web.Configuration.WebConfigurationManager.AppSettings["mailfrom"];
                    var setting = _formsettingsqlrepository.GetByWhere("ItemID=@1", new object[] { itemid });
                    if (setting.Count() > 0)
                    {
                        if (setting.First().SenderEMail.IsNullorEmpty() == false)
                        {
                            var maindata = _sqlrepository.GetByWhere("ID=@1", new object[] { itemid });
                            var NoticeSenderEMail = setting.First().SenderEMail;
                            var NoticeSenderName = setting.First().SenderName;
                            var NoticeSubject = setting.First().SenderTitle;
                            NoticeSenderEMail = string.IsNullOrEmpty(setting.First().SenderEMail) ? mailfrom : setting.First().SenderEMail;
                            NoticeSenderName = string.IsNullOrEmpty(setting.First().SenderName) ? "會員填寫表單回覆" : setting.First().SenderName;
                            NoticeSubject = string.IsNullOrEmpty(setting.First().SenderTitle) ? "會員填寫表單回覆通知信" : setting.First().SenderTitle;
                            var slist = setting.First().ReceiveMail.Split(';');
                            foreach (var sender in slist)
                            {
                                if (sender.Trim() == "") { continue; }
                                MailMessage message = new MailMessage();
                                message.From = new MailAddress(NoticeSenderEMail, NoticeSenderName);
                                message.To.Add(new MailAddress(sender));
                                message.SubjectEncoding = System.Text.Encoding.UTF8;
                                message.Subject = NoticeSubject;
                                message.BodyEncoding = System.Text.Encoding.UTF8;
                                string body = "您好：<br/>" +
                                name + "於" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "填寫表單【" + maindata.First().Name + "】<br>" + sendbodystr;
                                message.Body = body;
                                message.IsBodyHtml = true;
                                message.Priority = MailPriority.High;
                                mailmessage.Add(message);
                            }

                            if (mailmessage.Count() > 0)
                            {
                                ThreadPool.QueueUserWorkItem(new WaitCallback(DoThreadAndSendMail), new object[] { mailmessage, "" });
                            }
                        }

                    }

                }
                catch (Exception ex)
                {
                    NLogManagement.SystemLogInfo("通知信寄信失敗 原因:" + ex.Message);
                }
                return "";
            }

        }
        #endregion

        #region PagingMail
        public Paging<FormInputResult> PagingMail(MailSearchModel model)
        {
            var Paging = new Paging<FormInputResult>();
            var onepagecnt = model.Limit;
            var whereobj = new List<string>();
            var wherestr = new List<string>();
            wherestr.Add("MainID=@1");
            whereobj.Add(model.ModelID.ToString());
            var idx = 1;
            if (model.Search != "")
            {
                if (string.IsNullOrEmpty(model.Name) == false)
                {
                    idx += 1;
                    wherestr.Add("Name like @" + idx);
                    whereobj.Add("%" + model.Name + "%");
                }
                if (string.IsNullOrEmpty(model.EMail) == false)
                {
                    idx += 1;
                    wherestr.Add("EMail like @" + idx);
                    whereobj.Add("%" + model.EMail + "%");
                }
                if (string.IsNullOrEmpty(model.KeyWord) == false)
                {
                    idx += 1;
                    wherestr.Add("( JSONStr like @"+idx+")");
                    whereobj.Add("%" + model.KeyWord + "%");
                }
                if (string.IsNullOrEmpty(model.Process) == false)
                {

                    if (model.Process != "1")
                    {
                        wherestr.Add("Progress='" + model.Process + "'");
                    }
                    else
                    {
                        wherestr.Add("Progress>0 and Progress<100");
                    }

                }
                if (string.IsNullOrEmpty(model.Reply) == false)
                {
                    if (model.Reply == "1")
                    {
                        wherestr.Add("(ReplyAccount is Null or ReplyAccount='')");
                    }
                    else
                    {
                        wherestr.Add("(ReplyAccount is not Null and ReplyAccount<>'')");
                    }
                }
                if (string.IsNullOrEmpty(model.InputDateFrom) == false)
                {
                    idx += 1;
                    wherestr.Add("CreateDatetime >=@" + idx);
                    whereobj.Add(model.InputDateFrom);
                }
                if (string.IsNullOrEmpty(model.InputDateTo) == false)
                {
                    idx += 1;
                    wherestr.Add("CreateDatetime <=@" + idx);

                    whereobj.Add(DateTime.Parse(model.InputDateTo).AddDays(1).ToString("yyyy/MM/dd"));
                }
            }
            var str = string.Join(" and ", wherestr);
            List<FormInput> data = null;
            if (model.Limit != -1)
            {
                data = _inputsettingsqlrepository.GetDataUseWhereRange(str, whereobj.ToArray(), model.Offset, model.Limit, model.Sort);
            }
            else
            {
                data = _inputsettingsqlrepository.GetByWhere(str + " order by " + model.Sort, whereobj.ToArray()).ToList();
            }

            Paging.total = _inputsettingsqlrepository.GetCountUseWhere(str, whereobj.ToArray());
            foreach (var d in data)
            {
                var pstr = "";
                if (d.Progress == "0" || d.Progress == "") { pstr = "未處理"; }
                else if (d.Progress == "100") { pstr = "已完成"; }
                else { pstr = "處理中"; }
                Paging.rows.Add(new FormInputResult()
                {
                    ID = d.ID,
                    Name = d.Name,
                    Progress = pstr,
                    ReplyNote = string.IsNullOrEmpty(d.ReplyAccount) ? "未回覆" : "已回覆",
                    CreateDatetime = d.CreateDatetime.Value.ToString("yyyy/MM/dd HH:mm:ss")

                });
            }
            return Paging;
        }

        #endregion

        #region GetMailInput
        public MailInputModel GetMailInput(string id)
        {
            var data = _inputsettingsqlrepository.GetByWhere("ID=@1", new object[] { id });
            var model = new MailInputModel();
            model.CreateDatetime = data.First().CreateDatetime.Value;

            var processnotes = _inputnotesqlrepository.GetByWhere("Type='P' and InputId=@1  order by CreateDateTime", new object[] { id });
            var replynotes = _inputnotesqlrepository.GetByWhere("Type='R' and InputId=@1  order by CreateDateTime", new object[] { id });
            model.ProcessNote = processnotes.ToArray();
            model.ReplyNote = replynotes.ToArray();
            model.ID = data.First().ID;
            model.Progress = data.First().Progress;
            model.MainID = data.First().MainID.Value;
            var input = data.First().JSONStr;
            var dict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(input);
            var inputitem = _selitemsqlitemrepository.GetByWhere("ItemID=@1", new object[] { data.First().MainID }).ToDictionary(v => v.ID.ToString(), v => v.Title);
            foreach (var item in inputitem) {
                model.InputKey.Add(inputitem[item.Key]);
                if (dict.ContainsKey(item.Key))
                {
                    if (dict[item.Key].IndexOf("^") >= 0)
                    {
                        model.InputValue.Add(dict[item.Key].Replace("^", " "));
                    }
                    else
                    {
                        model.InputValue.Add(dict[item.Key]);
                    }

                }
                else {
                    model.InputValue.Add("");
                }
            }
            //foreach (var key in dict.Keys)
            //{
            //    if (inputitem.ContainsKey(key))
            //    {
            //        model.InputKey.Add(inputitem[key]);
            //        if (dict[key].IndexOf("^") >= 0)
            //        {
            //            model.InputValue.Add(dict[key].Replace("^", " "));
            //        }
            //        else
            //        {
            //            model.InputValue.Add(dict[key]);
            //        }

            //    }
            //}

            return model;
        }


        #endregion

        #region SaveProgressNote
        public string SaveProgressNote(string text, string id, string account)
        {
            var olddata = _inputsettingsqlrepository.GetByWhere("ID=@1", new object[] { id });
            var r = _inputnotesqlrepository.Create(new FormInputNote()
            {
                Account = account,
                CreateDateTime = DateTime.Now,
                InputID = int.Parse(id),
                NoteText = text,
                Type = "P",
                MainID = olddata.First().MainID.Value
            });
            if (r > 0)
            {
                r = _inputsettingsqlrepository.Update("ProcessAccount=@1,ProcessDatetime=@2", "ID=@3", new object[] { account, DateTime.Now, id });
                return "設定成功";
            }
            else { return "設定失敗"; }
        }
        #endregion

        #region SaveReply
        public string SaveReply(string text, string id, string account)
        {
            NLogManagement.SystemLogInfo("text="+ text+" id="+id+" account="+account);
            var olddata = _inputsettingsqlrepository.GetByWhere("ID=@1", new object[] { id });
            if (olddata.Count() == 0) {
                return "查無資料";
            }
            NLogManagement.SystemLogInfo("olddata.Count()=" + olddata.Count());
            var r = _inputnotesqlrepository.Create(new FormInputNote()
            {
                Account = account,
                CreateDateTime = DateTime.Now,
                InputID = int.Parse(id),
                NoteText = text,
                Type = "R",
                MainID = olddata.First().MainID.Value
            });
            NLogManagement.SystemLogInfo("r=" + r);
            if (r > 0)
            {
                r = _inputsettingsqlrepository.Update("ReplyAccount=@1,ReplyDatetime=@2", "ID=@3", new object[] { account, DateTime.Now, id });
                NLogManagement.SystemLogInfo("r2=" + r);
                var data = _inputsettingsqlrepository.GetByWhere("ID=@1", new object[] { id });
                var maindata = _sqlrepository.GetByWhere("ID=@1", new object[] { data.First().MainID });
                NLogManagement.SystemLogInfo("data count=" + data.Count());
                NLogManagement.SystemLogInfo("maindata count=" + maindata.Count());
                try
                {
                    IList<System.Net.Mail.MailMessage> mailmessage = new List<System.Net.Mail.MailMessage>();
                    var host = System.Web.Configuration.WebConfigurationManager.AppSettings["smtphost"];
                    var mailfrom = System.Web.Configuration.WebConfigurationManager.AppSettings["mailfrom"];
                    NLogManagement.SystemLogInfo("host=" + host);
                    NLogManagement.SystemLogInfo("mailfrom=" + mailfrom);
                    var NoticeSenderEMail = mailfrom;
                    var NoticeSenderName = "客服回覆";
                    var NoticeSubject = "客服信箱回覆通知信";
                    var setting = _formsettingsqlrepository.GetByWhere("ItemID=@1", new object[] { data.First().MainID });
                    NLogManagement.SystemLogInfo("setting count=" + setting.Count());
                    if (setting.Count() > 0)
                    {
                        if (setting.First().AdminSenderEMail.IsNullorEmpty() == false)
                        {
                            NoticeSenderEMail = string.IsNullOrEmpty(setting.First().AdminSenderEMail) ? mailfrom : setting.First().AdminSenderEMail;
                            NoticeSenderName = string.IsNullOrEmpty(setting.First().AdminSenderName) ? "管理者回覆" : setting.First().AdminSenderName;
                            NoticeSubject = string.IsNullOrEmpty(setting.First().AdminSenderTitle) ? "管理者回覆通知信" : setting.First().AdminSenderTitle;
                            NLogManagement.SystemLogInfo("NoticeSubject=" + NoticeSubject);
                            NLogManagement.SystemLogInfo("data.First().EMail=" + data.First().EMail);
                            var slist = data.First().EMail.Split(';');
                            foreach (var sender in slist)
                            {
                                if (sender.Trim() == "") { continue; }
                                MailMessage message = new MailMessage();
                                message.From = new MailAddress(NoticeSenderEMail, NoticeSenderName);
                                message.To.Add(new MailAddress(data.First().EMail));
                                message.SubjectEncoding = System.Text.Encoding.UTF8;
                                message.Subject = NoticeSubject;
                                message.BodyEncoding = System.Text.Encoding.UTF8;
                                string body = " " + data.First().Name + " ，您好：<br/>" + text;
                                message.Body = body;
                                message.IsBodyHtml = true;
                                message.Priority = MailPriority.High;
                                mailmessage.Add(message);
                                NLogManagement.SystemLogInfo("NoticeSenderEMail=" + NoticeSenderEMail);
                                NLogManagement.SystemLogInfo("NoticeSenderName=" + NoticeSenderName);
                                NLogManagement.SystemLogInfo("To=" + data.First().EMail);
                            }

                            if (mailmessage.Count() > 0)
                            {
                                ThreadPool.QueueUserWorkItem(new WaitCallback(DoThreadAndSendMail), new object[] { mailmessage, "" });
                            }
                        }

                    }

                }
                catch (Exception ex)
                {
                    NLogManagement.SystemLogInfo("通知信寄信失敗:寄信給" + data.First().EMail + "失敗 原因:" + ex.Message);
                    return "通知信寄信失敗:寄信給" + data.First().EMail + "失敗 原因:" + ex.Message;
                }
                return "更新成功";
            }
            else { return "更新失敗"; }
        }
        #endregion

        #region DoThreadAndSendMail
        public static void DoThreadAndSendMail(object dataarray)
        {
            try
            {
                NLogManagement.SystemLogInfo("======================");
                var host = System.Web.Configuration.WebConfigurationManager.AppSettings["smtphost"];
                var ur = System.Web.Configuration.WebConfigurationManager.AppSettings["mailuser"];
                var pw = System.Web.Configuration.WebConfigurationManager.AppSettings["mailpassword"];
                var port = System.Web.Configuration.WebConfigurationManager.AppSettings["mailport"];
                NLogManagement.SystemLogInfo("準備寄信給:host" + host);
                object[] objarr = (object[])dataarray;
                IList<System.Net.Mail.MailMessage> mailmessage = (IList<System.Net.Mail.MailMessage>)objarr[0];
                string filepath = (string)objarr[1];
                foreach (var ms in mailmessage)
                {
                    if (string.IsNullOrEmpty(filepath) == false)
                    {
                        using (Attachment attachment = new Attachment(filepath))
                        {
                            ms.Attachments.Add(attachment);
                        }
                    }
                    NLogManagement.SystemLogInfo("準備寄信給:" + ms.To.First().Address);
                    if (string.IsNullOrEmpty(pw) == false)
                    {
                        SmtpClient client = new SmtpClient(host, int.Parse(port));
                        client.EnableSsl = true;
                        client.Credentials = new NetworkCredential(ur, pw);
                        client.Send(ms);
                    }
                    else
                    {
                        SmtpClient client2 = new SmtpClient(host);
                        client2.Send(ms);
                    }
                }
                NLogManagement.SystemLogInfo("寄信成功:");
            }
            catch (Exception ex)
            {
                NLogManagement.SystemLogInfo("寄信失敗:" + ex.Message);
            }
        }
        #endregion

        #region SetMailDelete
        public string SetMailDelete(string[] idlist, string delaccount, string account, string username)
        {
            try
            {
                var r = 0;
                for (var idx = 0; idx < idlist.Length; idx++)
                {
                    r = _inputsettingsqlrepository.DelDataUseWhere("ID=@1", new object[] { idlist[idx] });
                    if (r <= 0)
                    {
                        NLogManagement.SystemLogInfo("刪除信件失敗:ID=" + idlist[idx]);
                    }
                }
                var rstr = "";
                if (r >= 0)
                {
                    NLogManagement.SystemLogInfo("刪除群組:" + delaccount);
                    rstr = "刪除成功";
                }
                else
                {
                    rstr = "刪除失敗";
                }
                return rstr;
            }
            catch (Exception ex)
            {
                NLogManagement.SystemLogInfo("刪除群組失敗:" + ex.Message);
                return "刪除失敗";
            }
        }
        #endregion

        #region SaveProgress
        public string SaveProgress(string progress, string id, string account)
        {
            var r = _inputsettingsqlrepository.Update("Progress=@1", "ID=@2", new object[] { progress, id });
            if (r > 0) { return "設定成功"; } else { return "設定失敗"; }
        }
        #endregion

        #region SetValue
        private void SetValue(ISheet sheet, string value, int _r, int _c, ICellStyle style)
        {
            if (sheet.GetRow(_r) == null)
            {
                sheet.CreateRow(_r);
            }
            if (sheet.GetRow(_r).GetCell(_c) == null)
            {
                sheet.GetRow(_r).CreateCell(_c);
            }
            sheet.GetRow(_r).GetCell(_c).CellStyle = style;
            if (value == null) { value = ""; }
            sheet.GetRow(_r).GetCell(_c).SetCellValue(value);
        }
        #endregion

        #region GetExport
        public byte[] GetExport(MailSearchModel model)
        {
            var whereobj = new List<string>();
            var wherestr = new List<string>();
            wherestr.Add("MainID=@1");
            whereobj.Add(model.ModelID.ToString());
            var sidx = 1;
            if (model.Search != "")
            {
                if (string.IsNullOrEmpty(model.Name) == false)
                {
                    sidx += 1;
                    wherestr.Add("Name like @" + sidx);
                    whereobj.Add("%" + model.Name + "%");
                }
                if (string.IsNullOrEmpty(model.EMail) == false)
                {
                    sidx += 1;
                    wherestr.Add("EMail like @" + sidx);
                    whereobj.Add("%" + model.EMail + "%");
                }
                if (string.IsNullOrEmpty(model.KeyWord) == false)
                {
                    sidx += 1;
                    wherestr.Add("(ProcessNote like @" + sidx + " or ReplyNote like @" + sidx + ")");
                    whereobj.Add("%" + model.KeyWord + "%");
                }
                if (string.IsNullOrEmpty(model.Process) == false)
                {

                    if (model.Process != "1")
                    {
                        wherestr.Add("Progress='" + model.Process + "'");
                    }
                    else
                    {
                        wherestr.Add("Progress>0 and Progress<100");
                    }

                }
                if (string.IsNullOrEmpty(model.Reply) == false)
                {
                    if (model.Reply == "1")
                    {
                        wherestr.Add("(ReplyAccount is Null or ReplyAccount='')");
                    }
                    else
                    {
                        wherestr.Add("(ReplyAccount is not Null and ReplyAccount<>'')");
                    }
                }
                if (string.IsNullOrEmpty(model.InputDateFrom) == false)
                {
                    sidx += 1;
                    wherestr.Add("CreateDatetime >=@" + sidx);
                    whereobj.Add(model.InputDateFrom);
                }
                if (string.IsNullOrEmpty(model.InputDateTo) == false)
                {
                    sidx += 1;
                    wherestr.Add("CreateDatetime <=@" + sidx);
                    whereobj.Add(DateTime.Parse(model.InputDateTo).AddDays(1).ToString("yyyy/MM/dd"));
                }
            }
            var str = string.Join(" and ", wherestr);
            List<FormInput> datalist = _inputsettingsqlrepository.GetByWhere(str + " order by " + model.Sort, whereobj.ToArray()).ToList();
            MemoryStream ms = new MemoryStream();
            XSSFWorkbook hssfworkbook = new XSSFWorkbook();
            IFont font = hssfworkbook.CreateFont();
            font.FontHeightInPoints = 9;
            ICellStyle style = hssfworkbook.CreateCellStyle();
            style.Alignment = HorizontalAlignment.Left;
            style.VerticalAlignment = VerticalAlignment.Center;
            style.WrapText = true;
            style.SetFont(font);

            ISheet sheet = hssfworkbook.CreateSheet("匯出資料");
            IRow row = sheet.CreateRow(0);
            row.HeightInPoints = 16;

            SetValue(sheet, "諮詢時間     ", 0, 0, style);

            var inputitem = _selitemsqlitemrepository.GetByWhere("ItemID=@1 order by sort", new object[] { model.ModelID }).ToDictionary(v => v.ID.ToString(), v => v.Title);
            var idx = 1;
            foreach (var value in inputitem.Values)
            {
                SetValue(sheet, value + "                  ", 0, idx, style);
                idx += 1;
            }
            SetValue(sheet, "處理進度     ", 0, idx, style);
            var endidx = idx;
            SetValue(sheet, "處理備註     ", 0, idx + 1, style);
            SetValue(sheet, "回覆內容     ", 0, idx + 2, style);

            sheet.SetColumnWidth(endidx + 1, 50 * 256);
            sheet.SetColumnWidth(endidx + 2, 50 * 256);

            var processnotes = _inputnotesqlrepository.GetByWhere("Type='P' and MainID=@1  order by CreateDateTime", new object[] { model.ModelID });
            var replynotes = _inputnotesqlrepository.GetByWhere("Type='R' and MainID=@1  order by CreateDateTime", new object[] { model.ModelID });
            for (var ridx = 1; ridx <= datalist.Count(); ridx++)
            {
                var data = datalist[ridx - 1];
                SetValue(sheet, data.CreateDatetime.Value.ToString("yyyy/MM/dd HH:mm:ss"), ridx, 0, style);
                var input = data.JSONStr;
                var dict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(input);
                idx = 1;
                foreach (var key in inputitem.Keys)
                {
                    if (dict.ContainsKey(key))
                    {
                        SetValue(sheet, dict[key].Replace("^", " "), ridx, idx, style);
                    }
                    else
                    {
                        SetValue(sheet, "", ridx, idx, style);
                    }
                    idx += 1;
                }
                if (data.Progress.IsNullorEmpty())
                {
                    SetValue(sheet, "0%", ridx, idx, style);
                }
                else
                {
                    SetValue(sheet, data.Progress + "%", ridx, idx, style);
                }
                var ProcessNote = processnotes.Where(v => v.InputID == data.ID);
                if (ProcessNote.Count() > 0)
                {
                    SetValue(sheet, string.Join(Environment.NewLine, ProcessNote.Select(v => v.NoteText).ToArray()), ridx, idx + 1, style);
                }
                else
                {
                    SetValue(sheet, "     ", ridx, idx + 1, style);
                }
                var ReplyNote = replynotes.Where(v => v.InputID == data.ID);
                if (ReplyNote.Count() > 0)
                {
                    SetValue(sheet, string.Join(Environment.NewLine, ReplyNote.Select(v => v.NoteText).ToArray()), ridx, idx + 2, style);
                }
                else
                {
                    SetValue(sheet, "     ", ridx, idx + 2, style);
                }
            }
            for (int i = 0; i <= endidx; i++)
            {
                sheet.AutoSizeColumn(i);
            }
            hssfworkbook.Write(ms);
            hssfworkbook = null;
            byte[] bytes = ms.ToArray();
            ms.Close();
            ms.Dispose();
            return bytes;
        }


        #endregion

        #region MainModelName
        public string MainModelName(string mainid)
        {
            var data = _sqlrepository.GetByWhere("ID=@1", new object[] { mainid });
            if (data.Count() > 0)
            {
                return data.First().Name;
            }
            else {
                return "";
            }
        }
        #endregion

        #region GetFormSelItemByItemID
        public List<FormSelItem> GetFormSelItemByItemID(string itemid)
        {
            var data = _selitemsqlitemrepository.GetByWhere("ItemID=@1", new object[] { itemid });
            if (data.Count() > 0)
            {
                return data.ToList();
            }
            else
            {
                return new List<FormSelItem>();
            }
        }
        #endregion

        #region GetFormListNoJs
        public string[] GetFormListNoJs(string itemid, List<string> erroritem, IDictionary<string, string> langdict)
        {
            var lists = _selitemsqlitemrepository.GetByWhere("ItemID=@1 Order by  Sort", new object[] { itemid });
            var sb = new StringBuilder();
            var requirestr = "<span class='red'>(" + langdict["mustinputstr"] + ")</span>";
            foreach (var list in lists)
            {
                if (list.ItemMode == 1)
                {
                    sb.Append("<div class='table-row formitem " + (list.MustInput == true ? "require" : "") + "' index='" + list.ID + "' type='1'><div class='form_q'><label for='item_" + list.ID + "'>" 
                        + list.Title + (list.MustInput == true ? requirestr : "")+ "</label></div>");
                    sb.Append("<div class='form_a'><input type='text' id='item_" + list.ID + "'  name='item_" + list.ID + "' class='form-control' maxlength='" + list.TextLength + "' value='" +
                        (list.DefaultText == null ? "" : list.DefaultText) + "' size='" + (list.ColumnNum == null ? "" : list.ColumnNum.Value.ToString()) + "' title='" + list.Title+"'>");
                    if (erroritem.Contains(list.ID.ToString())) {
                        sb.Append("<span  id='err_" + list.ID + "' class='error' >" + list.Title + langdict["mustinputstr"] + " ！</span>");
                    }
                    if (erroritem.Contains(list.ID.ToString()+ "Error2")){sb.Append("<span  id='err_" + list.ID + "' class='error' >" + langdict["Error2"] + " ！</span>"); }
                    if (erroritem.Contains(list.ID.ToString() + "Error3")) { sb.Append("<span  id='err_" + list.ID + "' class='error' >" + langdict["Error3"] + " ！</span>"); }
                    if (erroritem.Contains(list.ID.ToString() + "Error4")) { sb.Append("<span  id='err_" + list.ID + "' class='error' >" + langdict["Error4"] + " ！</span>"); }
                    sb.Append("<span  id='err2_" + list.ID + "' class='error' style='display:none'></span>");
                    sb.Append("<div class='required'>" + list.Description + "</div></div></div>");
                }
                else if (list.ItemMode == 2)
                {
                    sb.Append("<div class='table-row formitem " + (list.MustInput == true ? "require" : "") + "' index='" + list.ID + "' type='2'><div class='form_q'><label for='item_" + list.ID + "'>" +
                        list.Title + (list.MustInput == true ? requirestr : "") + "</label></div>");
                    var colsstr = list.ColumnNum == null ? "" : "cols='" + list.ColumnNum + "'";
                    sb.Append("<div class='form_a'><textarea rows='" + (list.RowNum == null ? "" : list.RowNum.Value.ToString()) + "'  id ='item_" + list.ID +
                        "' class='form-control'  maxlength='" + list.TextLength + "' type='text' " + colsstr + " title='" + list.Title+ "' name='item_" + list.ID + "'>" +
                       (list.DefaultText == null ? "" : list.DefaultText) + "</textarea>");
                    if (erroritem.Contains(list.ID.ToString()))
                    {
                        sb.Append("<span  id='err_" + list.ID + "' class='error' >" + list.Title + langdict["mustinputstr"] + " ！</span>");
                    }
                    sb.Append("<div class='required'>" + list.Description + "</div></div></div>");
                }
                else if (list.ItemMode == 3)
                {
                    var items = list.SelList.Split('^');
                    var iidx = 1;
                    sb.Append("<div class='table-row formitem " + (list.MustInput == true ? "require" : "") + "'  index='" + list.ID + "' type='3'><div class='form_q'><label for='"+ iidx+"_item_" + list.ID + "'>" +
                        list.Title + (list.MustInput == true ? requirestr : "") + "</label></div>");
                    sb.Append("<div class='form_a'>");
                    
                    foreach (var item in items)
                    {
                        sb.Append("<label class='mt-radio mt-radio-outline' for='"+ iidx+"_item_" + list.ID+"'><input type='radio' name='item_" + list.ID + "' value='" + item + "' id='"+ iidx+"_item_" + list.ID + "'>" + item + "<span></span></label>");
                        iidx++;
                    }
                    if (erroritem.Contains(list.ID.ToString()))
                    {
                        sb.Append("<div id='err_" + list.ID + "' class='error'>" + list.Title + langdict["mustinputstr"] + " ！</div>");
                    }
                
                    sb.Append("<div class='required'>" + list.Description + "</div></div></div>");
                }
                else if (list.ItemMode == 4)
                {
                    var iidx = 1;
                    var items = list.SelList.Split('^');
                    sb.Append("<div class='table-row formitem " + (list.MustInput == true ? "require" : "") + "' index='" + list.ID + "' type='4'><div class='form_q'><label for='"+ iidx+"_item_" + list.ID + "'>"
                    + list.Title + (list.MustInput == true ? requirestr : "") + "</label></div>");
                    sb.Append("<div class='form_a'>");
                    foreach (var item in items)
                    {
                        sb.Append("<label class='mt-checkbox mt-checkbox-outline' for='"+ iidx+"_item_" + list.ID + "'><input type='checkbox' class='checkboxes' value='" + item + "'  id='"+ iidx+"_item_" + 
                            list.ID + "'  name='item_" + list.ID + "'>" + item + "<span></span></label>");
                        iidx++;
                    }
                    if (erroritem.Contains(list.ID.ToString()))
                    {
                        sb.Append("<div  id='err_" + list.ID + "' class='error' >" + list.Title + langdict["mustinputstr"] + " ！</div>");
                    }
                    sb.Append("<div class='required'>" + list.Description + "</div></div></div>");
                }
                else if (list.ItemMode == 5)
                {
                    var items = list.SelList.Split('^');
                    sb.Append("<div class='table-row formitem " + (list.MustInput == true ? "require" : "") + "' index='" + list.ID + "' type='5'><div class='form_q'><label for='item_" + list.ID + "'>" +
                        list.Title + (list.MustInput == true ? requirestr : "") + "</label></div>");
                    sb.Append("<div class='form_a'><select class='form-control w-auto'  id='item_" + list.ID + "'  name='item_" + list.ID + "'><option value=''>" + langdict["pleaseselectStr"] + "</option>");
                    foreach (var item in items)
                    {
                        sb.Append("<option value='" + item + "'>" + item + "</option>");
                    }
                    sb.Append("</select>");
                    if (erroritem.Contains(list.ID.ToString()))
                    {
                        sb.Append("<div id='err_" + list.ID + "' class='error'>" + list.Title + langdict["mustinputstr"] + " ！</div>");
                    }
                    sb.Append("<div class='required'>" + list.Description + "</div></div></div>");
                }

            }
           
            var imagestrArr = _ILoginManager.GetCaptchImage();
            sb.Append("<div class='table-row' id='div_captch'><div class='form_q'><label for='img_captch'>" + langdict["checkcode"]  + requirestr + "</label></div><div class='form_a'>");
            sb.Append("<input type = 'text'  class='form-control input-small float_left' placeholder='" + langdict["checkcodeinput"] + "'  id='img_captch' name='img_captch'>");
            sb.Append("<span class='verification_code'><img src='" + imagestrArr[1] + "' alt='" + langdict["checkcode"]  + "' id='image'><a  id='a_captchRefresh' style='cursor:pointer' title='"+
               langdict["refresh"]     + "' href='#ReCaptch'><i class='fa fa-refresh'  aria-hidden='true'></i>" + langdict["refresh"]  + "</a>");
            sb.Append("<a href='javascript:void(0)' data-voice='"+ imagestrArr[0] + "' title='" + langdict["audioplay"] + "' id='i_voice_play'><i class='fa fa-volume-up' id='i_voice' style='cursor:pointer'  aria-hidden='true'></i><span class='sr-only'>語音撥放</span></a></span>");
            if (erroritem.Contains("CatchStr"))
            {
                sb.Append("<div  id='err_captch' class='error'> " +langdict["checkcode"] + langdict["mustinputstr"]  + "！</div>");
            }
            else if (erroritem.Contains("CatchStr2"))
            {
                sb.Append("<div  id='err_captch' class='error'> " + langdict["CatchStr2"] + "！</div>");
            }
            sb.Append("</div></div>");
            return new string[] { sb.ToString(), imagestrArr[0] };
        }
        #endregion

    }
}
