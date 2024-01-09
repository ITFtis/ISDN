using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace WebSiteProject.Code
{
    public class RssActionResult : ActionResult
    {
        private readonly SyndicationFeed feed;

        public RssActionResult()
        {
        }

        public RssActionResult(SyndicationFeed feed)
        {
            this.feed = feed;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            //context.HttpContext.Response.ContentType = "application/rss+xml";
            context.HttpContext.Response.ContentType = "application/xml";
            var formatter = new Rss20FeedFormatter(feed);
            using (var writer = XmlWriter.Create(context.HttpContext.Response.Output))
            {
                formatter.WriteTo(writer);
            }
        }
    }

    public class AtomActionResult : ActionResult
    {
        SyndicationFeed feed;

        public AtomActionResult() { }

        public AtomActionResult(SyndicationFeed feed)
        {
            this.feed = feed;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            //  context.HttpContext.Response.ContentType = "application/atom+xml";
            context.HttpContext.Response.ContentType = "application/xml";
            Atom10FeedFormatter formatter = new Atom10FeedFormatter(this.feed);
            using (XmlWriter writer = XmlWriter.Create(context.HttpContext.Response.Output))
            {
                formatter.WriteTo(writer);
            }
        }
    }
}