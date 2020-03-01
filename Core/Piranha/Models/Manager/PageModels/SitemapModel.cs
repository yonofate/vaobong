using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Piranha.Models.Manager.PageModels
{
	public class SitemapModel
	{
		public IList<Sitemap> Pages { get; set; }
		public IList<PageTemplate> Templates { get; set; }
		public string ParentTypes { get; set; }

		public SitemapModel() {
			Pages = new List<Sitemap>();
			Templates = new List<PageTemplate>();
			ParentTypes = "";
		}

		public bool Subpages(Sitemap page) {
			var template = Templates.SingleOrDefault(t => t.Id == page.TemplateId);

			if (template != null) {
				return !template.IsBlock && template.Subpages;
			}
			return false;
		}

		public Guid[] Blocks(Sitemap page) {
			var query = Templates.Where(t => t.BlockTypes.Contains(page.TemplateId));

			if (page.IsBlock)
				query = query.Where(t => t.IsBlock);
			return query.Select(t => t.Id).ToArray();
		}

		public string BlocksString(Sitemap page) {
			var blocks = Blocks(page);
			var sb = new StringBuilder();

			foreach (var block in blocks) {
				if (sb.Length > 0)
					sb.Append(",");
				sb.Append(block.ToString());
			}
			return sb.ToString();
		}
	}
}