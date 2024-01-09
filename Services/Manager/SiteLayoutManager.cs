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
using Utilities;
using System.Net;
using System.Xml.Linq;
using ViewModels.DBModels;
using System.Reflection;
using System.Web.Mvc;

namespace Services.Manager
{
    public class SiteLayoutManager : ISiteLayoutManager
    {
        readonly SQLRepository<SiteLayout> _sqlrepository;
        readonly SQLRepository<SiteConfig> _sqlconfigrepository;
        readonly SQLRepository<PageLayout> _pagelayoutsqlrepository;
        readonly SQLRepository<LangKey> _langkeysqlrepository;
        readonly SQLRepository<LangInputText> _langinputsqlrepository;
        readonly SQLRepository<Img> _imgsqlrepository;
        readonly SQLRepository<PageLayoutActivity> _pageLayoutActivityqlrepository;
        readonly SQLRepository<PageSeminar> _pageseminarsqlrepository;
        readonly SQLRepository<PageNewsEdit> _pagenewseditsqlrepository;
        readonly SQLRepository<PageActiveEdit> _pageactiveeditsqlrepository;
        readonly SQLRepository<PageJournalEdit> _pagejournalsqlrepository;
        readonly SQLRepository<MessageItem> _messagesqlrepository;
        readonly SQLRepository<ActiveItem> _activeitemsqlrepository;
        readonly SQLRepository<FileDownloadItem> _filedownloadsqlrepository;
        public SiteLayoutManager(SQLRepositoryInstances sqlinstance)
        {
            _sqlrepository = sqlinstance.SiteLayout;
            _imgsqlrepository = sqlinstance.Img;
            _pagelayoutsqlrepository = sqlinstance.PageLayout;
            _langkeysqlrepository = sqlinstance.LangKey;
            _langinputsqlrepository = sqlinstance.LangInputText;
            _pageLayoutActivityqlrepository = sqlinstance.PageLayoutActivity;
            _pageseminarsqlrepository = sqlinstance.PageSeminar;
            _pagenewseditsqlrepository = sqlinstance.PageNewsEdit;
            _pageactiveeditsqlrepository = sqlinstance.PageActiveEdit;
            _pagejournalsqlrepository = sqlinstance.PageJournalEdit;
            _messagesqlrepository = sqlinstance.MessageItem;
            _activeitemsqlrepository = sqlinstance.ActiveItem;
            _filedownloadsqlrepository = sqlinstance.FileDownloadItem;
            _sqlconfigrepository = sqlinstance.SiteConfig;
        }
        #region PagingMain
        public Paging<PageLayout> PagingMain(SearchModelBase model)
        {
            var Paging = new Paging<PageLayout>();
            var onepagecnt = model.Limit;
            var whereobj = new List<string>();
            var wherestr = new List<string>();
            wherestr.Add("Stype=@1");
            whereobj.Add(model.Key);
            wherestr.Add("LangID=@2");
            whereobj.Add(model.LangId);
            if (model.Search != "")
            {
                if (string.IsNullOrEmpty(model.Search) == false)
                {
                    wherestr.Add("Name like @3");
                    whereobj.Add("%" + model.Search + "%");
                }
            }
            var str = string.Join(" and ", wherestr);
            Paging.rows = _pagelayoutsqlrepository.GetDataUseWhereRange(str, whereobj.ToArray(), model.Offset, model.Limit, model.Sort);
            Paging.total = _pagelayoutsqlrepository.GetCountUseWhere(str, whereobj.ToArray());
            return Paging;
        }
        #endregion

        #region GetSiteLayout
        public SiteLayoutModel GetSiteLayout(string stype, string langid)
        {
            var data = _sqlrepository.GetByWhere("SType=@1 and LangID=@2", new object[] { stype, langid });
            if (data.Count() > 0)
            {
                var model = new SiteLayoutModel()
                {
                    FirstPageImageUrl = VirtualPathUtility.ToAbsolute("~/UploadImage/SiteLayout/" + data.First().FirstPageImgNameOri),
                    FirstPageImgNameThumb = data.First().FirstPageImgNameThumb,
                    FirstPageImgNameOri = data.First().FirstPageImgNameOri,
                    FirstPageImgShowName = data.First().FirstPageImgShowName,
                    HtmlContent = data.First().HtmlContent,
                    ID = data.First().ID,
                    InsidePageImageUrl = VirtualPathUtility.ToAbsolute("~/UploadImage/SiteLayout/" + data.First().InsidePageImgNameOri),
                    InsidePageImgNameOri = data.First().InsidePageImgNameOri,
                    InsidePageImgNameThumb = data.First().InsidePageImgNameThumb,
                    InsidePageImgShowName = data.First().InsidePageImgShowName,
                    LogoImageUrl = VirtualPathUtility.ToAbsolute("~/UploadImage/SiteLayout/" + data.First().LogoImgNameOri),
                    LogoImageUrlThumb = VirtualPathUtility.ToAbsolute("~/UploadImage/SiteLayout/" + data.First().LogoImgNameThumb),

                    LogoImgNameOri = data.First().LogoImgNameOri,
                    LogoImgNameThumb = data.First().LogoImgNameThumb,
                    LogoImgShowName = data.First().LogoImgShowName,

                    FowardImgNameOri = data.First().FowardImgNameOri,
                    FowardImgNameThumb = data.First().FowardImgNameThumb,
                    FowardImgShowName = data.First().FowardImgShowName,
                    FowardImageUrl = VirtualPathUtility.ToAbsolute("~/UploadImage/SiteLayout/" + data.First().FowardImgNameOri),
                    FowardHtmlContent = data.First().FowardHtmlContent,

                    PrintImgNameOri = data.First().PrintImgNameOri,
                    PrintImgNameThumb = data.First().PrintImgNameThumb,
                    PrintImgShowName = data.First().PrintImgShowName,
                    PrintImageUrl = data.First().PrintImgNameOri.IsNullorEmpty() ? "" : VirtualPathUtility.ToAbsolute("~/UploadImage/SiteLayout/" + data.First().PrintImgNameOri),
                    PrintHtmlContent = data.First().PrintHtmlContent,
                    Page404HtmlContent = data.First().Page404HtmlContent,
                    Page404Title = data.First().Page404Title,
                    SType = stype,
                    PublishContent = data.First().PublishContent,
                    InnerLogoImageUrl = VirtualPathUtility.ToAbsolute("~/UploadImage/SiteLayout/" + data.First().InnerLogoImgNameOri),
                    InnerLogoImageUrlThumb = VirtualPathUtility.ToAbsolute("~/UploadImage/SiteLayout/" + data.First().InnerLogoImgNameThumb),
                    InnerLogoImgNameOri = data.First().InnerLogoImgNameOri,
                    InnerLogoImgNameThumb = data.First().InnerLogoImgNameThumb,
                    InnerLogoImgShowName = data.First().InnerLogoImgShowName,
                };
                return model;
            }
            else
            {
                return new SiteLayoutModel();
            }
        }
        #endregion

        #region EditSiteLayout
        public string EditSiteLayout(SiteLayoutModel model)
        {

            var SiteLayout = new SiteLayout()
            {
                //FirstPageImgHeight = model.FirstPageImgHeight,
                ID = model.ID,
                LangID = int.Parse(model.LangID),
                FirstPageImgNameOri = model.FirstPageImgNameOri == null ? "" : model.FirstPageImgNameOri,
                FirstPageImgNameThumb = model.FirstPageImgNameThumb,
                FirstPageImgShowName = model.FirstPageImgShowName,
                HtmlContent = model.HtmlContent == null ? "" : model.HtmlContent,
                //InsidePageImgHeight = model.InsidePageImgHeight,
                InsidePageImgNameThumb = model.InsidePageImgNameThumb,
                InsidePageImgShowName = model.InsidePageImgShowName,
                InsidePageImgNameOri = model.InsidePageImgNameOri == null ? "" : model.InsidePageImgNameOri,
                //LogoImgHeight = model.LogoImgHeight,
                LogoImgNameOri = model.LogoImgNameOri == null ? "" : model.LogoImgNameOri,
                LogoImgNameThumb = model.LogoImgNameThumb,
                LogoImgShowName = model.LogoImgShowName,
                InnerLogoImgNameOri = model.InnerLogoImgNameOri == null ? "" : model.InnerLogoImgNameOri,
                InnerLogoImgNameThumb = model.InnerLogoImgNameThumb,
                InnerLogoImgShowName = model.InnerLogoImgShowName,
                PublishContent = model.PublishContent,
                SType = model.SType
            };
            var olddata = _sqlrepository.GetByWhere("ID=@1 and LangID=@2", new object[] { model.ID, model.LangID });
            var r = 0;
            if (olddata.Count() > 0)
            {

                r = _sqlrepository.Update(SiteLayout);
                if (r > 0)
                {
                    var oldfilename = olddata.First().LogoImgNameThumb;
                    var oldfileoriname = olddata.First().LogoImgNameOri;
                    var oldroot = System.Web.HttpContext.Current.Request.PhysicalApplicationPath +
                        "\\UploadImage\\SiteLayout\\" + oldfilename;
                    var oldoriroot = System.Web.HttpContext.Current.Request.PhysicalApplicationPath +
                      "\\UploadImage\\SiteLayout\\" + oldfileoriname;
                    if (model.LogoImgNameOri.IsNullorEmpty() || model.LogoImgNameOri != oldfileoriname)
                    {
                        //刪除舊檔案
                        if (System.IO.File.Exists(oldroot))
                        {
                            System.IO.File.Delete(oldroot);
                        }
                        if (System.IO.File.Exists(oldoriroot))
                        {
                            System.IO.File.Delete(oldoriroot);
                        }
                    }

                    oldfilename = olddata.First().InnerLogoImgNameThumb;
                    oldfileoriname = olddata.First().InnerLogoImgNameOri;
                    oldroot = System.Web.HttpContext.Current.Request.PhysicalApplicationPath +
                       "\\UploadImage\\SiteLayout\\" + oldfilename;
                    oldoriroot = System.Web.HttpContext.Current.Request.PhysicalApplicationPath +
                     "\\UploadImage\\SiteLayout\\" + oldfileoriname;
                    if (model.InnerLogoImgNameOri.IsNullorEmpty() || model.InnerLogoImgNameOri != oldfileoriname)
                    {
                        //刪除舊檔案
                        if (System.IO.File.Exists(oldroot))
                        {
                            System.IO.File.Delete(oldroot);
                        }
                        if (System.IO.File.Exists(oldoriroot))
                        {
                            System.IO.File.Delete(oldoriroot);
                        }
                    }
                }
            }
            else
            {
                r = _sqlrepository.Create(SiteLayout);
            }
            if (r > 0)
            {
                return "設定成功";
            }
            else
            {
                return "設定失敗";
            }
        }
        #endregion

        #region GetModel
        public PageLayoutModel GetModel(string title, string stype, string langid)
        {
            var model = new PageLayoutModel();
            model.SType = stype;
            var data = _pagelayoutsqlrepository.GetByWhere("Title=@1 and LangID=@2", new object[] { title, langid });
            if (data.Count() > 0)
            {
                model = new PageLayoutModel()
                {
                    BlockWidth = data.First().BlockWidth.ToString(),
                    Title = data.First().Title,
                    ID = data.First().ID,
                    ImageDesc = data.First().ImageDesc,
                    ImgNameOri = data.First().ImgNameOri,
                    ImgShowName = data.First().ImgShowName,
                    LangID = data.First().LangID,
                    LinkMode = data.First().LinkMode,
                    MenuItem = data.First().MenuItem,
                    MenuLevel1 = data.First().MenuLevel1,
                    MenuLevel2 = data.First().MenuLevel2,
                    MenuLevel3 = data.First().MenuLevel3,
                    ModelID = data.First().ModelID,
                    ModelItemID = data.First().ModelItemID,
                    OpenMode = data.First().OpenMode,
                    OpenModeCust = data.First().OpenModeCust,
                    SType = stype,
                    ModelItemList = data.First().ModelItemList
                };
            }
            model.ImageUrl = VirtualPathUtility.ToAbsolute("~/UploadImage/SiteLayout/" + model.ImgNameOri);
            return model;
        }
        #endregion

        #region Create
        public string Create(PageLayoutModel model, string langid, string account, string username)
        {

            //1.create message
            var datetime = DateTime.Now;
            model.SType = "P";
            //先將所有index+1
            var menucnt = _pagelayoutsqlrepository.GetByWhere("Stype=@1 and LangID=@2", new object[] { model.SType, langid }).Max(v => v.Sort);
            if (menucnt < 0 || menucnt == null) { menucnt = 0; }
            var sort = menucnt + 1;
            var maxid = _pagelayoutsqlrepository.GetDataCaculate("Max(ID)") + 1;
            var PageLayout = new PageLayout()
            {
                ID = maxid,
                ImageDesc = model.ImageDesc,
                ImageUrl = model.ImageUrl,
                ImgNameOri = model.ImgNameOri,
                ImgShowName = model.ImgShowName,
                LangID = model.LangID,
                LinkMode = 1,
                MenuItem = model.MenuItem,
                MenuLevel1 = model.MenuLevel1,
                MenuLevel2 = model.MenuLevel2,
                MenuLevel3 = model.MenuLevel3,
                ModelID = model.ModelID,
                ModelItemID = model.ModelItemID,
                OpenMode = model.OpenMode,
                OpenModeCust = model.OpenModeCust,
                Sort = sort,
                Status = true,
                Title = model.Title,
                UpdateDatetime = datetime,
                UpdateUser = account,
                SType = model.SType,
                ModelItemList = model.ModelItemList == null ? "" : model.ModelItemList
            };
            if (model.BlockWidth.IsNullorEmpty() == false)
            {
                PageLayout.BlockWidth = int.Parse(model.BlockWidth);
            }
            var r = _pagelayoutsqlrepository.Create(PageLayout);
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

        #region Update
        public string Update(PageLayoutModel model, string account, string username)
        {
            var olddata = _pagelayoutsqlrepository.GetByWhere("ID=@1", new object[] { model.ID });
            var oldfileoriname = olddata.First().ImgNameOri;
            var oldoriroot = System.Web.HttpContext.Current.Request.PhysicalApplicationPath +
              "\\UploadImage\\SiteLayout\\" + oldfileoriname;
            if (model.ImgNameOri.IsNullorEmpty())
            {
                model.ImgNameOri = "";
                model.ImgShowName = "";
            }
            //1.create message
            var datetime = DateTime.Now;
            //先將所有index+1
            var tr = _pagelayoutsqlrepository.Update("MenuLevel1=@1,MenuLevel2=@2,MenuLevel3=@3,ModelID=@4,ModelItemID=@5",
            "ID=@6", new object[] { DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, model.ID });

            var PageLayout = new PageLayout()
            {
                ID = model.ID,
                ImageDesc = model.ImageDesc,
                ImageUrl = model.ImageUrl,
                ImgNameOri = model.ImgNameOri,
                ImgShowName = model.ImgShowName,
                LangID = model.LangID,
                LinkMode = 1,
                MenuItem = model.MenuItem,
                MenuLevel1 = model.MenuLevel1,
                MenuLevel2 = model.MenuLevel2,
                MenuLevel3 = model.MenuLevel3,
                ModelID = model.ModelID,
                ModelItemID = model.ModelItemID,
                OpenMode = model.OpenMode,
                OpenModeCust = model.OpenModeCust == null ? "" : model.OpenModeCust,
                Title = model.Title = model.Title == null ? "" : model.Title,
                UpdateDatetime = datetime,
                UpdateUser = account,
                ModelItemList = model.ModelItemList == null ? "" : model.ModelItemList
            };

            var r = _pagelayoutsqlrepository.Update(PageLayout);
            if (r > 0)
            {
                if (model.ImgNameOri.IsNullorEmpty() || olddata.First().ImgNameOri != model.ImgNameOri)
                {
                    //刪除舊檔案
                    if (System.IO.File.Exists(oldoriroot))
                    {
                        System.IO.File.Delete(oldoriroot);
                    }
                }
                return "修改成功";
            }
            else
            {
                return "修改失敗";
            }

        }

        #endregion

        #region UpdateSeq
        public string UpdateSeq(int id, int seq, string langid, string type, string account, string username)
        {
            try
            {
                if (seq <= 0) { seq = 1; }
                var oldmodel = _pagelayoutsqlrepository.GetByWhere("ID=@1", new object[] { id });
                if (oldmodel.Count() == 0) { return "更新失敗"; }
                var diff = "";
                var stype = oldmodel.First().SType;
                diff = diff.TrimStart(',');
                var r = 0;
                if (oldmodel.First().Sort != seq)
                {
                    IList<PageLayout> mtseqdata = null;
                    mtseqdata = _pagelayoutsqlrepository.GetByWhere("Sort>@1 and Stype=@2 and LangID=@3", new object[] { seq, stype, langid }).OrderBy(v => v.Sort).ToArray();

                    var updatetime = DateTime.Now;
                    if (mtseqdata.Count() == 0)
                    {
                        var totalcnt = 0;
                        totalcnt = _pagelayoutsqlrepository.GetCountUseWhere("Stype=@1 and LangID=@2", new object[] { stype, langid });
                        seq = totalcnt;
                    }

                    var qseq = (seq > oldmodel.First().Sort) ? seq : oldmodel.First().Sort;
                    IList<PageLayout> ltseqdata = null;
                    ltseqdata = _pagelayoutsqlrepository.GetByWhere("Sort<=@1 and Stype=@2 and LangID=@3", new object[] { qseq, stype, langid }).OrderBy(v => v.Sort).ToArray();
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
                        _pagelayoutsqlrepository.Update("Sort=@1", "ID=@2", new object[] { tempmodel.Sort, tempmodel.ID });
                    }
                }
                //先取出大於目前seq的資料

                oldmodel.First().Sort = seq;
                r = _pagelayoutsqlrepository.Update("Sort=@1", "ID=@2", new object[] { oldmodel.First().Sort, oldmodel.First().ID });
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
                NLogManagement.SystemLogInfo("更新訊息管理排序失敗:" + " error:" + ex.Message);
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
                var oldroot = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\UploadImage\\SiteLayout\\";
                var stype = "";
                for (var idx = 0; idx < idlist.Length; idx++)
                {
                    var oldddata = _pagelayoutsqlrepository.GetByWhere("ID=@1", new object[] { idlist[idx] });
                    stype = oldddata.First().SType;
                    var entity = new PageLayout();
                    entity.ID = int.Parse(idlist[idx]);
                    r = _pagelayoutsqlrepository.Delete(entity);
                    if (r <= 0)
                    {
                        NLogManagement.SystemLogInfo("刪除SiteLayout失敗:ID=" + idlist[idx]);
                    }
                    else
                    {
                        if (System.IO.File.Exists(oldroot + "\\" + oldddata.First().ImgNameOri))
                        {
                            System.IO.File.Delete(oldroot + "\\" + oldddata.First().ImgNameOri);
                        }
                    }
                }
                var rstr = "";
                if (r >= 0)
                {
                    rstr = "刪除成功";
                }
                else
                {
                    NLogManagement.SystemLogInfo("刪除SiteLayout失敗:" + delaccount);
                    rstr = "刪除失敗";
                }
                var alldata = _pagelayoutsqlrepository.GetByWhere("SType=@1 and LangID=@2", new object[] { stype, langid }).OrderBy(v => v.Sort).ToArray();
                for (var idx = 1; idx <= alldata.Count(); idx++)
                {
                    var tempmodel = alldata[idx - 1];
                    tempmodel.Sort = idx;
                    _pagelayoutsqlrepository.Update("Sort=@1", "ID=@2", new object[] { idx, tempmodel.ID });
                }

                return rstr;
            }
            catch (Exception ex)
            {
                NLogManagement.SystemLogInfo("刪除訊息管理單元失敗:" + ex.Message);
                return "刪除失敗";
            }
        }
        #endregion

        #region EditFowardSiteLayout
        public string EditFowardSiteLayout(SiteLayoutModel model)
        {

            var r = 0;
            if (model.ID != -1)
            {
                var olddata = _sqlrepository.GetByWhere("ID=@1", new object[] { model.ID });
                var updatedata = olddata.First();
                var oldfilename = updatedata.FowardImgNameThumb;
                var oldfileoriname = updatedata.FowardImgNameOri;
                updatedata.FowardHtmlContent = model.FowardHtmlContent == null ? "" : model.FowardHtmlContent;
                if (string.IsNullOrEmpty(model.FowardImgNameOri))
                {
                    updatedata.FowardImgNameThumb = "";
                    updatedata.FowardImgShowName = "";
                    updatedata.FowardImgNameOri = "";
                }
                else
                {
                    if (updatedata.FowardImgNameOri != model.FowardImgNameOri)
                    {
                        updatedata.FowardImgNameOri = model.FowardImgNameOri;
                        updatedata.FowardImgNameThumb = model.FowardImgNameThumb;
                        updatedata.FowardImgShowName = model.FowardImgShowName;
                    }
                }
                r = _sqlrepository.Update(updatedata);
                if (r > 0)
                {

                    var oldroot = System.Web.HttpContext.Current.Request.PhysicalApplicationPath +
                        "\\UploadImage\\SiteLayout\\" + oldfilename;
                    var oldoriroot = System.Web.HttpContext.Current.Request.PhysicalApplicationPath +
                      "\\UploadImage\\SiteLayout\\" + oldfileoriname;
                    if (model.FowardImgNameOri.IsNullorEmpty() || model.FowardImgNameOri != oldfileoriname)
                    {
                        //刪除舊檔案
                        if (System.IO.File.Exists(oldroot))
                        {
                            System.IO.File.Delete(oldroot);
                        }
                        if (System.IO.File.Exists(oldoriroot))
                        {
                            System.IO.File.Delete(oldoriroot);
                        }
                    }
                }
            }
            else
            {
                var SiteLayout = new SiteLayout()
                {
                    ID = model.ID,
                    FowardHtmlContent = model.FowardHtmlContent,
                    FowardImgNameOri = model.FowardImgNameOri,
                    FowardImgNameThumb = model.FowardImgNameThumb,
                    FowardImgShowName = model.FowardImgShowName
                };
                r = _sqlrepository.Create(SiteLayout);
            }
            if (r > 0)
            {
                return "設定成功";
            }
            else
            {
                return "設定失敗";
            }
        }
        #endregion

        #region EditPrintSiteLayout
        public string EditPrintSiteLayout(SiteLayoutModel model)
        {
            var r = 0;
            if (model.ID != -1)
            {
                var olddata = _sqlrepository.GetByWhere("ID=@1", new object[] { model.ID });
                var updatedata = olddata.First();
                var oldfilename = updatedata.PrintImgNameThumb;
                var oldfileoriname = updatedata.PrintImgNameOri;
                updatedata.PrintHtmlContent = model.PrintHtmlContent == null ? "" : model.PrintHtmlContent;
                if (string.IsNullOrEmpty(model.PrintImgNameOri))
                {
                    updatedata.PrintImgNameThumb = "";
                    updatedata.PrintImgShowName = "";
                    updatedata.PrintImgNameOri = "";
                }
                else
                {
                    if (updatedata.PrintImgNameOri != model.PrintImgNameOri)
                    {
                        updatedata.PrintImgNameOri = model.PrintImgNameOri;
                        updatedata.PrintImgNameThumb = model.PrintImgNameThumb;
                        updatedata.PrintImgShowName = model.PrintImgShowName;
                    }
                }
                r = _sqlrepository.Update(updatedata);
                if (r > 0)
                {

                    var oldroot = System.Web.HttpContext.Current.Request.PhysicalApplicationPath +
                        "\\UploadImage\\SiteLayout\\" + oldfilename;
                    var oldoriroot = System.Web.HttpContext.Current.Request.PhysicalApplicationPath +
                      "\\UploadImage\\SiteLayout\\" + oldfileoriname;
                    if (model.PrintImgNameOri.IsNullorEmpty() || model.PrintImgNameOri != oldfileoriname)
                    {
                        //刪除舊檔案
                        if (System.IO.File.Exists(oldroot))
                        {
                            System.IO.File.Delete(oldroot);
                        }
                        if (System.IO.File.Exists(oldoriroot))
                        {
                            System.IO.File.Delete(oldoriroot);
                        }
                    }
                }
            }
            else
            {
                var SiteLayout = new SiteLayout()
                {
                    ID = model.ID,
                    PrintHtmlContent = model.PrintHtmlContent,
                    PrintImgNameOri = model.PrintImgNameOri,
                    PrintImgNameThumb = model.PrintImgNameThumb,
                    PrintImgShowName = model.PrintImgShowName
                };
                r = _sqlrepository.Create(SiteLayout);
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

        #region EditPage404SiteLayout
        public string EditPage404SiteLayout(SiteLayoutModel model)
        {
            var r = 0;
            if (model.ID != -1)
            {
                var olddata = _sqlrepository.GetByWhere("ID=@1", new object[] { model.ID });
                var updatedata = olddata.First();
                updatedata.Page404Title = model.Page404Title == null ? "" : model.Page404Title;
                updatedata.Page404HtmlContent = model.Page404HtmlContent == null ? "" : model.Page404HtmlContent;
                r = _sqlrepository.Update(updatedata);

            }
            else
            {
                var SiteLayout = new SiteLayout()
                {
                    ID = model.ID,
                    Page404HtmlContent = model.Page404HtmlContent,
                    Page404Title = model.Page404Title
                };
                r = _sqlrepository.Create(SiteLayout);
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

        #region GetSiteLangText
        public SiteLangTextModel GetSiteLangText(string langid)
        {
            var input = _langinputsqlrepository.GetByWhere("LangID=@1", new object[] { langid }).ToDictionary(v => v.LangTextID, v => v.Text);
            var data = _langkeysqlrepository.GetByWhere("GroupName=@1 and Used=1", new object[] { "BasicSetting" });
            if (data.Count() > 0)
            {
                var model = new SiteLangTextModel();
                model.GroupKey1 = data.Where(v => v.SubGroup == 1).OrderBy(v => v.Sort).ToDictionary(k => k.ID.Value.ToString(), i => i.Item);
                foreach (var key in data.Where(v => v.SubGroup == 1).OrderBy(v => v.Sort).Select(v => v.ID).ToArray())
                {
                    if (input.ContainsKey(key.Value))
                    {
                        model.Group1.Add(key.ToString(), input[key.Value]);
                    }
                    else
                    {
                        model.Group1.Add(key.ToString(), "");
                    }
                }

                model.GroupKey2 = data.Where(v => v.SubGroup == 2).OrderBy(v => v.Sort).ToDictionary(k => k.ID.Value.ToString(), i => i.Item);
                foreach (var key in data.Where(v => v.SubGroup == 2).OrderBy(v => v.Sort).Select(v => v.ID).ToArray())
                {
                    if (input.ContainsKey(key.Value))
                    {
                        model.Group2.Add(key.ToString(), input[key.Value]);
                    }
                    else
                    {
                        model.Group2.Add(key.ToString(), "");
                    }
                }

                model.GroupKey3 = data.Where(v => v.SubGroup == 3).OrderBy(v => v.Sort).ToDictionary(k => k.ID.Value.ToString(), i => i.Item);
                foreach (var key in data.Where(v => v.SubGroup == 3).OrderBy(v => v.Sort).Select(v => v.ID).ToArray())
                {
                    if (input.ContainsKey(key.Value))
                    {
                        model.Group3.Add(key.ToString(), input[key.Value]);
                    }
                    else
                    {
                        model.Group3.Add(key.ToString(), "");
                    }
                }

                model.GroupKey4 = data.Where(v => v.SubGroup == 4).OrderBy(v => v.Sort).ToDictionary(k => k.ID.Value.ToString(), i => i.Item);
                foreach (var key in data.Where(v => v.SubGroup == 4).OrderBy(v => v.Sort).Select(v => v.ID).ToArray())
                {
                    if (input.ContainsKey(key.Value))
                    {
                        model.Group4.Add(key.ToString(), input[key.Value]);
                    }
                    else
                    {
                        model.Group4.Add(key.ToString(), "");
                    }
                }

                model.GroupKey5 = data.Where(v => v.SubGroup == 5).OrderBy(v => v.Sort).ToDictionary(k => k.ID.Value.ToString(), i => i.Item);
                foreach (var key in data.Where(v => v.SubGroup == 5).OrderBy(v => v.Sort).Select(v => v.ID).ToArray())
                {
                    if (input.ContainsKey(key.Value))
                    {
                        model.Group5.Add(key.ToString(), input[key.Value]);
                    }
                    else
                    {
                        model.Group5.Add(key.ToString(), "");
                    }
                }
                model.GroupKey6 = data.Where(v => v.SubGroup == 6).OrderBy(v => v.Sort).ToDictionary(k => k.ID.Value.ToString(), i => i.Item);
                foreach (var key in data.Where(v => v.SubGroup == 6).OrderBy(v => v.Sort).Select(v => v.ID).ToArray())
                {
                    if (input.ContainsKey(key.Value))
                    {
                        model.Group6.Add(key.ToString(), input[key.Value]);
                    }
                    else
                    {
                        model.Group6.Add(key.ToString(), "");
                    }
                }
                return model;
            }
            else
            {
                return new SiteLangTextModel();
            }

        }
        #endregion

        #region SaveSiteLangText
        public string SaveSiteLangText(SiteLangTextModel model, string langid)
        {
            var input = _langinputsqlrepository.GetByWhere("LangID=@1", new object[] { langid }).ToDictionary(v => v.LangTextID.ToString(), v => v.Text);
            var r = 0;
            foreach (var g in model.Group1.Keys)
            {
                if (input.ContainsKey(g.ToString()))
                {
                    r = _langinputsqlrepository.Update("Text=@1", "LangID=@2 and LangTextID=@3", new object[] { model.Group1[g], langid, g });
                }
                else
                {
                    r = _langinputsqlrepository.Create(new LangInputText()
                    {
                        LangID = int.Parse(langid),
                        LangTextID = int.Parse(g),
                        Text = model.Group1[g]
                    });
                }
            }

            foreach (var g in model.Group2.Keys)
            {
                if (input.ContainsKey(g.ToString()))
                {
                    r = _langinputsqlrepository.Update("Text=@1", "LangID=@2 and LangTextID=@3", new object[] { model.Group2[g], langid, g });
                }
                else
                {
                    r = _langinputsqlrepository.Create(new LangInputText()
                    {
                        LangID = int.Parse(langid),
                        LangTextID = int.Parse(g),
                        Text = model.Group2[g]
                    });
                }
            }

            foreach (var g in model.Group3.Keys)
            {
                if (input.ContainsKey(g.ToString()))
                {
                    r = _langinputsqlrepository.Update("Text=@1", "LangID=@2 and LangTextID=@3", new object[] { model.Group3[g], langid, g });
                }
                else
                {
                    r = _langinputsqlrepository.Create(new LangInputText()
                    {
                        LangID = int.Parse(langid),
                        LangTextID = int.Parse(g),
                        Text = model.Group3[g]
                    });
                }
            }

            foreach (var g in model.Group4.Keys)
            {
                if (input.ContainsKey(g.ToString()))
                {
                    r = _langinputsqlrepository.Update("Text=@1", "LangID=@2 and LangTextID=@3", new object[] { model.Group4[g], langid, g });
                }
                else
                {
                    r = _langinputsqlrepository.Create(new LangInputText()
                    {
                        LangID = int.Parse(langid),
                        LangTextID = int.Parse(g),
                        Text = model.Group4[g]
                    });
                }
            }
            foreach (var g in model.Group5.Keys)
            {
                if (input.ContainsKey(g.ToString()))
                {
                    r = _langinputsqlrepository.Update("Text=@1", "LangID=@2 and LangTextID=@3", new object[] { model.Group5[g], langid, g });
                }
                else
                {
                    r = _langinputsqlrepository.Create(new LangInputText()
                    {
                        LangID = int.Parse(langid),
                        LangTextID = int.Parse(g),
                        Text = model.Group5[g]
                    });
                }
            }
            foreach (var g in model.Group6.Keys)
            {
                if (input.ContainsKey(g.ToString()))
                {
                    r = _langinputsqlrepository.Update("Text=@1", "LangID=@2 and LangTextID=@3", new object[] { model.Group6[g], langid, g });
                }
                else
                {
                    r = _langinputsqlrepository.Create(new LangInputText()
                    {
                        LangID = int.Parse(langid),
                        LangTextID = int.Parse(g),
                        Text = model.Group6[g]
                    });
                }
            }
            if (r > 0) { return ""; }
            else
            {
                return "設定失敗";
            }
        }
        #endregion

        #region GetPageSeminarModel
        public PageSeminarModel GetPageSeminarModel()
        {
            var oldmodel = _pageseminarsqlrepository.GetByWhere("LangID=@1", new object[] { "1" });
            if (oldmodel.Count() > 0)
            {
                return new PageSeminarModel()
                {
                    ActiveDate = oldmodel.First().ActiveDate == null ? "" : oldmodel.First().ActiveDate.Value.ToString("yyyy/MM/dd"),
                    BGStyle = oldmodel.First().BGStyle,
                    Introduction = oldmodel.First().Introduction == null ? "" : oldmodel.First().Introduction,
                    Link = oldmodel.First().Link == null ? "" : oldmodel.First().Link,
                    Title = oldmodel.First().Title == null ? "" : oldmodel.First().Title,
                    StyleUploadFileName = oldmodel.First().StyleUploadFileName == null ? "" : oldmodel.First().StyleUploadFileName,
                    StyleUploadFileNameOrg = oldmodel.First().StyleUploadFileNameOrg == null ? "" : oldmodel.First().StyleUploadFileNameOrg,
                    StyleUploadFileUrl = VirtualPathUtility.ToAbsolute("~/UploadImage/PageLayout/" + oldmodel.First().StyleUploadFileName),
                     ActiveMonth = oldmodel.First().ActiveDate == null ? "" : oldmodel.First().ActiveDate.Value.ToString("MM")
                };
            }
            else
            {
                return new PageSeminarModel() {
                     Link="",
                      ActiveMonth="",
                       BGStyle=1,
                        Title="",
                         
                };
            }
        }
        #endregion

        #region SavePageSeminar
        public string SavePageSeminar(PageSeminarModel model)
        {
            var oldmodel = _pageseminarsqlrepository.GetByWhere("LangID=@1", new object[] { "1" });
            if (oldmodel.Count() > 0)
            {
                var oldroot = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\UploadImage\\PageLayout\\";
                //刪除舊檔案
                if (model.StyleUploadFileName.IsNullorEmpty())
                {
                    if (System.IO.File.Exists(oldmodel.First().StyleUploadFilePath))
                    {
                        System.IO.File.Delete(oldmodel.First().StyleUploadFilePath);
                    }
                    model.StyleUploadFilePath = "";
                    model.StyleUploadFileName = "";
                }
                else
                {
                    if (oldmodel.First().StyleUploadFileName != model.StyleUploadFileName)
                    {
                        if (System.IO.File.Exists(oldmodel.First().StyleUploadFilePath))
                        {
                            System.IO.File.Delete(oldmodel.First().StyleUploadFilePath);
                        }
                    }
                }
                var savemodel = oldmodel.First();
                savemodel.ActiveDate = model.ActiveDate == null ? (DateTime?)null : DateTime.Parse(model.ActiveDate);
                savemodel.BGStyle = model.BGStyle;
                savemodel.Introduction = model.Introduction == null ? "" : model.Introduction;
                savemodel.LangID = 1;
                savemodel.Link = model.Link == null ? "" : model.Link;
                savemodel.StyleUploadFileName = model.StyleUploadFileName == null ? "" : model.StyleUploadFileName;
                savemodel.StyleUploadFileNameOrg = model.StyleUploadFileNameOrg == null ? "" : model.StyleUploadFileNameOrg;
                savemodel.StyleUploadFilePath = model.StyleUploadFilePath == null ? "" : model.StyleUploadFilePath;
                savemodel.Title = model.Title == null ? "" : model.Title;
                var r = _pageseminarsqlrepository.Update(savemodel);
                if (r < 0) { return "作業失敗"; }
            }
            else
            {
                var r = _pageseminarsqlrepository.Create(new PageSeminar()
                {
                    ActiveDate = model.ActiveDate == null ? (DateTime?)null : DateTime.Parse(model.ActiveDate),
                    BGStyle = model.BGStyle,
                    Introduction = model.Introduction == null ? "" : model.Introduction,
                    LangID = 1,
                    Link = model.Link == null ? "" : model.Link,
                    Title = model.Title == null ? "" : model.Title,
                    StyleUploadFileName = model.StyleUploadFileName == null ? "" : model.StyleUploadFileName,
                    StyleUploadFileNameOrg = model.StyleUploadFileNameOrg == null ? "" : model.StyleUploadFileNameOrg,
                    StyleUploadFilePath = model.StyleUploadFilePath == null ? "" : model.StyleUploadFilePath
                });
                if (r < 0) { return "作業失敗"; }
            }

            return "儲存成功";
            throw new NotImplementedException();
        }
        #endregion

        #region SavePageNewEdit
        public string SavePageNewEdit(PageNewsEditModel model)
        {
            var oldmodel = _pagenewseditsqlrepository.GetByWhere("LangID=@1", new object[] { "1" });
            if (oldmodel.Count() > 0) { 
                var savemodel = oldmodel.First();
                savemodel.LangID = 1;
                savemodel.ModelItemList = model.ModelItemList == null ? "" : model.ModelItemList;
                savemodel.SelModelID = model.SelModelID;
                savemodel.SelModelItemID = model.SelModelItemID;
                savemodel.Title = model.Title == null ? "" : model.Title;
                savemodel.TitleEng = model.TitleEng == null ? "" : model.TitleEng;
                savemodel.MoreInfo = model.MoreInfo == null ? "" : model.MoreInfo;
                var r = _pagenewseditsqlrepository.Update(savemodel);
                if (r < 0) { return "作業失敗"; }
            }
            else
            {
                var r = _pagenewseditsqlrepository.Create(new PageNewsEdit()
                {
                    LangID=1,
                     ModelItemList=model.ModelItemList==null?"": model.ModelItemList,
                      SelModelID=model.SelModelID,
                       SelModelItemID=model.SelModelItemID,
                        Title=model.Title==null?"": model.Title,
                         TitleEng = model.TitleEng == null ? "" : model.TitleEng
                });
                if (r < 0) { return "作業失敗"; }
            }

            return "儲存成功";
        }
        #endregion

        #region GetPageNewEdit
        public PageNewsEditModel GetPageNewEdit()
        {
            var oldmodel = _pagenewseditsqlrepository.GetByWhere("LangID=@1", new object[] { "1" });
            if (oldmodel.Count() > 0)
            {
                return new PageNewsEditModel()
                {
                    Title = oldmodel.First().Title == null ? "" : oldmodel.First().Title,
                     TitleEng = oldmodel.First().TitleEng == null ? "" : oldmodel.First().TitleEng,
                      SelModelID= oldmodel.First().SelModelID,
                       SelModelItemID= oldmodel.First().SelModelItemID,
                        ModelItemList = oldmodel.First().ModelItemList == null ? "" : oldmodel.First().ModelItemList,
                         MoreInfo= oldmodel.First().MoreInfo
                };
            }
            else
            {
                return new PageNewsEditModel();
            }
        }
        #endregion

        #region SavePageActiveEdit
        public string SavePageActiveEdit(PageActiveEditModel model)
        {
            var oldmodel = _pageactiveeditsqlrepository.GetByWhere("LangID=@1", new object[] { "1" });
            if (oldmodel.Count() > 0)
            {
                var savemodel = oldmodel.First();
                savemodel.LangID = 1;
                savemodel.ModelItemList = model.ModelItemList == null ? "" : model.ModelItemList;
                savemodel.SelModelID = model.SelModelID;
                savemodel.SelModelItemID = model.SelModelItemID;
                savemodel.Title = model.Title == null ? "" : model.Title;
                savemodel.TitleEng = model.TitleEng == null ? "" : model.TitleEng;
                savemodel.MoreInfo = model.ActiveMoreInfo == null ? "" : model.ActiveMoreInfo;
                var r = _pageactiveeditsqlrepository.Update(savemodel);
                if (r < 0) { return "作業失敗"; }
            }
            else
            {
                var r = _pageactiveeditsqlrepository.Create(new PageActiveEdit()
                {
                    LangID = 1,
                    ModelItemList = model.ModelItemList == null ? "" : model.ModelItemList,
                    SelModelID = model.SelModelID,
                    SelModelItemID = model.SelModelItemID,
                    Title = model.Title == null ? "" : model.Title,
                    TitleEng = model.TitleEng == null ? "" : model.TitleEng,
                    MoreInfo = model.ActiveMoreInfo == null ? "" : model.ActiveMoreInfo
                });
                if (r < 0) { return "作業失敗"; }
            }

            return "儲存成功";

        }
        #endregion

        #region GetPageActiveEdit
        public PageActiveEditModel GetPageActiveEdit()
        {
            var oldmodel = _pageactiveeditsqlrepository.GetByWhere("LangID=@1", new object[] { "1" });
            if (oldmodel.Count() > 0)
            {
                return new PageActiveEditModel()
                {
                    Title = oldmodel.First().Title == null ? "" : oldmodel.First().Title,
                    TitleEng = oldmodel.First().TitleEng == null ? "" : oldmodel.First().TitleEng,
                    SelModelID = oldmodel.First().SelModelID,
                    SelModelItemID = oldmodel.First().SelModelItemID,
                    ModelItemList = oldmodel.First().ModelItemList == null ? "" : oldmodel.First().ModelItemList,
                     ActiveMoreInfo= oldmodel.First().MoreInfo,
                };
            }
            else
            {
                return new PageActiveEditModel();
            }
        }
        #endregion

        #region SavePageActiveEdit
        public string SavePageJournalEdit(PageJournalEditModel model)
        {
            var oldmodel = _pagejournalsqlrepository.GetByWhere("LangID=@1", new object[] { "1" });
            if (oldmodel.Count() > 0)
            {
                var savemodel = oldmodel.First();
                savemodel.LangID = 1;
                savemodel.ModelItemList = model.ModelItemList == null ? "" : model.ModelItemList;
                savemodel.SelModelID = model.SelModelID;
                savemodel.SelModelItemID = model.SelModelItemID;
                savemodel.Title = model.Title == null ? "" : model.Title;
                savemodel.TitleEng = model.TitleEng == null ? "" : model.TitleEng;
                var r = _pagejournalsqlrepository.Update(savemodel);
                if (r < 0) { return "作業失敗"; }
            }
            else
            {
                var r = _pagejournalsqlrepository.Create(new PageJournalEdit()
                {
                    LangID = 1,
                    ModelItemList = model.ModelItemList == null ? "" : model.ModelItemList,
                    SelModelID = model.SelModelID,
                    SelModelItemID = model.SelModelItemID,
                    Title = model.Title == null ? "" : model.Title,
                    TitleEng = model.TitleEng == null ? "" : model.TitleEng
                });
                if (r < 0) { return "作業失敗"; }
            }

            return "儲存成功";
        }
        #endregion

        #region GetPageJournalEdit
        public PageJournalEditModel GetPageJournalEdit()
        {
            var oldmodel = _pagejournalsqlrepository.GetByWhere("LangID=@1", new object[] { "1" });
            if (oldmodel.Count() > 0)
            {
                 var model=new PageJournalEditModel()
                {
                    Title = oldmodel.First().Title == null ? "" : oldmodel.First().Title,
                    TitleEng = oldmodel.First().TitleEng == null ? "" : oldmodel.First().TitleEng,
                    SelModelID = oldmodel.First().SelModelID,
                    SelModelItemID = oldmodel.First().SelModelItemID,
                    ModelItemList = oldmodel.First().ModelItemList == null ? "" : oldmodel.First().ModelItemList
                };
                if (model.SelModelID == 2 && model.ModelItemList.IsNullorEmpty()==false) {
                    var selarr = model.ModelItemList.Split(',');
                    var oldmodeldata = _messagesqlrepository.GetByWhere("ModelId=@1", new object[] {
                       model.SelModelItemID
                   });
                    var existarr = selarr.Where(v => oldmodeldata.Any(x => x.ItemID.ToString() == v)).ToArray();
                    model.ModelItemList = existarr == null ? "" : string.Join(",", existarr);
                }
                else if (model.SelModelID == 3 && model.ModelItemList.IsNullorEmpty() == false)
                {
                    var selarr = model.ModelItemList.Split(',');
                    var oldmodeldata = _activeitemsqlrepository.GetByWhere("ModelId=@1", new object[] {
                       model.SelModelItemID
                   });
                    var existarr = selarr.Where(v => oldmodeldata.Any(x => x.ItemID.ToString() == v)).ToArray();
                    model.ModelItemList = existarr == null ? "" : string.Join(",", existarr);
                }
                else if (model.SelModelID == 4 && model.ModelItemList.IsNullorEmpty() == false)
                {
                    var selarr = model.ModelItemList.Split(',');
                    var oldmodeldata = _filedownloadsqlrepository.GetByWhere("ModelId=@1", new object[] {
                       model.SelModelItemID
                   });
                    var existarr = selarr.Where(v => oldmodeldata.Any(x => x.ItemID.ToString() == v)).ToArray();
                    model.ModelItemList = existarr == null ? "" : string.Join(",", existarr);
                }


                return model;
                //ModelItemList
            }
            else
            {
                return new PageJournalEditModel();
            }
        }
        #endregion

        public void SaveActiveMoreInfo(string url)
        {
            var olddata = _sqlrepository.GetAll();
            _sqlrepository.Update("ActiveMoreInfo=@1", "ID=@2", new object[] { url, olddata.First().ID });
        }
        public string GetActiveMoreInfo()
        {
            var olddata = _sqlrepository.GetAll();
            if (olddata.Count() > 0) { return olddata.First().ActiveMoreInfo; } else { return ""; }
        }

        public void SaveNewsMoreInfo(string url)
        {
            var olddata = _sqlrepository.GetAll();
            _sqlrepository.Update("NewsMoreInfo=@1", "ID=@2", new object[] { url, olddata.First().ID });
        }
        public string GetNewsMoreInfo()
        {
            var olddata = _sqlrepository.GetAll();
            if (olddata.Count() > 0) { return olddata.First().NewsMoreInfo; } else { return ""; }
        }
        public string SetNoJS(int isnojs)
        {
            _sqlconfigrepository.Update("IsNoJavascript=@1", "ID>@2", new object[] { isnojs ,-1});
            return "";
        }
    }
}
