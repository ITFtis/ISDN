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

namespace Services.Manager
{
    public class ADMobileManager : IADManager
    {
        readonly SQLRepository<ADMobile> _sqlrepository;
        readonly SQLRepository<ADSet> _adsetsqlrepository;
        public ADMobileManager(SQLRepository<ADMobile> sqlrepository)
        {
            _sqlrepository = sqlrepository;
            _adsetsqlrepository = new SQLRepository<ADSet>(sqlrepository._connectionString);
        }

        #region Paging
        public Paging<ADListResult> Paging(ADSearchModel model)
        {
            var Paging = new Paging<ADListResult>();
            var data = new List<ADBase>();

            var whereobj = new List<string>();
            var wherestr = new List<string>();
            var idx =1;
            wherestr.Add("Lang_ID=@" + idx);
            whereobj.Add(model.Lang_ID);
            idx += 1;
            wherestr.Add("SType=@" + idx);
            whereobj.Add(model.SType);
            if (string.IsNullOrEmpty(model.AD_Name) == false)
            {
                idx += 1;
                wherestr.Add("AD_Name like @" + idx);
                whereobj.Add("%" + model.AD_Name + "%");
            }


            if (wherestr.Count() > 0)
            {
                var str = string.Join(" and ", wherestr);
                data = _sqlrepository.GetDataUseWhereRange(str, whereobj.ToArray(), model.Offset, model.Limit, model.Sort).ToList<ADBase>();
                Paging.total = _sqlrepository.GetCountUseWhere(str, whereobj.ToArray());

            }
            else
            {
                if (model.ADType == "right")
                {
                    Paging.total = _sqlrepository.GetAllDataCount();
                    data = _sqlrepository.GetAllDataRange(model.Offset, model.Limit, model.Sort).ToList<ADBase>();
                }
                else
                {
                    Paging.total = _sqlrepository.GetAllDataCount();
                    data = _sqlrepository.GetAllDataRange(model.Offset, model.Limit, model.Sort).ToList<ADBase>();
                }
            }
            foreach (var d in data)
            {
                var isrange = false;
                if (d.StDate == null && d.EdDate == null) { isrange = true; }
                else if (d.StDate != null && d.EdDate == null)
                {
                    if (DateTime.Now >= d.StDate.Value)
                    {
                        isrange = true;
                    }
                }
                else if (d.StDate == null && d.EdDate != null)
                {
                    if (DateTime.Now <= d.EdDate.Value.AddDays(1))
                    {
                        isrange = true;
                    }
                }
                else if (d.StDate != null && d.EdDate != null)
                {
                    if (DateTime.Now >= d.StDate.Value && DateTime.Now <= d.EdDate.Value.AddDays(1))
                    {
                        isrange = true;
                    }
                }
                Paging.rows.Add(new ADListResult()
                {
                    ID = d.ID.Value,
                    Fixed = d.Fixed,
                    Sort = d.Sort,
                    AD_Name = d.AD_Name,
                    Enabled = d.Enabled,
                    IsRange = isrange == true ? "是" : "否"
                });

            }
            return Paging;
        }
        #endregion

        #region GetADSet
        public ADSet GetADSet(string langid, string type, string stype)
        {
            var adset = _adsetsqlrepository.GetByWhere("Lang_ID=@1 and Type_ID=@2 and SType=@3", new object[] { langid, type, stype });
            if (adset.Count() > 0) { return adset.First(); }
              return new ADSet() {
                Max_Num = 1
            };
        }
        #endregion

        #region SetMaxADCount
        public string SetMaxADCount(string langid, string type, string max, string stype)
        {
            var adset = _adsetsqlrepository.GetByWhere("Lang_ID=@1 and Type_ID=@2  and SType=@3", new object[] { langid, type, stype });
            var r = 0;
            if (adset.Count() > 0)
            {
                r = _adsetsqlrepository.Update(new ADSet()
                {
                    ID = adset.First().ID,
                    Max_Num =int.Parse( max)
                });
            }
            else {
                r = _adsetsqlrepository.Create(new ADSet()
                {
                    Max_Num = int.Parse(max),
                    Lang_ID = int.Parse(langid),
                    Type_ID = type,
                    SType = stype
                });
            }

            if (r > 0)
            {
                return ("修改成功");
            }
            else {
                return ("修改失敗");
            }
        }
        #endregion

        #region UpdateSeq
        public string UpdateSeq(int id, int seq, string type,string account, string username)
        {
            try
            {
                if (seq <= 0) { seq = 1; }
                var oldmodel = new List<ADMobile>();
                oldmodel = _sqlrepository.GetByWhere("ID=@1", new object[] { id }).ToList<ADMobile>();
                if (oldmodel.Count() == 0) { return "更新失敗"; }
                var stype = oldmodel.First().SType;
                var langid = oldmodel.First().Lang_ID;
                var diff = "";
                diff = diff.TrimStart(',');
                var r = 0;
                if (oldmodel.First().Sort != seq)
                {
                    IList<ADMobile> mtseqdata = null;
                    mtseqdata = _sqlrepository.GetByWhere("Sort>@1 and Lang_ID=@2 and Stype=@3 ", new object[] { seq, langid, stype }).OrderBy(v => v.Sort).ToArray();
                    var updatetime = DateTime.Now;
                    if (mtseqdata.Count() == 0)
                    {
                        var totalcnt = 0;
                        totalcnt = _sqlrepository.GetCountUseWhere("Stype=@1", new object[] { stype });
                        seq = totalcnt;
                    }

                    var qseq = (seq > oldmodel.First().Sort) ? seq : oldmodel.First().Sort;
                    IList<ADMobile> ltseqdata = null;
                    ltseqdata = _sqlrepository.GetByWhere("Sort<=@1 and Lang_ID=@2  and Stype=@3", new object[] { qseq, langid, stype }).OrderBy(v => v.Sort).ToArray();
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
                        _sqlrepository.Update("Sort=@1", "ID=@2", new object[] { tempmodel.Sort, tempmodel.ID });

                    }
                }
                oldmodel.First().Sort = seq;
                r = _sqlrepository.Update("Sort=@1", "ID=@2", new object[] { oldmodel.First().Sort, oldmodel.First().ID });
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
                NLogManagement.SystemLogInfo("更新群組排序失敗:" + " error:" + ex.Message);
                return "更新群組排序失敗:" + " error:" + ex.Message;
            }
        }
        #endregion

        #region Delete
        public string Delete(string[] idlist, string delaccount,string langid, string account, string username)
        {
            try
            {
                var r = 0;
                var stype = "";
                for (var idx = 0; idx < idlist.Length; idx++)
                {
                    var olddate = _sqlrepository.GetByWhere("ID=@1", new object[] { idlist[idx] });
                    stype = olddate.First().SType;
                    var entity = new ADMobile();
                    entity.ID = int.Parse(idlist[idx]);
                    r = _sqlrepository.Delete(entity);
                    if (r <= 0)
                    {
                        NLogManagement.SystemLogInfo("刪除輪播廣告失敗:ID=" + idlist[idx]);
                    }
                    else
                    {
                        var oldroot = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\UploadImage\\ADCenter\\";
                        if (System.IO.File.Exists(oldroot + "\\" + olddate.First().Img_Name_Ori))
                        {
                            System.IO.File.Delete(oldroot + "\\" + olddate.First().Img_Name_Ori);
                        }
                        if (System.IO.File.Exists(oldroot + "\\" + olddate.First().Img_Name_Thumb))
                        {
                            System.IO.File.Delete(oldroot + "\\" + olddate.First().Img_Name_Thumb);
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
                    NLogManagement.SystemLogInfo("刪除輪播廣告失敗:" + delaccount);
                    rstr = "刪除失敗";
                }
                var alldata = _sqlrepository.GetByWhere("Lang_ID=@1 and SType=@2", new object[] { langid, stype }).OrderBy(v => v.Sort).ToArray();
                for (var idx = 1; idx <= alldata.Count(); idx++)
                {
                    var tempmodel = alldata[idx - 1];
                    tempmodel.Sort = idx;
                    _sqlrepository.Update("Sort=@1", "ID=@2", new object[] { tempmodel.Sort, tempmodel.ID });
                }

                return rstr;
            }
            catch (Exception ex)
            {
                NLogManagement.SystemLogInfo("刪除輪播廣告失敗:" + ex.Message);
                return "刪除失敗";
            }
        }
        #endregion

        #region Create
        public string Create(ADEditModel model, string account, string username)
        {
            var datetime = DateTime.Now;
            var alldata = _sqlrepository.GetByWhere("Lang_ID=@1 and SType=@2", new object[] { model.Lang_ID, model.SType });
            foreach (var mdata in alldata)
            {
                mdata.Sort = mdata.Sort + 1;
                _sqlrepository.Update("Sort=@1","ID=@2",new object[] { mdata.Sort, mdata.ID });
            }
            var ad = new ADMobile()
            {
                Link_Href = model.Link_Href,
                Lang_ID = model.Lang_ID,
                AD_Height = model.AD_Height,
                AD_Name = model.AD_Name,
                AD_Width = model.AD_Width,
                Link_Mode = model.Link_Mode,
                Fixed = true,
                Create_Date = datetime,
                StDate = model.StDate,
                EdDate = model.EdDate,
                Enabled = true,
                Img_Name_Ori = model.Img_Name_Ori,
                Sort = 1,
                Type_ID = model.Type,
                UpdateDatetime = datetime,
                UpdateUser = account,
                Img_Name_Thumb = model.Img_Name_Thumb,
                Img_Show_Name = model.Img_Show_Name,
                SType = model.SType
            };

            var r = _sqlrepository.Create(ad);
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
        public string Update(ADEditModel model, string account, string username)
        {
            var olddata = _sqlrepository.GetByWhere("ID=@1", new object[] { model.ID });
            var oldfilename = olddata.First().Img_Name_Thumb;
            var oldfileoriname = olddata.First().Img_Name_Ori;
            var oldroot = System.Web.HttpContext.Current.Request.PhysicalApplicationPath +
                "\\UploadImage\\ADMobile\\" + oldfilename;
            var oldoriroot = System.Web.HttpContext.Current.Request.PhysicalApplicationPath +
              "\\UploadImage\\ADMobile\\" + oldfileoriname;
            if (model.Img_Name_Ori.IsNullorEmpty()) {
                model.Img_Name_Thumb = "";
                model.Img_Name_Ori = "";
                model.Img_Show_Name = "";
            }
            var datetime = DateTime.Now;
            var ad = new ADMobile()
            {
                Link_Href = model.Link_Href,
                AD_Height = model.AD_Height==null?150: model.AD_Height,
                AD_Name = model.AD_Name ,
                AD_Width = model.AD_Width == null ? 135 : model.AD_Width,
                Link_Mode = model.Link_Mode==null?"": model.Link_Mode,
                Fixed = true,
                StDate = model.StDate,
                EdDate = model.EdDate,
                Img_Name_Ori = model.Img_Name_Ori,
                UpdateDatetime = datetime,
                UpdateUser = account,
                Img_Name_Thumb = model.Img_Name_Thumb,
                ID =model.ID,
                Img_Show_Name=model.Img_Show_Name
                
            };

            var r = _sqlrepository.Update(ad);
            if (r > 0)
            {
                if (model.Img_Name_Ori.IsNullorEmpty()|| model.Img_Name_Thumb!= oldfilename) {
                    if(System.IO.File.Exists(oldroot)){
                        System.IO.File.Delete(oldroot);
                    }
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

        #region GetModel
        public ADEditModel GetModel(string id)
        {
            var model = new ADEditModel();
            var data = _sqlrepository.GetByWhere("ID=@1", new object[] { id });
            if (data.Count() > 0)
            {
                _sqlrepository.Mapping<ADMobile, ADEditModel>(data.First(), model);
            }
            if (model.StDate != null) {
                model.StDateStr = model.StDate.Value.ToString("yyyy/MM/dd");
            }
            if (model.EdDate != null)
            {
                model.EdDateStr = model.EdDate.Value.ToString("yyyy/MM/dd");
            }
            return model;
        }
        #endregion

        #region UpdateStatus
        public string UpdateStatus(string id, bool status, string account, string username)
        {
            try
            {
                //var entity = new ADMobile();
                //entity.Enabled = status ? true : false;
                //entity.UpdateDatetime = DateTime.Now;
                //entity.ID = int.Parse(id);
                //entity.UpdateUser = account;
                //var r = _sqlrepository.Update(entity);
                var r = _sqlrepository.Update("Enabled=@1,UpdateDatetime=@2,UpdateUser=@3", "ID=@4", new object[] { status, DateTime.Now, account, id });
                if (r >= 0)
                {
                    NLogManagement.SystemLogInfo("修改知識庫管理顯示狀態 id=" + id + " 為:" + status);
                    return "更新成功";
                }
                else
                {
                    return "更新失敗";
                }

            }
            catch (Exception ex)
            {
                NLogManagement.SystemLogInfo("修改知識庫管理顯示狀態失敗:" + ex.Message);
                return "更新失敗";
            }
        }
        #endregion

        #region UpdateFixed
        public string UpdateFixed(string id, bool status, string account, string username)
        {
            try
            {
                //var entity = new ADMobile();
                //entity.Fixed = status ? true : false;
                //entity.UpdateDatetime = DateTime.Now;
                //entity.ID = int.Parse(id);
                //entity.UpdateUser = account;
                //var r = _sqlrepository.Update(entity);
                var r = _sqlrepository.Update("Fixed=@1,UpdateDatetime=@2,UpdateUser=@3", "ID=@4", new object[] { status, DateTime.Now, account, id });
                if (r >= 0)
                {
                    NLogManagement.SystemLogInfo("修改知識庫管理顯示狀態 id=" + id + " 為:" + status);
                    return "更新成功";
                }
                else
                {
                    return "更新失敗";
                }

            }
            catch (Exception ex)
            {
                NLogManagement.SystemLogInfo("修改知識庫管理顯示狀態失敗:" + ex.Message);
                return "更新失敗";
            }
        }
        #endregion
    }
}
