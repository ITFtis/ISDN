using Services.Interface;
using Services.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModels.DBModels;

namespace Services
{
    public class ServiceInstances
    {
        public SQLRepositoryInstances _sqlinstance = null;
        public ServiceInstances(SQLRepositoryInstances sqlinstance) { _sqlinstance = sqlinstance; }
        public virtual IModelActiveManager ModelActiveManager { get { return new ModelActiveManager(_sqlinstance); } }
        public virtual IModelFileDownloadManager ModelFileDownloadManager { get { return new ModelFileDownloadManager(_sqlinstance); } }
        public virtual IModelMessageManager MessageManager { get { return new ModelMessageManager(_sqlinstance); } }
        public virtual IModelPageEditManager ModelPageEditManager { get { return new ModelPageEditManager(_sqlinstance); } }
        public virtual ISiteConfigManager SiteConfigManager { get { return new SiteConfigManager(_sqlinstance); } }
        public virtual IModelEventListManager ModelEventListManager { get { return new ModelEventListManager(_sqlinstance); } }
        public virtual ILoginManager LoginManager { get { return new LoginManager(_sqlinstance); } }
        public virtual ILangManager LangManager { get { return new LangManager(_sqlinstance); } }
        public virtual IMenuManager MenuManager { get { return new MenuManager(_sqlinstance); } }
        public virtual IModelFormManager ModelFormManager { get { return new ModelFormManager(_sqlinstance); } }
        public virtual IModelArticleManager ModelArticleManager { get { return new ModelArticleManager(_sqlinstance); } }
        public virtual IModelWebsiteMapManager ModelWebsiteMapManager { get { return new ModelWebsiteMapManager(_sqlinstance); } }
        public virtual IModelPatentManager ModelPatentManager { get { return new ModelPatentManager(_sqlinstance); } }
        public virtual ISiteLayoutManager SiteLayoutManager { get { return new SiteLayoutManager(_sqlinstance); } }
        public virtual IModelLinkManager ModelLinkManager { get { return new ModelLinkManager(_sqlinstance); } }
        public virtual IEPaperManager EPaperManager { get { return new EPaperManager(_sqlinstance); } }
        

    }
}
