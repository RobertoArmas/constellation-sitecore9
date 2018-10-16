﻿using System.Text;
using System.Web.Mvc;
using Constellation.Feature.PageTagging.Repositories;
using Sitecore.Links;
using Sitecore.Mvc.Presentation;

namespace Constellation.Feature.PageTagging.Controllers
{
	public class PageSocialMetadataController : Controller
	{
		private const string MetaName = "<meta name=\"{0}\" content=\"{1}\" />";
		private const string MetaProperty = "<meta property=\"{0}\" content=\"{1}\" />";

		#region Constructor

		public PageSocialMetadataController(ISocialMetadataRepository repository)
		{
			Repository = repository;
		}
		#endregion

		#region Properties
		protected ISocialMetadataRepository Repository { get; }
		#endregion

		public ActionResult Index()
		{
			var model = Repository.GetMetadata(RenderingContext.Current.ContextItem);

			var builder = new StringBuilder();

			if (!string.IsNullOrEmpty(model.TwitterCreator) || !string.IsNullOrEmpty(model.TwitterSite))
			{
				builder.AppendLine(string.Format(MetaName, "twitter:card", model.TwitterCardType));

				if (!string.IsNullOrEmpty(model.TwitterCreator))
				{
					builder.AppendLine(string.Format(MetaName, "twitter:creator", model.TwitterCreator));
				}

				if (!string.IsNullOrEmpty(model.TwitterSite))
				{
					builder.AppendLine(string.Format(MetaName, "twitter:site", model.TwitterSite));
				}
			}

			var options = new UrlOptions
			{
				AlwaysIncludeServerUrl = true,
				LowercaseUrls = true,
				SiteResolving = true
			};

			var url = LinkManager.GetItemUrl(RenderingContext.Current.PageContext.Item, options);

			builder.AppendLine(string.Format(MetaProperty, "og:url", url));
			builder.AppendLine(string.Format(MetaProperty, "og:title", model.BrowserTitle));
			if (!string.IsNullOrEmpty(model.MetaDescription))
			{
				builder.AppendLine(string.Format(MetaProperty, "og:description", model.MetaDescription));
			}

			if (!string.IsNullOrEmpty(model.SocialThumbnail))
			{
				builder.AppendLine(string.Format(MetaProperty, "og:image", model.SocialThumbnail));
			}

			return Content(builder.ToString());
		}
	}
}
