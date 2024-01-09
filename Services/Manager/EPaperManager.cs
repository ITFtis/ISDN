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
using System.Web.Mvc;
using System.ServiceModel.Syndication;
using ViewModels.DBModels;

namespace Services.Manager
{
    public class EPaperManager : IEPaperManager
    {
        readonly SQLRepository<EPaper> _sqlrepository;
        readonly SQLRepository<EPaperAD> _adsqlrepository;
        readonly SQLRepository<EPaperUnitSetting> _unitsqlitemrepository;
        readonly SQLRepository<ColumnSetting> _columnsqlrepository;
        readonly SQLRepository<EPaperContent> _epapercontnetsqlrepository;
        readonly SQLRepository<Menu> _menusqlrepository;
        readonly SQLRepository<MessageItem> _messagesqlrepository;
        readonly SQLRepository<ArticleItem> _articlesqlrepository;
        readonly SQLRepository<ActiveItem> _activesqlrepository;
        readonly SQLRepository<EPaperItem> _epaperitemsqlrepository;
        readonly SQLRepository<FileDownloadItem> _filedownloaditemsqlrepository;
        readonly SQLRepository<ModelMessageMain> _messagemainsqlrepository;
        readonly SQLRepository<ModelArticleMain> _articlemainsqlrepository;
        readonly SQLRepository<ModelActiveEditMain> _activemainsqlrepository;
        readonly SQLRepository<ModelFileDownloadMain> _filedownloadsqlrepository;
        readonly SQLRepository<EPaperSubscriber> _epapersubscribersqlrepository;
        readonly SQLRepository<SiteConfig> _siteconfigqlrepository;
        public EPaperManager(SQLRepositoryInstances sqlinstance)
        {
            _sqlrepository = sqlinstance.EPaper;
            _adsqlrepository = sqlinstance.EPaperAD;
            _unitsqlitemrepository = sqlinstance.EPaperUnitSetting;
            _columnsqlrepository = sqlinstance.ColumnSetting;
            _epapercontnetsqlrepository = sqlinstance.EPaperContent;
            _messagesqlrepository = sqlinstance.MessageItem;
            _articlesqlrepository = sqlinstance.ArticleItem;
            _activesqlrepository = sqlinstance.ActiveItem;
            _menusqlrepository = sqlinstance.Menu;
            _epaperitemsqlrepository = sqlinstance.EPaperItem;
            _messagemainsqlrepository = sqlinstance.ModelMessageMain;
            _articlemainsqlrepository = sqlinstance.ModelArticleMain;
            _activemainsqlrepository = sqlinstance.ModelActiveEditMain;
            _epapersubscribersqlrepository = sqlinstance.EPaperSubscriber;
            _filedownloadsqlrepository = sqlinstance.ModelFileDownloadMain;
            _filedownloaditemsqlrepository = sqlinstance.FileDownloadItem;
            _siteconfigqlrepository = sqlinstance.SiteConfig;
        }

        #region GetModel
        public EPaperEditModel GetModel(string id)
        {
            var model = new EPaperEditModel();
            if (id != "-1")
            {
                var olddata = _sqlrepository.GetByWhere("ID=@1", new object[] { id });
                model.ID = olddata.First().ID;
                model.PaperMode = olddata.First().PaperMode.Value;
                model.PaperStyle = olddata.First().PaperStyle.Value;
                model.PublicshStr = olddata.First().PublicshStr;
                model.Title = olddata.First().Title;
                model.Introduction = olddata.First().Introduction;
                model.TopBannerImgUrl = VirtualPathUtility.ToAbsolute("~/UploadImage/EPaper/" + olddata.First().TopBannerImgName);
                model.TopBannerImgPath = olddata.First().TopBannerImgPath;
                model.TopBannerImgOrgName = olddata.First().TopBannerImgOrgName;
                model.TopBannerImgName = olddata.First().TopBannerImgName;
                model.PageEndHtmlContent = olddata.First().PageEndHtmlContent;
                model.Enabled = olddata.First().Enabled.Value;
                model.TopHtmlContent = olddata.First().TopHtmlContent;
                model.LeftHtmlContent = olddata.First().LeftHtmlContent;
                model.CenterHtmlContent = olddata.First().CenterHtmlContent;
                model.BottomHtmlContent = olddata.First().BottomHtmlContent;
                var addata = _adsqlrepository.GetByWhere("MainID=@1", new object[] { id }).OrderBy(v => v.ADIndex);
                var ADID = new List<string>();
                var ADName = new List<string>();
                var ADLink = new List<string>();
                var ADFileName = new List<string>();
                var ADFilePath = new List<string>();
                foreach (var a in addata)
                {
                    ADID.Add(a.ID.ToString());
                    ADName.Add(a.ADName == null ? "" : a.ADName);
                    ADLink.Add(a.ADLink == null ? "" : a.ADLink);
                    ADFileName.Add(a.ADFileName == null ? "" : a.ADFileName);
                    ADFilePath.Add(a.ADFilePath == null ? "" : a.ADFilePath);
                }
                model.ADFileName = ADFileName.ToArray();
                model.ADID = ADID.ToArray();
                model.ADLink = ADLink.ToArray();
                model.ADName = ADName.ToArray();
                model.ADFilePath = ADFilePath.ToArray();
                if (model.PaperMode == 1)
                {
                    var cmodel = _epapercontnetsqlrepository.GetByWhere("EID=@1", new object[] { id });
                    if (cmodel.Count() > 0)
                    {
                        model.EPaperContent = cmodel.First().EPaperHtmlContent;
                    }
                }
            }
            else
            {
                var addata = _adsqlrepository.GetByWhere("MainID=@1", new object[] { "-1" }).OrderBy(v => v.ADIndex);
                var ADID = new List<string>();
                var ADName = new List<string>();
                var ADLink = new List<string>();
                var ADFileName = new List<string>();
                var ADFilePath = new List<string>();
                foreach (var a in addata)
                {
                    ADID.Add(a.ID.ToString());
                    ADName.Add(a.ADName == null ? "" : a.ADName);
                    ADLink.Add(a.ADLink == null ? "" : a.ADLink);
                    ADFileName.Add(a.ADFileName == null ? "" : a.ADFileName);
                    ADFilePath.Add(a.ADFilePath == null ? "" : a.ADFilePath);
                }
                model.ADFileName = ADFileName.ToArray();
                model.ADID = ADID.ToArray();
                model.ADLink = ADLink.ToArray();
                model.ADName = ADName.ToArray();
                model.ADFilePath = ADFilePath.ToArray();
            }

            return model;
        }
        #endregion

        #region Create
        public string Create(EPaperEditModel model, string account, string langid)
        {
            var basepath = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\UploadImage\\EPaperAD\\";
            if (System.IO.Directory.Exists(basepath) == false) { System.IO.Directory.CreateDirectory(basepath); }
            var savemodel = new EPaper()
            {
                BottomHtmlContent = model.BottomHtmlContent == null ? "" : model.BottomHtmlContent,
                CenterHtmlContent = model.CenterHtmlContent == null ? "" : model.CenterHtmlContent,
                Introduction = model.Introduction == null ? "" : model.Introduction,
                LeftHtmlContent = model.LeftHtmlContent == null ? "" : model.LeftHtmlContent,
                PageEndHtmlContent = model.PageEndHtmlContent == null ? "" : model.PageEndHtmlContent,
                PaperMode = model.PaperMode,
                PaperStyle = model.PaperStyle,
                PublicshStr = model.PublicshStr,
                Title = model.Title == null ? "" : model.Title,
                TopBannerImgName = model.TopBannerImgName,
                TopBannerImgOrgName = model.TopBannerImgOrgName,
                TopBannerImgPath = model.TopBannerImgPath,
                TopHtmlContent = model.TopHtmlContent == null ? "" : model.TopHtmlContent,
                UpdateDatetime = DateTime.Now,
                UpdateUser = account,
                IsEdit = false,
                Enabled = true,
                Sort = 1,
                LangID = int.Parse(langid),
                 CreareDatetime=DateTime.Now
            };
            var alldata = _sqlrepository.GetByWhere("LangID=@1", new object[] { langid }).OrderBy(v => v.Sort);
            var r = _sqlrepository.Create(savemodel);
            if (r > 0)
            {

                var sidx = 2;
                foreach (var mdata in alldata)
                {
                    mdata.Sort = sidx;
                    _sqlrepository.Update("Sort=@1", "ID=@2", new object[] { mdata.Sort, mdata.ID });
                    sidx += 1;
                }

                var newid = savemodel.ID;
                if (model.ADName != null)
                {
                    for (var idx = 0; idx < model.ADName.Length; idx++)
                    {
                        var smodel = new EPaperAD();
                        var adid = model.ADID[idx];
                        smodel.ADName = model.ADName[idx] == null ? "" : model.ADName[idx];
                        smodel.ADLink = model.ADLink[idx] == null ? "" : model.ADLink[idx];
                        smodel.ADIndex = idx;
                        smodel.LangID = savemodel.LangID.Value;
                        smodel.MainID = newid;
                        smodel.ADFileName = "";
                        if (adid == "-1")
                        {
                            if (model.ADImageFiles != null && model.ADImageFiles[idx] != null)
                            {
                                smodel.ADFileName = model.ADImageFiles[idx].FileName.Split('\\').Last();
                            }

                        }
                        r = _adsqlrepository.Create(smodel);
                        if (adid == "-1")
                        {
                            if (model.ADImageFiles != null && model.ADImageFiles[idx] != null)
                            {
                                if (r > 0)
                                {
                                    var path = (smodel.MainID) + "_" + (smodel.ID) + "_" + (idx) + "." + smodel.ADFileName.Split('.').Last();
                                    model.ADImageFiles[idx].SaveAs(basepath + path);
                                    _adsqlrepository.Update("ADFilePath=@1", "ID=@2", new object[] { path, smodel.ID });
                                }
                            }
                        }
                        else
                        {
                            var oldmodel = _adsqlrepository.GetByWhere("ID=@1", new object[] { adid }).First();
                            _adsqlrepository.Update("ADFileName=@1", "ID=@2", new object[] { oldmodel.ADFileName, smodel.ID });
                            //複製檔案
                            var filepath = (smodel.MainID) + "_" + (smodel.ID) + "_" + (idx) + "." + oldmodel.ADFileName.Split('.').Last();
                            var copytopath = basepath + filepath;
                            var copyfrompath = basepath + oldmodel.ADFilePath;
                            if (System.IO.File.Exists(copyfrompath))
                            {
                                System.IO.File.Copy(copyfrompath, copytopath, true);
                                _adsqlrepository.Update("ADFileName=@1,ADFilePath=@2", "ID=@3", new object[] { oldmodel.ADFileName, filepath, smodel.ID });
                            }
                        }

                    }
                }

                return "新增成功";
            }
            else
            {
                return "新增失敗";
            }


        }
        #endregion

        #region Update
        public string Update(EPaperEditModel model, string account, string langid)
        {
            var olddata = _sqlrepository.GetByWhere("ID=@1", new object[] { model.ID });
            var oldaddata = _adsqlrepository.GetByWhere("MainID=@1", new object[] { model.ID });
            var oldroot = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\UploadImage\\EPaper\\";
            var basepath = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\UploadImage\\EPaperAD\\";
            //刪除舊檔案
            if (model.TopBannerImgName.IsNullorEmpty())
            {

                if (System.IO.File.Exists(oldroot + "\\" + olddata.First().TopBannerImgName))
                {
                    System.IO.File.Delete(oldroot + "\\" + olddata.First().TopBannerImgName);
                }
                model.TopBannerImgOrgName = "";
                model.TopBannerImgPath = "";
                model.TopBannerImgName = "";
            }
            else
            {

                if (olddata.First().TopBannerImgName != model.TopBannerImgName)
                {
                    if (System.IO.File.Exists(oldroot + "\\" + olddata.First().TopBannerImgName))
                    {
                        System.IO.File.Delete(oldroot + "\\" + olddata.First().TopBannerImgName);
                    }
                }
            }

            var savemodel = new EPaper()
            {
                BottomHtmlContent = model.BottomHtmlContent == null ? "" : model.BottomHtmlContent,
                CenterHtmlContent = model.CenterHtmlContent == null ? "" : model.CenterHtmlContent,
                Introduction = model.Introduction == null ? "" : model.Introduction,
                LeftHtmlContent = model.LeftHtmlContent == null ? "" : model.LeftHtmlContent,
                PageEndHtmlContent = model.PageEndHtmlContent == null ? "" : model.PageEndHtmlContent,
                PaperMode = model.PaperMode,
                PaperStyle = model.PaperStyle,
                PublicshStr = model.PublicshStr,
                Title = model.Title == null ? "" : model.Title,
                TopBannerImgName = model.TopBannerImgName,
                TopBannerImgOrgName = model.TopBannerImgOrgName,
                TopBannerImgPath = model.TopBannerImgPath,
                TopHtmlContent = model.TopHtmlContent == null ? "" : model.TopHtmlContent,
                UpdateDatetime = DateTime.Now,
                UpdateUser = account,
                ID = model.ID
            };


            var r = _sqlrepository.Update(savemodel);
            if (r > 0)
            {
                _siteconfigqlrepository.Update("LastUpdateDate=@1", "", new object[] { DateTime.Now.ToString("yyyy/MM/dd") });
                if (model.ADName != null)
                {
                    for (var idx = 0; idx < model.ADName.Length; idx++)
                    {
                        var smodel = new EPaperAD();
                        smodel.MainID = model.ID;
                        smodel.ADName = model.ADName[idx] == null ? "" : model.ADName[idx];
                        smodel.ADLink = model.ADLink[idx] == null ? "" : model.ADLink[idx];
                        smodel.ADIndex = idx;
                        smodel.LangID = int.Parse(langid);
                        if (model.ADID[idx] == "-1")
                        {
                            if (model.ADImageFiles[idx] != null)
                            {
                                smodel.ADFileName = model.ADImageFiles[idx].FileName.Split('\\').Last();
                            }
                            r = _adsqlrepository.Create(smodel);
                            if (model.ADImageFiles[idx] != null)
                            {
                                if (r > 0)
                                {
                                    var path = (smodel.MainID) + "_" + (smodel.ID) + "_" + (idx) + "." + smodel.ADFileName.Split('.').Last();
                                    model.ADImageFiles[idx].SaveAs(basepath + path);
                                    _adsqlrepository.Update("ADFilePath=@1", "ID=@2", new object[] { path, smodel.ID });
                                }
                            }
                        }
                        else
                        {
                            var oldad = oldaddata.Where(v => v.ID == int.Parse(model.ADID[idx]));
                            if (oldad.Count() > 0)
                            {
                                smodel.ID = oldad.First().ID;
                                if (model.ADImageFiles[idx] != null)
                                {
                                    //刪除舊檔案
                                    if (System.IO.File.Exists(basepath + "\\" + oldad.First().ADFilePath))
                                    {
                                        System.IO.File.Delete(basepath + "\\" + oldad.First().ADFilePath);
                                    }
                                    smodel.ADFileName = model.ADImageFiles[idx].FileName.Split('\\').Last();
                                    r = _adsqlrepository.Update(smodel);
                                    if (model.ADImageFiles[idx] != null)
                                    {
                                        if (r > 0)
                                        {
                                            var path = (smodel.MainID) + "_" + (smodel.ID) + "_" + (idx) + "." + smodel.ADFileName.Split('.').Last();
                                            model.ADImageFiles[idx].SaveAs(basepath + path);
                                            _adsqlrepository.Update("ADFilePath=@1", "ID=@2", new object[] { path, smodel.ID });
                                        }
                                    }
                                }
                                else
                                {
                                    if (model.ADFilePath[idx] == "")
                                    {
                                        if (System.IO.File.Exists(basepath + "\\" + oldad.First().ADFilePath))
                                        {
                                            System.IO.File.Delete(basepath + "\\" + oldad.First().ADFilePath);
                                        }
                                        smodel.ADFileName = "";
                                        smodel.ADFilePath = "";
                                    }
                                    else
                                    {
                                        smodel.ADFileName = oldad.First().ADFileName;
                                        smodel.ADFilePath = oldad.First().ADFilePath;
                                    }
                                    r = _adsqlrepository.Update(smodel);
                                }
                                oldaddata = oldaddata.Where(v => v.ID != smodel.ID);
                            }
                            else
                            {
                                if (model.ADImageFiles[idx] != null)
                                {
                                    smodel.ADFileName = model.ADImageFiles[idx].FileName.Split('\\').Last();
                                }
                                r = _adsqlrepository.Create(smodel);
                                if (model.ADImageFiles[idx] != null)
                                {
                                    if (r > 0)
                                    {
                                        var path = (smodel.MainID) + "_" + (smodel.ID) + "_" + (idx) + "." + smodel.ADFileName.Split('.').Last();
                                        model.ADImageFiles[idx].SaveAs(basepath + path);
                                        _adsqlrepository.Update("ADFilePath=@1", "ID=@2", new object[] { path, smodel.ID });
                                    }
                                }
                            }
                        }
                    }
                }

                foreach (var oad in oldaddata)
                {
                    _adsqlrepository.DelDataUseWhere("ID=@1", new object[] { oad.ID });
                }
                return "修改成功";
            }
            else
            {
                return "修改失敗";
            }
        }
        #endregion

        #region PagingAdminItem
        public Paging<EPaperItemResult> PagingAdminItem(SearchModelBase model)
        {
            var Paging = new Paging<EPaperItemResult>();
            var data = new List<EPaper>();

            var whereobj = new List<object>();
            var wherestr = new List<string>();
            var idx = 1;
            wherestr.Add("LangID=@1");
            whereobj.Add(model.LangId);
            if (model.Status.IsNullorEmpty() == false) {
                wherestr.Add("IsEdit=@" + idx);
                wherestr.Add("Enabled=@" + idx);
                whereobj.Add("1");
                idx += 1;
            }
            if (string.IsNullOrEmpty(model.Key)==false)
            {
                var syear = model.Key + "/01/01";
                var eyear = model.Key + "/12/31";
                idx += 1;
                wherestr.Add("CreareDatetime>=@" + idx);
                whereobj.Add(syear);
                idx += 1;
                wherestr.Add("CreareDatetime<=@" + idx);
                whereobj.Add(eyear);
            }
            if (wherestr.Count() > 0)
            {
                var str = string.Join(" and ", wherestr);
                data = _sqlrepository.GetDataUseWhereRange(str, whereobj.ToArray(), model.Offset, model.Limit, model.Sort).ToList();
                Paging.total = _sqlrepository.GetCountUseWhere(str, whereobj.ToArray());

            }
            else
            {
                Paging.total = _sqlrepository.GetAllDataCount();
                data = _sqlrepository.GetAllDataRange(model.Offset, model.Limit, model.Sort).ToList();
            }
            foreach (var d in data)
            {

                Paging.rows.Add(new EPaperItemResult()
                {
                    ID = d.ID,
                    Title = d.Title,
                    Enabled = d.Enabled,
                    IsPublicshStr = d.IsEdit == true ? "已發佈" : ("<button class='btn blue isedit' id='editbtn_" + d.ID + "' value='" + d.ID + "'>發佈</button>"),
                    Sort = d.Sort.Value,
                    FormatStr = d.PaperMode == 1 ? "手動" : "自動",
                    IsEdit = d.IsEdit,
                    PublicshStr = d.PublicshStr,
                    Introduction = d.Introduction,
                    //IsPublicshStr= d.PublicshStr.IsNullorEmpty()?"未發佈":(DateTime.Now>=DateTime.Parse(d.PublicshStr)? "已發佈" : "未發佈"),
                    Year=d.CreareDatetime==null?"":d.CreareDatetime.Value.ToString("yyyy")
                });
            }
            return Paging;
        }
        #endregion

        #region PagingItem
        public Paging<EPaperItemResult> PagingItem(SearchModelBase model)
        {
            var Paging = new Paging<EPaperItemResult>();
            var data = new List<EPaper>();

            var whereobj = new List<object>();
            var wherestr = new List<string>();
            var idx = 1;
            wherestr.Add("LangID=@1");
            whereobj.Add(model.LangId);
            wherestr.Add("IsEdit=1");
            wherestr.Add("Enabled=1");
            if (wherestr.Count() > 0)
            {
                var str = string.Join(" and ", wherestr);
                if (model.Limit == -1)
                {
                    data = _sqlrepository.GetByWhere(str + " order by " + model.Sort, whereobj.ToArray()).ToList();
                }
                else
                {
                    data = _sqlrepository.GetDataUseWhereRange(str, whereobj.ToArray(), model.Offset, model.Limit, model.Sort).ToList();
                }


                Paging.total = _sqlrepository.GetCountUseWhere(str, whereobj.ToArray());

            }
            else
            {
                Paging.total = _sqlrepository.GetAllDataCount();
                data = _sqlrepository.GetAllDataRange(model.Offset, model.Limit, model.Sort).ToList();
            }
            foreach (var d in data)
            {

                Paging.rows.Add(new EPaperItemResult()
                {
                    ID = d.ID,
                    Title = d.Title,
                    Enabled = d.Enabled,
                    IsEditStr = d.IsEdit == true ? "完成" : ("<button class='btn blue isedit' id='editbtn_" + d.ID + "' value='" + d.ID + "'>確定</button>"),
                    Sort = d.Sort.Value,
                    FormatStr = d.PaperMode == 1 ? "手動" : "自動",
                    IsEdit = d.IsEdit,
                    PublicshStr = d.PublicshStr,
                    Introduction = d.Introduction
                });
            }
            return Paging;
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
                    IList<EPaper> mtseqdata = null;
                    mtseqdata = _sqlrepository.GetByWhere("Sort>@1 and LangID=@2", new object[] { seq, langid }).OrderBy(v => v.Sort).ToArray();

                    var updatetime = DateTime.Now;
                    if (mtseqdata.Count() == 0)
                    {
                        var totalcnt = 0;
                        totalcnt = _sqlrepository.GetCountUseWhere("LangID=@1", new object[] { langid });
                        seq = totalcnt;
                    }

                    var qseq = (seq > oldmodel.First().Sort) ? seq : oldmodel.First().Sort;
                    IList<EPaper> ltseqdata = null;
                    ltseqdata = _sqlrepository.GetByWhere("Sort<=@1  and LangID=@2", new object[] { qseq, langid }).OrderBy(v => v.Sort).ToArray();
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
                NLogManagement.SystemLogInfo("更新EPaper排序失敗:" + " error:" + ex.Message);
                return "更新EPaper排序失敗:" + " error:" + ex.Message;
            }
        }
        #endregion

        #region SetIsEdit
        public string SetIsEdit(string id, bool status, string account, string username)
        {
            try
            {
                var entity = new EPaper();
                entity.IsEdit = status ? true : false;
                entity.ID = int.Parse(id);
                var r = _sqlrepository.Update(entity);
                if (r >= 0)
                {
                    NLogManagement.SystemLogInfo("修改IsEdit顯示狀態 id=" + id + " 為:" + status);
                    return "更新成功";
                }
                else
                {
                    return "更新失敗";
                }

            }
            catch (Exception ex)
            {
                NLogManagement.SystemLogInfo("修改IsEdit顯示狀態失敗:" + ex.Message);
                return "更新失敗";
            }
        }
        #endregion

        #region Delete
        public string Delete(string[] idlist, string delaccount, string langid, string account, string username)
        {
            try
            {
                var r = 0;
                var oldroot = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\UploadImage\\EPaper\\";
                var adoldroot = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\UploadImage\\EPaperAD\\";
                for (var idx = 0; idx < idlist.Length; idx++)
                {
                    var olddata = _sqlrepository.GetByWhere("ID=@1", new object[] { idlist[idx] });
                    var entity = new EPaper();
                    entity.ID = int.Parse(idlist[idx]);
                    r = _sqlrepository.Delete(entity);
                    if (r <= 0)
                    {
                        NLogManagement.SystemLogInfo("刪除電子報單元失敗:ID=" + idlist[idx]);
                    }
                    if (olddata.Count() > 0)
                    {

                    }
                    if (olddata.First().TopBannerImgName.IsNullorEmpty() == false)
                    {
                        if (System.IO.File.Exists(oldroot + olddata.First().TopBannerImgName))
                        {
                            System.IO.File.Delete(oldroot + olddata.First().TopBannerImgName);
                        }
                    }
                    var addata = _adsqlrepository.GetByWhere("MainID=@1", new object[] { idlist[idx] });
                    foreach (var a in addata)
                    {
                        if (a.ADFilePath.IsNullorEmpty() == false)
                        {
                            if (System.IO.File.Exists(adoldroot + a.ADFilePath))
                            {
                                System.IO.File.Delete(adoldroot + a.ADFilePath);
                            }
                        }
                        _adsqlrepository.DelDataUseWhere("ID=@1", new object[] { a.ID });
                    }

                }
                var rstr = "";
                if (r >= 0)
                {
                    NLogManagement.SystemLogInfo("刪除電子報單元失敗:" + delaccount);
                    rstr = "刪除成功";
                }
                else
                {
                    rstr = "刪除失敗";
                }
                var alldata = _sqlrepository.GetByWhere("LangID=@1", new object[] { langid }).OrderBy(v => v.Sort).ToArray();
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
                NLogManagement.SystemLogInfo("刪除電子報單元失敗:" + ex.Message);
                return "刪除失敗";
            }
        }
        #endregion

        #region SetItemStatus
        public string SetItemStatus(string id, bool status, string account, string username)
        {
            try
            {
                var entity = new EPaper();
                entity.Enabled = status ? true : false;
                entity.ID = int.Parse(id);
                var r = _sqlrepository.Update("Enabled=@1","ID=@2", new object[] { entity.Enabled, entity.ID  });
                if (r >= 0)
                {
                    NLogManagement.SystemLogInfo("修改EPaper顯示狀態 id=" + id + " 為:" + status);
                    return "更新成功";
                }
                else
                {
                    return "更新失敗";
                }

            }
            catch (Exception ex)
            {
                NLogManagement.SystemLogInfo("修改EPaper顯示狀態失敗:" + ex.Message);
                return "更新失敗";
            }
        }
        #endregion
        //unit
        #region GetUnitModel
        public EPaperUnitSettingModel GetUnitModel(string langid)
        {
            var data = _unitsqlitemrepository.GetByWhere("LangID=@1", new object[] { langid });
            var model = new EPaperUnitSettingModel();
            model.ID = -1;
            if (data.Count() > 0)
            {
                model = new EPaperUnitSettingModel()
                {
                    FrontPagePath = data.First().FrontPagePath,
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
                    IsRSS = data.First().IsRSS,
                    ShowCount = data.First().ShowCount,
                    ID = data.First().ID,
                     Summary = data.First().Summary
                };
                var cs = data.First().ColumnSetting;
                if (cs.IsNullorEmpty() == false)
                {
                    var csarr = cs.Split('@');
                    var cname = csarr[0].Split(',');
                    var cuse = csarr[1].Split(',');
                    for (var v = 0; v < cname.Length; v++)
                    {
                        model.UnitSettingColumnList.Add(new UnitSettingColumn()
                        {
                            Name = cname[v],
                            Sellected = int.Parse(cuse[v])
                        });
                    }
                }
            }
            if (model.UnitSettingColumnList.Count() == 0)
            {
                var columnlist = _columnsqlrepository.GetByWhere("Type='EPaper'", null).OrderBy(v => v.Sort);
                foreach (var c in columnlist)
                {
                    model.UnitSettingColumnList.Add(new UnitSettingColumn()
                    {
                        Name = c.ColumnName,
                        Sellected = 1
                    });
                }
            }
            model.ColumnNameMapping = new Dictionary<string, string>();
            model.ColumnNameMapping.Add("序號", model.Column1.IsNullorEmpty() ? "序號" : model.Column1);
            model.ColumnNameMapping.Add("發佈日期", model.Column2.IsNullorEmpty() ? "發佈日期" : model.Column2);
            model.ColumnNameMapping.Add("電子報名稱", model.Column3.IsNullorEmpty() ? "電子報名稱" : model.Column3);
            model.ColumnNameMapping.Add("年份", model.Column4.IsNullorEmpty() ? "年份" : model.Column4);
            model.ColumnNameMapping.Add("電子報訂閱", model.Column5.IsNullorEmpty() ? "電子報訂閱" : model.Column5);
            model.ColumnNameMapping.Add("訂閱", model.Column6.IsNullorEmpty() ? "訂閱" : model.Column6);
            model.ColumnNameMapping.Add("取消訂閱", model.Column7.IsNullorEmpty() ? "取消訂閱" : model.Column7);
            model.ColumnNameMapping.Add("查閱電子報", model.Column8.IsNullorEmpty() ? "查閱電子報" : model.Column8);
            model.ColumnNameMapping.Add("Email", model.Column9.IsNullorEmpty() ? "Email" : model.Column9);
            model.ColumnNameMapping.Add("Email 格式有誤", model.Column10.IsNullorEmpty() ? "Email 格式有誤" : model.Column10);
            model.ColumnNameMapping.Add("此 Email 已有訂閱電子報!", model.Column11.IsNullorEmpty() ? "此 Email 已有訂閱電子報!" : model.Column11);
            model.ColumnNameMapping.Add("電子報訂閱成功!", model.Column12.IsNullorEmpty() ? "電子報訂閱成功!" : model.Column12);
            model.ColumnNameMapping.Add("此 Email 無訂閱電子報!", model.Column13.IsNullorEmpty() ? "此 Email 無訂閱電子報!" : model.Column13);
            model.ColumnNameMapping.Add("電子報取消訂閱成功!", model.Column14.IsNullorEmpty() ? "電子報取消訂閱成功!" : model.Column14);
            return model;
        }
        #endregion

        #region ColumnPaging
        public Paging<ColumnSetting> ColumnPaging(SearchModelBase model)
        {
            var Paging = new Paging<ColumnSetting>();
            var onepagecnt = model.Limit;
            var whereobj = new List<string>();
            var wherestr = new List<string>();
            wherestr.Add("Type='EPaper'");
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
            Paging.rows = _columnsqlrepository.GetDataUseWhereRange(str, whereobj.ToArray(), model.Offset, model.Limit, model.Sort);
            Paging.total = _columnsqlrepository.GetCountUseWhere(str, whereobj.ToArray());
            return Paging;
        }
        #endregion

        #region UpdateColumnStatus
        public string UpdateColumnStatus(string id, bool status, string account, string username)
        {
            try
            {
                var entity = new ColumnSetting();
                entity.Show = status ? true : false;
                entity.ID = int.Parse(id);
                var r = _columnsqlrepository.Update(entity);
                if (r >= 0)
                {
                    NLogManagement.SystemLogInfo("修改ColumnSetting顯示狀態 id=" + id + " 為:" + status);
                    return "更新成功";
                }
                else
                {
                    return "更新失敗";
                }

            }
            catch (Exception ex)
            {
                NLogManagement.SystemLogInfo("修改ColumnSetting顯示狀態失敗:" + ex.Message);
                return "更新失敗";
            }
        }
        #endregion

        #region UpdateColumnSeq
        public string UpdateColumnSeq(int id, int seq, string account, string username)
        {
            try
            {
                if (seq <= 0) { seq = 1; }
                var oldmodel = _columnsqlrepository.GetByWhere("ID=@1", new object[] { id });
                if (oldmodel.Count() == 0) { return "更新失敗"; }
                var diff = "";

                diff = diff.TrimStart(',');
                var r = 0;
                if (oldmodel.First().Sort != seq)
                {
                    IList<ColumnSetting> mtseqdata = null;
                    mtseqdata = _columnsqlrepository.GetByWhere("Sort>@1 and Type=@2", new object[] { seq, "EPaper" }).OrderBy(v => v.Sort).ToArray();

                    var updatetime = DateTime.Now;
                    if (mtseqdata.Count() == 0)
                    {
                        var totalcnt = 0;
                        totalcnt = _columnsqlrepository.GetCountUseWhere("Type=@1", new object[] { "EPaper" });
                        seq = totalcnt;
                    }

                    var qseq = (seq > oldmodel.First().Sort) ? seq : oldmodel.First().Sort;
                    IList<ColumnSetting> ltseqdata = null;
                    ltseqdata = _columnsqlrepository.GetByWhere("Sort<=@1  and Type=@2", new object[] { qseq, "EPaper" }).OrderBy(v => v.Sort).ToArray();
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
                        _columnsqlrepository.Update(tempmodel);
                    }
                }
                //先取出大於目前seq的資料

                oldmodel.First().Sort = seq;
                r = _columnsqlrepository.Update(oldmodel.First());
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
                NLogManagement.SystemLogInfo("更新ColumnSetting排序失敗:" + " error:" + ex.Message);
                return "更新ColumnSetting排序失敗:" + " error:" + ex.Message;
            }
        }
        #endregion

        #region SetUnitModel
        public string SetUnitModel(EPaperUnitSettingModel model, string account)
        {
            var newmodel = new EPaperUnitSetting();
            newmodel.UpdateDatetime = DateTime.Now;
            newmodel.UpdateUser = account;
            var r = 0;
            var columnsetting = string.Join(",", model.ColumnName) + "@" + string.Join(",", model.ColumnUse);
            if (model.ID == -1)
            {
                //newmodel.FrontPagePath = model.FrontPagePath;
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
                newmodel.LangID =1;
                newmodel.IsRSS = model.IsRSS;
                newmodel.ShowCount = model.ShowCount;
                newmodel.ColumnSetting = columnsetting;
                newmodel.Summary = model.Summary == null ? "" : model.Summary;
                r = _unitsqlitemrepository.Create(newmodel);
            }
            else
            {
                newmodel.ID = model.ID.Value;
                //newmodel.FrontPagePath = model.FrontPagePath==null?"" : model.FrontPagePath;
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
                newmodel.Summary = model.Summary == null ? "" : model.Summary;
                newmodel.IsRSS = model.IsRSS;
                newmodel.ShowCount = model.ShowCount;
                newmodel.ColumnSetting = columnsetting;
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

        #region PagingDefaultADItem
        public Paging<EPaperAD> PagingDefaultADItem(SearchModelBase model)
        {
            var Paging = new Paging<EPaperAD>();
            var data = new List<EPaper>();

            var whereobj = new List<object>();
            var wherestr = new List<string>();
            wherestr.Add("LangID=@1 and MainID=-1");
            whereobj.Add(model.LangId);

            var str = string.Join(" and ", wherestr);
            Paging.rows = _adsqlrepository.GetDataUseWhereRange(str, whereobj.ToArray(), model.Offset, model.Limit, model.Sort).ToList();
            Paging.total = _adsqlrepository.GetCountUseWhere(str, whereobj.ToArray());

            return Paging;
        }
        #endregion

        #region EditADSeq
        public string EditADSeq(int id, int seq, string langid, string account, string username)
        {
            try
            {
                if (seq <= 0) { seq = 1; }
                var oldmodel = _adsqlrepository.GetByWhere("ID=@1", new object[] { id });
                if (oldmodel.Count() == 0) { return "更新失敗"; }
                var diff = "";

                diff = diff.TrimStart(',');
                var r = 0;
                if (oldmodel.First().ADIndex != seq)
                {
                    IList<EPaperAD> mtseqdata = null;
                    mtseqdata = _adsqlrepository.GetByWhere("ADIndex>@1 and MainID=-1 and LangID=@2", new object[] { seq, langid }).OrderBy(v => v.ADIndex).ToArray();

                    var updatetime = DateTime.Now;
                    if (mtseqdata.Count() == 0)
                    {
                        var totalcnt = 0;
                        totalcnt = _adsqlrepository.GetCountUseWhere("MainID=-1 and LangID=@1", new object[] { langid });
                        seq = totalcnt;
                    }

                    var qseq = (seq > oldmodel.First().ADIndex) ? seq : oldmodel.First().ADIndex;
                    IList<EPaperAD> ltseqdata = null;
                    ltseqdata = _adsqlrepository.GetByWhere("ADIndex<=@1  and MainID=-1 and LangID=@2", new object[] { qseq, langid }).OrderBy(v => v.ADIndex).ToArray();
                    //更新seq+1
                    var sidx = 0;
                    for (var idx = 1; idx <= ltseqdata.Count(); idx++)
                    {
                        if (idx == seq && seq < oldmodel.First().ADIndex)
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
                        _adsqlrepository.Update("ADIndex=@1", "ID=@2", new object[] { sidx, tempmodel.ID });
                    }
                }
                //先取出大於目前seq的資料

                oldmodel.First().ADIndex = seq;
                r = _adsqlrepository.Update("ADIndex=@1", "ID=@2", new object[] { seq, oldmodel.First().ID });
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
                NLogManagement.SystemLogInfo("更新預設廣告排序失敗:" + " error:" + ex.Message);
                return "更新預設廣告排序失敗:" + " error:" + ex.Message;
            }
        }
        #endregion

        #region SetADDelete
        public string SetADDelete(string[] idlist, string delaccount, string langid, string account, string username)
        {
            try
            {
                var r = 0;
                var adoldroot = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\UploadImage\\EPaperAD\\";
                for (var idx = 0; idx < idlist.Length; idx++)
                {
                    var addata = _adsqlrepository.GetByWhere("ID=@1", new object[] { idlist[idx] });
                    foreach (var a in addata)
                    {
                        if (a.ADFilePath.IsNullorEmpty() == false)
                        {
                            if (System.IO.File.Exists(adoldroot + a.ADFilePath))
                            {
                                System.IO.File.Delete(adoldroot + a.ADFilePath);
                            }
                        }
                        r = _adsqlrepository.DelDataUseWhere("ID=@1", new object[] { a.ID });
                    }
                }
                var rstr = "";
                if (r >= 0)
                {
                    NLogManagement.SystemLogInfo("刪除電子報單元失敗:" + delaccount);
                    rstr = "刪除成功";
                }
                else
                {
                    rstr = "刪除失敗";
                }
                var alldata = _adsqlrepository.GetByWhere("LangID=@1 and MainID=-1", new object[] { langid }).OrderBy(v => v.ADIndex).ToArray();
                for (var idx = 1; idx <= alldata.Count(); idx++)
                {
                    var tempmodel = alldata[idx - 1];
                    tempmodel.ADIndex = idx;
                    _adsqlrepository.Update("ADIndex=@1", "ID=@2", new object[] { idx, tempmodel.ID });
                }

                return rstr;
            }
            catch (Exception ex)
            {
                NLogManagement.SystemLogInfo("刪除電子報單元失敗:" + ex.Message);
                return "刪除失敗";
            }
        }
        #endregion

        #region EPaperDefaultADModel
        public string EPaperDefaultADModel(EPaperDefaultADModel model, string account, string langid)
        {
            var basepath = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\UploadImage\\EPaperAD\\";
            var smodel = new EPaperAD();
            var r = 0;
            if (model.ID == -1)
            {
                var alldata = _adsqlrepository.GetByWhere("LangID=@1 and MainID=-1", new object[] { langid }).OrderBy(v => v.ADIndex).ToArray();
                smodel.MainID = -1;
                smodel.ADName = model.ADName == null ? "" : model.ADName;
                smodel.ADLink = model.ADLink == null ? "" : model.ADLink;
                smodel.ADIndex = 1;
                smodel.LangID = int.Parse(langid);
                if (model.ADImageFiles != null)
                {
                    smodel.ADFileName = model.ADImageFiles.FileName.Split('\\').Last();
                }
                r = _adsqlrepository.Create(smodel);
                if (model.ADImageFiles != null)
                {
                    if (r > 0)
                    {
                        var path = "0_" + (smodel.ID) + "_" + (smodel.ADIndex) + "." + smodel.ADFileName.Split('.').Last();
                        model.ADImageFiles.SaveAs(basepath + path);
                        _adsqlrepository.Update("ADFilePath=@1", "ID=@2", new object[] { path, smodel.ID });
                    }
                }
                var sidx = 2;
                foreach (var ad in alldata)
                {
                    _adsqlrepository.Update("ADIndex=@1", "ID=@2", new object[] { sidx, ad.ID });
                    sidx += 1;
                }
                if (r > 0)
                {
                    return "新增成功";
                }
                else
                {
                    return "新增失敗";
                }
            }
            else
            {
                var oldaddata = _adsqlrepository.GetByWhere("ID=@1", new object[] { model.ID });
                //刪除舊檔案
                if (model.ADFileName.IsNullorEmpty())
                {

                    if (System.IO.File.Exists(basepath + "\\" + oldaddata.First().ADFilePath))
                    {
                        System.IO.File.Delete(basepath + "\\" + oldaddata.First().ADFilePath);
                    }
                    smodel.ADFilePath = "";
                    smodel.ADFileName = "";
                }
                else
                {
                    if (oldaddata.First().ADFilePath != model.ADFilePath)
                    {
                        if (System.IO.File.Exists(basepath + "\\" + oldaddata.First().ADFilePath))
                        {
                            System.IO.File.Delete(basepath + "\\" + oldaddata.First().ADFilePath);
                        }
                    }
                }
                smodel.ID = model.ID;
                smodel.ADName = model.ADName == null ? "" : model.ADName;
                smodel.ADLink = model.ADLink == null ? "" : model.ADLink;
                if (model.ADImageFiles != null)
                {
                    smodel.ADFileName = model.ADImageFiles.FileName.Split('\\').Last();
                }
                r = _adsqlrepository.Update(smodel);
                if (model.ADImageFiles != null)
                {
                    if (r > 0)
                    {
                        var path = "0_" + (oldaddata.First().ID) + "_" + (oldaddata.First().ADIndex) + "." + smodel.ADFileName.Split('.').Last();
                        model.ADImageFiles.SaveAs(basepath + path);
                        _adsqlrepository.Update("ADFilePath=@1", "ID=@2", new object[] { path, smodel.ID });
                    }
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
        }
        #endregion

        #region GetModel
        public EPaperDefaultADModel DefaultADModel(string id)
        {
            var model = new EPaperDefaultADModel();
            var addata = _adsqlrepository.GetByWhere("ID=@1", new object[] { id }).OrderBy(v => v.ADIndex);
            if (addata.Count() > 0)
            {
                model.ADFileName = addata.First().ADName == null ? "" : addata.First().ADName;
                model.ID = addata.First().ID.Value;
                model.ADLink = addata.First().ADLink == null ? "" : addata.First().ADLink;
                model.ADName = addata.First().ADName == null ? "" : addata.First().ADName;
                model.ADFilePath = addata.First().ADFilePath == null ? "" : addata.First().ADFilePath;
                model.ADFileName = addata.First().ADFileName == null ? "" : addata.First().ADFileName;
                model.ADUrl = VirtualPathUtility.ToAbsolute("~/UploadImage/EPaperAD/" + model.ADFilePath);
            }

            return model;
        }
        #endregion

        #region GetEPaperManuallyContent
        public string GetEPaperManuallyContent(string id)
        {
            var model = _epapercontnetsqlrepository.GetByWhere("EID=@1", new object[] { id });
            if (model.Count() > 0)
            {
                return model.First().EPaperHtmlContent;
            }

            return "";
        }
        #endregion

        #region SavePaperManuallyContent
        public string SavePaperManuallyContent(string id, string content)
        {
            var model = _epapercontnetsqlrepository.GetByWhere("EID=@1", new object[] { id });
            if (model.Count() > 0)
            {
                _epapercontnetsqlrepository.Update("EPaperHtmlContent=@1", "EID=@2", new object[] { content, id });
            }
            else
            {
                _epapercontnetsqlrepository.Create(new EPaperContent()
                {
                    EPaperHtmlContent = content,
                    EID = int.Parse(id)
                });
            }
            _siteconfigqlrepository.Update("LastUpdateDate=@1", "", new object[] { DateTime.Now.ToString("yyyy/MM/dd") });
            return "設定完成";
        }
        #endregion

        #region PagingMenuItem
        public Paging<EPaperContentItem> PagingMenuItem(SearchModelBase model)
        {
            var menu = _menusqlrepository.GetByWhere("ID=@1", new object[] { model.ModelID }).First();
            var clickdata = _epaperitemsqlrepository.GetByWhere("MenuID=@1 and EPaperID=@2", new object[] { model.ModelID, model.Key });
            var Paging = new Paging<EPaperContentItem>();
            if (menu.ModelID == 2)
            {
                var messagemodel = _messagesqlrepository.GetByWhere("ModelID=@1 Order By Sort", new object[] { menu.ModelItemID });
                Paging.total = messagemodel.Count();
                messagemodel = messagemodel.Skip(model.Offset).Take(model.Limit);
                foreach (var m in messagemodel)
                {
                    Paging.rows.Add(new EPaperContentItem()
                    {
                        Selected = clickdata.Any(v => v.ItemID == m.ItemID),
                        ItemID = m.ItemID,
                        ModelID = m.ModelID.Value,
                        MenuID = model.ModelID,
                        Enabled = m.Enabled.Value,
                        Title = m.Title
                    });
                }

            }
            else if (menu.ModelID == 3)
            {
                var _active = _activesqlrepository.GetByWhere("ModelID=@1  Order By Sort", new object[] { menu.ModelItemID });
                Paging.total = _active.Count();
                _active = _active.Skip(model.Offset).Take(model.Limit);
                foreach (var m in _active)
                {
                    Paging.rows.Add(new EPaperContentItem()
                    {
                        Selected = clickdata.Any(v => v.ItemID == m.ItemID),
                        ItemID = m.ItemID,
                        ModelID = m.ModelID.Value,
                        MenuID = model.ModelID,
                        Enabled = m.Enabled.Value,
                        Title = m.Title
                    });
                }

            }
            else if (menu.ModelID == 4)
            {
                var _active = _filedownloaditemsqlrepository.GetByWhere("ModelID=@1  Order By Sort", new object[] { menu.ModelItemID });
                Paging.total = _active.Count();
                _active = _active.Skip(model.Offset).Take(model.Limit);
                foreach (var m in _active)
                {
                    Paging.rows.Add(new EPaperContentItem()
                    {
                        Selected = clickdata.Any(v => v.ItemID == m.ItemID),
                        ItemID = m.ItemID,
                        ModelID = m.ModelID.Value,
                        MenuID = model.ModelID,
                        Enabled = m.Enabled.Value,
                        Title = m.Title
                    });
                }

            }
            else if (menu.ModelID == 7)
            {
                var _article = _articlesqlrepository.GetByWhere("ModelID=@1  Order By Sort", new object[] { menu.ModelItemID });
                Paging.total = _article.Count();
                _article = _article.Skip(model.Offset).Take(model.Limit);
                foreach (var m in _article)
                {
                    Paging.rows.Add(new EPaperContentItem()
                    {
                        Selected = clickdata.Any(v => v.ItemID == m.ItemID),
                        ItemID = m.ItemID,
                        ModelID = m.ModelID.Value,
                        MenuID = model.ModelID,
                        Enabled = m.Enabled.Value,
                        Title = m.Title
                    });
                }
            }

            return Paging;
        }
        #endregion

        #region SetEpaperItem
        public string SetEpaperItem(bool issel, string id, string itemid, string menuid, string modelid, string mainid)
        {
            if (issel == false)
            {
                _epaperitemsqlrepository.DelDataUseWhere("menuid=@1 and itemid=@2 and EPaperID=@3", new object[] { menuid, itemid, id });
                var data = _epaperitemsqlrepository.GetByWhere("menuid=@1 and modelid=@2 and EPaperID=@3 Order By Sort", new object[] { menuid, modelid, id }).ToArray();
                for (var idx = 1; idx <= data.Count(); idx++)
                {
                    _epaperitemsqlrepository.Update("Sort=@1", "menuid=@2 and itemid=@3 and EPaperID=@4", new object[] { idx, menuid, data[idx - 1].ItemID, id });
                }
                if (data.Count() == 0)
                {
                    var alldata = _epaperitemsqlrepository.GetByWhere("EPaperID=@1 order by GroupSortID", new object[] { id });
                    if (alldata.Count() > 0)
                    {
                        var groupdata = alldata.GroupBy(v => v.MenuID).ToArray();
                        for (var idx = 1; idx <= groupdata.Count(); idx++)
                        {
                            _epaperitemsqlrepository.Update("GroupSortID=@1", "menuid=@2  and EPaperID=@3", new object[] { idx, groupdata[idx - 1].First().MenuID, id });
                        }

                    }
                }
            }
            else
            {
                var itemdata = _epaperitemsqlrepository.GetByWhere("menuid=@1 and modelid=@2 and EPaperID=@3", new object[] { menuid, modelid, id });
                var gsount = 1;
                if (itemdata.Count() > 0)
                {
                    gsount = itemdata.First().GroupSortID;
                }
                else
                {
                    var alldata = _epaperitemsqlrepository.GetByWhere("EPaperID=@1 order by GroupSortID", new object[] { id });
                    if (alldata.Count() > 0) { gsount = alldata.Last().GroupSortID + 1; }
                }
                var menu = _menusqlrepository.GetByWhere("ID=@1", new object[] { menuid });
                var edata = new EPaperItem()
                {
                    EPaperID = int.Parse(id),
                    ItemID = int.Parse(itemid),
                    MenuID = int.Parse(menuid),
                    ModelID = int.Parse(modelid),
                    MainID = int.Parse(mainid),
                    Sort = itemdata.Count() + 1,
                    GroupSortID = gsount
                };
                if (menu.First().MenuLevel == 1)
                {
                    edata.MenuLevel1Sort = menu.First().Sort.Value;
                }
                else if (menu.First().MenuLevel == 2)
                {
                    edata.MenuLevel2Sort = menu.First().Sort.Value;
                    var tempmenu = _menusqlrepository.GetByWhere("ID=@1", new object[] { menu.First().ParentID });
                    edata.MenuLevel1Sort = tempmenu.First().Sort.Value;
                }
                else if (menu.First().MenuLevel == 3)
                {
                    edata.MenuLevel3Sort = menu.First().Sort.Value;
                    var tempmenu = _menusqlrepository.GetByWhere("ID=@1", new object[] { menu.First().ParentID });
                    edata.MenuLevel2Sort = tempmenu.First().Sort.Value;
                    tempmenu = _menusqlrepository.GetByWhere("ID=@1", new object[] { tempmenu.First().ParentID });
                    edata.MenuLevel1Sort = tempmenu.First().Sort.Value;
                }
                _epaperitemsqlrepository.Create(edata);
            }
            return "";
        }
        #endregion

        #region GetEPaperItemEdit
        public List<EPaperItemEdit> GetEPaperItemEdit(string id)
        {
            var model = new List<EPaperItemEdit>();
            UrlHelper helper = new UrlHelper(HttpContext.Current.Request.RequestContext);
            //var list = _epaperitemsqlrepository.GetByWhere("EPaperID=@1", new object[] { id }).OrderBy(v=>v.MenuLevel1Sort)
            //    .ThenBy(v => v.MenuLevel2Sort).ThenBy(v => v.MenuLevel3Sort).ThenBy(v=>v.Sort);
            var list = _epaperitemsqlrepository.GetByWhere("EPaperID=@1 Order by GroupSortID", new object[] { id });
            var grouplist = list.GroupBy(v => v.MenuID);
            foreach (var g1 in grouplist)
            {
                var g2list = g1.OrderBy(v => v.Sort).GroupBy(x => x.MainID);
                foreach (var g2 in g2list)
                {
                    var EPaperItemEdit = new EPaperItemEdit();
                    if (g1.Count() > 0) {
                        EPaperItemEdit.MenuID = g1.First().MenuID.ToString();
                        EPaperItemEdit.SortID = g1.First().GroupSortID;
                        EPaperItemEdit.MainID = g1.First().MainID.ToString();
                    }
                 
                    EPaperItemEdit.ItemName = new List<string>();
                    EPaperItemEdit.ItemUrl = new List<string>();
                    EPaperItemEdit.ItemKey = new List<string>();
                  
                    var modelid = g2.First().ModelID;
                    foreach (var item in g2.ToList())
                    {
                        EPaperItemEdit.ItemKey.Add(item.ModelID + "_" + item.ItemID + "_" + item.MenuID + "_" + item.MainID);
                    }
                   if (modelid == 2)
                    {
                        var maindata = _messagemainsqlrepository.GetByWhere("ID=@1", new object[] { g2.Key });
                        if (maindata.Count() == 0)
                        {
                            continue;
                        }
                        EPaperItemEdit.Name = maindata.First().Name;
                        var itemlist = _messagesqlrepository.GetByWhere("ModelID=@1", new object[] { g2.Key });
                        foreach (var item in g2.ToList())
                        {
                            var data = itemlist.Where(v => v.ItemID == item.ItemID);
                            if (data.Count() > 0)
                            {
                                EPaperItemEdit.ItemName.Add(data.First().Title);
                                EPaperItemEdit.ItemUrl.Add(helper.Action("MessageView", "Message", new { Area = "" }) + "?id=" + item.ItemID + "&mid=" + item.MenuID);
                            }
                        }
                    }
                    else if (modelid == 3)
                    {
                        var maindata = _activemainsqlrepository.GetByWhere("ID=@1", new object[] { g2.Key });
                        if (maindata.Count() == 0)
                        {
                            continue;
                        }
                        EPaperItemEdit.Name = maindata.First().Name;
                        var itemlist = _activesqlrepository.GetByWhere("ModelID=@1", new object[] { g2.Key });
                        foreach (var item in g2.ToList())
                        {
                            var data = itemlist.Where(v => v.ItemID == item.ItemID);
                            if (data.Count() > 0)
                            {
                                EPaperItemEdit.ItemName.Add(data.First().Title);
                                EPaperItemEdit.ItemUrl.Add(helper.Action("ActiveView", "Active", new { Area = "" }) + "?id=" + item.ItemID + "&mid=" + item.MenuID);
                            }
                        }
                    }
                    else if (modelid == 4)
                    {
                        var maindata = _filedownloadsqlrepository.GetByWhere("ID=@1", new object[] { g2.Key });
                        if (maindata.Count() == 0)
                        {
                            continue;
                        }
                        EPaperItemEdit.Name = maindata.First().Name;
                        var itemlist = _filedownloaditemsqlrepository.GetByWhere("ModelID=@1", new object[] { g2.Key });
                        foreach (var item in g2.ToList())
                        {
                            var data = itemlist.Where(v => v.ItemID == item.ItemID);
                            if (data.Count() > 0)
                            {
                                EPaperItemEdit.ItemName.Add(data.First().Title);
                                EPaperItemEdit.ItemUrl.Add(helper.Action("Index", "Download", new { Area = "" }) + "?id=" + item.ItemID + "&mid=" + item.MenuID);
                            }
                        }
                    }
                    else if (modelid == 7)
                    {
                        var maindata = _articlemainsqlrepository.GetByWhere("ID=@1", new object[] { g2.Key });
                        if (maindata.Count() == 0)
                        {
                            continue;
                        }
                        EPaperItemEdit.Name = maindata.First().Name;
                        var itemlist = _articlesqlrepository.GetByWhere("ModelID=@1", new object[] { g2.Key });
                        foreach (var item in g2.ToList())
                        {
                            var data = itemlist.Where(v => v.ItemID == item.ItemID);
                            if (data.Count() > 0)
                            {
                                EPaperItemEdit.ItemName.Add(data.First().Title);
                                EPaperItemEdit.ItemUrl.Add(helper.Action("ArticleView", "Article", new { Area = "" }) + "?id=" + item.ItemID + "&mid=" + item.MenuID);
                            }
                        }
                    }
                    model.Add(EPaperItemEdit);
                }


            }
            return model;
        }
        #endregion

        #region GetSortTable
        public string GetSortTable(string id)
        {
            var list = _epaperitemsqlrepository.GetByWhere("EPaperID=@1 Order By GroupSortID", new object[] { id }).OrderBy(v => v.MenuLevel1Sort)
              .ThenBy(v => v.MenuLevel2Sort).ThenBy(v => v.MenuLevel3Sort).ThenBy(v => v.Sort);

            var grouplist = list.GroupBy(v => v.MenuID);
            var sb = new System.Text.StringBuilder();
            foreach (var g1 in grouplist)
            {
                var menu = _menusqlrepository.GetByWhere("ID=@1", new object[] { g1.Key });
                var lists = g1.ToList().OrderBy(v => v.Sort);
                if (menu.Count() > 0 && lists.Count() > 0)
                {
                    sb.Append("<div class='table-scrollable'>");
                    sb.Append("<table class='table table-bordered table-hover' border='0' cellspacing='0' cellpadding='0'>");
                    sb.Append("<thead><tr class='bg-grey_1' filed-class='odd gradeX'>");
                    sb.Append("<th class='text-center' width='80px'>刪除</th><th class='text-center' width='100px'>排序</th><th class='text-center'>標題</th></thead><tbody>");

                    if (lists.First().ModelID == 2)
                    {
                        var itemlist = _messagesqlrepository.GetByWhere("ModelID=@1", new object[] { lists.First().MainID });
                        foreach (var item in lists)
                        {
                            var citem = itemlist.Where(v => v.ItemID == item.ItemID);
                            if (citem.Count() > 0)
                            {
                                sb.Append("<tr><td class='text-center'><label class='mt-checkbox mt-checkbox-single mt-checkbox-outline'><input type='checkbox' class='checkboxes' id='"
                                    + (item.EPaperID + "_" + item.MenuID + "_" + item.ItemID) + "'/><span></span></label></td>");
                                sb.Append("<td class='text-center'><input type='hidden' value='" + item.Sort + "'/>" +
                                 "<input  type='text'  value='" + item.Sort + "' class='editinput form-control input-xsmall sortedit' idindex='" +
                                (item.EPaperID + "_" + item.MenuID + "_" + item.ItemID) + "'/><span class='required seqerror' style='color:red;display:none;margin-left:5px;font-size:12px'></span></td>");
                                sb.Append("<td class='text-center'>" + citem.First().Title + "</td></tr>");
                            }
                        }
                    }
                    else if (lists.First().ModelID == 3)
                    {
                        var itemlist = _activesqlrepository.GetByWhere("ModelID=@1", new object[] { lists.First().MainID });
                        foreach (var item in lists)
                        {
                            var citem = itemlist.Where(v => v.ItemID == item.ItemID);
                            if (citem.Count() > 0)
                            {
                                sb.Append("<tr><td class='text-center'><label class='mt-checkbox mt-checkbox-single mt-checkbox-outline'><input type='checkbox' class='checkboxes' id='"
                              + (item.EPaperID + "_" + item.MenuID + "_" + item.ItemID) + "'/><span></span></label></td>");
                                sb.Append("<td class='text-center'><input type='hidden' value='" + item.Sort + "'/>" +
                                 "<input  type='text'  value='" + item.Sort + "' class='editinput form-control input-xsmall sortedit' idindex='" +
                                (item.EPaperID + "_" + item.MenuID + "_" + item.ItemID) + "'/><span class='required seqerror' style='color:red;display:none;margin-left:5px;font-size:12px'></span></td>");
                                sb.Append("<td class='text-center'>" + citem.First().Title + "</td></tr>");
                            }
                        }
                    }
                    else if (lists.First().ModelID == 4)
                    {
                        var itemlist = _filedownloaditemsqlrepository.GetByWhere("ModelID=@1", new object[] { lists.First().MainID });
                        foreach (var item in lists)
                        {
                            var citem = itemlist.Where(v => v.ItemID == item.ItemID);
                            if (citem.Count() > 0)
                            {
                                sb.Append("<tr><td class='text-center'><label class='mt-checkbox mt-checkbox-single mt-checkbox-outline'><input type='checkbox' class='checkboxes' id='"
                              + (item.EPaperID + "_" + item.MenuID + "_" + item.ItemID) + "'/><span></span></label></td>");
                                sb.Append("<td class='text-center'><input type='hidden' value='" + item.Sort + "'/>" +
                                 "<input  type='text'  value='" + item.Sort + "' class='editinput form-control input-xsmall sortedit' idindex='" +
                                (item.EPaperID + "_" + item.MenuID + "_" + item.ItemID) + "'/><span class='required seqerror' style='color:red;display:none;margin-left:5px;font-size:12px'></span></td>");
                                sb.Append("<td class='text-center'>" + citem.First().Title + "</td></tr>");
                            }
                        }
                    }
                    else if (lists.First().ModelID == 7)
                    {
                        var itemlist = _articlesqlrepository.GetByWhere("ModelID=@1", new object[] { lists.First().MainID });
                        foreach (var item in lists)
                        {
                            var citem = itemlist.Where(v => v.ItemID == item.ItemID);
                            if (citem.Count() > 0)
                            {
                                sb.Append("<tr><td class='text-center'><label class='mt-checkbox mt-checkbox-single mt-checkbox-outline'><input type='checkbox' class='checkboxes' id='"
                            + (item.EPaperID + "_" + item.MenuID + "_" + item.ItemID) + "'/><span></span></label></td>");
                                sb.Append("<td class='text-center'><input type='hidden' value='" + item.Sort + "'/>" +
                                 "<input  type='text'  value='" + item.Sort + "' class='editinput form-control input-xsmall sortedit' idindex='" +
                                (item.EPaperID + "_" + item.MenuID + "_" + item.ItemID) + "'/><span class='required seqerror' style='color:red;display:none;margin-left:5px;font-size:12px'></span></td>");
                                sb.Append("<td class='text-center'>" + citem.First().Title + "</td></tr>");
                            }
                        }
                    }

                    sb.Append("</tbody></table></div>");
                }
            }
            return sb.ToString();
        }
        #endregion

        #region EditItemSeq
        public string EditItemSeq(string id, int seq, string type, string account, string username)
        {
            try
            {
                if (seq <= 0) { seq = 1; }
                var arrid = id.Split('_');
                var oldmodel = _epaperitemsqlrepository.GetByWhere("EPaperID=@1 and menuid=@2 and itemid=@3", new object[] { arrid[0], arrid[1], arrid[2] });
                if (oldmodel.Count() == 0) { return "更新失敗"; }
                var diff = "";

                diff = diff.TrimStart(',');
                var r = 0;
                if (oldmodel.First().Sort != seq)
                {
                    IList<EPaperItem> mtseqdata = null;
                    mtseqdata = _epaperitemsqlrepository.GetByWhere("Sort>@1 and EPaperID=@2 and menuid=@3", new object[] { seq, arrid[0], arrid[1] }).OrderBy(v => v.Sort).ToArray();

                    var updatetime = DateTime.Now;
                    if (mtseqdata.Count() == 0)
                    {
                        var totalcnt = 0;
                        totalcnt = _epaperitemsqlrepository.GetCountUseWhere("EPaperID=@1 and menuid=@2", new object[] { arrid[0], arrid[1] });
                        seq = totalcnt;
                    }

                    var qseq = (seq > oldmodel.First().Sort) ? seq : oldmodel.First().Sort;
                    IList<EPaperItem> ltseqdata = null;
                    ltseqdata = _epaperitemsqlrepository.GetByWhere("Sort<=@1  and EPaperID=@2 and menuid=@3", new object[] { qseq, arrid[0], arrid[1] }).OrderBy(v => v.Sort).ToArray();
                    //更新seq+1
                    var sidx = 0;
                    for (var idx = 1; idx <= ltseqdata.Count(); idx++)
                    {
                        if (idx == seq && seq < oldmodel.First().Sort)
                        {
                            sidx += 1;
                        }
                        var tempmodel = ltseqdata[idx - 1];
                        if (tempmodel.ItemID == int.Parse(arrid[2]))
                        {
                            continue;
                        }
                        else
                        {
                            sidx += 1;
                        }
                        tempmodel.Sort = sidx;
                        _epaperitemsqlrepository.Update("Sort=@1", "EPaperID=@2 and menuid=@3 and itemid=@4", new object[] { sidx, tempmodel.EPaperID, tempmodel.MenuID, tempmodel.ItemID });
                    }
                }
                //先取出大於目前seq的資料
                oldmodel.First().Sort = seq;
                r = _epaperitemsqlrepository.Update("Sort=@1", "EPaperID=@2 and menuid=@3 and itemid=@4", new object[] { seq, oldmodel.First().EPaperID, oldmodel.First().MenuID, oldmodel.First().ItemID });
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

        #region DeleteItems
        public string DeleteItems(string[] idlist)
        {
            try
            {
                if (idlist.Length == 0) { return "刪除成功"; }
                var r = 0;
                var menuidlist = new List<string>();
                var epaperid = idlist[0].Split('_')[0];
                for (var idx = 0; idx < idlist.Length; idx++)
                {
                    var arrid = idlist[idx].Split('_');
                    if (menuidlist.Contains(arrid[1]) == false) { menuidlist.Add(arrid[1]); }
                    _epaperitemsqlrepository.DelDataUseWhere("EPaperID=@1 and menuid=@2 and itemid=@3", new object[] { arrid[0], arrid[1], arrid[2] });
                }
                foreach (var mid in menuidlist)
                {
                    var data = _epaperitemsqlrepository.GetByWhere("menuid=@1  and EPaperID=@2 Order By Sort", new object[] { mid, epaperid }).ToArray();
                    for (var idx = 1; idx <= data.Count(); idx++)
                    {
                        _epaperitemsqlrepository.Update("Sort=@1", "menuid=@2 and itemid=@3 and EPaperID=@4", new object[] { idx, mid, data[idx - 1].ItemID, epaperid });
                    }
                }
                var rstr = "";
                if (r >= 0)
                {
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
                NLogManagement.SystemLogInfo("刪除電子報單元失敗:" + ex.Message);
                return "刪除失敗";
            }
        }
        #endregion

        #region UpdateEpaperOrder
        public string UpdateEpaperOrder(string id, string status)
        {
            return "更新成功";
            //try
            //{
            //    var r = _studentmainsqlrepository.Update("OrderEPaper=@1", "ID=@2", new object[] { status, id });
            //    if (r >= 0)
            //    {
            //        return "更新成功";
            //    }
            //    else
            //    {
            //        return "更新失敗";
            //    }
            //}
            //catch (Exception)
            //{
            //    return "更新失敗";
            //}
        }
        #endregion

        #region GetExport
        public byte[] GetExport(SubscriberSearchModel model)
        {

            var Paging = new Paging<EPaperSubscriber>();
            var whereobj = new List<object>();
            var wherestr = new List<string>();
            var idx = 0;
            if (model.CHNName != null)
            {
                idx += 1;
                wherestr.Add("CHNName like @" + idx);
                whereobj.Add("%" + model.CHNName + "%");
            }
            if (string.IsNullOrEmpty(model.EMail) == false)
            {
                idx += 1;
                wherestr.Add("EMail like @" + idx);
                whereobj.Add("%" + model.EMail + "%");
            }
            if (string.IsNullOrEmpty(model.Status) == false)
            {
                idx += 1;
                wherestr.Add("Status=@" + idx);
                whereobj.Add(model.Status);
            }
            if (string.IsNullOrEmpty(model.OPDateFrom) == false)
            {
                idx += 1;
                wherestr.Add("OPDateStr>=@" + idx);
                whereobj.Add(model.OPDateFrom);
            }
            if (string.IsNullOrEmpty(model.OPDateTo) == false)
            {
                idx += 1;
                wherestr.Add("OPDateStr<=@" + idx);
                whereobj.Add(model.OPDateTo);
            }
            if (wherestr.Count() > 0)
            {
                var str = string.Join(" and ", wherestr);
                Paging.rows = _epapersubscribersqlrepository.GetByWhere(str+" Order by "+ model.Sort, whereobj.ToArray()).ToList();
            }
            else
            {
                Paging.rows = _epapersubscribersqlrepository.GetAll("*", " Order by "+model.Sort).ToList();
            }

            MemoryStream ms = new MemoryStream();
            XSSFWorkbook hssfworkbook = new XSSFWorkbook();
            IFont font = hssfworkbook.CreateFont();
            font.FontHeightInPoints = 9;
            ICellStyle style = hssfworkbook.CreateCellStyle();
            style.Alignment = HorizontalAlignment.Left;
            style.VerticalAlignment = VerticalAlignment.Center;
            style.WrapText = true;
            style.SetFont(font);

            ISheet sheet = hssfworkbook.CreateSheet("數據資料庫列表");
            IRow row = sheet.CreateRow(0);
            row.HeightInPoints = 16;
            SetValue(sheet, "訂閱/取消日期", 0, 0, style);
            SetValue(sheet, "EMail", 0, 1, style);
            SetValue(sheet, "是否訂閱", 0, 2, style);
            sheet.SetColumnWidth(0, 5000);
            sheet.SetColumnWidth(1, 10000);
            sheet.SetColumnWidth(2, 3000);
            var ridx = 1;
            foreach (var d in Paging.rows)
            {
                SetValue(sheet, d.OPDateStr, ridx, 0, style);
                SetValue(sheet, d.EMail, ridx, 1, style);
                SetValue(sheet, d.Status == true ? "是" : "否", ridx, 2, style);
                ridx += 1;
            }
            hssfworkbook.Write(ms);
            hssfworkbook = null;
            byte[] bytes = ms.ToArray();
            ms.Close();
            ms.Dispose();
            return bytes;
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

        #region GetSyndicationFeedData
        public SyndicationFeed GetSyndicationFeedData(string langid, string menuid)
        {
            UrlHelper helper = new UrlHelper(HttpContext.Current.Request.RequestContext);
            var menu = _menusqlrepository.GetByWhere("ID=@1", new object[] { menuid });
            var title = "電子報";
            if (menu.Count() > 0) { title = menu.First().MenuName; }
            var hostUrl = string.Format("{0}://{1}",
                  System.Web.HttpContext.Current.Request.Url.Scheme,
                  System.Web.HttpContext.Current.Request.Url.Authority);
            var feed = new SyndicationFeed(
              "電子報",
              title + " - Rss Feed",
              new Uri(HttpContext.Current.Request.Url.AbsoluteUri));
            var whereobj = new List<object>();
            var wherestr = new List<string>();
            wherestr.Add("LangID=@1");
            whereobj.Add(langid);
            wherestr.Add("IsEdit=1");
            wherestr.Add("Enabled=1");
            var str = string.Join(" and ", wherestr);
            var data = _sqlrepository.GetDataUseWhereRange(str, whereobj.ToArray(), 0, 100, "Sort Desc").ToList();
            List<SyndicationItem> items = new List<SyndicationItem>();
            foreach (var d in data)
            {
                var content = d.Introduction == null ? "" : d.Introduction.TrimgHtmlTag();
                if (content.Length >= 100) { content = content.Substring(0, 100); }
                var url = "";
                if (menuid.IsNullorEmpty())
                {
                    url = helper.Action("Review", new { id = d.ID });
                }
                else
                {
                    url = helper.Action("Review", new { id = d.ID, mid = menuid });
                }
                SyndicationItem item = new SyndicationItem(
                d.Title.TrimgHtmlTag(), content, new Uri(hostUrl + url), d.ID.ToString(),
               DateTime.Parse(d.PublicshStr));
                if (d.PublicshStr.IsNullorEmpty() == false)
                {
                    item.PublishDate = DateTime.Parse(d.PublicshStr);
                }
                else
                {
                    item.PublishDate = DateTime.Now;
                }
                items.Add(item);
            }
            feed.Items = items;
            return feed;
        }
        #endregion

        #region PagingEpaperOrder
        public Paging<EPaperSubscriber> PagingEpaperOrder(SubscriberSearchModel model)
        {
            var Paging = new Paging<EPaperSubscriber>();
            var whereobj = new List<object>();
            var wherestr = new List<string>();
            var idx = 0;
            if (model.CHNName != null)
            {
                idx += 1;
                wherestr.Add("CHNName like @" + idx);
                whereobj.Add("%" + model.CHNName + "%");
            }
            if (string.IsNullOrEmpty(model.EMail) == false)
            {
                idx += 1;
                wherestr.Add("EMail like @" + idx);
                whereobj.Add("%" + model.EMail + "%");
            }
            if (string.IsNullOrEmpty(model.Status) == false)
            {
                idx += 1;
                wherestr.Add("Status=@" + idx);
                whereobj.Add(model.Status);
            }
            if (string.IsNullOrEmpty(model.OPDateFrom) == false)
            {
                idx += 1;
                wherestr.Add("OPDateStr>=@" + idx);
                whereobj.Add(model.OPDateFrom);
            }
            if (string.IsNullOrEmpty(model.OPDateTo) == false)
            {
                idx += 1;
                wherestr.Add("OPDateStr<=@" + idx);
                whereobj.Add(model.OPDateTo);
            }
            if (wherestr.Count() > 0)
            {
                var str = string.Join(" and ", wherestr);
                Paging.rows = _epapersubscribersqlrepository.GetDataUseWhereRange(str, whereobj.ToArray(), model.Offset, model.Limit, model.Sort).ToList();
                Paging.total = _epapersubscribersqlrepository.GetCountUseWhere(str, whereobj.ToArray());
            }
            else
            {
                Paging.rows = _epapersubscribersqlrepository.GetAllDataRange(model.Offset, model.Limit, model.Sort).ToList();
                Paging.total = _epapersubscribersqlrepository.GetAllDataCount();
            }
            return Paging;
        }
        #endregion

        #region AddSubscriber
        public string AddSubscriber(string email,  string account)
        {
            var alldata = _epapersubscribersqlrepository.GetByWhere("EMail=@1", new object[] { email });
            if (alldata.Count() > 0) {
                return "此EMail已經訂閱";
            }
            var nowdate = DateTime.Now;
            var Model = new EPaperSubscriber()
            {
                EMail = email,
                Status = true,
                CreateDate = nowdate,
                CreateUser = account,
                UpdateDate = nowdate,
                UpdateUser = account,
                OPDateStr= nowdate.ToString("yyyy/MM/dd")
            };
            var r = _epapersubscribersqlrepository.Create(Model);
            if (r > 0)
            {
                return "訂閱成功";
            }
            else
            {
                return "訂閱失敗";
            }
        }
        #endregion

        #region DelSubscriber
        public string DelSubscriber(string[] idlist, string delaccount, string langid, string account, string username)
        {
            try
            {
                var r = 0;
                for (var idx = 0; idx < idlist.Length; idx++)
                {
                    var entity = new EPaperSubscriber();
                    entity.ID = int.Parse(idlist[idx]);
                    r = _epapersubscribersqlrepository.Delete(entity);
                    if (r <= 0)
                    {
                        NLogManagement.SystemLogInfo("刪除訂閱者單元成功:ID=" + idlist[idx]);
                    }
                    else
                    {
                    }
                }
                var rstr = "";
                if (r >= 0)
                {
                    NLogManagement.SystemLogInfo("刪除訂閱者單元失敗:" + delaccount);
                    rstr = "取消訂閱成功";
                }
                else
                {
                    rstr = "取消訂閱失敗";
                }
                return rstr;
            }
            catch (Exception ex)
            {
                NLogManagement.SystemLogInfo("刪除訂閱者單元失敗:" + ex.Message);
                return "取消訂閱失敗";
            }
        }
        #endregion

        #region SetSubscriberStatus
        public string SetSubscriberStatus(string id, bool status, string account, string username)
        {
            try
            {
                var entity = new  EPaperSubscriber();
                entity.Status = status ? true : false;
                entity.ID = int.Parse(id);
                var r = 0;
                var nowdate = DateTime.Now.ToString("yyyy/MM/dd");
                if (entity.Status==true)
                {
                 r=   _epapersubscribersqlrepository.Update("Status=@1,UpdateDate=@2,UpdateUser=@3,OPDateStr=@4", "ID=@5",
                     new object[] { entity.Status,  nowdate, account, nowdate, entity.ID });
                }
                else {
                    r = _epapersubscribersqlrepository.Update("Status=@1,DisableDate=@2,UpdateDate=@3,UpdateUser=@4,OPDateStr=@5", "ID=@6", 
                        new object[] { entity.Status, nowdate, nowdate,account, nowdate, entity.ID });
                }
              
                if (r >= 0)
                {
                    NLogManagement.SystemLogInfo("修改MessageItem顯示狀態 id=" + id + " 為:" + status);
                    return "更新成功";
                }
                else
                {
                    return "更新失敗";
                }

            }
            catch (Exception ex)
            {
                NLogManagement.SystemLogInfo("修改MessageItem顯示狀態失敗:" + ex.Message);
                return "更新失敗";
            }
        }
        #endregion

        #region GetEPaperYearstr
        public string GetEPaperYearstr(string year="")
        {
            var allopdata = _sqlrepository.GetAll().Where(v => v.CreareDatetime != null);
            var allgroupyear = allopdata.Select(v => v.CreareDatetime.Value.ToString("yyyy")).GroupBy(v => v);
            var sb = new System.Text.StringBuilder();
            foreach (var yg in allgroupyear)
            {
                if (year == yg.Key)
                {
                    sb.Append("<option value='" + yg.Key + "' selected>" + yg.Key + "</option>");
                }
                else {
                    sb.Append("<option value='" + yg.Key + "'>" + yg.Key + "</option>");
                }
           
            }
            return sb.ToString();
        }
        #endregion

        #region SaveEPaperItemSort
        public string SaveEPaperItemSort(Dictionary<string, string> allitemkey,string eid,string iseditsub)
        {
            try
            {
                var idx = 1;
                foreach (var key in allitemkey.Keys)
                {
                    var menuid = key.Replace("trm_", "");
                   var r= _epaperitemsqlrepository.Update("GroupSortID=@1", "EPaperID=@2 and MenuID=@3", new object[] { idx, eid, menuid });
                    idx++;
                    if (iseditsub=="1") {
                        var keyvalue = allitemkey[key];
                        if (keyvalue.Length > 0) {
                            var keyarr = keyvalue.Split(',');
                            for (var sidx = 0; sidx < keyarr.Length; sidx++) {
                                var keys = keyarr[sidx].Split('_');
                                r = _epaperitemsqlrepository.Update("Sort=@1", "EPaperID=@2 and ModelID=@3 and ItemID=@4 and MenuID=@5 and MainID=@6",
                                    new object[] { sidx+1, eid, keys[0], keys[1], keys[2], keys[3] });
                            }
                        }

                    }
                }
                _siteconfigqlrepository.Update("LastUpdateDate=@1", "", new object[] { DateTime.Now.ToString("yyyy/MM/dd") });
                return "更新成功";
            }
            catch (Exception ex) {
                return "更新失敗";
            }
        }

        #endregion

        #region DeleteEPaperItemSort
        public string DeleteEPaperItemSort(string[] delarrid, string eid)
        {//
            try
            {
                if (delarrid.Length == 0) { return "更新成功"; }
                var r = 0;
                foreach (var delid in delarrid)
                {
                    if (delid.IndexOf("chk_m_") == 0)
                    {
                        var menuid = delid.Replace("chk_m_", "");
                        r =_epaperitemsqlrepository.DelDataUseWhere("EPaperID=@1 and MenuID=@2", new object[] {eid, menuid });
                    }
                    else
                    {
                        var subitemkey = delid.Replace("chk_s_", "");
                        var keys = subitemkey.Split('_');
                        r = _epaperitemsqlrepository.DelDataUseWhere("EPaperID=@1 and ModelID=@2 and ItemID=@3 and MenuID=@4 and MainID=@5",
                            new object[] { eid, keys[0], keys[1], keys[2], keys[3] });
                    }
                }
                var allitem = _epaperitemsqlrepository.GetByWhere("EPaperID=@1 Order by GroupSortID,Sort", new object[] { eid });
                var grouplist = allitem.GroupBy(v => v.MenuID).ToList();
                for (var gidx = 0; gidx < grouplist.Count(); gidx++) {
                    var menuid = grouplist[gidx].Key;
                    r = _epaperitemsqlrepository.Update("GroupSortID=@1", "EPaperID=@2 and MenuID=@3", new object[] { gidx+1, eid, menuid });
                    if (r > 0) {
                        var subitem = grouplist[gidx].OrderBy(v=>v.Sort).ToList();
                        for (var sidx = 0; sidx < subitem.Count(); sidx++)
                        {
                            r = _epaperitemsqlrepository.Update("Sort=@1", "EPaperID=@2 and ModelID=@3 and ItemID=@4 and MenuID=@5 and MainID=@6",
                                new object[] { sidx + 1, eid, subitem[sidx].ModelID, subitem[sidx].ItemID, subitem[sidx].MenuID, subitem[sidx].MainID });
                        }
                    }
                }
                _siteconfigqlrepository.Update("LastUpdateDate=@1", "", new object[] { DateTime.Now.ToString("yyyy/MM/dd") });
                return "更新成功";
            }
            catch (Exception ex) {
                return "更新失敗";
            }
        }
        #endregion

        #region DelSubscriber
        public string DelSubscriber(string email, string delaccount)
        {
            try
            {
                var r = 0;
                r = _epapersubscribersqlrepository.DelDataUseWhere("EMail=@1", new object[] { email });
                if (r >= 0)
                {
                    NLogManagement.SystemLogInfo("刪除訂閱者單元失敗:" + delaccount);
                    return "取消訂閱成功";
                }
                else
                {
                    return "取消訂閱失敗";
                }
            }
            catch (Exception ex)
            {
                NLogManagement.SystemLogInfo("刪除訂閱者單元失敗:" + ex.Message);
                return "取消訂閱失敗";
            }
        }
        #endregion

        #region GetEPaperItemEdit
        public List<EPaperItem> GetEPaperItemList(string epaperid)
        {
            return _epaperitemsqlrepository.GetByWhere("EPaperID=@1", new object[] { epaperid }).ToList();
        }
        #endregion
    }
}
