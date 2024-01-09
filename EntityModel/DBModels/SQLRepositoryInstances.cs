using SQLModel;
using SQLModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels.DBModels
{
    public class SQLRepositoryInstances : DbContext
    {
        public SQLRepositoryInstances(string dbname) : base(dbname) { }
        public SQLRepository<T> GetSQInstances<T>() where T : class { return new SQLRepository<T>(base._dbname); }
    
        public virtual SQLRepository<ActiveItem> ActiveItem { get { return new SQLRepository<ActiveItem>(base._dbname); } }
        public virtual SQLRepository<ActiveDateRange> ActiveDateRange { get { return new SQLRepository<ActiveDateRange>(base._dbname); } }
        public virtual SQLRepository<AdminFunctionAuth> AdminFunctionAuth { get { return new SQLRepository<AdminFunctionAuth>(base._dbname); } }
        public virtual SQLRepository<ActiveUnitSetting> ActiveUnitSetting { get { return new SQLRepository<ActiveUnitSetting>(base._dbname); } }
        public virtual SQLRepository<ActivePhoto> ActivePhoto { get { return new SQLRepository<ActivePhoto>(base._dbname); } }
        public virtual SQLRepository<ActivePhotoCount> ActivePhotoCount { get { return new SQLRepository<ActivePhotoCount>(base._dbname); } }
        public virtual SQLRepository<AdminFunction> AdminFunction { get { return new SQLRepository<AdminFunction>(base._dbname); } }
        public virtual SQLRepository<ColumnSetting> ColumnSetting { get { return new SQLRepository<ColumnSetting>(base._dbname); } }
        public virtual SQLRepository<ClickCountTable> ClickCountTable { get { return new SQLRepository<ClickCountTable>(base._dbname); } }
        public virtual SQLRepository<FileDownloadItem> FileDownloadItem { get { return new SQLRepository<FileDownloadItem>(base._dbname); } }
        public virtual SQLRepository<FileDownloadUnitSetting> FileDownloadUnitSetting { get { return new SQLRepository<FileDownloadUnitSetting>(base._dbname); } }
        public virtual SQLRepository<Menu> Menu { get { return new SQLRepository<Menu>(base._dbname); } }
        public virtual SQLRepository<MessageItem> MessageItem { get { return new SQLRepository<MessageItem>(base._dbname); } }
        public virtual SQLRepository<ModelActiveEditMain> ModelActiveEditMain { get { return new SQLRepository<ModelActiveEditMain>(base._dbname); } }
        public virtual SQLRepository<ModelFileDownloadMain> ModelFileDownloadMain { get { return new SQLRepository<ModelFileDownloadMain>(base._dbname); } }
        public virtual SQLRepository<MessageUnitSetting> MessageUnitSetting { get { return new SQLRepository<MessageUnitSetting>(base._dbname); } }
        public virtual SQLRepository<ModelMessageMain> ModelMessageMain { get { return new SQLRepository<ModelMessageMain>(base._dbname); } }
        public virtual SQLRepository<ModelPageEditMain> ModelPageEditMain { get { return new SQLRepository<ModelPageEditMain>(base._dbname); } }
        public virtual SQLRepository<GroupActive> GroupActive { get { return new SQLRepository<GroupActive>(base._dbname); } }
        public virtual SQLRepository<GroupFileDownload> GroupFileDownload { get { return new SQLRepository<GroupFileDownload>(base._dbname); } }
        public virtual SQLRepository<GroupMessage> GroupMessage { get { return new SQLRepository<GroupMessage>(base._dbname); } }
        public virtual SQLRepository<GroupUser> GroupUser { get { return new SQLRepository<GroupUser>(base._dbname); } }
        public virtual SQLRepository<Img> Img { get { return new SQLRepository<Img>(base._dbname); } }
        public virtual SQLRepository<PageIndexItem> PageIndexItem { get { return new SQLRepository<PageIndexItem>(base._dbname); } }
        public virtual SQLRepository<PageLayout> PageLayout { get { return new SQLRepository<PageLayout>(base._dbname); } }
        public virtual SQLRepository<PageUnitSetting> PageUnitSetting { get { return new SQLRepository<PageUnitSetting>(base._dbname); } }
        public virtual SQLRepository<SiteConfig> SiteConfig { get { return new SQLRepository<SiteConfig>(base._dbname); } }
        public virtual SQLRepository<SiteFlow> SiteFlow { get { return new SQLRepository<SiteFlow>(base._dbname); } }
        public virtual SQLRepository<SEO> SEO { get { return new SQLRepository<SEO>(base._dbname); } }
        public virtual SQLRepository<Users> Users { get { return new SQLRepository<Users>(base._dbname); } }
        public virtual SQLRepository<VerifyData> VerifyData { get { return new SQLRepository<VerifyData>(base._dbname); } }
        public virtual SQLRepository<ModelEventListMain> ModelEventListMain { get { return new SQLRepository<ModelEventListMain>(base._dbname); } }
        public virtual SQLRepository<EventListItem> EventListItem { get { return new SQLRepository<EventListItem>(base._dbname); } }
        public virtual SQLRepository<ModelWebsiteMapMain> ModelWebsiteMapMain { get { return new SQLRepository<ModelWebsiteMapMain>(base._dbname); } }
        public virtual SQLRepository<ModelLangMain> ModelLangMain { get { return new SQLRepository<ModelLangMain>(base._dbname); } }
        public virtual SQLRepository<MenuUrl> MenuUrl { get { return new SQLRepository<MenuUrl>(base._dbname); } }
        public virtual SQLRepository<ModelFormMain> ModelFormMain { get { return new SQLRepository<ModelFormMain>(base._dbname); } }
        public virtual SQLRepository<Lang> Lang { get { return new SQLRepository<Lang>(base._dbname); } }
        public virtual SQLRepository<StudentFormSetting> StudentFormSetting { get { return new SQLRepository<StudentFormSetting>(base._dbname); } }
        public virtual SQLRepository<FormUnitSetting> FormUnitSetting { get { return new SQLRepository<FormUnitSetting>(base._dbname); } }
        public virtual SQLRepository<FormSelItem> FormSelItem { get { return new SQLRepository<FormSelItem>(base._dbname); } }
        public virtual SQLRepository<FormSetting> FormSetting { get { return new SQLRepository<FormSetting>(base._dbname); } }
        public virtual SQLRepository<FormInput> FormInput { get { return new SQLRepository<FormInput>(base._dbname); } }
        public virtual SQLRepository<FormInputNote> FormInputNote { get { return new SQLRepository<FormInputNote>(base._dbname); } }
        public virtual SQLRepository<ModelArticleMain> ModelArticleMain { get { return new SQLRepository<ModelArticleMain>(base._dbname); } }
        public virtual SQLRepository<GroupArticle> GroupArticle { get { return new SQLRepository<GroupArticle>(base._dbname); } }
        public virtual SQLRepository<ArticleItem> ArticleItem { get { return new SQLRepository<ArticleItem>(base._dbname); } }
        public virtual SQLRepository<ArticleUnitSetting> ArticleUnitSetting { get { return new SQLRepository<ArticleUnitSetting>(base._dbname); } }
        public virtual SQLRepository<WebSiteMapInfo> WebSiteMapInfo { get { return new SQLRepository<WebSiteMapInfo>(base._dbname); } }
        public virtual SQLRepository<ModelPatentMain> ModelPatentMain { get { return new SQLRepository<ModelPatentMain>(base._dbname); } }
        public virtual SQLRepository<PatentItem> PatentItem { get { return new SQLRepository<PatentItem>(base._dbname); } }
        public virtual SQLRepository<PatentUnitSetting> PatentUnitSetting { get { return new SQLRepository<PatentUnitSetting>(base._dbname); } }
        public virtual SQLRepository<GroupPatent> GroupPatent { get { return new SQLRepository<GroupPatent>(base._dbname); } }
        public virtual SQLRepository<PatentDetail> PatentDetail { get { return new SQLRepository<PatentDetail>(base._dbname); } }
        public virtual SQLRepository<SiteLayout> SiteLayout { get { return new SQLRepository<SiteLayout>(base._dbname); } }
        public virtual SQLRepository<LangKey> LangKey { get { return new SQLRepository<LangKey>(base._dbname); } }
        public virtual SQLRepository<LangInputText> LangInputText { get { return new SQLRepository<LangInputText>(base._dbname); } }
        public virtual SQLRepository<LinkItem> LinkItem { get { return new SQLRepository<LinkItem>(base._dbname); } }
        public virtual SQLRepository<PageLayoutActivity> PageLayoutActivity { get { return new SQLRepository<PageLayoutActivity>(base._dbname); } }

        public virtual SQLRepository<PageSeminar> PageSeminar { get { return new SQLRepository<PageSeminar>(base._dbname); } }
        public virtual SQLRepository<PageNewsEdit> PageNewsEdit { get { return new SQLRepository<PageNewsEdit>(base._dbname); } }
        public virtual SQLRepository<PageActiveEdit> PageActiveEdit { get { return new SQLRepository<PageActiveEdit>(base._dbname); } }
        public virtual SQLRepository<PageJournalEdit> PageJournalEdit { get { return new SQLRepository<PageJournalEdit>(base._dbname); } }
        public virtual SQLRepository<FileDownloadFiles> FileDownloadFiles { get { return new SQLRepository<FileDownloadFiles>(base._dbname); } }
        public virtual SQLRepository<ActiveFiles> ActiveFiles { get { return new SQLRepository<ActiveFiles>(base._dbname); } }
        public virtual SQLRepository<GroupItemArticle> GroupItemArticle { get { return new SQLRepository<GroupItemArticle>(base._dbname); } }
        public virtual SQLRepository<ArticelGroupItem> ArticelGroupItem { get { return new SQLRepository<ArticelGroupItem>(base._dbname); } }
        public virtual SQLRepository<EPaper> EPaper { get { return new SQLRepository<EPaper>(base._dbname); } }
        public virtual SQLRepository<EPaperAD> EPaperAD { get { return new SQLRepository<EPaperAD>(base._dbname); } }
        public virtual SQLRepository<EPaperUnitSetting> EPaperUnitSetting { get { return new SQLRepository<EPaperUnitSetting>(base._dbname); } }
        public virtual SQLRepository<EPaperContent> EPaperContent { get { return new SQLRepository<EPaperContent>(base._dbname); } }
        public virtual SQLRepository<EPaperItem> EPaperItem { get { return new SQLRepository<EPaperItem>(base._dbname); } }
        public virtual SQLRepository<EPaperSubscriber> EPaperSubscriber { get { return new SQLRepository<EPaperSubscriber>(base._dbname); } }
        public virtual SQLRepository<PageActiveItem> PageActiveItem { get { return new SQLRepository<PageActiveItem>(base._dbname); } }
    }
}
