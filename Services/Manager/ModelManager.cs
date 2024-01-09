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

namespace Services.Manager
{
    public class ModelManager : IModelManager
    {
        readonly SQLRepository<PageIndexSetting> _indexsqlrepository;
        readonly SQLRepository<CountSearchKey> _countsearchkeysqlrepository;
        string _connectionStr = "";
        public ModelManager(string connectionStr)
        {
            _connectionStr = connectionStr;
            _indexsqlrepository = new SQLRepository<PageIndexSetting>(connectionStr);
            _countsearchkeysqlrepository=new SQLRepository<CountSearchKey>(connectionStr);
        }

        #region GetPageIndexSettingModel
        public PageIndexSettingModel GetPageIndexSettingModel(string lang_id)
        {
            var data = _indexsqlrepository.GetByWhere("Lang_ID=@1", new object[] { lang_id });
            var model = new PageIndexSettingModel();
            if (data.Count() > 0)
            {
                model= new PageIndexSettingModel()
                {
                    ShowCount = data.First().ShowCount,
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
                    Column21 = data.First().Column21,
                     HtmlContent= data.First().HtmlContent,
                      IsFirstPage= data.First().IsFirstPage,
                      IsInPage= data.First().IsInPage,
                      ID=data.First().ID
                };
            }
            model.ColumnNameMapping = new Dictionary<string, string>();
            model.ColumnNameMapping.Add("上方選單", model.Column1.IsNullorEmpty() ? "上方選單" : model.Column1);
            model.ColumnNameMapping.Add("主要選單", model.Column2.IsNullorEmpty() ? "主要選單" : model.Column2);
            model.ColumnNameMapping.Add("下方選單", model.Column3.IsNullorEmpty() ? "下方選單" : model.Column3);
            model.ColumnNameMapping.Add("查詢詞彙", model.Column4.IsNullorEmpty() ? "查詢詞彙" : model.Column4);
            model.ColumnNameMapping.Add("請輸入查詢詞", model.Column5.IsNullorEmpty() ? "請輸入查詢詞" : model.Column5);
            model.ColumnNameMapping.Add("每頁顯示", model.Column6.IsNullorEmpty() ? "每頁顯示" : model.Column6);
            model.ColumnNameMapping.Add("搜尋範圍", model.Column7.IsNullorEmpty() ? "搜尋範圍" : model.Column7);

            model.ColumnNameMapping.Add("標題及內文", model.Column8.IsNullorEmpty() ? "標題及內文" : model.Column8);
            model.ColumnNameMapping.Add("搜尋結果", model.Column9.IsNullorEmpty() ? "搜尋結果" : model.Column9);
            model.ColumnNameMapping.Add("標題", model.Column10.IsNullorEmpty() ? "標題" : model.Column10);
            model.ColumnNameMapping.Add("內文", model.Column11.IsNullorEmpty() ? "內文" : model.Column11);
            model.ColumnNameMapping.Add("網站選項", model.Column12.IsNullorEmpty() ? "網站選項" : model.Column12);
            var hotkey= _countsearchkeysqlrepository.GetByWhere("LangID=@1 Order By Count Desc", new object[] { lang_id }).ToArray();
            if (hotkey.Count() > 0) { model.HotKey1 = hotkey[0].SearchKey; }
            if (hotkey.Count() > 1) { model.HotKey2 = hotkey[1].SearchKey; }
            if (hotkey.Count() > 2) { model.HotKey3 = hotkey[2].SearchKey; }
            return model;
        }
        #endregion

        #region SetPageIndexSettingModel
        public string SetPageIndexSettingModel(PageIndexSettingModel model,string  langid, string account)
        {
           var savemodel = new PageIndexSetting()
           {
               Column1 = model.Column1==null?"" : model.Column1,
               Column2 = model.Column2 == null ? "" : model.Column2,
               Column3 = model.Column3 == null ? "" : model.Column3,
               Column4 = model.Column4 == null ? "" : model.Column4,
               Column5 = model.Column5 == null ? "" : model.Column5,
               Column6 = model.Column6 == null ? "" : model.Column6,
               Column7 = model.Column7 == null ? "" : model.Column7,
               Column8 = model.Column8 == null ? "" : model.Column8,
               Column9 = model.Column9 == null ? "" : model.Column9,
               Column10 = model.Column10 == null ? "" : model.Column10,
               Column11 = model.Column11 == null ? "" : model.Column11,
               Column12 = model.Column12 == null ? "" : model.Column12,
               Column13 = model.Column13 == null ? "" : model.Column13,
               Column14 = model.Column14 == null ? "" : model.Column14,
               Column15 = model.Column15 == null ? "" : model.Column15,
               Column16 = model.Column16 == null ? "" : model.Column16,
               Column17 = model.Column17 == null ? "" : model.Column17,
               Column18 = model.Column18 == null ? "" : model.Column18,
               Column19 = model.Column19 == null ? "" : model.Column19,
               Column20 = model.Column20 == null ? "" : model.Column20,
               Column21 = model.Column21 == null ? "" : model.Column21,
               HtmlContent = model.HtmlContent == null ? "" : model.HtmlContent,
               IsFirstPage = model.IsFirstPage,
               IsInPage = model.IsInPage,
               ID=model.ID,
               Lang_ID=int.Parse(langid),
                ShowCount=model.ShowCount
           };
            var r = 0;
            _countsearchkeysqlrepository.DelDataUseWhere("LangID=@1", new object[] { langid });
            if (model.HotKey1.IsNullorEmpty() == false) {
                _countsearchkeysqlrepository.Create(new CountSearchKey()
                {
                    Count = 102,
                    LangID = int.Parse(langid),
                    SearchKey = model.HotKey1
                });
            }
            if (model.HotKey2.IsNullorEmpty() == false)
            {
                _countsearchkeysqlrepository.Create(new CountSearchKey()
                {
                    Count = 101,
                    LangID = int.Parse(langid),
                    SearchKey = model.HotKey2
                });
            }
            if (model.HotKey3.IsNullorEmpty() == false)
            {
                _countsearchkeysqlrepository.Create(new CountSearchKey()
                {
                    Count = 100,
                    LangID = int.Parse(langid),
                    SearchKey = model.HotKey3
                });
            }


            if (model .ID> 0)
            {
                r = _indexsqlrepository.Update(savemodel);
            }
            else {
                r = _indexsqlrepository.Create(savemodel);
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
    }
}
